using EldenBingo.Net;
using EldenBingo.Net.DataContainers;
using EldenBingo.UI;
using EldenBingoCommon;
using System.Diagnostics;

namespace EldenBingo.GameInterop
{
    internal class MapCoordinateProviderHandler
    {
        private MapWindow _window;
        private Client _client;
        private GameProcessHandler _gameHandler;
        private LocalCoordinateProvider _localProvider;
        private IDictionary<Guid, NetUserCoordinateProvider> _netProviders;
        private object _localProviderLock = new object();

        public MapCoordinateProviderHandler(MapWindow window, GameProcessHandler processHandler, Client client)
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

        private void initProviders(Client client)
        {
            if(client?.Room == null)
            {
                _netProviders.Clear();
                return;
            }
            var notPresent = new HashSet<Guid>(_netProviders.Keys);
            //No reason to create coordinate providers for spectators
            foreach(var user in client.Room.Clients.Where(u => !u.IsSpectator))
            {
                //Don't create a net coordinate provider for myself
                if(client.LocalUser != null && user.Guid == client.LocalUser.Guid)
                    continue;

                notPresent.Remove(user.Guid);
                if (!_netProviders.ContainsKey(user.Guid))
                {
                    var netUserProvider = new NetUserCoordinateProvider(user);
                    _netProviders[user.Guid] = netUserProvider;
                    _window.AddCoordinateProvider(netUserProvider);
                }
            }
            if(notPresent.Any())
            {
                //Remove all clients that are no longer here
                foreach (var g in notPresent)
                {
                    _window.RemoveCoordinateProvider(g);
                    _netProviders.Remove(g);
                }
            }
        }

        private void addClientListeners(Client client)
        {
            client.RoomChanged += _client_RoomChanged;
            client.UsersChanged += client_UsersChanged;
            client.IncomingData += client_IncomingData;
        }

        private void removeClientListeners(Client client)
        {
            client.RoomChanged -= _client_RoomChanged;
        }

        private void _client_RoomChanged(object? sender, RoomChangedEventArgs e)
        {
            _localProvider.User = _client.LocalUser;
        }

        private void client_UsersChanged(object? sender, EventArgs e)
        {
            initProviders(_client);
        }

        private void client_IncomingData(object? sender, ObjectEventArgs e)
        {
            if(e.PacketType == NetConstants.PacketTypes.ServerUserCoordinates)
            {
                if(e.Object is CoordinateData coords && _netProviders.TryGetValue(coords.User.Guid, out var np))
                {
                    np.MapCoordinates = coords.Coordinates;
                }
            }
        }

        internal class LocalCoordinateProvider : ICoordinateProvider
        {
            GameProcessHandler _processHandler;
            private MapCoordinates? _lastCoordinates;

            private static readonly SFML.Graphics.Color DefaultColor = new SFML.Graphics.Color(240, 198, 53);

            private object _userLock = new object();
            private UserInRoom? _user;
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

            public LocalCoordinateProvider(GameProcessHandler processHandler)
            {
                _processHandler = processHandler;
                _lastCoordinates = processHandler.LastCoordinates;
                _processHandler.CoordinatesChanged += _processHandler_CoordinatesChanged;
                Changed = _lastCoordinates != null;
            }

            private void _processHandler_CoordinatesChanged(object? sender, MapCoordinateEventArgs e)
            {
                MapCoordinates = e.Coordinates;
            }

            public Guid Guid { get; } = Guid.NewGuid();

            public bool Changed { get; private set; }

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

            public SFML.Graphics.Color Color
            {
                get
                {
                    lock (_userLock)
                    {
                        if (User == null)
                            return DefaultColor;
                        var c = User.ConvertedColor;
                        return new SFML.Graphics.Color(c.R, c.G, c.B);
                    }
                }
            }

            public string Name => string.Empty;
        }

        internal class NetUserCoordinateProvider : ICoordinateProvider
        {
            private UserInRoom _user;
            private MapCoordinates? _lastCoordinates;

            public NetUserCoordinateProvider(UserInRoom user)
            {
                _user = user;
            }

            public Guid Guid {
                get
                {
                    lock (_user)
                    {
                        return _user.Guid;
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

            public bool Changed { get; private set; }

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

            public SFML.Graphics.Color Color
            {
                get
                {
                    lock (_user)
                    {
                        var col = _user.ConvertedColor;
                        return new SFML.Graphics.Color(col.R, col.B, col.B);
                    }
                }
            }
        }
    }
}
