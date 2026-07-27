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
        private PopoutBoardForm? _popout;

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

                var selectedClient = _instance._clientList.SelectedUser;
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

        public int GetSelectedSquareIndex()
        {
            return _bingoControl.GetSelectedSquareIndex();
        }

        public void OpenPopoutForm()
        {
            if (_popout != null)
                return;
            _popout = new PopoutBoardForm();
            _popout.Client = Client;
            _popout.FormClosed += (o, e) =>
            {
                // Reconnect key bindings to the regular bingo board when the popup form closes
                _bingoControl.ConnectHotkeys();
                _popout.Client = null;
                _popout = null;
            };
            _popout.SetScoreboardFromScoreboard(_scoreboardControl);
            _popout.SetActiveTeams(_bingoControl.ActiveTeams);
            _popout.Show();
            // Disconnect the key bindings for the regular bingo control. Only the popup control will read keys as long as its open
            _bingoControl.DisconnectHotkeys();
        }

        protected override void AddClientListeners()
        {
            if (Client == null)
                return;
            Client.OnRoomChanged += client_RoomChanged;
            Client.AddListener<ServerUserJoinedRoom>(userJoined);
            Client.AddListener<ServerUserLeftRoom>(userLeft);
            Client.AddListener<ServerUserBannedFromRoom>(userBanned);
            Client.AddListener<ServerPromoteToAdmin>(userPromoted);
            Client.AddListener<ServerEntireBingoBoardUpdate>(gotBingoBoard);
            Client.AddListener<ServerUserChat>(userChat);
            Client.AddListener<ServerTeamNameChanged>(teamNameChanged);
            Client.AddListener<ServerUserChangedTeam>(userChangedTeam);
            Client.AddListener<ServerBroadcastMessage>(serverMessage);
            Client.AddListener<ServerMatchEvents>(gotMatchEvents);
            Client.AddListener<ServerEntireMatchLogReceived>(matchLogUpdate);
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
            if (Client == null)
                return;
            Client.OnRoomChanged -= client_RoomChanged;
            Client.RemoveListener<ServerUserJoinedRoom>(userJoined);
            Client.RemoveListener<ServerUserLeftRoom>(userLeft);
            Client.RemoveListener<ServerUserBannedFromRoom>(userBanned);
            Client.RemoveListener<ServerPromoteToAdmin>(userPromoted);
            Client.RemoveListener<ServerEntireBingoBoardUpdate>(gotBingoBoard);
            Client.RemoveListener<ServerUserChat>(userChat);
            Client.RemoveListener<ServerTeamNameChanged>(teamNameChanged);
            Client.RemoveListener<ServerUserChangedTeam>(userChangedTeam);
            Client.RemoveListener<ServerBroadcastMessage>(serverMessage);
            Client.RemoveListener<ServerMatchEvents>(gotMatchEvents);
            Client.RemoveListener<ServerEntireMatchLogReceived>(matchLogUpdate);
        }

        private void gotMatchEvents(ClientModel? model, ServerMatchEvents matchEventMessage)
        {
            foreach (var ev in matchEventMessage.Events)
                logEvent(ev);
        }

        private void logEvent(MatchEvent matchEvent)
        {
            if (Client?.Room != null && Client.BingoBoard != null)
            {
                if (matchEvent.EventType == MatchEventType.Bingo)
                {
                    string linename = "unknown";
                    if (matchEvent.SquareIndex < 5)
                    {
                        linename = $"column {matchEvent.SquareIndex + 1}";
                    }
                    else if (matchEvent.SquareIndex < 10)
                    {
                        linename = $"row {matchEvent.SquareIndex - 4}";
                    }
                    else if (matchEvent.SquareIndex == 10)
                    {
                        linename = $"diagonal TL->BR";
                    }
                    else if (matchEvent.SquareIndex == 11)
                    {
                        linename = $"diagonal BL->TR";
                    }
                    updateMatchLog([matchEvent.Player, $"BINGO on {linename}!"], [BingoConstants.GetTeamColorBright(matchEvent.Team), null], true, matchEvent.Timestamp / 1000);
                }
                else if (matchEvent.SquareIndex >= 0 && matchEvent.SquareIndex < Client.BingoBoard.SquareCount)
                {
                    Color playerColor = matchEvent.EventType == MatchEventType.RefereeCheck ? BingoConstants.AdminSpectatorColor : BingoConstants.GetTeamColorBright(matchEvent.Team);
                    Color checkColor = matchEvent.Team > -1 ? BingoConstants.GetTeamColorBright(matchEvent.Team) : playerColor;
                    var square = Client.BingoBoard.Squares[matchEvent.SquareIndex];
                    updateMatchLog([matchEvent.Player, matchEvent.Checked ? "marked" : "unmarked", square.Text],
                                   [playerColor, null, checkColor], true, matchEvent.Timestamp / 1000);
                }
            }
        }

        private void userJoined(ClientModel? _, ServerUserJoinedRoom userJoinedArgs)
        {
            if (Client?.Room != null)
            {
                updateMatchLog([userJoinedArgs.User.Nick, "joined the lobby"],
                        [userJoinedArgs.User.ColorBright, null], true);
            }
        }

        private void userLeft(ClientModel? _, ServerUserLeftRoom userLeftArgs)
        {
            if (Client?.Room != null)
            {
                updateMatchLog([userLeftArgs.User.Nick, "left the lobby"],
                        [userLeftArgs.User.ColorBright, null], true);
            }
        }

        private void userBanned(ClientModel? _, ServerUserBannedFromRoom userBannedArgs)
        {
            if (Client?.Room != null)
            {
                updateMatchLog([userBannedArgs.User.Nick, "was banned from this lobby by", userBannedArgs.Banner.Nick],
                        [userBannedArgs.User.ColorBright, null, userBannedArgs.Banner.ColorBright], true);
            }
        }

        private void userPromoted(ClientModel? _, ServerPromoteToAdmin userPromotedArgs)
        {
            if (Client?.Room != null)
            {
                // If own user was upgraded to admin, show the admin controls
                if (Client?.ClientGuid == userPromotedArgs.User.Guid && Client.LocalUser != null)
                    // Set admin flag just to be sure, even though it's probably already set by Client.cs
                    Client.LocalUser.IsAdmin = true;
                showHideAdminControls();
                updateMatchLog([userPromotedArgs.User.Nick, "was promoted to admin by", userPromotedArgs.Promoter.Nick],
                       [userPromotedArgs.User.ColorBright, null, userPromotedArgs.Promoter.ColorBright], true);
            }
        }

        private void gotBingoBoard(ClientModel? _, ServerEntireBingoBoardUpdate bingoBoardArgs)
        {
            // Log available classes immediately when entering room and board is fetched
            if (bingoBoardArgs.AvailableClasses.Length > 0)
            {

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
        }

        private void userChat(ClientModel? _, ServerUserChat chatArgs)
        {
            if (Client?.Room != null)
            {
                var user = Client.Room.GetUser(chatArgs.UserGuid);
                if (user != null)
                {
                    updateMatchLog(new[] { user.Nick, ":", chatArgs.Message },
                        [user.ColorBright, null, null], true);
                }
            }
        }

        private void teamNameChanged(ClientModel? model, ServerTeamNameChanged teamNameChanged)
        {
            if (Client?.Room != null)
            {
                var user = Client.Room.GetUser(teamNameChanged.UserGuid);
                if (user != null)
                {
                    var teamColor = BingoConstants.GetTeamColorBright(teamNameChanged.Team);
                    updateMatchLog(
                        [user.Nick, $"changed name of", teamNameChanged.TeamColorName, "to", teamNameChanged.Name],
                        [BingoConstants.GetTeamColorBright(user.Team), null, teamColor, null, teamColor],
                        true);
                }
            }
        }

        private void userChangedTeam(ClientModel? model, ServerUserChangedTeam teamChanged)
        {
            if (Client?.Room != null)
            {
                var user = Client.Room.GetUser(teamChanged.UserGuid);
                if (user != null)
                {
                    var oldTeamColor = BingoConstants.GetTeamColorBright(user.Team);
                    var newTeamColor = BingoConstants.GetTeamColorBright(teamChanged.Team);
                    user.Team = teamChanged.Team;
                    updateMatchLog(
                        [user.Nick, $"changed team to", teamChanged.TeamColorName],
                        [oldTeamColor, null, newTeamColor],
                        true);
                }
            }
        }

        private void serverMessage(ClientModel? model, ServerBroadcastMessage message)
        {
            updateMatchLog([$"Server: {message.Message}"], [Color.Orange], true);
        }

        private void matchLogUpdate(ClientModel? model, ServerEntireMatchLogReceived matchLogResults)
        {
            void saveLog()
            {
                void saveLogInner()
                {
                    if (string.IsNullOrEmpty(matchLogResults.MatchLog))
                    {
                        updateMatchLog("Couldn't get match log", Color.OrangeRed, false);
                        return;
                    }
                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = matchLogResults.SuggestedFilename;
                    saveFileDialog.OverwritePrompt = true;
                    saveFileDialog.Filter = matchLogResults.Json ? "Json File|*.json" : "Text File|*.txt";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            File.WriteAllText(saveFileDialog.FileName, matchLogResults.MatchLog);
                            updateMatchLog("Match log saved", matchLogResults.Json ? _requestJsonLinkLabel.LinkColor : _requestLogLinkLabel.LinkColor, false);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error saving log: " + ex.Message, Application.ProductName, MessageBoxButtons.OK);
                        }
                    }
                }
                if (InvokeRequired)
                {
                    BeginInvoke(saveLogInner);
                    return;
                }
                saveLogInner();
            }
            Task.Run(saveLog);
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
            var match = Client?.Room?.Match;
            if (match != null)
                setMatchTimerLabel(match.TimerString);
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
                updateBingoPanelSize();
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
            if (InvokeRequired)
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
                if (_popout != null)
                {
                    _popout.SetTimerLabel(text);
                }
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
                updateBingoPanelSize();
            }
            if (InvokeRequired)
            {
                BeginInvoke(showHide);
                return;
            }
            showHide();
        }

        private void updateBingoPanelSize()
        {
            var maxWidth = splitContainer1.Panel1.Width - splitContainer1.SplitterWidth - Convert.ToInt32(270f * this.DefaultScaleFactors().Width);
            var maxHeight = splitContainer1.Panel1.Height - (adminControl1.Visible ? _adminHeight : 0);
            if (Properties.Settings.Default.BingoBoardMaximumSize)
            {
                maxWidth = Math.Min(maxWidth, Properties.Settings.Default.BingoMaxSizeX + _bingoControl.Location.X + 3);
                maxHeight = Math.Min(maxHeight, Properties.Settings.Default.BingoMaxSizeY + _bingoControl.Location.Y + 3);
            }
            if (maxWidth > maxHeight * 1.1f)
            {
                maxWidth = (int)(maxHeight * 1.1f);
            }
            else if (maxHeight > maxHeight / 1.1f)
            {
                maxHeight = (int)(maxHeight / 1.1f);
            }
            _bingoBoardPanel.Width = maxWidth;
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

        private void updateMatchLog(string text, Color? color, bool timestamp)
        {
            updateMatchLog([text], [color], timestamp);
        }

        private void updateMatchLog(string[] text, Color?[] color, bool timestamp, int? customTimeStampSeconds = null)
        {
            void update()
            {
                if (timestamp && Client?.Room?.Match != null)
                {
                    if (customTimeStampSeconds.HasValue)
                    {
                        appendText($"[{Match.MatchTimeToString(customTimeStampSeconds.Value)}] ", Color.Gray);
                    }
                    else
                    {
                        appendText($"[{Client.Room.Match.TimerString}] ", Color.Gray);
                    }
                }
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
                // Show the log download link when match status is changed to finish
                // Hide it again when a new match starts
                if (match.MatchStatus == MatchStatus.Finished)
                {
                    _requestLogParentPanel.Visible = true;
                }
                else if (match.MatchStatus >= MatchStatus.Preparation)
                {
                    _requestLogParentPanel.Visible = false;
                }
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

        private void _requestLogLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Client != null && _requestLogParentPanel.Visible)
            {
                _ = Client.SendPacketToServer(new Packet(new ClientRequestEntireMatchLog(false)));
            }
        }

        private void _requestJsonLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Client != null && _requestLogParentPanel.Visible)
            {
                _ = Client.SendPacketToServer(new Packet(new ClientRequestEntireMatchLog(true)));
            }
        }
    }
}