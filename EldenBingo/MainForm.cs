using EldenBingo.GameInterop;
using EldenBingo.Net;
using EldenBingo.Properties;
using EldenBingo.Rendering;
using EldenBingo.Settings;
using EldenBingo.UI;
using EldenBingoCommon;
using EldenBingoServer;
using Neto.Shared;
using SFML.System;
using System.Security.Principal;

namespace EldenBingo
{
    public partial class MainForm : Form
    {
        private readonly Client _client;
        private readonly GameProcessHandler _processHandler;
        private MapCoordinateProviderHandler? _mapCoordinateProviderHandler;
        private MapWindow? _mapWindow;
        private Thread? _mapWindowThread;
        private Server? _server = null;
        private string _lastRoom = string.Empty;

        public MainForm()
        {
            InitializeComponent();
            Icon = Resources.icon;
            _processHandler = new GameProcessHandler();
            _processHandler.StatusChanged += _processHandler_StatusChanged;
            _processHandler.CoordinatesChanged += _processHandler_CoordinatesChanged;

            if (Properties.Settings.Default.MainWindowSizeX > 0 && Properties.Settings.Default.MainWindowSizeY > 0)
            {
                Width = Properties.Settings.Default.MainWindowSizeX;
                Height = Properties.Settings.Default.MainWindowSizeY;
            }

            FormClosing += (o, e) =>
            {
                _processHandler.Dispose();
                _mapWindow?.DisposeDrawablesOnExit();
                _mapWindow?.Stop();
                _client?.Disconnect();
            };
            _client = new Client();
            addClientListeners(_client);

            if (Properties.Settings.Default.HostServerOnLaunch)
            {
                hostServer();
            }
            listenToSettingsChanged();
            SizeChanged += mainForm_SizeChanged;
        }

        public static Font GetFontFromSettings(Font defaultFont, float size, float defaultSize = 12f)
        {
            var ffName = Properties.Settings.Default.BingoFont;
            Font? font = null;
            var scale = Properties.Settings.Default.BingoFontSize / defaultSize;
            if (!string.IsNullOrWhiteSpace(ffName))
            {
                var ff2 = new FontFamily(ffName);
                font = new Font(ff2, size * scale, (FontStyle)Properties.Settings.Default.BingoFontStyle);
                if (font.Name == ffName)
                    return font;
            }
            return defaultFont;
        }

        /// <summary>
        /// Checks if the user has called this application as administrator.
        /// </summary>
        /// <returns>True if application is running as administrator.</returns>
        private static bool IsAdministrator()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }

        private async void _connectButton_Click(object sender, EventArgs e)
        {
            var form = new ConnectForm();
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                await initClientAsync(form.Address, form.Port);
            }
        }

        private async void _createLobbyButton_Click(object sender, EventArgs e)
        {
            if (_client?.IsConnected != true)
                return;

            var form = new CreateLobbyForm(_client, true);
            _ = _client.RequestRoomName();
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                await _client.CreateRoom(
                    form.RoomName,
                    form.AdminPassword,
                    form.Nickname,
                    form.Team,
                    GameSettingsHelper.ReadFromSettings(Properties.Settings.Default));
            }
        }

        private async void _disconnectButton_Click(object sender, EventArgs e)
        {
            if (_client?.IsConnected != true)
                return;

            var res = MessageBox.Show(this, "Disconnect from server?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                await _client.Disconnect();
                updateButtonAvailability();
            }
        }

        private async void _joinLobbyButton_Click(object sender, EventArgs e)
        {
            if (_client?.IsConnected != true)
                return;

            var form = new CreateLobbyForm(_client, false);
            form.RoomName = _lastRoom;
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                await _client.JoinRoom(
                    form.RoomName,
                    form.AdminPassword,
                    form.Nickname,
                    form.Team);
            }
        }

        private async void _leaveRoomButton_Click(object sender, EventArgs e)
        {
            if (_client?.IsConnected != true)
                return;

            var res = MessageBox.Show(this, "Leave current lobby?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                await _client.LeaveRoom();
                updateButtonAvailability();
            }
        }

        private void _openMapButton_Click(object sender, EventArgs e)
        {
            openMapWindow();
        }

        private void _processHandler_CoordinatesChanged(object? sender, MapCoordinateEventArgs e)
        {
            if (_client?.IsConnected == true && _client.Room != null && _client?.LocalUser?.IsSpectator == false)
            {
                ClientCoordinates coords;
                if (e.Coordinates.HasValue)
                    coords = new ClientCoordinates(e.Coordinates.Value.X, e.Coordinates.Value.Y, e.Coordinates.Value.Angle, e.Coordinates.Value.IsUnderground);
                else
                    coords = new ClientCoordinates(0, 0, 0, false);
                
                _ = _client.SendPacketToServer(new Packet(coords));
            }
        }

        private void _processHandler_StatusChanged(object? sender, StatusEventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                _processMonitorStatusTextBox.Text = e.Status;
                _processMonitorStatusTextBox.BackColor = e.Color;
            }));
        }

        private void _settingsButton_Click(object sender, EventArgs e)
        {
            var settingsDialog = new SettingsDialog();
            var res = settingsDialog.ShowDialog(this);
            if (res == DialogResult.OK)
            {
            }
        }

        private async void _startGameButton_Click(object sender, EventArgs e)
        {
            try
            {
                await tryStartingGameWithoutEAC();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void addClientListeners(Client? client)
        {
            if (client == null)
                return;

            client.Connected += client_Connected;
            client.Disconnected += client_Disconnected;
            client.OnStatus += client_OnStatus;
            client.OnError += client_OnError;
            client.OnRoomChanged += client_RoomChanged;

            client.AddListener<ServerJoinRoomAccepted>(joinRoomAccepted);
            client.AddListener<ServerJoinRoomDenied>(joinRoomDenied);
            client.AddListener<ServerEntireBingoBoardUpdate>(gotBingoBoard);
        }

        private void client_Connected(object? sender, EventArgs e)
        {
            updateButtonAvailability();
        }

        private void client_Disconnected(object? sender, StringEventArgs e)
        {
            updateButtonAvailability();
        }

        private void joinRoomAccepted(ClientModel? _, ServerJoinRoomAccepted joinRoomAcceptedArgs)
        {
            updateButtonAvailability();
        }

        private void joinRoomDenied(ClientModel? _, ServerJoinRoomDenied joinRoomDeniedArgs)
        {
            updateButtonAvailability();
        }

        private void gotBingoBoard(ClientModel? _, ServerEntireBingoBoardUpdate bingoBoardArgs)
        {
            if (Properties.Settings.Default.ShowClassesOnMap &&
                _mapWindow != null && _client.Room != null && _client.Room.Match.MatchStatus == MatchStatus.Running && _client.Room.Match.MatchMilliseconds < 10000)
                _mapWindow.ShowAvailableClasses(bingoBoardArgs.AvailableClasses);
        }

        private void client_RoomChanged(object? sender, RoomChangedEventArgs e)
        {
            if (_client == null)
                return;

            void update()
            {
                if(_client.Room != null)
                {
                    _lastRoom = _client.Room.Name;
                }
                if (_client.Room != null && _lobbyPage.Parent == null)
                {
                    tabControl1.TabPages.Add(_lobbyPage);
                    tabControl1.SelectedIndex = 1;
                }
                if (_client.Room == null && _lobbyPage.Parent != null)
                {
                    tabControl1.TabPages.Remove(_lobbyPage);
                    tabControl1.SelectedIndex = 0;
                }
                _clientStatusTextBox.Text = _client.GetConnectionStatusString();
            }
            if (InvokeRequired)
            {
                BeginInvoke(update);
                return;
            }
            update();
        }

        private void client_OnStatus(object? sender, StringEventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                _consoleControl.PrintToConsole(e.Message, Color.LightBlue);
                _clientStatusTextBox.Text = _client.GetConnectionStatusString();
            }));
        }

        private void client_OnError(object? sender, StringEventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                _consoleControl.PrintToConsole(e.Message, Color.Red);
                _clientStatusTextBox.Text = _client.GetConnectionStatusString();
            }));
        }

        private void default_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Properties.Settings.Default.ControlBackColor))
            {
                BackColor = Properties.Settings.Default.ControlBackColor;
                _consoleControl.BackColor = Properties.Settings.Default.ControlBackColor;
                _lobbyControl.BackColor = Properties.Settings.Default.ControlBackColor;
            }
            if (e.PropertyName == nameof(Properties.Settings.Default.HostServerOnLaunch))
            {
                if (_server == null && Properties.Settings.Default.HostServerOnLaunch)
                    hostServer();
            }
        }

        private void hostServer()
        {
            if (_server == null)
            {
                _server = new Server(Properties.Settings.Default.Port);
                _server.OnStatus += server_OnStatus;
                _server.OnError += server_OnError;
                _server.Host();
            }
        }

        private async Task initClientAsync(string address, int port)
        {
            if (_client.IsConnected == true)
                await _client.Disconnect();
            updateButtonAvailability();

            var ipendpoint = Client.EndPointFromAddress(address, port, out string error);
            if (ipendpoint == null)
            {
                MessageBox.Show(this, error, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                await _client.Connect(ipendpoint);
                updateButtonAvailability();
            }
        }

        private void listenToSettingsChanged()
        {
            Properties.Settings.Default.PropertyChanged += default_PropertyChanged;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            var c = Properties.Settings.Default.ControlBackColor;

            _consoleControl.BackColor = c;
            _lobbyControl.BackColor = c;
            _lobbyControl.HandleCreated += _lobbyControl_HandleCreated;
            //Select (and initialize) the lobby control
            tabControl1.SelectedIndex = 1;

            updateButtonAvailability();

            //Set the initial status of the status text box
            _clientStatusTextBox.Text = _client.GetConnectionStatusString();

            //Start looking for Elden Ring process
            _processHandler.StartScan();

            if (Properties.Settings.Default.AutoConnect && !string.IsNullOrWhiteSpace(Properties.Settings.Default.ServerAddress))
            {
                await initClientAsync(Properties.Settings.Default.ServerAddress, Properties.Settings.Default.Port);
            }
        }

        private void _lobbyControl_HandleCreated(object? sender, EventArgs e)
        {
            //Make sure the lobby control has been visible once, so it's controls are initialized
            tabControl1.TabPages.Remove(_lobbyPage);
            tabControl1.SelectedIndex = 0;
            _lobbyControl.HandleCreated -= _lobbyControl_HandleCreated;
            _lobbyControl.Client = _client;
        }

        private void mainForm_SizeChanged(object? sender, EventArgs e)
        {
            Properties.Settings.Default.MainWindowSizeX = Width;
            Properties.Settings.Default.MainWindowSizeY = Height;
        }

        private void openMapWindow()
        {
            if (_mapWindowThread?.ThreadState == ThreadState.Running)
                return;

            if (_mapCoordinateProviderHandler != null)
            {
                _mapCoordinateProviderHandler.Dispose();
                _mapCoordinateProviderHandler = null;
            }

            _mapWindowThread = new Thread(() =>
            {
                Vector2u windowSize;
                if (Properties.Settings.Default.MapWindowCustomSize && Properties.Settings.Default.MapWindowWidth >= 0 && Properties.Settings.Default.MapWindowHeight >= 0)
                {
                    windowSize = new Vector2u((uint)Properties.Settings.Default.MapWindowWidth, (uint)Properties.Settings.Default.MapWindowHeight);
                }
                else if (!Properties.Settings.Default.MapWindowCustomSize && Properties.Settings.Default.MapWindowLastWidth >= 0 && Properties.Settings.Default.MapWindowLastHeight >= 0)
                {
                    windowSize = new Vector2u((uint)Properties.Settings.Default.MapWindowLastWidth, (uint)Properties.Settings.Default.MapWindowLastHeight);
                }
                else
                {
                    windowSize = new Vector2u(500, 500);
                }
                _mapWindow = new MapWindow(windowSize.X, windowSize.Y);
                if (Properties.Settings.Default.MapWindowCustomPosition && Properties.Settings.Default.MapWindowX >= 0 && Properties.Settings.Default.MapWindowY >= 0)
                {
                    _mapWindow.Position = new Vector2i(Properties.Settings.Default.MapWindowX, Properties.Settings.Default.MapWindowY);
                }
                else
                {
                    _mapWindow.Position = new Vector2i(Left + Width, Top);
                }
                _mapCoordinateProviderHandler = new MapCoordinateProviderHandler(_mapWindow, _processHandler, _client);
                _mapWindow.Start();
            });
            _mapWindowThread.Start();
        }

        private void removeClientListeners(Client? client)
        {
            if (client == null)
                return;

            client.Connected -= client_Connected;
            client.Disconnected -= client_Disconnected;
            client.OnRoomChanged -= client_RoomChanged;
            client.RemoveListener<ServerJoinRoomAccepted>(joinRoomAccepted);
            client.RemoveListener<ServerJoinRoomDenied>(joinRoomDenied);
            client.RemoveListener<ServerEntireBingoBoardUpdate>(gotBingoBoard);
        }

        private void server_OnStatus(object? sender, StringEventArgs e)
        {
            _consoleControl.PrintToConsole(e.Message, Color.Orange);
        }

        private void server_OnError(object? sender, StringEventArgs e)
        {
            _consoleControl.PrintToConsole(e.Message, Color.Red);
        }

        private async Task tryStartingGameWithoutEAC()
        {
            if (_processHandler == null)
                return;

            GameRunningStatus res = await _processHandler.GetGameRunningStatus();
            if (res == GameRunningStatus.NotRunning)
            {
                await _processHandler.SafeStartGame();
            }
            else if (res == GameRunningStatus.RunningWithEAC)
            {
                if (IsAdministrator())
                {
                    DialogResult result = MessageBox.Show("Game is already running with EAC active!\n\n" +
                        "Do you want to close and restart it in offline mode without EAC?\n\n", Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        _processHandler.KillGameAndEAC();
                        await Task.Delay(2000);
                        await _processHandler.SafeStartGame();
                    }
                }
                else
                {
                    MessageBox.Show("Game is already running with EAC active!\n\n" +
                        "Restart this application as administrator if you want it to be able to restart Elden Ring without EAC", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            //Do nothing if game is already running without EAC
        }

        private void updateButtonAvailability()
        {
            BeginInvoke(new Action(() =>
            {
                bool connected = _client?.IsConnected == true;
                _connectButton.Visible = !connected;
                _disconnectButton.Visible = false; //connected;
                toolStripSeparator1.Visible = !connected;

                bool inRoom = _client?.Room != null;
                _createLobbyButton.Visible = !inRoom;
                _joinLobbyButton.Visible = !inRoom;
                _leaveRoomButton.Visible = inRoom;

                _createLobbyButton.Enabled = connected;
                _joinLobbyButton.Enabled = connected;
                _leaveRoomButton.Enabled = connected;
            }));
        }
    }
}