using EldenBingo.GameInterop;
using EldenBingo.Net;
using EldenBingo.Net.DataContainers;
using EldenBingo.Properties;
using EldenBingo.UI;
using EldenBingoCommon;
using EldenBingoServer;
using System.Security.Principal;

namespace EldenBingo
{
    public partial class MainForm : Form
    {
        private readonly GameProcessHandler _processHandler;
        private MapWindow? _mapWindow;
        private readonly Client _client;
        private MapCoordinateProviderHandler? _mapCoordinateProviderHandler;

        public MainForm()
        {
            InitializeComponent();
            Icon = Resources.icon;
            _processHandler = new GameProcessHandler();
            _processHandler.StatusChanged += _processHandler_StatusChanged;
            _processHandler.CoordinatesChanged += _processHandler_CoordinatesChanged;
            
            if(Properties.Settings.Default.MainWindowSizeX > 0 && Properties.Settings.Default.MainWindowSizeY > 0)
            {
                Width = Properties.Settings.Default.MainWindowSizeX;
                Height = Properties.Settings.Default.MainWindowSizeY;
            }
            
            FormClosing += (o, e) =>
            {
                _processHandler.Dispose();
                _mapWindow?.DisposeStaticTextureData();
                _mapWindow?.Stop();
                _client?.Disconnect();
            };
            _client = new Client();
            addClientListeners(_client);
            #if DEBUG
                var server = new Server(NetConstants.DefaultPort);
                server.Host();
#endif

            listenToSettingsChanged();
            SizeChanged += mainForm_SizeChanged;
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            var c = Properties.Settings.Default.ControlBackColor;

            _consoleControl.Client = _client;
            _consoleControl.BackColor = c;

            _lobbyControl.Client = _client;
            _lobbyControl.BackColor = c;

            tabControl1.TabPages.Remove(_lobbyPage);

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

        private void mainForm_SizeChanged(object? sender, EventArgs e)
        {
            Properties.Settings.Default.MainWindowSizeX = Width;
            Properties.Settings.Default.MainWindowSizeY = Height;
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
                } else
                {
                    MessageBox.Show("Game is already running with EAC active!\n\n" +
                        "Restart this application as administrator if you want it to be able to restart Elden Ring without EAC", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            //Do nothing if game is already running without EAC
        }

        /// <summary>
        /// Checks if the user has called this application as administrator.
        /// </summary>
        /// <returns>True if application is running as administrator.</returns>
        private static bool IsAdministrator()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void openMapWindow()
        {
            if (_mapCoordinateProviderHandler != null)
            {
                _mapCoordinateProviderHandler.Dispose();
                _mapCoordinateProviderHandler = null;
            }

            _mapWindow = new MapWindow();
            if(Properties.Settings.Default.MapWindowCustomPosition && Properties.Settings.Default.MapWindowX >= 0 && Properties.Settings.Default.MapWindowY >= 0)
            {
                _mapWindow.Position = new SFML.System.Vector2i(Properties.Settings.Default.MapWindowX, Properties.Settings.Default.MapWindowY); 
            } 
            else
            {
                _mapWindow.Position = new SFML.System.Vector2i(Left + Width, Top);
            }
            if (Properties.Settings.Default.MapWindowCustomSize && Properties.Settings.Default.MapWindowWidth >= 0 && Properties.Settings.Default.MapWindowHeight >= 0)
            {
                _mapWindow.Size = new SFML.System.Vector2u((uint)Properties.Settings.Default.MapWindowWidth, (uint)Properties.Settings.Default.MapWindowHeight);
            }
            else if (!Properties.Settings.Default.MapWindowCustomSize && Properties.Settings.Default.MapWindowLastWidth >= 0 && Properties.Settings.Default.MapWindowLastHeight >= 0)
            {
                _mapWindow.Size = new SFML.System.Vector2u((uint)Properties.Settings.Default.MapWindowLastWidth, (uint)Properties.Settings.Default.MapWindowLastHeight);
            }
            _mapCoordinateProviderHandler = new MapCoordinateProviderHandler(_mapWindow, _processHandler, _client);
            
            _mapWindow.Show();
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

        private async void _connectButton_Click(object sender, EventArgs e)
        {
            var form = new ConnectForm();
            if(form.ShowDialog(this) == DialogResult.OK)
            {
                await initClientAsync(form.Address, form.Port);
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

        private async void _createLobbyButton_Click(object sender, EventArgs e)
        {
            if (_client?.IsConnected != true)
                return;

            var form = new CreateLobbyForm(_client, true);
            var req = new Packet(NetConstants.PacketTypes.ClientRequestRoomName, Array.Empty<byte>());
            await _client.SendPacketToServer(req);
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                await _client.CreateRoom(
                    form.RoomName,
                    form.AdminPassword,
                    form.Nickname,
                    form.Color.ToArgb() | 0xFF << 24,
                    form.Team);
            }
        }

        private async void _joinLobbyButton_Click(object sender, EventArgs e)
        {
            if (_client?.IsConnected != true)
                return;

            var form = new CreateLobbyForm(_client, false);
            var req = new Packet(NetConstants.PacketTypes.ClientRequestRoomName, Array.Empty<byte>());
            await _client.SendPacketToServer(req);
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                await _client.JoinRoom(
                    form.RoomName,
                    form.AdminPassword,
                    form.Nickname,
                    form.Color.ToArgb() | 0xFF << 24,
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

        private void _processHandler_StatusChanged(object? sender, StatusEventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                _processMonitorStatusTextBox.Text = e.Status;
                _processMonitorStatusTextBox.BackColor = e.Color;
            }));
        }

        private void _processHandler_CoordinatesChanged(object? sender, MapCoordinateEventArgs e)
        {
            if (_client?.IsConnected == true && _client.Room != null && _client?.LocalUser?.IsSpectator == false)
            {
                var coordinatesPacket = PacketHelper.CreateCoordinatesPacket(e.Coordinates);
                _ = _client.SendPacketToServer(coordinatesPacket);
            }
        }

        private async void _startGameButton_Click(object sender, EventArgs e)
        {
            try
            {
                await tryStartingGameWithoutEAC();
            } 
            catch(Exception ex) 
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

        private void addClientListeners(Client? client)
        {
            if (client == null)
                return;

            client.Connected += client_Connected;
            client.Disconnected += client_Disconnected;
            client.IncomingData += client_IncomingData;
            client.StatusChanged += client_StatusChanged;
            client.RoomChanged += client_RoomChanged;
        }

        private void listenToSettingsChanged()
        {
            Properties.Settings.Default.PropertyChanged += default_PropertyChanged;
        }

        private void default_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(Properties.Settings.Default.ControlBackColor))
            {
                BackColor = Properties.Settings.Default.ControlBackColor;
                _consoleControl.BackColor = Properties.Settings.Default.ControlBackColor;
                _lobbyControl.BackColor = Properties.Settings.Default.ControlBackColor;
            }
        }

        private void client_RoomChanged(object? sender, RoomChangedEventArgs e)
        {
            if (_client == null)
                return;

            void update()
            {
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

        private void removeClientListeners(Client? client)
        {
            if (client == null)
                return;

            client.Connected -= client_Connected;
            client.Disconnected -= client_Disconnected;
            client.IncomingData -= client_IncomingData;
            client.StatusChanged -= client_StatusChanged;
            client.RoomChanged -= client_RoomChanged;
        }

        private void client_Connected(object? sender, EventArgs e)
        {
            updateButtonAvailability();
        }

        private void client_Disconnected(object? sender, StringEventArgs e)
        {
            updateButtonAvailability();
        }

        private void client_IncomingData(object? sender, ObjectEventArgs e)
        {
            if(e.Object is JoinedRoomData roomData)
            {
                updateButtonAvailability();
            }
            if (e.Object is JoinRoomDeniedData)
            {
                updateButtonAvailability();
            }
        }

        private void client_StatusChanged(object? sender, StatusEventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                _clientStatusTextBox.Text = _client.GetConnectionStatusString();
            }));
        }

        private void _openMapButton_Click(object sender, EventArgs e)
        {
            openMapWindow();
        }

        private void _settingsButton_Click(object sender, EventArgs e)
        {
            var settingsDialog = new SettingsDialog();
            var res = settingsDialog.ShowDialog(this);
            if(res == DialogResult.OK)
            {

            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        public static Font GetFontFromSettings(float size, float defaultSize = 12f, Font defaultFont = null)
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
    }
}