using EldenBingo.Net.DataContainers;
using EldenBingoCommon;

namespace EldenBingo.UI
{
    internal class ScoreboardControl : ClientUserControl
    {
        private Room<UserInRoom>? _room;
        private IList<ScoreboardRowControl> _rows;

        public ScoreboardControl()
        {
            _rows = new List<ScoreboardRowControl>();
            SizeChanged += scoreboardControl_SizeChanged;
        }

        protected override void ClientChanged()
        {
        }

        protected override void AddClientListeners()
        {
            if (Client == null)
                return;

            Client.IncomingData += client_IncomingData;
        }

        protected override void RemoveClientListeners()
        {
            if (Client == null)
                return;

            Client.IncomingData -= client_IncomingData;
        }

        private void client_IncomingData(object? sender, ObjectEventArgs e)
        {
            BingoBoard? board = null;
            //Checking and marking use the same type of data object, and contain the full board (colors and markings)
            if (e.PacketType == NetConstants.PacketTypes.ServerBingoBoardCheckChanged && e.Object is CheckChangedData checkData)
            {
                _room = checkData.Room;
                updateRows();
            }
            else if (e.PacketType == NetConstants.PacketTypes.ServerJoinAcceptedRoomData && e.Object is JoinedRoomData roomData)
            {
                _room = roomData.Room;
                updateRows();
            }
            else if (e.PacketType == NetConstants.PacketTypes.ServerMatchStatusChanged && e.Object is MatchStatusData match)
            {
                updateRows();
            }
            else if ((e.PacketType == NetConstants.PacketTypes.ServerUserJoinedRoom || e.PacketType == NetConstants.PacketTypes.ServerUserLeftRoom) && e.Object is UserJoinedLeftRoomData joinLeftData)
            {
                updateRows();
            }

        }

        private void updateRows()
        {
            void update()
            {
                Controls.Clear();
                _rows.Clear();

                if (_room == null)
                    return;

                var currentY = 0;
                const int rowHeight = 25;
                const int rowPaddingBottom = 3;
                foreach (var teamCount in _room.GetCheckedSquaresPerTeam())
                {
                    var control = new ScoreboardRowControl();
                    var team = teamCount.Item1;
                    control.Color = NetConstants.GetTeamColor(team);
                    control.CounterText = teamCount.Item3.ToString();
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

        private void scoreboardControl_SizeChanged(object? sender, EventArgs e)
        {
            foreach (var row in _rows)
            {
                row.Width = Width;
            }
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

        internal class ScoreboardRowControl : Control
        {
            public int Team { get; set; }
            public string CounterText { get; set; }
            public string NameText { get; set; }
            public Color Color { get; set; }

            public ScoreboardRowControl()
            {
                NameText = "";
                CounterText = "0";
                Color = Color.Empty;
            }

            public void Update(int team, string name, string counter)
            {
                NameText = name;
                CounterText = counter;
                Color = NetConstants.GetTeamColor(team);
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
                TextRenderer.DrawText(e, NameText, Font, nameRect, Color, flags: TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
        }
    }
}
