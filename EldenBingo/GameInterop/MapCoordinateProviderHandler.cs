using EldenBingo.Net;
using EldenBingo.Net.DataContainers;
using EldenBingo.Rendering;
using EldenBingoCommon;

namespace EldenBingo.GameInterop
{
    internal class MapCoordinateProviderHandler
    {
        private Client _client;
        private GameProcessHandler _gameHandler;
        private LocalCoordinateProvider _localProvider;
        private object _localProviderLock = new object();
        private IDictionary<Guid, NetUserCoordinateProvider> _netProviders;
        private MapWindow2 _window;

        public MapCoordinateProviderHandler(MapWindow2 window, GameProcessHandler processHandler, Client client)
        {
            _window = window;
            _gameHandler = processHandler;
            _client = client;

            _localProvider = new LocalCoordinateProvider(_gameHandler);
            _localProvider.User = _client.LocalUser;
            _window.AddCoordinateProvider(_localProvider);

            _netProviders = new Dictionary<Guid, NetUserCoordinateProvider>();

            initProviders(client);
            addClientListeners(client);
        }

        public void Dispose()
        {
            removeClientListeners(_client);
        }

        private void _client_RoomChanged(object? sender, RoomChangedEventArgs e)
        {
            _localProvider.User = _client.LocalUser;
        }

        private void addClientListeners(Client client)
        {
            client.RoomChanged += _client_RoomChanged;
            client.UsersChanged += client_UsersChanged;
            client.IncomingData += client_IncomingData;
        }

        private void client_IncomingData(object? sender, ObjectEventArgs e)
        {
            if (e.PacketType == NetConstants.PacketTypes.ServerUserCoordinates)
            {
                if (e.Object is CoordinateData coords && _netProviders.TryGetValue(coords.User.Guid, out var np))
                {
                    np.MapCoordinates = coords.Coordinates;
                }
            }
        }

        private void client_UsersChanged(object? sender, EventArgs e)
        {
            initProviders(_client);
        }

        private void initProviders(Client client)
        {
            if (client?.Room == null)
            {
                _netProviders.Clear();
                return;
            }
            var notPresent = new HashSet<Guid>(_netProviders.Keys);
            //No reason to create coordinate providers for spectators
            foreach (var user in client.Room.Clients.Where(u => !u.IsSpectator))
            {
                //Don't create a net coordinate provider for myself
                if (client.LocalUser != null && user.Guid == client.LocalUser.Guid)
                    continue;

                notPresent.Remove(user.Guid);
                if (!_netProviders.ContainsKey(user.Guid))
                {
                    var netUserProvider = new NetUserCoordinateProvider(user);
                    _netProviders[user.Guid] = netUserProvider;
                    _window.AddCoordinateProvider(netUserProvider);
                }
            }
            if (notPresent.Any())
            {
                //Remove all clients that are no longer here
                foreach (var g in notPresent)
                {
                    _window.RemoveCoordinateProvider(g);
                    _netProviders.Remove(g);
                }
            }
        }

        private void removeClientListeners(Client client)
        {
            client.RoomChanged -= _client_RoomChanged;
        }

        internal class LocalCoordinateProvider : ICoordinateProvider
        {
            private static readonly SFML.Graphics.Color DefaultColor = new SFML.Graphics.Color(240, 198, 53);
            private MapCoordinates? _lastCoordinates;
            private GameProcessHandler _processHandler;
            private UserInRoom? _user;
            private object _userLock = new object();

            public LocalCoordinateProvider(GameProcessHandler processHandler)
            {
                _processHandler = processHandler;
                _lastCoordinates = processHandler.LastCoordinates;
                _processHandler.CoordinatesChanged += _processHandler_CoordinatesChanged;
                Changed = _lastCoordinates != null;
            }

            public bool Changed { get; private set; }

            public SFML.Graphics.Color Color
            {
                get
                {
                    lock (_userLock)
                    {
                        if (User == null)
                            return DefaultColor;
                        var c = User.ColorBright;
                        return new SFML.Graphics.Color(c.R, c.G, c.B);
                    }
                }
            }

            public Guid Guid { get; } = Guid.NewGuid();

            public MapCoordinates? MapCoordinates
            {
                get
                {
                    Changed = false;
                    return _lastCoordinates;
                }
                private set
                {
                    lock (this)
                    {
                        Changed = true;
                        _lastCoordinates = value;
                    }
                }
            }

            public string Name => string.Empty;

            public UserInRoom? User
            {
                get
                {
                    lock (_userLock)
                        return _user;
                }
                set
                {
                    lock (_userLock)
                    {
                        _user = value;
                    }
                }
            }

            private void _processHandler_CoordinatesChanged(object? sender, MapCoordinateEventArgs e)
            {
                MapCoordinates = e.Coordinates;
            }
        }

        internal class NetUserCoordinateProvider : ICoordinateProvider
        {
            private MapCoordinates? _lastCoordinates;
            private UserInRoom _user;

            public NetUserCoordinateProvider(UserInRoom user)
            {
                _user = user;
            }

            public bool Changed { get; private set; }

            public SFML.Graphics.Color Color
            {
                get
                {
                    lock (_user)
                    {
                        var col = _user.ColorBright;
                        return new SFML.Graphics.Color(col.R, col.B, col.B);
                    }
                }
            }

            public Guid Guid
            {
                get
                {
                    lock (_user)
                    {
                        return _user.Guid;
                    }
                }
            }

            public MapCoordinates? MapCoordinates
            {
                get
                {
                    Changed = false;
                    return _lastCoordinates;
                }
                internal set
                {
                    lock (this)
                    {
                        Changed = true;
                        _lastCoordinates = value;
                    }
                }
            }

            public string Name
            {
                get
                {
                    lock (_user)
                    {
                        return _user.Nick;
                    }
                }
            }
        }

        internal class MockUserCoordinateProvider : ICoordinateProvider
        {
            private string _name;
            private Color _color;
            private MapCoordinates _coordinates;
            private Guid _guid;
            

            public MockUserCoordinateProvider(string name, Color color, MapCoordinates coordinates)
            {
                _name = name;
                _color = color;
                _coordinates = coordinates;
                _guid = Guid.NewGuid();
            }

            public bool Changed => true;

            public SFML.Graphics.Color Color => new SFML.Graphics.Color(_color.R, _color.B, _color.B);

            public Guid Guid => _guid;

            public MapCoordinates? MapCoordinates => _coordinates;

            public string Name => _name;
        }
    }
}