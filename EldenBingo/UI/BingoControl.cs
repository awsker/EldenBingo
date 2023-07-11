using EldenBingo.Net;
using EldenBingoCommon;
using Neto.Shared;
using System.Drawing.Drawing2D;

namespace EldenBingo.UI
{
    internal partial class BingoControl : ClientUserControl
    {
        private static readonly Color BgColor = Color.FromArgb(18, 20, 20);
        private static readonly Color TextColor = Color.FromArgb(232, 230, 227);
        private BoardStatusEnum _boardStatus;
        private string[] _boardStatusStrings = { "Waiting for match to start...", "Click to reveal...", "Match Starting...", "" };
        private bool _revealed = false;
        private BingoSquareControl[] Squares;

        public BingoControl() : base()
        {
            InitializeComponent();
            _boardStatusLabel.ForeColor = TextColor;
            _boardStatusLabel.BackColor = BgColor;
            _gridControl.SetAspectRatio(1.1f);
            _gridControl.MaintainAspectRatio = true;
            Squares = new BingoSquareControl[25];
            for (int i = 0; i < 25; ++i)
            {
                var c = new BingoSquareControl(i, string.Empty, string.Empty);
                Squares[i] = c;
                _gridControl.Controls.Add(c);
                c.MouseDown += square_MouseDown;
                c.MouseWheel += square_MouseWheel;
            }
            Load += bingoControl_Load;
            SizeChanged += bingoControl_SizeChanged;
            _gridControl.SizeChanged += _gridControl_SizeChanged;
            Properties.Settings.Default.PropertyChanged += default_PropertyChanged;
        }

        private enum BoardStatusEnum
        {
            NoBoard,
            BoardSetNotRevealed,
            MatchStarting,
            BoardRevealed,
        }

        protected override void AddClientListeners()
        {
            if (Client == null)
                return;

            Client.OnRoomChanged += onRoomChanged;
            Client.AddListener<ServerUserChecked>(userChecked);
            Client.AddListener<ServerUserMarked>(userMarked);
            Client.AddListener<ServerUserSetCounter>(userSetCounter);
            Client.AddListener<ServerMatchStatusUpdate>(matchStatusUpdate);
            Client.AddListener<ServerEntireBingoBoardUpdate>(entireBingoBoardUpdate);
        }
        
        protected override void ClientChanged()
        {
            if (Client?.Room == null)
                return;

            setBoard(Client.Room.Match.Board);
            updateBoardStatus(Client.Room.Match);
        }

        protected override void RemoveClientListeners()
        {
            if (Client == null)
                return;

            Client.OnRoomChanged -= onRoomChanged;
            Client.RemoveListener<ServerUserChecked>(userChecked);
            Client.RemoveListener<ServerUserMarked>(userMarked);
            Client.RemoveListener<ServerUserSetCounter>(userSetCounter);
            Client.RemoveListener<ServerMatchStatusUpdate>(matchStatusUpdate);
            Client.RemoveListener<ServerEntireBingoBoardUpdate>(entireBingoBoardUpdate);
        }

        private void onRoomChanged(object? sender, RoomChangedEventArgs e)
        {
            _revealed = false;
            if (Client?.BingoBoard == null)
            {
                clearBoard();
            }
            if (Client?.Room?.Match != null)
            {
                updateBoardStatus(Client.Room.Match);
            }
        }

        private void userChecked(ClientModel? _, ServerUserChecked userCheckedArgs)
        {
            if (Client?.BingoBoard != null && userCheckedArgs.Index >= 0 && userCheckedArgs.Index < 25)
            {
                var status = Client.BingoBoard.Squares[userCheckedArgs.Index];
                status.Team = userCheckedArgs.TeamChecked;
                Client.BingoBoard.Squares[userCheckedArgs.Index] = status;
                updateSquareStatus(Client.BingoBoard, userCheckedArgs.Index);
            }
        }

        private void userMarked(ClientModel? _, ServerUserMarked userMarkedArgs)
        {
            if (Client?.BingoBoard != null && userMarkedArgs.Index >= 0 && userMarkedArgs.Index < 25)
            {
                var status = Client.BingoBoard.Squares[userMarkedArgs.Index];
                status.Marked = userMarkedArgs.Marked;
                Client.BingoBoard.Squares[userMarkedArgs.Index] = status;
                updateSquareStatus(Client.BingoBoard, userMarkedArgs.Index);
            }
        }

        private void userSetCounter(ClientModel? _, ServerUserSetCounter userCounterArgs)
        {
            if (Client?.BingoBoard != null && userCounterArgs.Index >= 0 && userCounterArgs.Index < 25)
            {
                var status = Client.BingoBoard.Squares[userCounterArgs.Index];
                status.Counters = userCounterArgs.Counters;
                Client.BingoBoard.Squares[userCounterArgs.Index] = status;
                updateSquareStatus(Client.BingoBoard, userCounterArgs.Index);
            }
        }

        private void matchStatusUpdate(ClientModel? _, ServerMatchStatusUpdate matchStatus)
        {
            if (Client?.Room?.Match != null)
            {
                updateBoardStatus(Client.Room.Match);
            }
        }

        private void entireBingoBoardUpdate(ClientModel? _, ServerEntireBingoBoardUpdate boardUpdate)
        {
            if (Client?.Room == null)
                return;

            setBoard(Client.Room.Match.Board);
            updateBoardStatus(Client.Room.Match);
        }

        private void _boardStatusLabel_Click(object sender, EventArgs e)
        {
            if (_boardStatus == BoardStatusEnum.BoardSetNotRevealed)
            {
                _revealed = true;
                _boardStatus = BoardStatusEnum.BoardRevealed;
                _boardStatusLabel.Text = _boardStatusStrings[(int)_boardStatus];
                _boardStatusLabel.Visible = false;
            }
        }

        private void _gridControl_SizeChanged(object? sender, EventArgs e)
        {
            _boardStatusLabel.Location = new Point(_gridControl.Location.X + _gridControl.BorderX, _gridControl.Location.Y + _gridControl.BorderY);
            _boardStatusLabel.Width = _gridControl.Width - _gridControl.BorderX * 2;
            _boardStatusLabel.Height = _gridControl.Height - _gridControl.BorderY * 2;
        }

        private void bingoControl_Load(object? sender, EventArgs e)
        {
            _gridControl.UpdateGrid();
            recalculateFontSizeForSquares();
        }

        private void bingoControl_SizeChanged(object? sender, EventArgs e)
        {
            recalculateFontSizeForSquares();
        }

        private void clearBoard()
        {
            void update()
            {
                for (int i = 0; i < 25; ++i)
                {
                    Squares[i].Text = string.Empty;
                    Squares[i].ToolTip = string.Empty;
                    Squares[i].Color = Color.Empty;
                    Squares[i].Marked = false;
                    Squares[i].Counters = Array.Empty<TeamCounter>();
                }
                Invalidate();
            }
            if (InvokeRequired)
            {
                BeginInvoke(update);
                return;
            }
            update();
        }

        private void default_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Properties.Settings.Default.BingoFont) ||
                e.PropertyName == nameof(Properties.Settings.Default.BingoFontStyle) ||
                e.PropertyName == nameof(Properties.Settings.Default.BingoFontSize))
            {
                recalculateFontSizeForSquares();
            }
        }

        private UserInRoom? getUserToSetFor()
        {
            return LobbyControl.CurrentlyOnBehalfOfUser;
        }

        private void recalculateFontSizeForSquares()
        {
            const float minHeight = 1f;
            const float maxHeight = 200f;
            const float minFont = 1f;
            const float maxFont = 20f;
            var squareHeight = Squares[0].Height;
            var frac = Math.Clamp((squareHeight - minHeight) / (maxHeight - minHeight), 0f, 1f);
            var fontSize = minFont + (maxFont - minFont) * frac;
            _boardStatusLabel.Font = MainForm.GetFontFromSettings(Font, fontSize * 2f);
            var font = MainForm.GetFontFromSettings(Font, fontSize);
            for (int i = 0; i < 25; ++i)
            {
                Squares[i].Font = font;
            }
        }

        private void setBoard(BingoBoard? board)
        {
            void update()
            {
                if (board == null)
                {
                    clearBoard();
                    return;
                }
                for (int i = 0; i < 25; ++i)
                {
                    updateSquare(board, i);
                }
                Invalidate();
            }
            if (InvokeRequired)
            {
                BeginInvoke(update);
                return;
            }
            update();
        }

        private void updateSquare(BingoBoard board, int index)
        {
            var s = board.Squares[index];
            Squares[index].Text = s.Text;
            Squares[index].ToolTip = s.Tooltip;
            updateSquareStatus(board, index);
        }

        private void updateSquareStatus(BingoBoard board, int index)
        {
            var s = board.Squares[index];
            Squares[index].Color = s.Team.HasValue ? BingoConstants.GetTeamColor(s.Team.Value) : Color.Empty;
            Squares[index].Marked = s.Marked;
            Squares[index].Counters = s.Counters;
            Squares[index].Invalidate();
        }

        private async void square_MouseDown(object? sender, MouseEventArgs e)
        {
            if (Client == null || sender is not BingoSquareControl c)
                return;

            if (e.Button == MouseButtons.Left)
            {
                //Must be in a room
                if (Client.Room?.Match?.Board != null)
                {
                    var userToSetFor = getUserToSetFor();
                    if (userToSetFor == null)
                        return;

                    var p = new Packet(new ClientTryCheck(c.Index, userToSetFor.Guid));
                    await Client.SendPacketToServer(p);
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                //Must be in a room
                if (Client.Room?.Match?.Board != null)
                {
                    var p = new Packet(new ClientTryMark(c.Index));
                    await Client.SendPacketToServer(p);
                }
            }
        }

        private async void square_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (Client == null || sender is not BingoSquareControl c)
                return;

            if (e.Delta != 0)
            {
                //Must be in a room
                if (Client.Room?.Match?.Board != null)
                {
                    var userToSetFor = getUserToSetFor();
                    if (userToSetFor == null || userToSetFor.IsSpectator)
                        return;
                    var change = Math.Max(-1, Math.Min(1, e.Delta));

                    var p = new Packet(new ClientTrySetCounter(c.Index, change, userToSetFor.Guid));
                    await Client.SendPacketToServer(p);
                }
            }
        }

        private void updateBoardStatus(Match match)
        {
            void update()
            {
                if (match.Board == null)
                {
                    _boardStatus = match.MatchStatus == MatchStatus.Starting ? BoardStatusEnum.MatchStarting : BoardStatusEnum.NoBoard;
                }
                else
                {
                    if (match.MatchStatus < MatchStatus.Running)
                    {
                        _boardStatus = BoardStatusEnum.BoardSetNotRevealed;
                    }
                    else
                    {
                        _boardStatus = BoardStatusEnum.BoardRevealed;
                        _revealed = false; //Reset revealed status, so the board is not automatically revealed next time
                    }
                }
                _boardStatusLabel.Text = _boardStatusStrings[(int)_boardStatus];
                _boardStatusLabel.Visible = !_revealed && _boardStatus < BoardStatusEnum.BoardRevealed;
            }
            if (InvokeRequired)
            {
                BeginInvoke(update);
                return;
            }
            update();
        }

        private class BingoSquareControl : Control
        {
            private readonly ToolTip _toolTip;
            private Color _color;
            private TeamCounter[] _counters;
            private bool _marked;
            private bool _mouseOver;

            public BingoSquareControl(int index, string text, string tooltip)
            {
                Index = index;
                BackColor = BgColor;
                Text = text;
                _toolTip = new ToolTip();
                ToolTip = tooltip;
                _counters = new TeamCounter[0];
                var control = this;
                MouseEnter += (o, e) =>
                {
                    _mouseOver = true;
                    control.Invalidate();
                };
                MouseLeave += (o, e) =>
                {
                    _mouseOver = false;
                    control.Invalidate();
                };
            }

            public Color Color
            {
                get { return _color; }
                set
                {
                    if (_color != value)
                    {
                        _color = value;
                        Invalidate();
                    }
                }
            }

            public TeamCounter[] Counters
            {
                get { return _counters; }
                set
                {
                    if (_counters.Length != value.Length)
                    {
                        _counters = value;
                        Invalidate();
                    }
                    else
                    {
                        var changed = false;
                        for (int i = 0; i < value.Length; ++i)
                        {
                            if (!_counters[i].Equals(value[i]))
                            {
                                _counters[i] = value[i];
                                changed = true;
                            }
                        }
                        if (changed)
                            Invalidate();
                    }
                }
            }

            public int Index { get; init; }

            public bool Marked
            {
                get { return _marked; }
                set
                {
                    if (_marked != value)
                    {
                        _marked = value;
                        Invalidate();
                    }
                }
            }

            public string ToolTip
            {
                get { return _toolTip.GetToolTip(this); }
                set
                {
                    _toolTip.AutoPopDelay = 32766;
                    _toolTip.SetToolTip(this, string.IsNullOrWhiteSpace(value) ? null : value);
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                // Call the OnPaint method of the base class.
                base.OnPaint(e);
                // Call methods of the System.Drawing.Graphics object.
                drawRectangle(e);

                drawBingoText(e);

                if (Marked)
                    drawMarkedStar(e);

                bool isChecked = _color.A == 255;
                //If square is checked by any player, don't render counters
                if (isChecked)
                    return;

                drawCounters(e);
            }

            private static string? findLongestWord(Graphics g, string text, Font font)
            {
                string? longest = null;
                float longestLength = 0f;
                foreach (var word in text.Split(" "))
                {
                    var len = g.MeasureString(word, font);
                    if (len.Width > longestLength)
                    {
                        longest = word;
                        longestLength = len.Width;
                    }
                }
                return longest;
            }

            private void drawBingoText(PaintEventArgs e)
            {
                const int textUp = 3;
                var flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak;
                var f = Font;
                Size size = TextRenderer.MeasureText(Text, f, new Size(ClientRectangle.Width, ClientRectangle.Height));
                while (size.Width > ClientRectangle.Width || size.Height > ClientRectangle.Height + textUp)
                {
                    f = new Font(Font.FontFamily, f.Size - .2f, Font.Style);
                    size = TextRenderer.MeasureText(Text, f, new Size(ClientRectangle.Width, ClientRectangle.Height), flags);
                }
                var textRect = new Rectangle(0, 0 - textUp, ClientRectangle.Width, ClientRectangle.Height + textUp);
                var shadowRect = new Rectangle(1, 1 - textUp, ClientRectangle.Width, ClientRectangle.Height + textUp);
                var shadowColor = Color.FromArgb(96, 0, 0, 0);
                TextRenderer.DrawText(e, Text, f, shadowRect, shadowColor, flags);
                TextRenderer.DrawText(e, Text, f, textRect, TextColor, flags);
            }

            private void drawCounters(PaintEventArgs e)
            {
                var counterFont = new Font(Font.FontFamily, Font.Size * 1.2f);
                var counterFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
                var shadowColor = Color.FromArgb(96, 0, 0, 0);
                for (int i = 0; i < _counters.Length; ++i)
                {
                    var c = _counters[i];
                    if (c.Counter == 0)
                        continue;
                    var size = TextRenderer.MeasureText(c.Counter.ToString(), counterFont);
                    int leftXPos;
                    if (i == 0)
                        leftXPos = 0;
                    else if (i == _counters.Length - 1)
                        leftXPos = Width - size.Width;
                    else
                        leftXPos = Convert.ToInt32(Width / (i + 1f) - size.Width / 2f);

                    int yPos = Height - size.Height;
                    var color = BingoConstants.GetTeamColorBright(c.Team);
                    TextRenderer.DrawText(e, c.Counter.ToString(), counterFont, new Rectangle(leftXPos + 1, yPos + 1, size.Width, size.Height), shadowColor, flags: counterFlags);
                    TextRenderer.DrawText(e, c.Counter.ToString(), counterFont, new Rectangle(leftXPos, yPos, size.Width, size.Height), color, flags: counterFlags);
                }
            }

            private void drawMarkedStar(PaintEventArgs e)
            {
                e.Graphics.DrawImage(Properties.Resources.tinystar, new Point(3, 3));
            }

            private void drawRectangle(PaintEventArgs e)
            {
                var g = e.Graphics;
                bool isChecked = _color.A == 255;
                var color = isChecked ? _color : BgColor;

                if (_mouseOver)
                {
                    color = color.Brighten(0.14f);
                }
                var brush = new SolidBrush(color);
                g.FillRectangle(brush, new Rectangle(0, 0, Width, Height));

                var gradientColor = isChecked ? Color.FromArgb(35, 0, 0, 0) : Color.FromArgb(50, 0, 0, 0);
                var gBrush = new LinearGradientBrush(new Point(0, 0), new Point(0, Height), gradientColor, Color.Transparent);
                g.FillRectangle(gBrush, new Rectangle(0, 0, Width, Height));
            }
        }
    }
}