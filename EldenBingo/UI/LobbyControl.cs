using EldenBingo.Net;
using EldenBingo.Net.DataContainers;
using EldenBingoCommon;

namespace EldenBingo.UI
{
    internal partial class LobbyControl : ClientUserControl
    {
        private int _adminHeight = 0;

        public LobbyControl() : base()
        {
            InitializeComponent();
            _adminHeight = adminControl1.Height;
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

        protected override void ClientChanged()
        {
            _bingoControl.Client = Client;
            _clientList.Client = Client;
            if (adminControl1 != null)
            {
                adminControl1.Client = Client;
            }
        }

        protected override void AddClientListeners()
        {
            Client.RoomChanged += client_RoomChanged;
            Client.IncomingData += client_IncomingData;
        }

        protected override void RemoveClientListeners()
        {
            Client.RoomChanged -= client_RoomChanged;
            Client.IncomingData -= client_IncomingData;
        }

        private void client_RoomChanged(object? sender, RoomChangedEventArgs e)
        {
            if (e.PreviousRoom != null)
            {
                e.PreviousRoom.Match.MatchStatusChanged -= match_MatchStatusChanged;
                e.PreviousRoom.Match.MatchTimerChanged -= match_MatchTimerChanged;
            }
            showHideAdminControls();
            if (e.NewRoom != null)
            {
                updateMatchStatus(e.NewRoom.Match.MatchStatus);
                setMatchTimerLabel(e.NewRoom.Match.TimerString);
                e.NewRoom.Match.MatchStatusChanged += match_MatchStatusChanged;
                e.NewRoom.Match.MatchTimerChanged += match_MatchTimerChanged;
                
            }
        }

        private void match_MatchStatusChanged(object? sender, EventArgs e)
        {
            if(Client?.Room != null)
                updateMatchStatus(Client.Room.Match.MatchStatus);
        }

        private void match_MatchTimerChanged(object? sender, EventArgs e)
        {
            if (Client?.Room != null)
            {
                setMatchTimerLabel(Client.Room.Match.TimerString);
            }
        }

        private void client_IncomingData(object? sender, ObjectEventArgs e)
        {
            if (e.PacketType == NetConstants.PacketTypes.ServerBingoBoardCheckChanged && e.Object is CheckChangedData ccd)
            {
                //Can not add this line if no board is set
                if (Client?.Room?.Match.Board == null)
                    return;

                var playerName = ccd.User?.Nick ?? "Unknown";
                Color? color = ccd.User?.ConvertedColor ?? null;

                var square = Client.Room.Match.Board.Squares[ccd.Index];
                var unmarked = square.Color == Color.Empty || square.Color.A < 255;
                updateMatchLog(new[] { playerName, unmarked ? "unmarked" : "marked", square.Text },
                               new Color?[] { color, null, color }, true);
            }
            if(e.PacketType == NetConstants.PacketTypes.ServerMatchStatusChanged && e.Object is MatchStatusData msd)
            {
                updateMatchLog(new[] { "Match status changed to ", Match.MatchStatusToString(msd.Match.MatchStatus, out var color2) },
                               new Color?[] { null, color2 }, true);
            }
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

        private void appendText(string text, Color color)
        {
            _logTextBox.SelectionStart = _logTextBox.TextLength;
            _logTextBox.SelectionLength = 0;

            _logTextBox.SelectionColor = color;
            _logTextBox.AppendText(text);
            _logTextBox.SelectionColor = _logTextBox.ForeColor;
        }

        private void updateMatchStatus(MatchStatus status)
        {
            void update()
            {
                _matchStatusLabel.Text = Match.MatchStatusToString(status, out var color);
                _matchStatusLabel.ForeColor = color;
            }
            if (InvokeRequired)
            {
                BeginInvoke(update);
                return;
            }
            update();
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
            }
            if (InvokeRequired)
            {
                BeginInvoke(showHide);
                return;
            }
            showHide();
        }
    }
}
