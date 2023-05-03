using EldenBingoCommon;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace EldenBingoServer
{
    public class Server
    {
        const int MatchStartCountdown = 9999;

        private readonly IPAddress[] _ipAddress;
        private readonly int _port;
        private bool _hosting;

        private readonly ConcurrentBag<TcpListener> _tcpListeners;
        private readonly ConcurrentBag<ClientModel> _clients;
        private readonly ConcurrentDictionary<string, ServerRoom> _rooms;

        private CancellationTokenSource _cancelToken;

        public event EventHandler<StringEventArgs>? OnError;

        public Server(int port)
        {
            _port = port;
            _ipAddress = getIpAddresses();

            _tcpListeners = new ConcurrentBag<TcpListener>();
            _clients = new ConcurrentBag<ClientModel>();
            _rooms = new ConcurrentDictionary<string, ServerRoom>();
            _cancelToken = new CancellationTokenSource();
        }

        #region Public properties
        public int Port { get { return _port; } }
        public IPAddress[] IPAddresses { get {  return _ipAddress;} }
        #endregion

        #region Public methods
        public void Host()
        {
            if (_hosting)
                throw new Exception("Already hosting");

            _tcpListeners.Clear();

            _cancelToken = new CancellationTokenSource();
            foreach (var ip in getIpAddresses())
            {
                var localIp = ip;
                var thread = new Thread(() => runTcpListener(localIp));
                thread.Start();
            }
            _hosting = true;
        }

        public async void Stop()
        {
            if (!_hosting)
                throw new Exception("Not hosting");

            _hosting = false;

            await sendShutdownToAll();
            foreach (var tcp in _tcpListeners)
            {
                tcp.Stop();
            }
            _cancelToken.Cancel();
        }

        #endregion

        #region Private methods
        private static IPAddress[] getIpAddresses()
        {
            var addresses = Dns.GetHostAddresses(Dns.GetHostName());
            var local = IPAddress.Parse("127.0.0.1");
            if (!addresses.Any(a => a.Equals(local)))
            {
                return addresses.Concat(new IPAddress[] {local}).ToArray();
            }
            return addresses;
        }

        private async void runTcpListener(IPAddress ip)
        {
            try
            {
                var tcp = new TcpListener(ip, _port);
                tcp.Start();
                _tcpListeners.Add(tcp);
                while (!_cancelToken.IsCancellationRequested)
                {
                    await acceptIncomingConnections(tcp);
                }
            }
            catch (Exception e)
            {
                _cancelToken.Cancel();
                OnError?.Invoke(this, new StringEventArgs(e.Message));
            }
            try
            {
                await sendShutdownToAll();
            }
            catch (Exception e)
            {
                _cancelToken.Cancel();
                OnError?.Invoke(this, new StringEventArgs(e.Message));
            }
        }

        private async Task acceptIncomingConnections(TcpListener tcp)
        {
            try
            {
                var tcpClient = await tcp.AcceptTcpClientAsync(_cancelToken.Token);
                var client = new ClientModel(tcpClient);
                _clients.Add(client);
                _ = Task.Run(() => clientTcpListenerTask(client));
            }
            catch (SocketException)
            {
                _cancelToken.Cancel();
            }
        }


        private async Task clientTcpListenerTask(ClientModel client)
        {
            try
            {
                while (!client.CancellationToken.IsCancellationRequested)
                {
                    await waitForPacketAsync(client);
                }
            } 
            catch (Exception)
            {
                await dropClient(client);
            }
        }

        private async Task waitForPacketAsync(ClientModel client)
        {
            var stream = client.TcpClient.GetStream();
            var size = client.TcpClient.ReceiveBufferSize;
            try
            {
                byte[] buffer = new byte[size];
                var bytesRead = await stream.ReadAsync(buffer.AsMemory(0, size), client.CancellationToken.Token);
                int offset = 1;
                var expectedLength = Packet.HeaderSize + PacketHelper.ReadInt(buffer, ref offset); 
                List<byte[]>? dataChunks = new List<byte[]> { buffer };
                //Read data until entire expected packet has arrived
                while (bytesRead < expectedLength)
                {
                    buffer = new byte[size];
                    bytesRead += await stream.ReadAsync(buffer.AsMemory(0, size), client.CancellationToken.Token);
                    dataChunks.Add(buffer);
                }
                await handleIncomingPacket(client, new Packet(PacketHelper.ConcatBytes(dataChunks)));
            }
            catch (Exception e)
            {
                //Stream was closed, most likely due to the server shutting down
                //but could also be because client sent malformed packet
                await dropClient(client);
                OnError?.Invoke(this, new StringEventArgs(e.Message));
            }
        }

        private async Task handleIncomingPacket(ClientModel client, Packet packet)
        {
            int offset = 0;
            //A non-registered client's first packet better be ClientRegister, otherwise kicked and socket closed
            if (!client.IsRegistered && packet.PacketType != NetConstants.PacketTypes.ClientRegister)
            {
                client.Stop();
                return;
            }
            switch(packet.PacketType)
            {   
                case NetConstants.PacketTypes.ClientRegister:
                    {
                        var registerString = PacketHelper.ReadString(packet.DataBytes, ref offset);
                        if (registerString == NetConstants.UserRegisterString && !client.IsRegistered)
                        {
                            client.IsRegistered = true;
                            var registerAccepted = PacketHelperServer.CreateUserRegisterAcceptedPacket(client.UserGuid);
                            await sendPacketToClient(registerAccepted, client);
                        }
                        break;
                    }
                case NetConstants.PacketTypes.ClientRequestRoomName:
                    {
                        var availableRoomName = generateAvailableRoomName();
                        var p = new Packet(NetConstants.PacketTypes.ServerAvailableRoomName, PacketHelper.GetStringBytes(availableRoomName));
                        await sendPacketToClient(p, client);
                        break;
                    }
                case NetConstants.PacketTypes.ClientRequestCreateRoom:
                    {
                        var roomName = PacketHelper.ReadString(packet.DataBytes, ref offset).Trim().ToUpper();
                        var adminPass = PacketHelper.ReadString(packet.DataBytes, ref offset).Trim();
                        var nick = PacketHelper.ReadString(packet.DataBytes, ref offset).Trim();
                        var team = PacketHelper.ReadInt(packet.DataBytes, ref offset);
                        string? deniedReason = null;
                        ServerRoom? room = null;
                        if (string.IsNullOrWhiteSpace(roomName))
                            deniedReason = "Invalid room name";
                        else if (_rooms.TryGetValue(roomName, out room))
                            deniedReason = "Room already exists";
                        else if (string.IsNullOrWhiteSpace(nick))
                            deniedReason = "Invalid nickname";
                        if (deniedReason != null)
                        {
                            var deniedPacket = PacketHelperServer.CreateJoinRoomDeniedPacket(deniedReason);
                            await sendPacketToClient(deniedPacket, client);
                            break;
                        }
                        _rooms[roomName] = room = new ServerRoom(roomName, adminPass, client);
                        _rooms[roomName].TimerElapsed += (o,e) => onMatchTimerElapsed(room);
                        //Leave old room
                        await leaveUserRoom(client);
                        //Join new room
                        if (room != null)
                        {
                            await joinUserRoom(client, nick, adminPass, team, room, created: true);
                        }
                        break;
                    }
                case NetConstants.PacketTypes.ClientRequestJoinRoom:
                    {
                        var roomName = PacketHelper.ReadString(packet.DataBytes, ref offset).Trim().ToUpper();
                        var adminPass = PacketHelper.ReadString(packet.DataBytes, ref offset).Trim(); 
                        var nick = PacketHelper.ReadString(packet.DataBytes, ref offset).Trim();
                        var team = PacketHelper.ReadInt(packet.DataBytes, ref offset);
                        string? deniedReason = null;
                        ServerRoom? room = null;
                        if(string.IsNullOrWhiteSpace(roomName))
                            deniedReason = "Invalid room name";
                        else if (!_rooms.TryGetValue(roomName, out room))
                            deniedReason = "Room doesn't exist";
                        else if(room != null && client.Room == room)
                            deniedReason = "Already in this room";
                        else if (string.IsNullOrWhiteSpace(nick))
                            deniedReason = "Invalid nickname";
                        else if (room != null && room.Clients.Any(c => c.Nick == nick))
                            deniedReason = "Nickname already in use";
                        else if (room != null && !string.IsNullOrWhiteSpace(adminPass) && !room.IsCorrectAdminPassword(adminPass))
                            deniedReason = "Wrong admin password";
                        if(deniedReason != null)
                        {
                            var deniedPacket = PacketHelperServer.CreateJoinRoomDeniedPacket(deniedReason);
                            await sendPacketToClient(deniedPacket, client);
                            break;
                        }
                        //Leave old room
                        await leaveUserRoom(client);
                        //Join new room
                        if (room != null)
                        {
                            await joinUserRoom(client, nick, adminPass, team, room, created: false);
                        }
                        break;
                    }
                case NetConstants.PacketTypes.ClientRequestLeaveRoom:
                    {
                        await leaveUserRoom(client);
                        break;
                    }
                case NetConstants.PacketTypes.ClientCoordinates:
                    {
                        if (client.Room != null)
                        {
                            var coords = PacketHelper.ReadMapCoordinates(packet.DataBytes, ref offset);
                            var clientInfo = client.Room.GetClient(client.UserGuid);
                            //Don't allow spectators to send coordinates
                            if (clientInfo != null && !clientInfo.IsSpectator)
                            {
                                var p = PacketHelperServer.CreateServerUserCoordinatesPacket(clientInfo.Guid, coords);
                                await sendPacketToClients(p, getApplicableClientsForCoordinates(client.Room, clientInfo));
                            }
                        }
                        break;
                    }
                case NetConstants.PacketTypes.ClientChat:
                    {
                        if(client.Room != null)
                        {
                            var text = PacketHelper.ReadString(packet.DataBytes, ref offset);
                            var clientInfo = client.Room.GetClient(client.UserGuid);
                            if (clientInfo != null) {
                                var p = PacketHelperServer.CreateServerUserChatMessagePacket(clientInfo.Guid, text);
                                await sendPacketToRoom(p, client.Room);
                            }
                        }
                        break;
                    }
                case NetConstants.PacketTypes.ClientBingoJson:
                    {
                        if (await confirm(client, admin: true, inRoom: true))
                        {
                            var json = PacketHelper.ReadString(packet.DataBytes, ref offset);
                            ServerBingoBoard? board = null;
                            try
                            {
                                client.Room.BoardGenerator = new BingoBoardGenerator(json);
                                board = client.Room.BoardGenerator.CreateBingoBoard(client.Room, 0);
                            }
                            catch (Exception e)
                            {
                                await sendAdminStatusMessage(client, $"Error reading bingo json file: {e.Message}", System.Drawing.Color.Red);
                                //Ignore errors when reading the Json
                            }
                            if (board != null && client.Room.Match.MatchStatus == MatchStatus.NotRunning) //Don't update bingo board if match is running
                            {
                                await setRoomBingoBoard(client.Room, board);
                                await sendAdminStatusMessage(client, $"Bingo json file successfully uploaded and bingo board generated!", System.Drawing.Color.Green);
                            }
                        }
                        break;
                    }
                case NetConstants.PacketTypes.ClientRandomizeBoard:
                    {
                        //Don't update bingo board if match is running
                        if (await confirm(client, admin: true, inRoom: true))
                        {
                            if (client.Room.BoardGenerator == null)
                            {
                                await sendAdminStatusMessage(client, "No bingo json set", System.Drawing.Color.Red);
                            }
                            else if (await confirm(client, gameStarted: false))
                            {
                                ServerBingoBoard? board = client.Room.BoardGenerator.CreateBingoBoard(client.Room, 0);
                                if (board != null)
                                {
                                    await setRoomBingoBoard(client.Room, board);
                                    await sendAdminStatusMessage(client, "New board generated!", System.Drawing.Color.Green);
                                }
                            }
                        }
                        break;
                    }
                case NetConstants.PacketTypes.ClientChangeMatchStatus:
                    {
                        if (await confirm(client, admin: true, inRoom: true))
                        {
                            var status = (MatchStatus)PacketHelper.ReadByte(packet.DataBytes, ref offset);
                            //Can always change status to "Not running", all others require that bingo board is generated
                            if (status > MatchStatus.NotRunning && !await confirm(client, hasBingoBoard: true))
                                break;

                            var result = await setRoomMatchStatus(client.Room, status);
                            if(!result.Item1 && result.Item2 != null)
                            {
                                await sendAdminStatusMessage(client, result.Item2, System.Drawing.Color.Red);
                            }
                        } 
                        break;
                    }
                case NetConstants.PacketTypes.ClientTryCheck:
                    {
                        if (client.Room?.Match?.Board is ServerBingoBoard board &&
                            (client.Room.Match.MatchStatus == MatchStatus.Running || client.Room.Match.MatchStatus == MatchStatus.Paused))
                        {
                            int i = PacketHelper.ReadByte(packet.DataBytes, ref offset);
                            var targetUser = PacketHelper.ReadGuid(packet.DataBytes, ref offset);
                            var user = client.Room.GetClient(client.UserGuid);
                            if (user == null) //No matching user
                                return;
                            if (targetUser == client.UserGuid || //Setting my own count
                                user.IsAdmin && user.IsSpectator) //Setting someone elses count as admin+spectator
                            {
                                var userToSet = targetUser == client.UserGuid ? user : client.Room.GetClient(targetUser);
                                if (userToSet == null)
                                    return;

                                if (board.UserClicked(i, user, userToSet))
                                {
                                    foreach (var recipient in client.Room.Clients)
                                    {
                                        var p = PacketHelperServer.CreateBoardCheckStatusPacket(userToSet.Guid, i, recipient, board);
                                        await sendPacketToClient(p, recipient.Client);
                                    }
                                }
                            }
                        }
                        break;
                    }
                case NetConstants.PacketTypes.ClientTryMark:
                    {
                        if (client.Room?.Match?.Board is ServerBingoBoard board)
                        {
                            int i = PacketHelper.ReadByte(packet.DataBytes, ref offset);
                            var user = client.Room.GetClient(client.UserGuid);
                            if (user != null && board.UserMarked(i, user))
                            {
                                foreach (var recipient in client.Room.Clients)
                                {
                                    var p = PacketHelperServer.CreateBoardMarkedStatusPacket(user.Guid, i, recipient, board);
                                    await sendPacketToClient(p, recipient.Client);
                                }
                            }
                        }
                        break;
                    }
                case NetConstants.PacketTypes.ClientSetCounter:
                    {
                        if (client.Room?.Match?.Board is ServerBingoBoard board && client.Room.Match.MatchStatus >= MatchStatus.Running)
                        {
                            int i = PacketHelper.ReadByte(packet.DataBytes, ref offset);
                            int count = PacketHelper.ReadInt(packet.DataBytes, ref offset);
                            var targetUser = PacketHelper.ReadGuid(packet.DataBytes, ref offset);
                            var user = client.Room.GetClient(client.UserGuid);
                            if (user == null) //No matching user
                                return;
                            if(targetUser == client.UserGuid || //Setting my own count
                                user.IsAdmin && user.IsSpectator) //Setting someone elses count as admin+spectator
                            {
                                var userToSet = targetUser == client.UserGuid ? user : client.Room.GetClient(targetUser);
                                if (userToSet == null || userToSet.IsSpectator)
                                    return;

                                if (board.UserChangeCount(i, userToSet, count))
                                {
                                    foreach (var recipient in client.Room.Clients)
                                    {
                                        var p = PacketHelperServer.CreateBoardCountStatusPacket(user.Guid, i, recipient, board);
                                        await sendPacketToClient(p, recipient.Client);
                                    }
                                }
                            }
                        }
                        break;
                    }
                case NetConstants.PacketTypes.ClientDisconnect:
                    {
                        await dropClient(client);
                        break;
                    }
            }
        }

        private async Task<bool> confirm(ClientModel client, bool? admin = null, bool? spectator = null, bool? inRoom = null, bool? hasBingoBoard = null, bool? gameStarted = null)
        {
            if(admin.HasValue)
            {
                if(admin.Value && !client.IsAdmin)
                {
                    await sendAdminStatusMessage(client, "You are not admin", System.Drawing.Color.Red);
                    return false;
                }
                if(!admin.Value && client.IsAdmin)
                {
                    await sendAdminStatusMessage(client, "You are admin", System.Drawing.Color.Red);
                    return false;
                }
            }
            if (spectator.HasValue)
            {
                if (spectator.Value && !client.IsSpectator)
                {
                    await sendAdminStatusMessage(client, "You are not spectator", System.Drawing.Color.Red);
                    return false;
                }
                if (!spectator.Value && client.IsSpectator)
                {
                    await sendAdminStatusMessage(client, "You are spectator", System.Drawing.Color.Red);
                    return false;
                }
            }
            if (inRoom.HasValue)
            {
                if (inRoom.Value && client.Room == null)
                {
                    await sendAdminStatusMessage(client, "You are not in a lobby", System.Drawing.Color.Red);
                    return false;
                }
                if (!inRoom.Value && client.Room != null)
                {
                    await sendAdminStatusMessage(client, "You are in a lobby", System.Drawing.Color.Red);
                    return false;
                }
            }
            if (hasBingoBoard.HasValue)
            {
                if (hasBingoBoard.Value && client.Room?.Match?.Board == null)
                {
                    await sendAdminStatusMessage(client, "No bingo board has been generated", System.Drawing.Color.Red);
                    return false;
                }
                if (!hasBingoBoard.Value && client.Room?.Match?.Board != null)
                {
                    await sendAdminStatusMessage(client, "Bingo board already generated", System.Drawing.Color.Red);
                    return false;
                }
            }
            if (gameStarted.HasValue)
            {
                var matchInProgress = client.Room?.Match?.MatchStatus > MatchStatus.NotRunning && client.Room?.Match?.MatchStatus < MatchStatus.Finished;
                if (gameStarted.Value && !matchInProgress)
                {
                    await sendAdminStatusMessage(client, "Match has not yet started", System.Drawing.Color.Red);
                    return false;
                }
                if (!gameStarted.Value && matchInProgress)
                {
                    await sendAdminStatusMessage(client, "Match has already started", System.Drawing.Color.Red);
                    return false;
                }
            }
            return true;
        }

        private IEnumerable<ClientModel> getApplicableClientsForCoordinates(ServerRoom room, ClientInRoom sender)
        {
            if (room == null)
                return Enumerable.Empty<ClientModel>();

            //Send coordinates to spectators or players on the same team
            return room.Clients.Where(c => c.Guid != sender.Guid && (c.IsSpectator || c.Team == sender.Team)).Select(cp => cp.Client);
        }

        private string generateAvailableRoomName()
        {
            var r = new Random();
            string roomName;
            do
            {
                roomName = string.Empty;
                for (int i = 0; i < 4; ++i)
                {
                    roomName += (char)(r.Next(65, 91));
                }
            } while(_rooms.ContainsKey(roomName));
            return roomName;
        }

        private async Task joinUserRoom(ClientModel client, string nick, string adminPass, int team, ServerRoom room, bool created = false)
        {
            if (client.Room != null)
                await leaveUserRoom(client);
            
            ClientInRoom clientInRoom = room.AddClient(client, nick, adminPass, team);
            
            //Only send new user to room if there are any other clients present
            if (room.NumClients > 1)
            {
                var joinPacket = PacketHelperServer.CreateUserEnteredRoomPacket(clientInRoom);
                //Send join message to all clients already in the room
                await sendPacketToRoomExcept(joinPacket, room, client.UserGuid);
            }
            var usersPacket = PacketHelperServer.CreateRoomDataPacket(room, clientInRoom);
            //Send all users currently present in the room to the new client
            await sendPacketToClient(usersPacket, client);
        }

        private async Task leaveUserRoom(ClientModel client)
        {
            if (client.Room == null)
                return;
            var room = client.Room;
            client.Room.RemoveClient(client);
            client.Room = null;
            var leftPacket = PacketHelperServer.CreateUserLeftRoomPacket(client.UserGuid);
            //Send user leaving packet to all users remaining in the room
            await sendPacketToRoom(leftPacket, room);
        }

        private async Task<(bool, string?)> setRoomMatchStatus(ServerRoom room, MatchStatus status)
        {
            string? error = null;
            var currentStatus = room.Match.MatchStatus;
            var matchLive = currentStatus >= MatchStatus.Running && room.Match.MatchMilliseconds >= 0;
            switch (status)
            {
                case MatchStatus.NotRunning:
                    {
                        if (currentStatus == MatchStatus.NotRunning || currentStatus == MatchStatus.Finished)
                        {
                            room.Match.UpdateMatchStatus(status, 0, null);
                            break;
                        }
                        error = "Stop match before resetting";
                        break;
                    }
                case MatchStatus.Starting:
                    {
                        if (currentStatus == MatchStatus.Starting)
                        {
                            error = "Match already starting";
                            break;
                        }
                        else if (currentStatus == MatchStatus.NotRunning || currentStatus == MatchStatus.Finished ||
                            currentStatus == MatchStatus.Paused && room.Match.MatchMilliseconds < 0)
                        {
                            room.Match.UpdateMatchStatus(status, -MatchStartCountdown, null); // 10 second countdown until match starts
                            break;
                        }
                        error = "Match already started";
                        break;
                    }
                case MatchStatus.Running:
                    {
                        if (currentStatus == MatchStatus.Starting)
                        {
                            room.Match.UpdateMatchStatus(status, 0, null);
                            break;
                        }
                        else if (currentStatus == MatchStatus.Paused && matchLive)
                        {
                            room.Match.UpdateMatchStatus(status, room.Match.MatchMilliseconds, null);
                            break;
                        }
                        if (currentStatus == MatchStatus.Running)
                            error = "Match is already running";
                        if (currentStatus == MatchStatus.Finished)
                            error = "Match is finished";
                        break;
                    }
                case MatchStatus.Paused:
                    {
                        if (currentStatus == MatchStatus.NotRunning || currentStatus == MatchStatus.Finished)
                        {
                            error = "Can't pause. Match is not running";
                            break;
                        }
                        else if (currentStatus == MatchStatus.Paused)
                        {
                            //If match has already started (Running) then unpause
                            if (matchLive)
                            {
                                room.Match.UpdateMatchStatus(MatchStatus.Running, room.Match.MatchMilliseconds, null);
                            }
                            //If still hasn't started, restart Starting countdown
                            else
                            {
                                room.Match.UpdateMatchStatus(MatchStatus.Starting, -MatchStartCountdown, null); // 10 second countdown until match starts
                            }
                            break;
                        }
                        else
                        {
                            room.Match.UpdateMatchStatus(MatchStatus.Paused, room.Match.MatchMilliseconds, null);
                            break;
                        }
                    }
                default:
                    {
                        room.Match.UpdateMatchStatus(status, room.Match.MatchMilliseconds, null);
                        break;
                    }
            }
            sendMatchData(room);
            return (error != null, error);
        }

        private async void sendMatchData(ServerRoom room)
        {
            //Recalculate match live status
            bool matchLive = room.Match.MatchStatus >= MatchStatus.Running && room.Match.MatchMilliseconds >= 0;
            var (adminSpectators, others) = splitClients(room, c => c.IsAdmin && c.IsSpectator);

            foreach (var k in adminSpectators)
            {
                //Admin spectators get the bingo board regardless of status
                var adminPacket = new Packet(NetConstants.PacketTypes.ServerMatchStatusChanged, room.Match.GetBytes(k));
                await sendPacketToClient(adminPacket, k.Client);
            }
            foreach (var k in others)
            {
                //All other users gets the packet without bingo board if match hasn't started
                var nonAdminsPacket = matchLive ? new Packet(NetConstants.PacketTypes.ServerMatchStatusChanged, room.Match.GetBytes(k)) : new Packet(NetConstants.PacketTypes.ServerMatchStatusChanged, room.Match.GetBytesWithoutBoard());
                await sendPacketToClient(nonAdminsPacket, k.Client);
            }
        }

        private async void onMatchTimerElapsed(ServerRoom room)
        {
            //Start game when countdown reaches 0 
            if (room.Match.MatchStatus == MatchStatus.Starting)
            {
                await setRoomMatchStatus(room, MatchStatus.Running);
            }
        }

        private async Task sendAdminStatusMessage(ClientModel client, string message, System.Drawing.Color color)
        {
            var data = PacketHelper.ConcatBytes(BitConverter.GetBytes(color.ToArgb()), PacketHelper.GetStringBytes(message));
            var p = new Packet(NetConstants.PacketTypes.ServerToAdminStatusMessage, data);
            await sendPacketToClient(p, client);
        }

        private async Task setRoomBingoBoard(ServerRoom room, ServerBingoBoard board)
        {
            room.Match.Board = board;
            //await setRoomMatchStatus(room, MatchStatus.NotRunning);
            sendMatchData(room);
        }

        private async Task sendPacketToAllClients(Packet p, bool onlyRegistered = false)
        {
            var clientsToInclude = onlyRegistered ? _clients.Where(c => c.IsRegistered) : _clients;
            await sendPacketToClients(p, clientsToInclude);
        }

        private async Task sendPacketToRoom(Packet p, ServerRoom room)
        {
            await sendPacketToClients(p, room.ClientModels);
        }

        private async Task sendPacketToRoomExcept(Packet p, ServerRoom room, Guid except)
        {
            await sendPacketToClients(p, room.ClientModels.Where(c => c.UserGuid != except));
        }

        /*
        private async Task sendPacketToRoomPredicate(Packet p, ServerRoom room, Predicate<ClientModel> pred)
        {
            await sendPacketToClients(p, room.ClientModels.Where(c => pred(c)));
        }

        private async Task sendPacketToRoomNonAdmins(Packet p, ServerRoom room)
        {
            await sendPacketToClients(p, room.ClientModels.Where(c => !c.IsAdmin));
        }

        private async Task sendPacketToAllClientsExcept(Packet p, Guid except, bool onlyRegistered = false)
        {
            var clientsToInclude = onlyRegistered ? _clients.Where(c => c.IsRegistered && c.UserGuid != except) : _clients.Where(c => c.UserGuid != except);
            await sendPacketToClients(p, clientsToInclude);
        }
        */

        private async Task sendPacketToClients(Packet p, IEnumerable<ClientModel> clients)
        {
            foreach (var client in clients)
            {
                await sendPacketToClient(p, client);
            }
        }

        private async Task sendPacketToClient(Packet p, ClientModel client)
        {
            try
            {
                var stream = client.TcpClient.GetStream();
                await stream.WriteAsync(p.Bytes.AsMemory(0, p.TotalSize));
            }
            catch
            {
                await dropClient(client);
            }
        }

        private async Task sendShutdownToAll()
        {
            var packet = new Packet(NetConstants.PacketTypes.ServerShutdown, Array.Empty<byte>());
            await sendPacketToAllClients(packet);
            foreach (var c in _clients)
            {
                c.Stop();
            }
        }

        private async Task dropClient(ClientModel client)
        {
            if (_hosting)
                await leaveUserRoom(client);
            client.Stop();
        }

        private async Task kickClient(ClientModel client)
        {
            var packet = new Packet(NetConstants.PacketTypes.ServerUserConnectionClosed, Array.Empty<byte>());
            await sendPacketToClient(packet, client);
            await dropClient(client);
        }

        private (IList<ClientInRoom>, IList<ClientInRoom>) splitClients(ServerRoom room, Predicate<ClientInRoom> pred)
        {
            var truelist = new List<ClientInRoom>();
            var falselist = new List<ClientInRoom>();
            foreach(var c in room.Clients)
            {
                (pred(c) ? truelist : falselist).Add(c);
            }
            return (truelist, falselist);
        }

        #endregion

    }
}
