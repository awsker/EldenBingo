using EldenBingo.Net;
using EldenBingo.Settings;
using EldenBingoCommon;
using Neto.Shared;

namespace EldenBingo.UI
{
    internal partial class AdminControl : ClientUserControl
    {
        private System.Windows.Forms.Timer _hideAdminMessageTimer;

        public AdminControl()
        {
            InitializeComponent();
        }

        public override Color BackColor
        {
            get => base.BackColor;
            set
            {
                base.BackColor = value;
                //_consoleTextBox.BackColor = value;
            }
        }

        protected override void AddClientListeners()
        {
            if (Client == null)
                return;
            Client.Connected += client_Connected;
            Client.Disconnected += client_Disconnected;
            Client.OnRoomChanged += client_RoomChanged;
            Client.AddListener<ServerAdminStatusMessage>(adminStatusMessage);
            Client.AddListener<ServerCurrentGameSettings>(receivedGameSettings);
        }

        protected override void ClientChanged()
        {
            updateButtonsStatus();
        }

        protected override void RemoveClientListeners()
        {
            if (Client == null)
                return;
            Client.Connected -= client_Connected;
            Client.Disconnected -= client_Disconnected;
            Client.OnRoomChanged -= client_RoomChanged;
            Client.RemoveListener<ServerAdminStatusMessage>(adminStatusMessage);
            Client.RemoveListener<ServerCurrentGameSettings>(receivedGameSettings);
        }

        private void _browseJsonButton_Click(object sender, EventArgs e)
        {
            var file = _bingoJsonTextBox.Text;
            var dir = Path.GetDirectoryName(file);
            var dialog = new OpenFileDialog()
            {
                Filter = ".Json Files (*.json)|*.json|All Files (*.*)|*.*",
                InitialDirectory = string.IsNullOrWhiteSpace(dir) ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : dir,
                FileName = string.IsNullOrWhiteSpace(file) || !File.Exists(file) ? string.Empty : _bingoJsonTextBox.Text,
            };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                _bingoJsonTextBox.Text = dialog.FileName;
                Properties.Settings.Default.LastBingoFile = _bingoJsonTextBox.Text;
                Properties.Settings.Default.Save();
            }
        }

        private async void _generateNewBoardButton_Click(object sender, EventArgs e)
        {
            await randomizeNewBoard();
        }

        private async void _pauseMatchButton_Click(object sender, EventArgs e)
        {
            await tryChangeMatchStatus(MatchStatus.Paused);
        }

        private async void _startMatchButton_Click(object sender, EventArgs e)
        {
            await tryChangeMatchStatus(MatchStatus.Starting);
        }

        private async void _stopMatchButton_Click(object sender, EventArgs e)
        {
            await tryChangeMatchStatus(MatchStatus.Finished);
        }

        private async void _uploadJsonButton_Click(object sender, EventArgs e)
        {
            await uploadJsonData(_bingoJsonTextBox.Text);
        }

        private void AdminControl_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                _bingoJsonTextBox.Text = Properties.Settings.Default.LastBingoFile;
            }
        }

        private void client_Connected(object? sender, EventArgs e)
        {
            updateButtonsStatus();
        }

        private void client_Disconnected(object? sender, StringEventArgs e)
        {
            updateButtonsStatus();
        }

        private void adminStatusMessage(ClientModel? _, ServerAdminStatusMessage message)
        {
            updateAdminStatusText(message.Message, Color.FromArgb(message.Color));
        }

        private void client_RoomChanged(object? sender, RoomChangedEventArgs e)
        {
            updateButtonsStatus();
            if (e.PreviousRoom != null)
                e.PreviousRoom.Match.MatchStatusChanged -= match_MatchStatusChanged;
            if (e.NewRoom != null)
                e.NewRoom.Match.MatchStatusChanged += match_MatchStatusChanged;
        }

        private void match_MatchStatusChanged(object? sender, EventArgs e)
        {
            updateButtonsStatus();
        }

        private async Task randomizeNewBoard()
        {
            if (Client?.Room == null)
            {
                errorProvider1.SetError(_generateNewBoardButton, "Not in a room");
                return;
            }
            if (Client?.LocalUser?.IsAdmin != true)
            {
                errorProvider1.SetError(_generateNewBoardButton, "Not admin");
                return;
            }
            errorProvider1.SetError(_bingoJsonTextBox, null);
            var p = new Packet(new ClientRandomizeBoard());
            await Client.SendPacketToServer(p);
        }

        private async Task tryChangeMatchStatus(MatchStatus status)
        {
            if (Client == null)
                return;

            var p = new Packet(new ClientChangeMatchStatus(status));
            await Client.SendPacketToServer(p);
        }

        private void updateAdminStatusText(string text, Color color)
        {
            void update()
            {
                if(_hideAdminMessageTimer != null)
                    _hideAdminMessageTimer.Tick -= _hideAdminMessageTimer_Tick;
                _adminStatusLabel.Text = text;
                _adminStatusLabel.ForeColor = color;
                _hideAdminMessageTimer = new System.Windows.Forms.Timer();
                _hideAdminMessageTimer.Interval = 6000;
                _hideAdminMessageTimer.Tick += _hideAdminMessageTimer_Tick;
                _hideAdminMessageTimer.Start();
            }
            if (InvokeRequired)
            {
                BeginInvoke(update);
                return;
            }
            update();
        }

        private void _hideAdminMessageTimer_Tick(object? sender, EventArgs e)
        {
            hideText();
            _hideAdminMessageTimer.Stop();
        }

        private void hideText()
        {
            void update()
            {
                _adminStatusLabel.Text = string.Empty;
            }
            if (InvokeRequired)
            {
                BeginInvoke(update);
                return;
            }
            update();
        }

        private void updateButtonsStatus()
        {
            void update()
            {
                var inRoom = Client?.Room != null;
                var admin = inRoom && Client?.LocalUser?.IsAdmin == true;
                var matchInProgress = inRoom && (Client.Room.Match.Running || Client.Room.Match.MatchStatus == MatchStatus.Paused);
                _browseJsonButton.Enabled = admin;
                _uploadJsonButton.Enabled = admin;
                _lobbySettingsButton.Enabled = admin;
                _generateNewBoardButton.Enabled = admin;
                _startMatchButton.Enabled = admin && !matchInProgress;
                _pauseMatchButton.Enabled = admin && matchInProgress;
                if (admin)
                    _pauseMatchButton.Text = Client.Room.Match.MatchStatus == MatchStatus.Paused ? "Unpause Match" : "Pause Match";
                _stopMatchButton.Enabled = admin && matchInProgress;
            }
            if (InvokeRequired)
            {
                BeginInvoke(update);
                return;
            }
            update();
        }

        private async Task uploadJsonData(string file)
        {
            if (!File.Exists(file))
            {
                errorProvider1.SetError(_bingoJsonTextBox, "File not found");
                return;
            }
            if (Client?.Room == null)
            {
                errorProvider1.SetError(_uploadJsonButton, "Not in a room");
                return;
            }
            if (Client?.LocalUser?.IsAdmin != true)
            {
                errorProvider1.SetError(_uploadJsonButton, "Not admin");
                return;
            }
            try
            {
                string json = File.ReadAllText(file);
                errorProvider1.SetError(_bingoJsonTextBox, null);
                errorProvider1.SetError(_uploadJsonButton, null);

                var p = new Packet(new ClientBingoJson(json));
                await Client.SendPacketToServer(p);
            }
            catch (IOException ex)
            {
                errorProvider1.SetError(_bingoJsonTextBox, $"Could not read file: {ex.Message}");
            }
        }

        private async void _lobbySettingsButton_Click(object sender, EventArgs e)
        {
            if (Client?.Room == null)
            {
                return; //Not in a room
            }
            var request = new ClientRequestCurrentGameSettings();
            await Client.SendPacketToServer(new Packet(request));
            
        }

        private void receivedGameSettings(ClientModel? _, ServerCurrentGameSettings gameSettingsArgs)
        {
            openSettingsWindow(gameSettingsArgs.GameSettings);
        }

        private void openSettingsWindow(BingoGameSettings settings)
        {
            if (Client?.Room == null)
            {
                return; //Not in a room
            }

            async void openWindow()
            {
                var form = new GameSettingsForm();
                form.Settings = settings;
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    GameSettingsHelper.SaveToSettings(form.Settings, Properties.Settings.Default);
                    var request = new ClientSetGameSettings(form.Settings);
                    await Client.SendPacketToServer(new Packet(request));
                }
            }
            if (InvokeRequired)
            {
                BeginInvoke(openWindow);
                return;
            }
            openWindow();
        }
    }
}