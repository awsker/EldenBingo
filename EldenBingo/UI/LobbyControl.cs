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
        private bool _lastPaused;
        private System.Timers.Timer? _timer;

        public LobbyControl() : base()
        {
            InitializeComponent();
            _instance = this;
            _adminHeight = adminControl1.Height;
            
            listenToSettingsChanged();
            Load += lobbyControl_Load;
            
            splitContainer1.SplitterDistance = Width - Convert.ToInt32(200f * this.DefaultScaleFactors().Width);
            _adminInfoLabel.Height = Convert.ToInt32(_adminInfoLabel.Height * this.DefaultScaleFactors().Height);

            SizeChanged += lobbyControl_SizeChanged;
            splitContainer1.Panel1.SizeChanged += bingoPanel_SizeChanged;
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
            Client.AddListener<ServerEntireBingoBoardUpdate>(gotBingoBoard);
            Client.AddListener<ServerUserChat>(userChat);
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
            if (Client?.Room != null)
            {
                showHideAdminControls();
                updateMatchStatus(Client.Room.Match);
                setMatchTimerLabel(Client.Room.Match.TimerString);
                restartAndListenToTimer();
            }
        }

        protected override void RemoveClientListeners()
        {
            Client.OnRoomChanged -= client_RoomChanged;
            Client.RemoveListener<ServerUserChecked>(userChecked);
            Client.RemoveListener<ServerUserJoinedRoom>(userJoined);
            Client.RemoveListener<ServerUserLeftRoom>(userLeft);
            Client.RemoveListener<ServerEntireBingoBoardUpdate>(gotBingoBoard);
            Client.RemoveListener<ServerUserChat>(userChat);
        }

        private void userChecked(ClientModel? _, ServerUserChecked userCheckedArgs)
        {
            if (Client?.Room != null && Client.BingoBoard != null && userCheckedArgs.Index >= 0 && userCheckedArgs.Index < 25)
            {
                var user = Client.Room.GetUser(userCheckedArgs.UserGuid);
                var playerName = user?.Nick ?? "Unknown";
                Color? playerColor = user?.ColorBright;
                Color? checkColor = userCheckedArgs.TeamChecked.HasValue ? BingoConstants.GetTeamColorBright(userCheckedArgs.TeamChecked.Value) : playerColor;

                var square = Client.BingoBoard.Squares[userCheckedArgs.Index];
                updateMatchLog(new[] { playerName, square.Checked ? "marked" : "unmarked", square.Text },
                               new Color?[] { playerColor, null, checkColor }, true);
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

        private void gotBingoBoard(ClientModel? _, ServerEntireBingoBoardUpdate bingoBoardArgs)
        {
            if (bingoBoardArgs.AvailableClasses.Length <= 0)
                return;

            var prep = bingoBoardArgs.AvailableClasses.Length == 1 ? "Required class is:" : "Valid classes are:";
            var strings = new List<string>();
            var colors = new List<Color?>();
            foreach (var cl in bingoBoardArgs.AvailableClasses)
            {
                if (strings.Count == 0)
                    strings.Add(prep);
                else
                    strings.Add(",");
                strings.Add(cl.ToString());
                colors.Add(null);
                colors.Add(BingoConstants.ClassColors[(int)cl]);
            }
            colors.Add(null);
            updateMatchLog(strings.ToArray(), colors.ToArray(), false);
        }

        private void userChat(ClientModel? _, ServerUserChat chatArgs)
        {
            if (Client?.Room != null)
            {
                var user = Client.Room.GetUser(chatArgs.UserGuid);
                if (user != null)
                {
                    updateMatchLog(new[] { user.Nick, ":", chatArgs.Message },
                        new Color?[] { user.ColorBright, null, null }, true);
                }
            }
        }

        private void _scoreboardControl_SizeChanged(object sender, EventArgs e)
        {
            updateScoreboardControlLocationAndSize();
        }

        private void updateScoreboardControlLocationAndSize()
        {
            void update()
            {
                var startPosY = _scoreboardControl.Bottom + 3;
                _logBoxBorderPanel.Location = new Point(_logBoxBorderPanel.Location.X, startPosY);
                _logBoxBorderPanel.Height = _lobbyStatusPanel.Height - _logBoxBorderPanel.Location.Y - 3;
            }
            if (InvokeRequired)
            {
                BeginInvoke(update);
                return;
            }
            update();
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
                _lastPaused = e.NewRoom.Match.Paused;
                updateMatchStatus(e.NewRoom.Match);
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
            if (e.PropertyName == nameof(Properties.Settings.Default.BingoFont) ||
                e.PropertyName == nameof(Properties.Settings.Default.BingoFontSize) ||
                e.PropertyName == nameof(Properties.Settings.Default.BingoFontStyle))
            {
                updateScoreboardFont();
            }
        }

        private void updateScoreboardFont()
        {
            void update()
            {
                var font = MainForm.GetFontFromSettings(_scoreboardControl.Font, 12f);
                if (font != _scoreboardControl.Font)
                {
                    _scoreboardControl.Font = font;
                }
            }
            if(InvokeRequired)
            {
                BeginInvoke(update);
                return;
            }
            update();
        }

        private void initHideLabel()
        {
            var ll = new LinkLabel() { Text = "(Hide)", AutoSize = true, Font = _adminInfoLabel.Font };
            _adminInfoLabel.Controls.Add(ll);
            ll.Location = new Point(_adminInfoLabel.Width - ll.Width, _adminInfoLabel.Height - ll.Height);
            ll.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
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
            updateBingoMaximumSize();
            updateBingoPanelSize();
            updateScoreboardFont();
            updateScoreboardControlLocationAndSize();
        }

        private void lobbyControl_SizeChanged(object? sender, EventArgs e)
        {
            updateBingoPanelSize();
        }

        private void bingoPanel_SizeChanged(object? sender, EventArgs e)
        {
            updateBingoPanelSize();
        }

        private void match_MatchStatusChanged(object? sender, EventArgs e)
        {
            if (Client?.Room != null)
            {
                updateMatchStatus(Client.Room.Match);
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

        private void updateBingoPanelSize()
        {
            var maxWidth = splitContainer1.Panel1.Width - splitContainer1.SplitterWidth - Convert.ToInt32(270f * this.DefaultScaleFactors().Width);
            var maxHeight = splitContainer1.Panel1.Height - (adminControl1.Visible ? _adminHeight : 0);
            if(Properties.Settings.Default.BingoBoardMaximumSize)
            {
                maxWidth = Math.Min(maxWidth, Properties.Settings.Default.BingoMaxSizeX + _bingoControl.Location.X + 3);
                maxHeight = Math.Min(maxHeight, Properties.Settings.Default.BingoMaxSizeY + _bingoControl.Location.Y + 3);
            }
            if(maxWidth > maxHeight * 1.1f)
            {
                maxWidth = (int)(maxHeight * 1.1f);
            }
            else if (maxHeight > maxHeight / 1.1f)
            {
                maxHeight = (int)(maxHeight / 1.1f);
            }
            _bingoBoardPanel.Width = maxWidth;
            updateBingoSize();
        }

        private void updateBingoSize()
        {
            var maxSize = _bingoBoardPanel.Size - new Size(_bingoControl.Location) - new Size(3, 3);
            _bingoControl.Size = maxSize;
        }

        private void clearMatchLog()
        {
            void update()
            {
                _logTextBox.Clear();
            }
            if (InvokeRequired)
            {
                BeginInvoke(update);
                return;
            }
            update();
        }

        private void updateMatchLog(string text, Color color, bool timestamp)
        {
            updateMatchLog(new[] { text }, new Color?[] { color }, timestamp);
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

        private void updateMatchStatus(Match match)
        {
            void update()
            {
                _matchStatusLabel.Text = Match.MatchStatusToString(match.MatchStatus, match.Paused, out var color);
                _matchStatusLabel.ForeColor = color;
                if (_lastMatchStatus != match.MatchStatus || _lastPaused != match.Paused)
                {
                    updateMatchLog(new[] { "Match status changed to", Match.MatchStatusToString(match.MatchStatus, match.Paused, out var color2) }, new Color?[] { null, color2 }, true);
                }
                _lastMatchStatus = match.MatchStatus;
                _lastPaused = match.Paused;
            }
            if (InvokeRequired)
            {
                BeginInvoke(update);
                return;
            }
            update();
        }

        private async void _chatTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                await sendChat();
            }
        }

        private async Task sendChat()
        {
            async Task send()
            {
                var text = _chatTextBox.Text;
                if (Client?.Room == null)
                    updateMatchLog("Not in a room", Color.Red, false);
                if (Client?.Room != null && !string.IsNullOrWhiteSpace(text))
                {
                    var message = new ClientChat(text);
                    await Client.SendPacketToServer(new Packet(message));
                }
                _chatTextBox.Clear();
            }
            if (InvokeRequired)
            {
                BeginInvoke(send, null);
                return;
            }
            await send();
        }

        private void _logTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            openUrl(e.LinkText);
        }

        private void openUrl(string? url)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(url))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                updateMatchLog(ex.Message, Color.Red, false);
            }
        }
    }
}