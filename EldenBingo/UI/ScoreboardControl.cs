using EldenBingoCommon;
using Neto.Shared;

namespace EldenBingo.UI
{
    internal class ScoreboardControl : ClientUserControl
    {
        private IList<ScoreboardRowControl> _rows;

        public ScoreboardControl()
        {
            _rows = new List<ScoreboardRowControl>();
            SizeChanged += scoreboardControl_SizeChanged;
        }

        public override Font Font
        {
            get { return base.Font; }
            set
            {
                base.Font = value;
                foreach (var row in _rows)
                    row.Font = value;
            }
        }

        protected override void AddClientListeners()
        {
            if (Client == null)
                return;

            Client.AddListener<ServerUserChecked>(userChecked);
            Client.AddListener<ServerJoinRoomAccepted>(joinRoomAccepted);
            Client.AddListener<ServerMatchStatusUpdate>(matchStatusUpdate);
            Client.AddListener<ServerUserJoinedRoom>(userJoined);
            Client.AddListener<ServerUserLeftRoom>(userLeft);
        }

        protected override void ClientChanged()
        {
        }

        protected override void RemoveClientListeners()
        {
            if (Client == null)
                return;

            Client.RemoveListener<ServerUserChecked>(userChecked);
            Client.RemoveListener<ServerJoinRoomAccepted>(joinRoomAccepted);
            Client.RemoveListener<ServerMatchStatusUpdate>(matchStatusUpdate);
            Client.RemoveListener<ServerUserJoinedRoom>(userJoined);
            Client.RemoveListener<ServerUserLeftRoom>(userLeft);
        }

        private void userChecked(ClientModel? _, ServerUserChecked userCheckedArgs)
        {
            updateRows();
        }

        private void joinRoomAccepted(ClientModel? _, ServerJoinRoomAccepted joinAccepted)
        {
            updateRows();
        }

        private void matchStatusUpdate(ClientModel? _, ServerMatchStatusUpdate matchStatusUpdate)
        {
            updateRows();
        }

        private void userJoined(ClientModel? _, ServerUserJoinedRoom userJoinedArgs)
        {
            updateRows();
        }

        private void userLeft(ClientModel? _, ServerUserLeftRoom userLeftArgs)
        {
            updateRows();
        }


        private void scoreboardControl_SizeChanged(object? sender, EventArgs e)
        {
            foreach (var row in _rows)
            {
                row.Width = Width;
            }
        }

        private void updateRows()
        {
            void update()
            {
                Controls.Clear();
                _rows.Clear();

                var room = Client?.Room;
                if (room == null)
                    return;

                var currentY = 0;
                const int rowHeight = 25;
                const int rowPaddingBottom = 3;
                foreach (var teamCount in room.GetCheckedSquaresPerTeam())
                {
                    var control = new ScoreboardRowControl();
                    var team = teamCount.Item1;
                    control.Color = BingoConstants.GetTeamColor(team);
                    control.CounterText = teamCount.Item3.ToString();
                    control.TextColor = BingoConstants.GetTeamColorBright(team);
                    control.NameText = teamCount.Item2;
                    control.Width = Width;
                    control.Height = rowHeight;
                    control.Font = Font;
                    Controls.Add(control);
                    control.Location = new Point(0, currentY);
                    currentY += rowHeight + rowPaddingBottom;
                    _rows.Add(control);
                }
                Height = _rows.Count * (rowHeight + rowPaddingBottom);
            }
            if (InvokeRequired)
            {
                BeginInvoke(update);
                return;
            }
            update();
        }

        internal class ScoreboardRowControl : Control
        {
            public ScoreboardRowControl()
            {
                NameText = "";
                CounterText = "0";
                Color = Color.Empty;
            }

            public Color Color { get; set; }
            public string CounterText { get; set; }
            public Color TextColor { get; set; }
            public string NameText { get; set; }
            public int Team { get; set; }

            public void Update(int team, string name, string counter)
            {
                NameText = name;
                CounterText = counter;
                Color = BingoConstants.GetTeamColor(team);
                TextColor = BingoConstants.GetTeamColorBright(team);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                if (Font == null)
                    return;
                const int squareWidth = 35;
                const int padding = 4;
                const int nameTextMarginLeft = 7;
                var g = e.Graphics;
                var measure = TextRenderer.MeasureText(CounterText, Font);
                var rect = new Rectangle(0, 0, Math.Max(squareWidth, measure.Width + padding), Math.Min(Height, measure.Height + padding));
                var nameRect = new Rectangle(rect.Width + nameTextMarginLeft, 0, Math.Max(0, Width - rect.Width - nameTextMarginLeft), rect.Height);
                g.FillRectangle(new SolidBrush(Color), rect);
                g.DrawRectangle(new Pen(Color.FromArgb(96, 0, 0, 0)), new Rectangle(0, 0, Math.Max(squareWidth, measure.Width + padding) - 1, Math.Min(Height, measure.Height + padding - 1)));
                TextRenderer.DrawText(e, CounterText, Font, rect, Color.White, flags: TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                TextRenderer.DrawText(e, NameText, Font, nameRect, TextColor, flags: TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
        }
    }
}