using EldenBingo.Net;
using EldenBingoCommon;
using Neto.Shared;

namespace EldenBingo.UI
{
    internal partial class LobbyControl : ClientUserControl
    {
        private static LobbyControl? _instance;
        private int _adminHeight = 0;
        private MatchStatus _lastMatchStatus;
        private System.Timers.Timer? _timer;

        public LobbyControl() : base()
        {
            InitializeComponent();
            _instance = this;
            _adminHeight = adminControl1.Height;
            listenToSettingsChanged();
            Load += lobbyControl_Load;
            SizeChanged += lobbyControl_SizeChanged;
        }

        public static UserInRoom? CurrentlyOnBehalfOfUser
        {
            get
            {
                if (_instance == null || _instance.Client == null || _instance.Client.LocalUser == null)
                    return null;

                if (_instance.Client.LocalUser.IsAdmin != true || _instance.Client.LocalUser.IsSpectator != true)
                    return _instance.Client.LocalUser;

                var selectedClient = _instance._clientList.SelectedItem as UserInRoom;
                return selectedClient ?? _instance.Client.LocalUser;
            }
        }

        public override Color BackColor
        {
            get => base.BackColor;
            set
            {
                base.BackColor = value;
                _clientList.BackColor = value;
                adminControl1.BackColor = value;
            }
        }

        protected override void AddClientListeners()
        {
            Client.OnRoomChanged += client_RoomChanged;
            Client.AddListener<ServerUserChecked>(userChecked);
            Client.AddListener<ServerUserJoinedRoom>(userJoined);
            Client.AddListener<ServerUserLeftRoom>(userLeft);

        }

        protected override void ClientChanged()
        {
            _bingoControl.Client = Client;
            _clientList.Client = Client;
            _scoreboardControl.Client = Client;
            if (adminControl1 != null)
            {
                adminControl1.Client = Client;
            }
        }

        protected override void RemoveClientListeners()
        {
            Client.OnRoomChanged -= client_RoomChanged;
            Client.RemoveListener<ServerUserChecked>(userChecked);
            Client.RemoveListener<ServerUserJoinedRoom>(userJoined);
            Client.RemoveListener<ServerUserLeftRoom>(userLeft);
        }

        private void userChecked(ClientModel? _, ServerUserChecked userCheckedArgs)
        {
            if (Client?.Room != null && Client.BingoBoard != null && userCheckedArgs.Index >= 0 && userCheckedArgs.Index < 25)
            {
                var user = Client.Room.GetUser(userCheckedArgs.UserGuid);
                var playerName = user?.Nick ?? "Unknown";
                Color? color = user?.ColorBright;

                var square = Client.BingoBoard.Squares[userCheckedArgs.Index];
                updateMatchLog(new[] { playerName, square.Checked ? "marked" : "unmarked", square.Text },
                               new Color?[] { color, null, color }, true);

            }
        }
        
        private void userJoined(ClientModel? _, ServerUserJoinedRoom userJoinedArgs)
        {
            if (Client?.Room != null)
            {
                updateMatchLog(new[] { userJoinedArgs.User.Nick, "joined the lobby" },
                        new Color?[] { userJoinedArgs.User.ColorBright, null }, true);
                
            }
        }

        private void userLeft(ClientModel? _, ServerUserLeftRoom userLeftArgs)
        {
            if (Client?.Room != null)
            {
                updateMatchLog(new[] { userLeftArgs.User.Nick, "left the lobby" },
                        new Color?[] { userLeftArgs.User.ColorBright, null }, true);

            }
        }


        private void _scoreboardControl_SizeChanged(object sender, EventArgs e)
        {
            var startPosY = _scoreboardControl.Bottom + 3;
            var height = panel1.Height - startPosY;
            _logBoxBorderPanel.Location = new Point(_logBoxBorderPanel.Location.X, startPosY);
            _logBoxBorderPanel.Height = height;
        }

        private void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (Client?.Room?.Match != null)
                setMatchTimerLabel(Client.Room.Match.TimerString);
        }

        private void appendText(string text, Color color)
        {
            _logTextBox.SelectionStart = _logTextBox.TextLength;
            _logTextBox.SelectionLength = 0;

            _logTextBox.SelectionColor = color;
            _logTextBox.AppendText(text);
            _logTextBox.SelectionColor = _logTextBox.ForeColor;
        }

        private void client_RoomChanged(object? sender, RoomChangedEventArgs e)
        {
            if (e.PreviousRoom != null)
            {
                e.PreviousRoom.Match.MatchStatusChanged -= match_MatchStatusChanged;
            }
            clearMatchLog();
            showHideAdminControls();
            if (e.NewRoom != null)
            {
                //Set this so it doesn't print the new value
                _lastMatchStatus = e.NewRoom.Match.MatchStatus;
                updateMatchStatus(e.NewRoom.Match.MatchStatus);
                setMatchTimerLabel(e.NewRoom.Match.TimerString);
                restartAndListenToTimer();
                e.NewRoom.Match.MatchStatusChanged += match_MatchStatusChanged;
            }
        }

        private void default_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Properties.Settings.Default.BingoMaxSizeX) ||
                e.PropertyName == nameof(Properties.Settings.Default.BingoMaxSizeY) ||
                e.PropertyName == nameof(Properties.Settings.Default.BingoBoardMaximumSize))
            {
                updateBingoMaximumSize();
                updateBingoSize();
            }
        }

        private void initHideLabel()
        {
            var ll = new LinkLabel() { Text = "(Hide)", AutoSize = true, Font = _adminInfoLabel.Font };
            _adminInfoLabel.Controls.Add(ll);
            ll.Location = new Point(_adminInfoLabel.Width - ll.Width, _adminInfoLabel.Height - ll.Height);
            ll.Click += (o, e) =>
            {
                _adminInfoLabel.Hide();
            };
        }

        private void listenToSettingsChanged()
        {
            Properties.Settings.Default.PropertyChanged += default_PropertyChanged;
        }

        private void lobbyControl_Load(object? sender, EventArgs e)
        {
            initHideLabel();
            updateBingoSize();
            updateBingoMaximumSize();
        }

        private void lobbyControl_SizeChanged(object? sender, EventArgs e)
        {
            updateBingoSize();
        }

        private void match_MatchStatusChanged(object? sender, EventArgs e)
        {
            if (Client?.Room != null)
            {
                updateMatchStatus(Client.Room.Match.MatchStatus);
                restartAndListenToTimer();
            }
        }

        private void match_MatchTimerChanged(object? sender, EventArgs e)
        {
            if (Client?.Room != null)
            {
                setMatchTimerLabel(Client.Room.Match.TimerString);
            }
        }

        private void restartAndListenToTimer()
        {
            if (Client?.Room?.Match != null)
                setMatchTimerLabel(Client.Room.Match.TimerString);

            if (_timer != null)
            {
                _timer.Elapsed -= _timer_Elapsed;
                _timer.Stop();
            }

            _timer = new System.Timers.Timer();
            _timer.Interval = 50;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        private void setMatchTimerLabel(string text)
        {
            void update()
            {
                _timerLabel.Text = text;
            }
            if (InvokeRequired)
            {
                BeginInvoke(update);
                return;
            }
            update();
        }

        private void showHideAdminControls()
        {
            void showHide()
            {
                var isAdmin = Client?.LocalUser?.IsAdmin == true;
                adminControl1.Visible = isAdmin;
                adminControl1.Height = isAdmin ? _adminHeight : 0;
                _adminInfoLabel.Visible = isAdmin && Client?.LocalUser?.IsSpectator == true;
            }
            if (InvokeRequired)
            {
                BeginInvoke(showHide);
                return;
            }
            showHide();
        }

        private void updateBingoMaximumSize()
        {
            if (Properties.Settings.Default.BingoBoardMaximumSize && Properties.Settings.Default.BingoMaxSizeX > 0 && Properties.Settings.Default.BingoMaxSizeY > 0)
            {
                _bingoControl.MaximumSize = new Size(Properties.Settings.Default.BingoMaxSizeX, Properties.Settings.Default.BingoMaxSizeY);
            }
            else
            {
                _bingoControl.MaximumSize = new Size();
            }
        }

        private void updateBingoSize()
        {
            _bingoControl.Size = new Size(panel2.Width - 10, panel2.Height - 10);
        }

        private void clearMatchLog()
        {
            void update()
            {
                _logTextBox.Clear();
            }
            if(InvokeRequired)
            {
                BeginInvoke(update);
                return;
            }
            update();
        }


        private void updateMatchLog(string[] text, Color?[] color, bool timestamp)
        {
            void update()
            {
                if (timestamp && Client?.Room?.Match != null)
                    appendText($"[{Client.Room.Match.TimerString}] ", Color.Gray);
                for (int i = 0; i < text.Length; i++)
                {
                    Color? col = i < color.Length ? color[i] : null;
                    appendText(text[i], col ?? _logTextBox.ForeColor);
                    if (i < text.Length - 1)
                        _logTextBox.AppendText(" ");
                }
                _logTextBox.AppendText(Environment.NewLine);
                _logTextBox.ScrollToCaret();
            }
            if (InvokeRequired)
            {
                BeginInvoke(update);
                return;
            }
            update();
        }

        private void updateMatchStatus(MatchStatus status)
        {
            void update()
            {
                _matchStatusLabel.Text = Match.MatchStatusToString(status, out var color);
                _matchStatusLabel.ForeColor = color;
                if (_lastMatchStatus != status)
                {
                    updateMatchLog(new[] { "Match status changed to", Match.MatchStatusToString(status, out var color2) }, new Color?[] { null, color2 }, true);
                }
                _lastMatchStatus = status;
            }
            if (InvokeRequired)
            {
                BeginInvoke(update);
                return;
            }
            update();
        }
    }
}