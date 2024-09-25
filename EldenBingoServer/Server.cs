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
        private const int MatchStartCountdown = 9999;

        private const int RoomInactivityRemovalSeconds = 3600;
        private readonly ConcurrentDictionary<string, ServerRoom> _rooms;
        private System.Timers.Timer _roomClearTimer;

        public override string Version => EldenBingoCommon.Version.CurrentVersion;

        public Server(int port) : base(port)
        {
            _rooms = new ConcurrentDictionary<string, ServerRoom>(StringComparer.OrdinalIgnoreCase);
            //Always register the EldenBingoCommon assembly
            RegisterAssembly(Assembly.GetAssembly(typeof(BingoBoard)));
            registerHandlers();

            //Start the room clearing timer
            _roomClearTimer = new System.Timers.Timer(RoomInactivityRemovalSeconds * 1000); //Check for inactive rooms once every hour (3600 seconds * 1000 ms)
            _roomClearTimer.Elapsed += _roomClearTimer_Elapsed;
            _roomClearTimer.Start();
        }

        public IEnumerable<ServerRoom> Rooms => _rooms.Values;

        protected override async Task DropClient(BingoClientModel client)
        {
            if (client.Room != null)
                await leaveUserRoom(client);
            await base.DropClient(client);
        }

        private async Task<bool> confirm(BingoClientModel client, bool? admin = null, bool? spectator = null, bool? inRoom = null, bool? hasBingoBoard = null, bool? gameStarted = null)
        {
            if (admin.HasValue)
            {
                if (admin.Value && !client.IsAdmin)
                {
                    await sendAdminErrorMessage(client, "You are not admin");
                    return false;
                }
                if (!admin.Value && client.IsAdmin)
                {
                    await sendAdminErrorMessage(client, "You are admin");
                    return false;
                }
            }
            if (spectator.HasValue)
            {
                if (spectator.Value && !client.IsSpectator)
                {
                    await sendAdminErrorMessage(client, "You are not spectator");
                    return false;
                }
                if (!spectator.Value && client.IsSpectator)
                {
                    await sendAdminErrorMessage(client, "You are spectator");
                    return false;
                }
            }
            if (inRoom.HasValue)
            {
                if (inRoom.Value && client.Room == null)
                {
                    await sendAdminErrorMessage(client, "You are not in a lobby");
                    return false;
                }
                if (!inRoom.Value && client.Room != null)
                {
                    await sendAdminErrorMessage(client, "You are in a lobby");
                    return false;
                }
            }
            if (hasBingoBoard.HasValue)
            {
                if (hasBingoBoard.Value && client.Room?.Match?.Board == null)
                {
                    await sendAdminErrorMessage(client, "No bingo board has been generated");
                    return false;
                }
                if (!hasBingoBoard.Value && client.Room?.Match?.Board != null)
                {
                    await sendAdminErrorMessage(client, "Bingo board already generated");
                    return false;
                }
            }
            if (gameStarted.HasValue)
            {
                var matchInProgress = client.Room?.Match?.MatchStatus > MatchStatus.NotRunning && client.Room?.Match?.MatchStatus < MatchStatus.Finished;
                if (gameStarted.Value && !matchInProgress)
                {
                    await sendAdminErrorMessage(client, "Match has not yet started");
                    return false;
                }
                if (!gameStarted.Value && matchInProgress)
                {
                    await sendAdminErrorMessage(client, "Match has already started");
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
            AddListener<ClientTogglePause>(clientTogglePause);
            AddListener<ClientSetTeamName>(clientSetTeamName);
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
                deniedReason = "Invalid lobby name";
            else if (_rooms.TryGetValue(request.RoomName, out _))
                deniedReason = "Lobby with that name already exists";
            else if (string.IsNullOrWhiteSpace(request.Nick))
                deniedReason = "Invalid nickname";
            if (deniedReason != null)
            {
                var deniedPacket = new ServerJoinRoomDenied(deniedReason);
                await SendPacketToClient(new Packet(deniedPacket), sender);
                return;
            }
            ServerRoom? room = createRoom(request.RoomName, request.AdminPass, sender, request.Settings);
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
                throw new ApplicationException("Lobby already exists");
            }
            settings = validateGameSettings(settings);
            var room = new ServerRoom(roomName, adminPass, creator, settings);
            room.TimerElapsed += onRoomTimerElapsed;
            _rooms[roomName] = room;
            FireOnStatus($"Lobby '{roomName}' was created");
            return room;
        }

        private BingoGameSettings validateGameSettings(BingoGameSettings settings)
        {
            settings.BoardSize = Math.Clamp(settings.BoardSize, 3, 8);
            settings.PreparationTime = Math.Max(0, settings.PreparationTime);
            settings.NumberOfClasses = Math.Max(1, settings.NumberOfClasses);
            settings.CategoryLimit = Math.Max(0, settings.CategoryLimit);
            return settings;
        }


        private async void onRoomTimerElapsed(object? sender, RoomEventArgs e)
        {
            var room = e.Room;
            //Start game when countdown reaches 0
            if (room.Match.MatchStatus == MatchStatus.Starting)
            {
                if (room.GameSettings.PreparationTime > 0)
                {
                    await startPreparation(room);
                }
                else
                {
                    await startMatch(room);
                }
            }
            else if (room.Match.MatchStatus == MatchStatus.Preparation)
            {
                await startMatch(room);
            }
        }

        private async void joinRoomRequested(BingoClientModel? sender, ClientRequestJoinRoom request)
        {
            if (sender == null)
                return;

            string? deniedReason = null;
            ServerRoom? room = null;
            if (string.IsNullOrWhiteSpace(request.RoomName))
                deniedReason = "Invalid lobby name";
            else if (!_rooms.TryGetValue(request.RoomName, out room))
                deniedReason = "Lobby doesn't exist";
            else if (room != null && sender.Room == room)
                deniedReason = "Already in this lobby";
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
                var packet = new ServerUserCoordinates(sender.ClientGuid, coordinates.X, coordinates.Y, coordinates.Angle, coordinates.IsUnderground, coordinates.MapInstance);
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
                var packet = new Packet(new ServerUserChat(sender.ClientGuid, chatMessage.Message));
                if (userInfo.IsSpectator && !userInfo.IsAdmin)
                {
                    await SendPacketToClients(packet, sender.Room.ClientModels.Where(u => u.IsSpectator && !u.IsAdmin));
                }
                else
                {
                    await sendPacketToRoom(packet, sender.Room);
                }
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
                await setRoomBingoBoard(sender.Room, board);
                await sendAdminStatusMessage(sender, $"Bingo json file successfully uploaded and bingo board generated!", System.Drawing.Color.Green);
            }
            else
            {
                await sendAdminStatusMessage(sender, $"Bingo json file successfully uploaded!", System.Drawing.Color.Green);
            }
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
                if (board == null)
                {
                    await sendAdminStatusMessage(sender, $"Error generating bingo board: No valid board possible", System.Drawing.Color.Red);
                    return;
                } 
                else
                {
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

            if (matchStatus.MatchStatus == MatchStatus.Starting && sender.Room.Match != null && sender.Room.BoardGenerator != null)
            {
                //Generate a board if no board is set or if the current board was used in last bingo, or if current board is wrong size
                if(sender.Room.Match.Board == null || sender.Room.BoardAlreadyUsed || sender.Room.Match.Board.Size != sender.Room.GameSettings.BoardSize)
                {
                    var board = sender.Room.BoardGenerator.CreateBingoBoard(sender.Room);
                    if (board != null)
                    {
                        await setRoomBingoBoard(sender.Room, board);
                    }
                }
            }

            //Can always change status to "Not running", all others require that bingo board is generated
            if (matchStatus.MatchStatus > MatchStatus.NotRunning && !await confirm(sender, hasBingoBoard: true))
                return;

            var result = await setRoomMatchStatus(sender.Room, matchStatus.MatchStatus);
            //If error occured when setting room match status, send response to the requester
            if (!result.Success && result.ErrorMessage != null)
            {
                await sendAdminStatusMessage(sender, result.ErrorMessage, System.Drawing.Color.Red);
            }
            if (result.Success)
            {
                if (matchStatus.MatchStatus == MatchStatus.Starting)
                {
                    var p = new Packet(new ServerEntireBingoBoardUpdate(0, Array.Empty<BingoBoardSquare>(), Array.Empty<EldenRingClasses>()));
                    //Reset the board for all players (except AdminSpectators, who already have the new board)
                    await SendPacketToClients(p, sender.Room.ClientModels.Where(c => !(c.IsAdmin && c.IsSpectator)));
                } 
                if (matchStatus.MatchStatus == MatchStatus.Finished)
                {
                    sender.Room.BoardAlreadyUsed = true;
                }
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

                    var bingosBefore = board.BingoSet;
                    if (board.UserClicked(tryCheck.Index, userInfo, userToSet))
                    {
                        var tasks = new List<Task>();
                        var check = board.CheckStatus[tryCheck.Index];
                        ServerSquareUpdate squareUpdate;
                        ServerUserChecked userCheck;
                        ServerScoreboardUpdate scoreboard = createScoreboardUpdatePacket(sender.Room);
                        ServerBingoAchievedUpdate[] bingos = board.BingoSet.Except(bingosBefore).Select(b => new ServerBingoAchievedUpdate(b)).ToArray();
                        lock (check)
                        {
                            userCheck = new ServerUserChecked(userInfo.Guid, tryCheck.Index, check.Team);
                            foreach (var recipient in sender.Room.Users)
                            {
                                squareUpdate = new ServerSquareUpdate(board.GetSquareDataForUser(recipient, tryCheck.Index), tryCheck.Index);
                                var packet = new Packet(squareUpdate, userCheck, scoreboard);
                                foreach (var bingo in bingos)
                                    packet.AddObject(bingo);
                                var task = SendPacketToClient(packet, recipient.Client);
                                tasks.Add(task);
                            }
                        }
                        await Task.WhenAll(tasks);
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
                    var check = board.CheckStatus[tryMark.Index];
                    ServerSquareUpdate squareUpdate;
                    lock (check)
                    {
                        squareUpdate = new ServerSquareUpdate(board.GetSquareDataForUser(userInfo, tryMark.Index), tryMark.Index);
                    }
                    await SendPacketToClient(new Packet(squareUpdate), sender);
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
                        var check = board.CheckStatus[trySetCounter.Index];
                        lock (check)
                        {
                            foreach (var recipient in sender.Room.Users.Where(u => u.IsSpectator || u == userToSet))
                            {
                                ServerSquareUpdate squareUpdate = new ServerSquareUpdate(board.GetSquareDataForUser(recipient, trySetCounter.Index), trySetCounter.Index);
                                var task = SendPacketToClient(new Packet(squareUpdate), recipient.Client);
                                tasks.Add(task);
                            }
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

            var oldScorePerBingo = sender.Room.GameSettings.PointsPerBingoLine;
            var settings = validateGameSettings(gameSettingsRequest.GameSettings);
            sender.Room.GameSettings = settings;
            if (sender.Room.BoardGenerator != null)
            {
                //Update the current board generator with the new random seed
                sender.Room.BoardGenerator.RandomSeed = settings.RandomSeed;
                sender.Room.BoardGenerator.CategoryLimit = settings.CategoryLimit;
            }
            if (oldScorePerBingo != settings.PointsPerBingoLine)
            {
                _ = sendScoreboard(sender.Room);
            }
            var matchInProgress = sender.Room?.Match?.MatchStatus > MatchStatus.NotRunning && sender.Room?.Match?.MatchStatus < MatchStatus.Finished;
            //If size was changed when match was not running and the generated board size is different than the new one -> Generate new board
            if (!matchInProgress && sender.Room?.Match?.Board != null && sender.Room.Match.Board.Size != settings.BoardSize)
            {
                clientRandomizeBoard(sender, new ClientRandomizeBoard());
            }
            else
            {
                await sendAdminStatusMessage(sender, "New lobby settings set", Color.Green);
            }
        }

        private async void clientTogglePause(BingoClientModel? sender, ClientTogglePause pauseRequest)
        {
            if (sender == null)
                return;
            if (!await confirm(sender, admin: true, inRoom: true))
                return;

            if(!sender.Room.Match.Running)
            {
                await sendAdminErrorMessage(sender, "Match not running");
                return;
            }
            if (sender.Room.Match.Paused)
            {
                sender.Room.UnpauseMatch();
            }
            else
            {
                sender.Room.PauseMatch();
            }
            await sendMatchStatus(sender.Room);
        }

        private async void clientSetTeamName(BingoClientModel? sender, ClientSetTeamName setNameRequest)
        {
            if (sender == null)
                return;
            if (!await confirm(sender, inRoom: true))
                return;

            var userInfo = sender.Room.GetUser(sender.ClientGuid);
            if(userInfo != null)
            {
                if(userInfo.IsAdmin || userInfo.Team == setNameRequest.Team)
                {
                    var oldName = sender.Room.GetTeamNameIgnoreUsers(setNameRequest.Team);
                    sender.Room.SetTeamName(setNameRequest.Team, cleanUpString(setNameRequest.Name));
                    var newName = sender.Room.GetTeamNameIgnoreUsers(setNameRequest.Team);
                    if (oldName == newName)
                        return;
                    var teamColorName = BingoConstants.GetTeamName(setNameRequest.Team);
                    var packet = new Packet();
                    packet.AddObject(createScoreboardUpdatePacket(sender.Room));
                    var teamNameChangedPacket = new ServerTeamNameChanged(userInfo.Guid, userInfo.Team, teamColorName, newName);
                    packet.AddObject(teamNameChangedPacket);
                    _ = sendPacketToRoom(packet, sender.Room);
                }
            }
        }

        private ServerEntireBingoBoardUpdate createEntireBoardPacket(ServerBingoBoard? board, UserInRoom user)
        {
            if (board == null)
                return new ServerEntireBingoBoardUpdate(0, Array.Empty<BingoBoardSquare>(), Array.Empty<EldenRingClasses>());
            var squareData = board.GetSquareDataForUser(user);
            return new ServerEntireBingoBoardUpdate(board.Size, squareData, board.AvailableClasses);
        }

        private ServerScoreboardUpdate createScoreboardUpdatePacket(ServerRoom room)
        {
            var teams = room.GetActiveTeams();
            var teamScores = new List<TeamScore>(teams.Select(t => new TeamScore(t.Index, t.Name, 0)));

            if (room.Match.Board is ServerBingoBoard board)
            {
                var squaresPerTeam = board.GetSquaresPerTeam();
                var bingosPerTeam = board.BingosPerTeam;

                for (int i = 0; i < teamScores.Count; ++i)
                {
                    if (squaresPerTeam.TryGetValue(teamScores[i].Team, out int squares))
                    {
                        var score = teamScores[i];
                        score.Score += squares;
                        teamScores[i] = score;
                    }
                    if (bingosPerTeam.TryGetValue(teamScores[i].Team, out var bingoLines))
                    {
                        var score = teamScores[i];
                        score.Score += bingoLines.Count * room.GameSettings.PointsPerBingoLine;
                        teamScores[i] = score;
                    }
                }
            }
            return new ServerScoreboardUpdate(teamScores.ToArray());
        }

        private async Task joinUserRoom(BingoClientModel client, string nick, string adminPass, int team, ServerRoom room, bool created = false)
        {
            if (client.Room != null)
                await leaveUserRoom(client);

            BingoClientInRoom clientInRoom = room.AddUser(client, nick, adminPass, team);

            var scoreboard = createScoreboardUpdatePacket(room);
            //Only send new user to room if there are any other clients present
            if (room.NumUsers > 1)
            {
                //Send the user as a UserInRoom (we don't want to send a BingoClientInRoom since this type is unrecognized by the client)
                var user = new UserInRoom(clientInRoom);
                var joinPacket = new ServerUserJoinedRoom(user);
                //Send join message to all clients already in the room
                await sendPacketToRoomExcept(new Packet(joinPacket, scoreboard), room, client.ClientGuid);
            }

            //Construct a list of all users as UserInRoom and send these (we don't want to send the users as BingoClientInRoom since this type is unrecognized by the client)
            var currentUsers = new List<UserInRoom>();
            foreach (var user in room.Users)
            {
                currentUsers.Add(new UserInRoom(user));
            }
            var joinAccepted = new ServerJoinRoomAccepted(room.Name, currentUsers.ToArray(), room.Match.MatchStatus, room.Match.Paused, room.Match.MatchMilliseconds);
            var packet = new Packet(joinAccepted);
            if (room.Match.Board is ServerBingoBoard board)
            {
                //Also send the bingo board if user should have it
                bool boardIsAvailableToAll = room.Match.MatchStatus >= MatchStatus.Preparation;
                if (boardIsAvailableToAll || clientInRoom.IsAdmin && clientInRoom.IsSpectator)
                {
                    packet.AddObject(createEntireBoardPacket(board, clientInRoom));
                }
            }
            packet.AddObject(scoreboard);
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
                var scoreboard = createScoreboardUpdatePacket(room);
                //Send user leaving packet to all users remaining in the room
                await sendPacketToRoom(new Packet(leftPacket, scoreboard), room);
            }
        }

        private async Task startPreparation(ServerRoom room)
        {
            await setRoomMatchStatus(room, MatchStatus.Preparation);
            await sendBoardAndClasses(room);
        }

        private async Task startMatch(ServerRoom room)
        {
            await setRoomMatchStatus(room, MatchStatus.Running);
            if(room.GameSettings.PreparationTime == 0) //Send board and classes only if they weren't sent in preparation phase
                await sendBoardAndClasses(room);
        }

        private async Task sendAdminStatusMessage(BingoClientModel client, string message, Color color)
        {
            await SendPacketToClient(new Packet(new ServerAdminStatusMessage(message, color.ToArgb())), client);
        }

        private async Task sendAdminErrorMessage(BingoClientModel client, string message)
        {
            await SendPacketToClient(new Packet(new ServerAdminStatusMessage(message, Color.Red.ToArgb())), client);
        }

        private async Task sendMatchStatus(ServerRoom room)
        {
            var matchStatus = new ServerMatchStatusUpdate(room.Match.MatchStatus, room.Match.Paused, room.Match.MatchMilliseconds);
            await sendPacketToRoom(new Packet(matchStatus), room);
        }

        private async Task sendBoardAndClasses(ServerRoom room)
        {
            var (adminSpectators, others) = splitClients(room, c => c.IsAdmin && c.IsSpectator);

            if (room.Match?.Board == null || room.Match?.Board is not ServerBingoBoard board)
            {
                //No board set, so we send an empty board
                await sendPacketToRoom(new Packet(new ServerEntireBingoBoardUpdate(0, Array.Empty<BingoBoardSquare>(), Array.Empty<EldenRingClasses>())), room);
                return;
            }

            //Board is set
            foreach (var k in adminSpectators)
            {
                //Admin spectators get the bingo board regardless of status
                var adminPacket = new Packet(createEntireBoardPacket(board, k));
                await SendPacketToClient(adminPacket, k.Client);
            }
            bool matchLive = room.Match.MatchStatus >= MatchStatus.Preparation;
            foreach (var k in others)
            {
                //All other users gets the packet without bingo board if match hasn't started
                var nonAdminsPacket = new Packet(createEntireBoardPacket(matchLive ? board : null, k));
                await SendPacketToClient(nonAdminsPacket, k.Client);
            }
        }

        private async Task sendScoreboard(ServerRoom room)
        {
            await sendPacketToRoom(new Packet(createScoreboardUpdatePacket(room)), room);
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
            room.BoardAlreadyUsed = false;
            await setRoomMatchStatus(room, MatchStatus.NotRunning);
            await sendBoardAndClasses(room);
            await sendScoreboard(room);
        }

        private record struct SetRoomStatusResult(bool Success, string? ErrorMessage);

        private async Task<SetRoomStatusResult> setRoomMatchStatus(ServerRoom room, MatchStatus status)
        {
            string? error = null;
            var currentStatus = room.Match.MatchStatus;
            var matchLive = currentStatus >= MatchStatus.Preparation;
            switch (status)
            {
                case MatchStatus.NotRunning:
                    {
                        if (currentStatus == MatchStatus.NotRunning || currentStatus == MatchStatus.Finished)
                        {
                            room.Match.UpdateMatchStatus(status, false, 0, null);
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
                        else if (currentStatus == MatchStatus.NotRunning || currentStatus == MatchStatus.Finished)
                        {
                            room.Match.UpdateMatchStatus(status, -MatchStartCountdown, null); // 10 second countdown until match starts
                            break;
                        }
                        error = "Match already started";
                        break;
                    }
                case MatchStatus.Preparation:
                    {
                        if (currentStatus != MatchStatus.Starting)
                        {
                            error = "Match not starting";
                            break;
                        }
                        else if(room.GameSettings.PreparationTime > 0)
                        {
                            room.Match.UpdateMatchStatus(status, -room.GameSettings.PreparationTime * 1000 + 1, null); // 10 second countdown until match starts
                            break;
                        }
                        break;
                    }
                case MatchStatus.Running:
                    {
                        if (currentStatus == MatchStatus.Starting || currentStatus == MatchStatus.Preparation)
                        {
                            room.Match.UpdateMatchStatus(status, 0, null);
                            break;
                        }
                        if (currentStatus == MatchStatus.Running)
                            error = "Match is already running";
                        if (currentStatus == MatchStatus.Finished)
                            error = "Match is finished";
                        break;
                    }
                default:
                    {
                        room.Match.UpdateMatchStatus(status, false, room.Match.MatchMilliseconds, null);
                        break;
                    }
            }
            if (error == null)
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

        private void _roomClearTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            removeNonActiveRooms(RoomInactivityRemovalSeconds);
        }

        private void removeNonActiveRooms(int maxSecondsInactive)
        {
            var obsoleteRooms = new HashSet<string>();
            var now = DateTime.Now;
            foreach (var room in _rooms)
            {
                //Don't remove rooms with users still connected
                if (room.Value.NumUsers > 0)
                    continue;

                //Check if too long since last activity
                if ((now - room.Value.LastActivity).TotalSeconds > maxSecondsInactive)
                {
                    obsoleteRooms.Add(room.Key);
                }
            }
            //Remove rooms after loop to avoid IEnumerable changed during enumeration exception
            foreach (var roomName in obsoleteRooms)
            {
                removeRoom(roomName);
                FireOnStatus($"Removed inactive lobby '{roomName}'");
            }
        }
        
        private void removeRoom(string roomName)
        {
            if (_rooms.Remove(roomName, out var room))
            {
                room.TimerElapsed -= onRoomTimerElapsed;
            }
        }

        private string cleanUpString(string input)
        {
            return input.Trim().Replace("&", "&&");
        }
    }
}