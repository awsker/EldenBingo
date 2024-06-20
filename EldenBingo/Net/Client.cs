using EldenBingo.Net;
using EldenBingoCommon;
using Neto.Client;
using Neto.Shared;
using System.Reflection;

namespace EldenBingo
{
    public class Client : NetoClient
    {
        private Room? _room;

        private ISet<string> _delayTypes;

        /// <summary>
        /// Artificial delay for all match related packets, in milliseconds
        /// </summary>
        public int PacketDelayMs { get; set; } = 0;

        public Client() : base(Properties.Settings.Default.IdentityToken)
        {
            //Always register the EldenBingoCommon assembly
            RegisterAssembly(Assembly.GetAssembly(typeof(BingoBoard)));
            registerHandlers();
            _delayTypes = new HashSet<string>()
            {
                nameof(ServerUserCoordinates),
                nameof(ServerMatchStatusUpdate),
                nameof(ServerEntireBingoBoardUpdate),
                nameof(ServerScoreboardUpdate),
                nameof(ServerScoreboardUpdate),
                nameof(ServerBingoAchievedUpdate),
                nameof(ServerSquareUpdate),
                nameof(ServerUserChecked)
            };
            Disconnected += client_Disconnected;
        }

        internal event EventHandler? OnUsersChanged;

        internal event EventHandler<RoomChangedEventArgs>? OnRoomChanged;

        public UserInRoom? LocalUser { get; private set; }
        public BingoBoard? BingoBoard => Room?.Match.Board;

        public override string Version => EldenBingoCommon.Version.CurrentVersion;

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
                    fireOnRoomChanged(oldRoom);
                }
            }
        }

        public override string GetConnectionStatusString()
        {
            if (!IsConnected)
                return "Not connected";
            if (CancellationToken.IsCancellationRequested)
                return "Stopping...";
            if (Room == null)
                return "Connected - Not in a lobby";
            else
                return "Connected - Lobby: " + Room.Name;
        }

        public async Task RequestRoomName()
        {
            var req = new Packet(new ClientRequestRoomName());
            await SendPacketToServer(req);
        }

        public async Task CreateRoom(string roomName, string adminPass, string nickname, int team, BingoGameSettings settings)
        {
            var request = new ClientRequestCreateRoom(roomName, adminPass, nickname, team, settings);
            await SendPacketToServer(new Packet(request));
        }

        public async Task JoinRoom(string roomName, string adminPass, string nickname, int team)
        {
            var request = new ClientRequestJoinRoom(roomName, adminPass, nickname, team);
            await SendPacketToServer(new Packet(request));
        }

        public async Task LeaveRoom()
        {
            Room = null;
            FireOnStatus("Left lobby");
            await SendPacketToServer(new Packet(new ClientRequestLeaveRoom()));
        }

        protected override async void DispatchObjects(ClientModel? sender, IEnumerable<object> objects)
        {
            if (PacketDelayMs > 0 && LocalUser != null && LocalUser.IsSpectator)
            {
                var ordinaryPackets = new Queue<object>();
                var delayPackets = new Queue<object>();
                foreach (var o in objects)
                {
                    var t = o.GetType();
                    if (t?.FullName != null && _delayTypes.Contains(t.Name))
                    {
                        delayPackets.Enqueue(o);
                    }
                    else
                    {
                        ordinaryPackets.Enqueue(o);
                    }
                }
                base.DispatchObjects(sender, ordinaryPackets);
                if (delayPackets.Count > 0)
                {
                    await Task.Delay(PacketDelayMs);
                    base.DispatchObjects(sender, delayPackets);
                }
            }
            else
            {
                base.DispatchObjects(sender, objects);
            }
        }

        private void client_Disconnected(object? sender, StringEventArgs e)
        {
            Room = null;
        }

        private void registerHandlers()
        {
            AddListener<ServerUserJoinedRoom>(userJoinedRoom);
            AddListener<ServerUserLeftRoom>(userLeftRoom);
            AddListener<ServerCreateRoomDenied>(createRoomDenied);
            AddListener<ServerJoinRoomDenied>(joinRoomDenied);
            AddListener<ServerJoinRoomAccepted>(joinRoomAccepted);
            AddListener<ServerEntireBingoBoardUpdate>(entireBingoBoardUpdate);
            AddListener<ServerMatchStatusUpdate>(matchStatusUpdate);
        }

        private void userJoinedRoom(ClientModel? _, ServerUserJoinedRoom userJoined)
        {
            if (Room != null)
            {
                Room.AddUser(userJoined.User);
                fireOnUsersChanged();
            }
        }

        private void userLeftRoom(ClientModel? _, ServerUserLeftRoom userLeft)
        {
            if (Room != null)
            {
                Room.RemoveUser(userLeft.User);
                fireOnUsersChanged();
            }
        }

        private void createRoomDenied(ClientModel? _, ServerCreateRoomDenied createRoomDenied)
        {
            Room = null;
            FireOnStatus($"Create lobby failed: {createRoomDenied.Reason}");
        }

        private void joinRoomDenied(ClientModel? _, ServerJoinRoomDenied joinDenied)
        {
            Room = null;
            FireOnStatus($"Join lobby failed: {joinDenied.Reason}");
        }

        private void joinRoomAccepted(ClientModel? _, ServerJoinRoomAccepted joinAccepted)
        {
            var sameRoomAsBefore = Room != null && Room.Name == joinAccepted.RoomName;

            if (!sameRoomAsBefore)
                FireOnStatus($"Joined lobby");
            var room = new Room(joinAccepted.RoomName);
            room.Match.UpdateMatchStatus(joinAccepted.MatchStatus, joinAccepted.Paused, joinAccepted.Timer);
            foreach (var user in joinAccepted.Users)
                room.AddUser(user);

            //Store a reference to my own User
            LocalUser = room.GetUser(ClientGuid);

            //Set the new current room (which fires the RoomChanged event)
            Room = room;
        }

        private void entireBingoBoardUpdate(ClientModel? _, ServerEntireBingoBoardUpdate boardUpdate)
        {
            if (Room != null)
            {
                Room.Match.Board = boardUpdate.Size > 0 && boardUpdate.Squares.Length == boardUpdate.Size * boardUpdate.Size ?
                    new BingoBoard(boardUpdate.Size, boardUpdate.Squares, boardUpdate.AvailableClasses) : 
                    null;
            }
        }

        private void matchStatusUpdate(ClientModel? _, ServerMatchStatusUpdate matchStatus)
        {
            if (Room != null)
            {
                Room.Match.UpdateMatchStatus(matchStatus.MatchStatus, matchStatus.Paused, matchStatus.Timer);
            }
        }

        private void fireOnRoomChanged(Room? oldRoom)
        {
            OnRoomChanged?.Invoke(this, new RoomChangedEventArgs(oldRoom, Room));
        }

        private void fireOnUsersChanged()
        {
            OnUsersChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}