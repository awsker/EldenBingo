using EldenBingoCommon;
using Neto.Server;
using Neto.Shared;
using System.Collections.Concurrent;
using System.Drawing;
using System.Reflection;

namespace EldenBingoServer
{
    public class Server : NetoServer<BingoClientModel>
    {
        //10 seconds countdown before match starts
        private const int MatchStartCountdown = 999;

        private readonly ConcurrentDictionary<string, ServerRoom> _rooms;

        public Server(int port) : base(port)
        {
            _rooms = new ConcurrentDictionary<string, ServerRoom>();
            RegisterAssembly(Assembly.GetAssembly(typeof(BingoBoard)));
            registerHandlers();
        }

        protected override async Task DropClient(BingoClientModel client)
        {
            if (Hosting && client.Room != null)
                await leaveUserRoom(client);
            client.Stop();
        }

        private async Task<bool> confirm(BingoClientModel client, bool? admin = null, bool? spectator = null, bool? inRoom = null, bool? hasBingoBoard = null, bool? gameStarted = null)
        {
            if (admin.HasValue)
            {
                if (admin.Value && !client.IsAdmin)
                {
                    await sendAdminStatusMessage(client, "You are not admin", System.Drawing.Color.Red);
                    return false;
                }
                if (!admin.Value && client.IsAdmin)
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
            } while (_rooms.ContainsKey(roomName));
            return roomName;
        }

        private IEnumerable<BingoClientModel> getApplicableClientsForCoordinates(ServerRoom room, BingoClientInRoom sender)
        {
            if (room == null)
                return Enumerable.Empty<BingoClientModel>();

            //Send coordinates to spectators or players on the same team
            return room.Users.Where(c => c.Guid != sender.Guid && (c.IsSpectator || c.Team == sender.Team)).Select(cp => cp.Client);
        }

        private void registerHandlers()
        {
            AddListener<ClientRequestRoomName>(roomNameRequested);
            AddListener<ClientRequestCreateRoom>(createRoomRequested);
            AddListener<ClientRequestJoinRoom>(joinRoomRequested);
            AddListener<ClientRequestLeaveRoom>(leaveRoomRequested);
            AddListener<ClientCoordinates>(clientCoordinates);
            AddListener<ClientChat>(clientChat);
            AddListener<ClientBingoJson>(clientBingoJson);
            AddListener<ClientRandomizeBoard>(clientRandomizeBoard);
            AddListener<ClientChangeMatchStatus>(clientChangeMatchStatus);
            AddListener<ClientTryCheck>(clientTryCheck);
            AddListener<ClientTryMark>(clientTryMark);
            AddListener<ClientTrySetCounter>(clientTrySetCounter);
            AddListener<ClientRequestCurrentGameSettings>(clientRequestGameSettings);
            AddListener<ClientSetGameSettings>(clientSetGameSettings);
        }

        private async void roomNameRequested(BingoClientModel? sender, ClientRequestRoomName request)
        {
            if (sender == null)
                return;
            var availableRoomName = generateAvailableRoomName();
            var roomNamePacket = new ServerRoomNameSuggestion(availableRoomName);
            await SendPacketToClient(new Packet(roomNamePacket), sender);
        }

        private async void createRoomRequested(BingoClientModel? sender, ClientRequestCreateRoom request)
        {
            if (sender == null)
                return;
            string? deniedReason = null;
            if (string.IsNullOrWhiteSpace(request.RoomName))
                deniedReason = "Invalid room name";
            else if (_rooms.TryGetValue(request.RoomName, out _))
                deniedReason = "Room already exists";
            else if (string.IsNullOrWhiteSpace(request.Nick))
                deniedReason = "Invalid nickname";
            if (deniedReason != null)
            {
                var deniedPacket = new ServerJoinRoomDenied(deniedReason);
                await SendPacketToClient(new Packet(deniedPacket), sender);
                return;
            }
            ServerRoom? room = createRoom(request.RoomName, request.AdminPass, sender, request.Settings);
            //Leave old room
            await leaveUserRoom(sender);
            //Join new room
            if (room != null)
            {
                await joinUserRoom(sender, request.Nick, request.AdminPass, request.Team, room, created: true);
            }
        }

        private ServerRoom createRoom(string roomName, string adminPass, ClientModel creator, BingoGameSettings settings)
        {
            if (_rooms.TryGetValue(roomName, out _))
            {
                throw new ApplicationException("Room already exists");
            }
            var room = new ServerRoom(roomName, adminPass, creator, settings);
            room.TimerElapsed += (o, e) => onMatchTimerElapsed(room);
            _rooms[roomName] = room;
            return room;
        }

        private async void joinRoomRequested(BingoClientModel? sender, ClientRequestJoinRoom request)
        {
            if (sender == null)
                return;

            string? deniedReason = null;
            ServerRoom? room = null;
            if (string.IsNullOrWhiteSpace(request.RoomName))
                deniedReason = "Invalid room name";
            else if (!_rooms.TryGetValue(request.RoomName, out room))
                deniedReason = "Room doesn't exist";
            else if (room != null && sender.Room == room)
                deniedReason = "Already in this room";
            else if (string.IsNullOrWhiteSpace(request.Nick))
                deniedReason = "Invalid nickname";
            else if (room != null && room.Users.Any(c => c.Nick == request.Nick))
                deniedReason = "Nickname already in use";
            else if (room != null && !string.IsNullOrWhiteSpace(request.AdminPass) && !room.IsCorrectAdminPassword(request.AdminPass))
                deniedReason = "Wrong admin password";
            if (deniedReason != null)
            {
                var deniedPacket = new ServerJoinRoomDenied(deniedReason);
                await SendPacketToClient(new Packet(deniedPacket), sender);
                return;
            }
            //Leave old room
            await leaveUserRoom(sender);
            //Join new room
            if (room != null)
            {
                await joinUserRoom(sender, request.Nick, request.AdminPass, request.Team, room, created: true);
            }
        }

        private async void leaveRoomRequested(BingoClientModel? sender, ClientRequestLeaveRoom request)
        {
            if (sender == null)
                return;

            await leaveUserRoom(sender);
        }

        private async void clientCoordinates(BingoClientModel? sender, ClientCoordinates coordinates)
        {
            if (sender?.Room == null)
                return;

            var userInfo = sender.Room.GetUser(sender.ClientGuid);
            //Don't allow spectators to send coordinates
            if (userInfo != null && !userInfo.IsSpectator)
            {
                var packet = new ServerUserCoordinates(sender.ClientGuid, coordinates.X, coordinates.Y, coordinates.Angle, coordinates.IsUnderground);
                await SendPacketToClients(new Packet(packet), getApplicableClientsForCoordinates(sender.Room, userInfo));
            }
        }

        private async void clientChat(BingoClientModel? sender, ClientChat chatMessage)
        {
            if (sender?.Room == null)
                return;

            var userInfo = sender.Room.GetUser(sender.ClientGuid);
            if (userInfo != null)
            {
                var packet = new ServerUserChat(sender.ClientGuid, chatMessage.Message);
                await sendPacketToRoom(new Packet(packet), sender.Room);
            }
        }

        private async void clientBingoJson(BingoClientModel? sender, ClientBingoJson bingoJson)
        {
            if (sender?.Room == null)
                return;

            if (!await confirm(sender, admin: true, inRoom: true))
                return;

            ServerBingoBoard? board = null;
            try
            {
                var bg = new BingoBoardGenerator(bingoJson.Json, sender.Room.GameSettings.RandomSeed);
                bg.CategoryLimit = sender.Room.GameSettings.CategoryLimit;
                sender.Room.BoardGenerator = bg;
                //Don't update bingo board if match is running
                if (sender.Room.Match.MatchStatus == MatchStatus.NotRunning || sender.Room.Match.MatchStatus == MatchStatus.Finished)
                {
                    board = sender.Room.BoardGenerator.CreateBingoBoard(sender.Room);
                    if (board == null)
                    {
                        await sendAdminStatusMessage(sender, $"Error generating bingo board: No valid board possible", System.Drawing.Color.Red);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                await sendAdminStatusMessage(sender, $"Error reading bingo json file: {e.Message}", System.Drawing.Color.Red);
                return;
            }
            
            if (board != null)
            {
                randomizeClassesIfNeeded(sender.Room);
                await setRoomBingoBoard(sender.Room, board);
                await sendAdminStatusMessage(sender, $"Bingo json file successfully uploaded and bingo board generated!", System.Drawing.Color.Green);
            }
            else
            {
                await sendAdminStatusMessage(sender, $"Bingo json file successfully uploaded!", System.Drawing.Color.Green);
            }

        }

        private void randomizeClassesIfNeeded(ServerRoom room)
        {
            EldenRingClasses[] availableClasses = room.GameSettings.RandomClasses && room.BoardGenerator != null ?
               room.BoardGenerator.RandomizeAvailableClasses(room.GameSettings.ValidClasses, room.GameSettings.NumberOfClasses) :
               Array.Empty<EldenRingClasses>();
            room.Match.Classes = availableClasses;
        }

        private async void clientRandomizeBoard(BingoClientModel? sender, ClientRandomizeBoard randomizeBoard)
        {
            if (sender?.Room == null)
                return;

            if (!await confirm(sender, admin: true, inRoom: true))
                return;

            if (sender.Room.BoardGenerator == null)
            {
                await sendAdminStatusMessage(sender, "No bingo json set", System.Drawing.Color.Red);
            }
            else if (await confirm(sender, gameStarted: false))
            {
                ServerBingoBoard? board = sender.Room.BoardGenerator.CreateBingoBoard(sender.Room);
                if (board != null)
                {
                    randomizeClassesIfNeeded(sender.Room);
                    await setRoomBingoBoard(sender.Room, board);
                    await sendAdminStatusMessage(sender, "New board generated!", System.Drawing.Color.Green);
                }
            }
        }

        private async void clientChangeMatchStatus(BingoClientModel? sender, ClientChangeMatchStatus matchStatus)
        {
            if (sender?.Room == null)
                return;

            if (!await confirm(sender, admin: true, inRoom: true))
                return;

            //Can always change status to "Not running", all others require that bingo board is generated
            if (matchStatus.MatchStatus > MatchStatus.NotRunning && !await confirm(sender, hasBingoBoard: true))
                return;

            var result = await setRoomMatchStatus(sender.Room, matchStatus.MatchStatus);
            //If error occured when setting room match status, send response to the requester
            if (!result.Success && result.ErrorMessage != null)
            {
                await sendAdminStatusMessage(sender, result.ErrorMessage, System.Drawing.Color.Red);
            }
            if (result.Success && matchStatus.MatchStatus == MatchStatus.Starting)
            {
                var p = new Packet(new ServerEntireBingoBoardUpdate(Array.Empty<BingoBoardSquare>()));
                //Reset the board for all players
                await SendPacketToClients(p, sender.Room.ClientModels.Where(c => !(c.IsAdmin && c.IsSpectator)));
            }
        }

        private async void clientTryCheck(BingoClientModel? sender, ClientTryCheck tryCheck)
        {
            if (sender?.Room == null)
                return;

            if (sender.Room.Match?.Board is ServerBingoBoard board && sender.Room.Match.MatchStatus >= MatchStatus.Running)
            {
                var targetUser = tryCheck.ForUser;
                var userInfo = sender.Room.GetUser(sender.ClientGuid);
                if (userInfo == null)
                    return;

                if (targetUser == sender.ClientGuid || //Setting my own count
                    userInfo.IsAdmin && userInfo.IsSpectator) //Setting someone elses count as admin+spectator
                {
                    var userToSet = targetUser == sender.ClientGuid ? userInfo : sender.Room.GetUser(targetUser);
                    if (userToSet == null)
                        return;

                    if (board.UserClicked(tryCheck.Index, userInfo, userToSet))
                    {
                        var userCheck = new ServerUserChecked(userInfo.Guid, tryCheck.Index, board.CheckStatus[tryCheck.Index].CheckedBy);
                        await sendPacketToRoom(new Packet(userCheck), sender.Room);
                    }
                }
            }
        }

        private async void clientTryMark(BingoClientModel? sender, ClientTryMark tryMark)
        {
            if (sender?.Room == null)
                return;

            if (sender.Room.Match?.Board is ServerBingoBoard board)
            {
                var userInfo = sender.Room.GetUser(sender.ClientGuid);
                if (userInfo == null)
                    return;

                if (board.UserMarked(tryMark.Index, userInfo))
                {
                    //Send marking to all players on the same team
                    var userMarked = new ServerUserMarked(userInfo.Guid, tryMark.Index, board.CheckStatus[tryMark.Index].IsMarked(userInfo.Team));
                    await SendPacketToClients(new Packet(userMarked), sender.Room.Users.Where(u => u.Team == userInfo.Team).Select(c => c.Client));
                }
            }
        }

        private async void clientTrySetCounter(BingoClientModel? sender, ClientTrySetCounter trySetCounter)
        {
            if (sender?.Room == null)
                return;

            if (sender.Room.Match?.Board is ServerBingoBoard board && sender.Room.Match.MatchStatus >= MatchStatus.Running)
            {
                var targetUser = trySetCounter.ForUser;
                var userInfo = sender.Room.GetUser(sender.ClientGuid);
                if (userInfo == null)
                    return;

                if (targetUser == sender.ClientGuid || //Setting my own count
                    userInfo.IsAdmin && userInfo.IsSpectator) //Setting someone elses count as admin+spectator
                {
                    var userToSet = targetUser == sender.ClientGuid ? userInfo : sender.Room.GetUser(targetUser);
                    if (userToSet == null)
                        return;

                    if (board.UserChangeCount(trySetCounter.Index, userToSet, trySetCounter.Change))
                    {
                        var tasks = new List<Task>();
                        foreach (var recipient in sender.Room.Users.Where(u => u.IsSpectator || u.Team == userToSet.Team))
                        {
                            var userCounter = new ServerUserSetCounter(userInfo.Guid, trySetCounter.Index, board.CheckStatus[trySetCounter.Index].GetCounters(recipient, sender.Room.Users));
                            var task = SendPacketToClient(new Packet(userCounter), recipient.Client);
                            tasks.Add(task);
                        }
                        await Task.WhenAll(tasks);
                    }
                }
            }
        }

        private async void clientRequestGameSettings(BingoClientModel? sender, ClientRequestCurrentGameSettings gameSettingsRequest)
        {
            if (sender == null)
                return;
            if (!await confirm(sender, admin: true, inRoom: true))
                return;

            var packet = new ServerCurrentGameSettings(sender.Room.GameSettings);
            await SendPacketToClient(new Packet(packet), sender);
        }

        private async void clientSetGameSettings(BingoClientModel? sender, ClientSetGameSettings gameSettingsRequest)
        {
            if (sender == null)
                return;
            if (!await confirm(sender, admin: true, inRoom: true))
                return;

            sender.Room.GameSettings = gameSettingsRequest.GameSettings;
            if(sender.Room.BoardGenerator != null)
            {
                //Update the current board generator with the new random seed
                sender.Room.BoardGenerator.RandomSeed = gameSettingsRequest.GameSettings.RandomSeed;
                sender.Room.BoardGenerator.CategoryLimit = gameSettingsRequest.GameSettings.CategoryLimit;
            }
            await sendAdminStatusMessage(sender, "New lobby settings set", System.Drawing.Color.Green);
        }

        private ServerEntireBingoBoardUpdate createEntireBoardPacket(ServerBingoBoard? board, UserInRoom user)
        {
            if(board == null)
                return new ServerEntireBingoBoardUpdate(Array.Empty<BingoBoardSquare>());
            var squareData = board.GetSquareDataForUser(user);
            var boardCopy = new BingoBoard(board.Squares);
            for (int i = 0; i < 25; ++i)
                boardCopy.Squares[i] = squareData[i];
            return new ServerEntireBingoBoardUpdate(boardCopy.Squares);
        }

        /*
        private ServerBingoBoardStatusUpdate createBoardStatusPacket(ServerBingoBoard board, UserInRoom user)
        {
            var squareData = board.GetSquareDataForUser(user);
            return new ServerBingoBoardStatusUpdate(squareData);
        }*/

        private async Task joinUserRoom(BingoClientModel client, string nick, string adminPass, int team, ServerRoom room, bool created = false)
        {
            if (client.Room != null)
                await leaveUserRoom(client);

            BingoClientInRoom clientInRoom = room.AddUser(client, nick, adminPass, team);

            //Only send new user to room if there are any other clients present
            if (room.NumUsers > 1)
            {
                //Send the user as a UserInRoom (we don't want to a BingoClientInRoom since this type is unrecognized by the client)
                var user = new UserInRoom(clientInRoom);
                var joinPacket = new ServerUserJoinedRoom(user);
                //Send join message to all clients already in the room
                await sendPacketToRoomExcept(new Packet(joinPacket), room, client.ClientGuid);
            }

            //Construct a list of all users as UserInRoom and send these (we don't want to send the users as BingoClientInRoom since this type is unrecognized by the client)
            var currentUsers = new List<UserInRoom>();
            foreach(var user in room.Users)
            {
                currentUsers.Add(new UserInRoom(user));
            }
            var joinAccepted = new ServerJoinRoomAccepted(room.Name, currentUsers.ToArray(), room.Match.MatchStatus, room.Match.MatchMilliseconds);
            var packet = new Packet(joinAccepted);
            if (room.Match.Board is ServerBingoBoard board)
            {
                //Also send the bingo board if user should have it
                bool matchLive = room.Match.MatchStatus >= MatchStatus.Running && room.Match.MatchMilliseconds >= 0;
                if(matchLive || clientInRoom.IsAdmin && clientInRoom.IsSpectator)
                {
                    packet.AddObject(createEntireBoardPacket(board, clientInRoom));
                }
                if(matchLive && room.Match.Classes.Length > 0)
                {
                    var availableClassesPacket = new ServerAvailableClasses(room.Match.Classes);
                    packet.AddObject(availableClassesPacket);
                }
            }
            //Send all users currently present in the room to the new client
            await SendPacketToClient(packet, client);
        }

        private async Task leaveUserRoom(BingoClientModel client)
        {
            if (client.Room == null)
                return;
            
            var room = client.Room;
            var user = client.Room.RemoveUser(client);
            client.Room = null;

            if (user != null)
            {
                var leftPacket = new ServerUserLeftRoom(new UserInRoom(user));
                //Send user leaving packet to all users remaining in the room
                await sendPacketToRoom(new Packet(leftPacket), room);
            }
        }

        private async void onMatchTimerElapsed(ServerRoom room)
        {
            //Start game when countdown reaches 0
            if (room.Match.MatchStatus == MatchStatus.Starting)
            {
                await startMatch(room);
            }
        }

        private async Task startMatch(ServerRoom room)
        {
            await setRoomMatchStatus(room, MatchStatus.Running);
            await sendBoardAndClasses(room);
        }

        private async Task sendAdminStatusMessage(BingoClientModel client, string message, Color color)
        {
            await SendPacketToClient(new Packet(new ServerAdminStatusMessage(message, color.ToArgb())), client);
        }

        private async Task sendMatchStatus(ServerRoom room)
        {
            var matchStatus = new ServerMatchStatusUpdate(room.Match.MatchStatus, room.Match.MatchMilliseconds);
            await sendPacketToRoom(new Packet(matchStatus), room);
        }

        private async Task sendBoardAndClasses(ServerRoom room)
        {
            //Recalculate match live status
            bool matchLive = room.Match.MatchStatus >= MatchStatus.Running && room.Match.MatchMilliseconds >= 0;
            var (adminSpectators, others) = splitClients(room, c => c.IsAdmin && c.IsSpectator);

            if (room.Match?.Board == null || room.Match?.Board is not ServerBingoBoard board)
            {
                //No board set, so we send an empty board
                await sendPacketToRoom(new Packet(new ServerEntireBingoBoardUpdate(Array.Empty<BingoBoardSquare>())), room);
                return;
            }

            //Board is set
            foreach (var k in adminSpectators)
            {
                //Admin spectators get the bingo board regardless of status
                var adminPacket = new Packet(createEntireBoardPacket(board, k));
                if (room.Match.Classes.Length > 0 && room.Match.MatchMilliseconds < 10000) //Include the classes for the first 10 seconds of the game
                {
                    adminPacket.AddObject(new ServerAvailableClasses(room.Match.Classes));
                }
                await SendPacketToClient(adminPacket, k.Client);
            }
            foreach (var k in others)
            {
                //All other users gets the packet without bingo board if match hasn't started
                var nonAdminsPacket = new Packet(createEntireBoardPacket(matchLive ? board : null, k));
                if (room.Match.MatchStatus == MatchStatus.Running && room.Match.Classes.Length > 0 && room.Match.MatchMilliseconds < 10000)
                {
                    nonAdminsPacket.AddObject(new ServerAvailableClasses(room.Match.Classes));
                }
                await SendPacketToClient(nonAdminsPacket, k.Client);
            }
        }

        private async Task sendPacketToRoom(Packet p, ServerRoom room)
        {
            await SendPacketToClients(p, room.ClientModels);
        }

        private async Task sendPacketToRoomExcept(Packet p, ServerRoom room, Guid except)
        {
            await SendPacketToClients(p, room.ClientModels.Where(c => c.ClientGuid != except));
        }

        private async Task setRoomBingoBoard(ServerRoom room, ServerBingoBoard board)
        {
            room.Match.Board = board;
            await setRoomMatchStatus(room, MatchStatus.NotRunning);
            await sendBoardAndClasses(room);
        }

        private record struct SetRoomStatusResult(bool Success, string? ErrorMessage);

        private async Task<SetRoomStatusResult> setRoomMatchStatus(ServerRoom room, MatchStatus status)
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
            if(error == null)
                await sendMatchStatus(room);
            return new SetRoomStatusResult(error == null, error);
        }

        private (IList<BingoClientInRoom>, IList<BingoClientInRoom>) splitClients(ServerRoom room, Predicate<BingoClientInRoom> pred)
        {
            var truelist = new List<BingoClientInRoom>();
            var falselist = new List<BingoClientInRoom>();
            foreach (var c in room.Users)
            {
                (pred(c) ? truelist : falselist).Add(c);
            }
            return (truelist, falselist);
        }
    }
}