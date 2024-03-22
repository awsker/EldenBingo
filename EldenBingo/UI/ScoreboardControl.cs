using EldenBingoCommon;
using Neto.Shared;

namespace EldenBingo.UI
{
    internal class ScoreboardControl : ClientUserControl
    {
        private const int RowPaddingBottom = 3;
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
                updateHeight();
            }
        }

        protected override void AddClientListeners()
        {
            if (Client == null)
                return;

            Client.AddListener<ServerScoreboardUpdate>(scoreBoardUpdate);
        }

        protected override void RemoveClientListeners()
        {
            if (Client == null)
                return;

            Client.RemoveListener<ServerScoreboardUpdate>(scoreBoardUpdate);
        }

        private void scoreboardControl_SizeChanged(object? sender, EventArgs e)
        {
            foreach (var row in _rows)
            {
                row.Width = Width;
            }
        }

        private void scoreBoardUpdate(ClientModel? _, ServerScoreboardUpdate update)
        {
            updateRows(update.Scoreboard);
        }

        private void updateHeight()
        {
            void update()
            {
                var currentY = 0;
                int? squareHeight = null;
                if (_rows.Count == 0)
                {
                    squareHeight = ScoreboardRowControl.SquareSize(Font).Height;
                }
                else
                {
                    foreach (var row in _rows)
                    {
                        row.Location = new Point(0, currentY);
                        if (!squareHeight.HasValue)
                            squareHeight = row.SquareSize().Height;
                        currentY += squareHeight.Value + RowPaddingBottom;
                    }
                }
                Height = Math.Max(2, _rows.Count) * ((squareHeight ?? 0) + RowPaddingBottom);
            }
            if (InvokeRequired)
            {
                BeginInvoke(update);
                return;
            }
            update();
        }

        private void updateRows(TeamScore[] scores)
        {
            void update()
            {
                Controls.Clear();
                _rows.Clear();

                var room = Client?.Room;
                if (room == null)
                    return;

                var currentY = 0;

                int? squareHeight = null;
                foreach (var teamScore in scores)
                {
                    var control = new ScoreboardRowControl();

                    control.Color = BingoConstants.GetTeamColor(teamScore.Team);
                    control.CounterText = teamScore.Score.ToString();
                    control.TextColor = BingoConstants.GetTeamColorBright(teamScore.Team);
                    control.NameText = teamScore.Name;
                    control.Width = Width;
                    control.Font = Font;
                    if (!squareHeight.HasValue)
                        squareHeight = control.SquareSize().Height;
                    control.Height = squareHeight.Value;
                    Controls.Add(control);
                    control.Location = new Point(0, currentY);
                    currentY += squareHeight.Value + RowPaddingBottom;
                    _rows.Add(control);
                }
                updateHeight();
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
            private const int PaddingInsideSquare = 4;
            private const int NameTextMarginLeft = 7;

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

            public Size SquareSize()
            {
                return SquareSize(Font);
            }

            public static Size SquareSize(Font font)
            {
                var measure = TextRenderer.MeasureText("00", font);
                return new Size(Math.Max(35, measure.Width + PaddingInsideSquare), Math.Max(22, measure.Height + PaddingInsideSquare));
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                if (Font == null)
                    return;

                var squareSize = SquareSize();
                Height = squareSize.Height;

                var rect = new Rectangle(0, 0, squareSize.Width, squareSize.Height);
                var nameRect = new Rectangle(rect.Width + NameTextMarginLeft, 0, Math.Max(0, Width - rect.Width - NameTextMarginLeft), rect.Height);

                var g = e.Graphics;
                g.FillRectangle(new SolidBrush(Color), rect);
                g.DrawRectangle(new Pen(Color.FromArgb(96, 0, 0, 0)), new Rectangle(0, 0, rect.Width - 1, rect.Height - 1));
                TextRenderer.DrawText(e, CounterText, Font, rect, Color.White, flags: TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                TextRenderer.DrawText(e, NameText, Font, nameRect, TextColor, flags: TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
        }
    }
}