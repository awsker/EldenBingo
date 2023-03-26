using EldenBingo.Net;
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
        }

        protected override void RemoveClientListeners()
        {
            Client.RoomChanged -= client_RoomChanged;
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
