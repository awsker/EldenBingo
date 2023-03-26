using EldenBingo.Net;
using EldenBingo.Net.DataContainers;
using EldenBingoCommon;
using System.Net;
using System.Net.Sockets;

namespace EldenBingo
{
    public class Client
    {
        private TcpClient? _tcp;
        private CancellationTokenSource _cancelToken;
        private Room? _room;

        internal Room? Room
        {
            get
            {
                return _room;
            }
            set
            {
                if (_room != value)
                {
                    var oldRoom = _room;
                    _room = value;
                    if (_room == null)
                        LocalUser = null;
                    onRoomChanged(oldRoom);
                }
            }
        }

        public event EventHandler? UsersChanged;
        public event EventHandler<ObjectEventArgs>? IncomingData;
        public event EventHandler? Connected;
        internal event EventHandler<RoomChangedEventArgs>? RoomChanged;
        public event EventHandler<StringEventArgs>? Disconnected;
        public event EventHandler<StatusEventArgs>? StatusChanged;

        public Guid ClientGuid { get; private set; }
        public UserInRoom? LocalUser { get; private set; }

        private static readonly Color IdleColor = Color.Blue;
        private static readonly Color WorkingColor = Color.Orange;
        private static readonly Color ErrorColor = Color.Red;
        private static readonly Color SuccessColor = Color.Green;

        public Client()
        {
            _cancelToken = new CancellationTokenSource();
        }

        public string GetConnectionStatusString()
        {
            if (!IsConnected)
                return "Not connected";
            if (_cancelToken.IsCancellationRequested)
                return "Stopping...";
            if (Room == null)
                return "Connected - Not in a lobby";
            else
                return "Connected - Lobby: " + Room.Name;
        }

        public static IPEndPoint? EndPointFromAddress(string address, int port, out string error)
        {
            error = string.Empty;
            if (port < 1 || port > 65535)
            {
                error = "Invalid port";
                return null;
            }
            if (IPAddress.TryParse(address, out var ipAddress))
            {
                var endpoint = new IPEndPoint(ipAddress, port);
                return endpoint;
            }
            else
            {
                try
                {
                    IPAddress[] addresses = Dns.GetHostAddresses(address);
                    foreach(var ip in addresses)
                    {
                        if(ip.ToString() == "::1")
                            continue;
                        var endpoint = new IPEndPoint(ip, port);
                        return endpoint;
                    }
                    error = $"Unable to resolve hostname {address}";
                    return null;

                } 
                catch(Exception e)
                {
                    error = $"Unable to resolve hostname {address}: {e.Message}";
                }
            }
            error = "Unable to parse address";
            return null;
        }

        #region Public properties
        public bool IsConnected => _tcp?.Connected == true;
        #endregion

        #region Public methods
        public async Task<bool> Connect(IPEndPoint ipEndpoint)
        {
            if (_tcp != null && _tcp.Connected)
            {
                onStatus("Already connected", ErrorColor);
                return false;
            }
            _cancelToken = new CancellationTokenSource();
            _tcp = new TcpClient(ipEndpoint.AddressFamily);
            try
            {
                onStatus($"Connecting to {ipEndpoint}...", WorkingColor);
                await _tcp.ConnectAsync(ipEndpoint, _cancelToken.Token);

                if (_tcp.Connected)
                {
                    onStatus($"Connected to server", SuccessColor);
                    _ = Task.Run(run);
                }
                else
                {
                    onStatus($"Could not connect to {ipEndpoint.Address}:{ipEndpoint.Port}", ErrorColor);
                    _cancelToken.Cancel();
                }
            }
            catch (Exception e)
            {
                onStatus($"Connect Error: {e.Message}", ErrorColor);
            }
            return _tcp.Connected;
        }

        public async Task Disconnect()
        {
            await SendPacketToServer(new Packet(NetConstants.PacketTypes.ClientDisconnect, Array.Empty<byte>()));
            _cancelToken.Cancel();
        }

        public async Task CreateRoom(string roomName, string adminPass, string nickname, int color, Team team)
        {
            bool spectator = team == Team.Spectator;
            int teamI = team == Team.Spectator ? 0 : (int)team;

            var p = PacketHelper.CreateCreateRoomPacket(roomName, adminPass, nickname, color, teamI, spectator);
            await SendPacketToServer(p);
        }

        public async Task JoinRoom(string roomName, string adminPass, string nickname, int color, Team team)
        {
            bool spectator = team == Team.Spectator;
            int teamI = team == Team.Spectator ? 0 : (int)team;

            var p = PacketHelper.CreateJoinRoomPacket(roomName, adminPass, nickname, color, teamI, spectator);
            await SendPacketToServer(p);
        }

        public async Task LeaveRoom()
        {
            Room = null;
            onStatus($"Left lobby", SuccessColor);
            var p = new Packet(NetConstants.PacketTypes.ClientRequestLeaveRoom, Array.Empty<byte>());
            await SendPacketToServer(p);
        }

        public async Task SendPacketToServer(Packet p)
        {
            if (_cancelToken.IsCancellationRequested || _tcp == null || !_tcp.Connected)
            {
                onStatus($"Error sending message to server: Not connected", ErrorColor);
                return;
            }
            try
            {
                var stream = _tcp.GetStream();
                await stream.WriteAsync(p.Bytes, 0, p.TotalSize, _cancelToken.Token);
            }
            catch (OperationCanceledException)
            { }
            catch (Exception e)
            {
                _cancelToken.Cancel();
                onStatus($"Error sending message to server: {e.Message}", ErrorColor);
            }

        }
        #endregion

        #region Event helpers

        private void onConnected()
        {
            Connected?.Invoke(this, EventArgs.Empty);
        }

        private void onDisconnected(string message)
        {
            Disconnected?.Invoke(this, new StringEventArgs(message));
        }

        private void onIncomingData(NetConstants.PacketTypes packetType, object o)
        {
            IncomingData?.Invoke(this, new ObjectEventArgs(packetType, o));
        }

        private void onUsersChanged()
        {
            UsersChanged?.Invoke(this, EventArgs.Empty);
        }

        private void onRoomChanged(Room? oldRoom)
        {
            RoomChanged?.Invoke(this, new RoomChangedEventArgs(oldRoom, Room));
        }

        private void onStatus(string message, Color color)
        {
            StatusChanged?.Invoke(this, new StatusEventArgs(message, color));
        }

        #endregion

        #region Private methods
        private async Task run()
        {
            try
            {
                var registerPacket = PacketHelper.CreateUserRegistrationPacket();
                await SendPacketToServer(registerPacket);
                while (!_cancelToken.IsCancellationRequested)
                {
                    await waitForPacketAsync();
                }
            }
            catch (Exception e)
            {
                _cancelToken.Cancel();
                onStatus(e.Message, ErrorColor);
            }
            onDisconnected("Disconnected");
            onStatus("Disconnected", IdleColor);
            onComplete();
        }

        private async Task waitForPacketAsync()
        {
            var stream = _tcp.GetStream();
            var size = _tcp.ReceiveBufferSize;
            try
            {
                byte[] buffer = new byte[size];
                await stream.ReadAsync(buffer.AsMemory(0, size), _cancelToken.Token);
                await handleIncomingPacket(new Packet(buffer));
            }
            catch (IOException e)
            {
                _cancelToken.Cancel();
                onStatus(e.Message, ErrorColor);
            }
            catch (OperationCanceledException)
            { }
            catch (Exception e)
            {
                //Allow work to continue for other exceptions
                onStatus(e.Message, ErrorColor);
            }
        }

        private async Task handleIncomingPacket(Packet packet)
        {
            int offset = 0;
            switch (packet.PacketType)
            {
                case NetConstants.PacketTypes.ServerRegisterAccepted:
                    {
                        if (packet.DataSize == 0)
                            return;
                        var text = PacketHelper.ReadString(packet.DataBytes, ref offset);
                        if (text == NetConstants.ServerRegisterString)
                        {
                            var guid = PacketHelper.ReadGuid(packet.DataBytes, ref offset);
                            ClientGuid = guid;
                            onConnected();
                        }
                        else
                        {
                            onDisconnected("Invalid server response");
                            await Disconnect();
                        }
                        break;
                    }
                case NetConstants.PacketTypes.ServerAvailableRoomName:
                    {
                        var roomname = PacketHelper.ReadString(packet.DataBytes, ref offset);
                        onIncomingData(packet.PacketType, new AvailableRoomNameData(roomname));
                        break;
                    }
                case NetConstants.PacketTypes.ServerUserJoinedRoom:
                    {
                        if (Room != null)
                        {
                            var user = PacketHelper.ReadUserInRoom(packet.DataBytes, ref offset);
                            Room.AddClient(user);
                            onIncomingData(packet.PacketType, new UserJoinedLeftRoomData(Room, user, true));
                            onUsersChanged();
                        }
                        break;
                    }
                case NetConstants.PacketTypes.ServerUserLeftRoom:
                    {
                        if (Room != null)
                        {
                            var guid = PacketHelper.ReadGuid(packet.DataBytes, ref offset);
                            var user = Room.RemoveClient(guid);
                            if (user != null)
                            {
                                onIncomingData(packet.PacketType, new UserJoinedLeftRoomData(Room, user, false));
                                onUsersChanged();
                            }
                        }
                        break;
                    }
                case NetConstants.PacketTypes.ServerJoinRoomDenied:
                    {
                        var message = PacketHelper.ReadString(packet.DataBytes, ref offset);
                        onStatus($"Join lobby failed: {message}", ErrorColor);
                        Room = null;
                        onIncomingData(packet.PacketType, new JoinRoomDeniedData(message));
                        break;
                    }
                case NetConstants.PacketTypes.ServerJoinAcceptedRoomData:
                    {
                        //Joining room accepted
                        var incomingRoom = new Room(packet.DataBytes, ref offset);
                        var sameRoomAsBefore = Room != null && Room.Name == incomingRoom.Name;
                       
                        //Store a reference to my own User
                        LocalUser = incomingRoom.GetClient(ClientGuid);
                        
                        if (!sameRoomAsBefore)
                            onStatus($"Joined lobby '{incomingRoom.Name}'", SuccessColor);

                        Room = incomingRoom;
                        onIncomingData(packet.PacketType, new JoinedRoomData(Room));
                        onUsersChanged();
                        break;
                    }
                case NetConstants.PacketTypes.ServerUserCoordinates:
                    {
                        if (Room != null)
                        {
                            var userGuid = PacketHelper.ReadGuid(packet.DataBytes, ref offset);
                            var user = Room.GetClient(userGuid);
                            if (user != null)
                            {
                                var coords = PacketHelper.ReadMapCoordinates(packet.DataBytes, ref offset);
                                onIncomingData(packet.PacketType, new CoordinateData(user, coords));
                            }
                        }
                        break;
                    }
                case NetConstants.PacketTypes.ServerToAdminStatusMessage:
                    {
                        var color = PacketHelper.ReadInt(packet.DataBytes, ref offset);
                        var message = PacketHelper.ReadString(packet.DataBytes, ref offset);
                        onIncomingData(packet.PacketType, new AdminStatusMessageData(color, message));
                        break;
                    }
                case NetConstants.PacketTypes.ServerUserChat:
                    {
                        if (Room != null)
                        {
                            var userGuid = PacketHelper.ReadGuid(packet.DataBytes, ref offset);
                            var user = Room.GetClient(userGuid);
                            if (user != null)
                            {
                                var message = PacketHelper.ReadString(packet.DataBytes, ref offset);
                                onIncomingData(packet.PacketType, new ChatData(user, message));
                            }
                        }
                        break;
                    }
                case NetConstants.PacketTypes.ServerMatchStatusChanged:
                    {
                        if (Room != null)
                        {
                            var match = new Match(packet.DataBytes, ref offset);
                            Room.Match.UpdateMatchStatus(match.MatchStatus, match.ServerTimer, match.Board);
                            //Reset the board if no board received when status was changed to "Not running"
                            if (match.Board == null && match.MatchStatus == MatchStatus.NotRunning) 
                                Room.Match.Board = null;
                            onIncomingData(packet.PacketType, new MatchStatusData(Room.Match));
                        }
                        break;
                    }
                case NetConstants.PacketTypes.ServerBingoBoardCheckChanged:
                    {
                        if (Room?.Match.Board != null)
                        {
                            var userGuid = PacketHelper.ReadGuid(packet.DataBytes, ref offset);
                            var user = Room.GetClient(userGuid);
                            int indexChanged = PacketHelper.ReadByte(packet.DataBytes, ref offset);
                            for(int i = 0; i < 25; ++i)
                            {
                                Room.Match.Board.Squares[i].Color = Color.FromArgb(PacketHelper.ReadInt(packet.DataBytes, ref offset));
                                Room.Match.Board.Squares[i].Marked = PacketHelper.ReadBoolean(packet.DataBytes, ref offset);
                            }
                            onIncomingData(packet.PacketType, new CheckChangedData(Room, user, indexChanged));
                        }
                        break;
                    }
                case NetConstants.PacketTypes.ServerUserConnectionClosed:
                    {
                        _cancelToken.Cancel();
                        onDisconnected("Kicked from server");
                        break;
                    }
                case NetConstants.PacketTypes.ServerShutdown:
                    {
                        _cancelToken.Cancel();
                        onDisconnected("Server shutting down");
                        break;
                    }
            }
        }

        private void onComplete()
        {
            _tcp = null;
            Room = null;
        }
        #endregion

    }

}
