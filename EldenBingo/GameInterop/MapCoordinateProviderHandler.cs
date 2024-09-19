using EldenBingo.Net;
using EldenBingo.Rendering;
using EldenBingoCommon;
using Neto.Shared;

namespace EldenBingo.GameInterop
{
    internal class MapCoordinateProviderHandler
    {
        private Client _client;
        private GameProcessHandler _gameHandler;
        private LocalCoordinateProvider? _localProvider = null;
        private IDictionary<Guid, NetUserCoordinateProvider> _netProviders;
        private MapWindow _window;

        public MapCoordinateProviderHandler(MapWindow window, GameProcessHandler processHandler, Client client)
        {
            _window = window;
            _gameHandler = processHandler;
            _gameHandler.ProcessReadingChanged += _gameHandler_ProcessReadingChanged;
            _client = client;

            initLocalProvider();

            _netProviders = new Dictionary<Guid, NetUserCoordinateProvider>();

            initProviders(client);
            addClientListeners(client);
        }

        public void Dispose()
        {
            removeClientListeners(_client);
        }

        private void _gameHandler_ProcessReadingChanged(object? sender, EventArgs e)
        {
            initLocalProvider();
        }

        private void initLocalProvider()
        {
            if (_localProvider == null)
            {
                if (_gameHandler.ReadingProcess && (_client == null || _client?.LocalUser?.IsSpectator != true))
                {
                    _localProvider = new LocalCoordinateProvider(_gameHandler);
                    _localProvider.User = _client?.LocalUser;
                    _window.AddCoordinateProvider(_localProvider);
                }
            } 
            else
            {
                if (!_gameHandler.ReadingProcess || _client != null && _client?.LocalUser?.IsSpectator == true)
                {
                    _window.RemoveCoordinateProvider(_localProvider.Guid);
                    _localProvider = null;
                }
            }
        }

        private void _client_RoomChanged(object? sender, RoomChangedEventArgs e)
        {
            if (_localProvider != null)
            {
                _localProvider.User = _client.LocalUser;
            }
            initLocalProvider();
        }

        private void addClientListeners(Client client)
        {
            client.OnRoomChanged += _client_RoomChanged;
            client.OnUsersChanged += client_UsersChanged;
            client.AddListener<ServerUserCoordinates>(incomingCoordinates);
        }

        private void incomingCoordinates(ClientModel? sender, ServerUserCoordinates coords)
        {
            if (_netProviders.TryGetValue(coords.UserGuid, out var np))
            {
                np.MapCoordinates = new MapCoordinates(coords.X, coords.Y, coords.IsUnderground, coords.Angle);
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
            foreach (var user in client.Room.Users.Where(u => !u.IsSpectator))
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
            client.OnRoomChanged -= _client_RoomChanged;
        }

        internal class LocalCoordinateProvider : ICoordinateProvider
        {
            private static readonly Guid LocalProviderGuid = Guid.NewGuid();
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

            public Guid Guid => LocalProviderGuid;

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
                        if (_user == null || _user.IsSpectator)
                        {
                            MapCoordinates = null;
                        }
                    }
                }
            }

            private void _processHandler_CoordinatesChanged(object? sender, MapCoordinateEventArgs e)
            {
                MapCoordinates = _user?.IsSpectator == true ? null : e.Coordinates;
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
                        return new SFML.Graphics.Color(col.R, col.G, col.B);
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