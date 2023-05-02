using EldenBingo.Net.DataContainers;
using EldenBingoCommon;
using System.Drawing.Drawing2D;

namespace EldenBingo.UI
{
    internal partial class BingoControl : ClientUserControl
    {
        private BingoSquareControl[] Squares;
        private float _fontSize;

        private static readonly Color BgColor = Color.FromArgb(18, 20, 20);
        private static readonly Color TextColor = Color.FromArgb(232, 230, 227);

        private BoardStatusEnum _boardStatus;

        private string[] _boardStatusStrings = { "No board set", "Click to reveal...", "Match Starting...", "" };

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

            recalculateFontSizeForSquares();
        }

        private void bingoControl_Load(object? sender, EventArgs e)
        {
            _gridControl.UpdateGrid();
        }

        private void bingoControl_SizeChanged(object? sender, EventArgs e)
        {
            recalculateFontSizeForSquares();
        }

        private void _gridControl_SizeChanged(object? sender, EventArgs e)
        {
            _boardStatusLabel.Location = new Point(_gridControl.Location.X + _gridControl.BorderX, _gridControl.Location.Y + _gridControl.BorderY);
            _boardStatusLabel.Width = _gridControl.Width - _gridControl.BorderX * 2;
            _boardStatusLabel.Height = _gridControl.Height - _gridControl.BorderY * 2;
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

        private void recalculateFontSizeForSquares()
        {
            const float minHeight = 1f;
            const float maxHeight = 200f;
            const float minFont = 1f;
            const float maxFont = 20f;
            var squareHeight = Squares[0].Height;
            var frac = Math.Clamp((squareHeight - minHeight) / (maxHeight - minHeight), 0f, 1f);
            _fontSize = minFont + (maxFont - minFont) * frac;
            _boardStatusLabel.Font = MainForm.GetFontFromSettings(_fontSize * 2f, defaultFont: Font);
            var font = MainForm.GetFontFromSettings(_fontSize, defaultFont: Font);
            for (int i = 0; i < 25; ++i)
            {
                Squares[i].Font = font;
            }
        }

        private async void square_MouseDown(object? sender, MouseEventArgs e)
        {
            if(Client == null || sender is not BingoSquareControl c) 
                return;

            if (e.Button == MouseButtons.Left)
            {
                //Must be in a room 
                if (Client.Room?.Match?.Board != null)
                {
                    var userToSetFor = getUserToSetFor();
                    if (userToSetFor == null)
                        return;
                    var p = PacketHelper.CreateUserCheckPacket((byte)c.Index, userToSetFor.Guid);
                    await Client.SendPacketToServer(p);
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                //Must be in a room 
                if (Client.Room?.Match?.Board != null)
                {
                    var p = new Packet(NetConstants.PacketTypes.ClientTryMark, new byte[] { (byte)c.Index });
                    await Client.SendPacketToServer(p);
                }
            }
        }

        private async void square_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (Client == null || sender is not BingoSquareControl c)
                return;

            if(e.Delta != 0)
            {
                //Must be in a room 
                if (Client.Room?.Match?.Board != null)
                {
                    var userToSetFor = getUserToSetFor();
                    if (userToSetFor == null || userToSetFor.IsSpectator)
                        return;
                    var change = Math.Max(-1, Math.Min(1, e.Delta));
                    var p = PacketHelper.CreateUserSetCountPacket((byte)c.Index, change, userToSetFor.Guid);
                    await Client.SendPacketToServer(p);
                }
            }
        }

        private UserInRoom? getUserToSetFor()
        {
            return LobbyControl.CurrentlyOnBehalfOfUser;
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
            //Checking and marking use the same type of data object, and contain the full board (colors and markings)
            if ((e.PacketType == NetConstants.PacketTypes.ServerBingoBoardCheckChanged || 
                e.PacketType == NetConstants.PacketTypes.ServerBingoBoardMarkChanged ||
                e.PacketType == NetConstants.PacketTypes.ServerBingoBoardCountChanged) &&
                e.Object is CheckChangedData checkData)
            {
                if (checkData.Room.Match.Board != null)
                {
                    setBoard(checkData.Room.Match.Board);
                }
                updateBoardStatus(checkData.Room.Match);
            }
            else if (e.PacketType == NetConstants.PacketTypes.ServerJoinAcceptedRoomData &&
                e.Object is JoinedRoomData roomData)
            {
                if (roomData.Room.Match.Board == null)
                {
                    clearBoard();
                } 
                else
                {
                    setBoard(roomData.Room.Match.Board);
                }
                updateBoardStatus(roomData.Room.Match);
            }
            else if (e.PacketType == NetConstants.PacketTypes.ServerMatchStatusChanged && e.Object is MatchStatusData match)
            {
                if (match.Match.Board == null)
                {
                    clearBoard();
                }
                else
                {
                    setBoard(match.Match.Board);
                }
                updateBoardStatus(match.Match);
            }
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

        private void setBoard(BingoBoard board)
        {
            void update()
            {
                for (int i = 0; i < 25; ++i)
                {
                    var s = board.Squares[i];
                    Squares[i].Text = s.Text;
                    Squares[i].ToolTip = s.Tooltip;
                    Squares[i].Color = s.CheckOwner.Player == Guid.Empty ? Color.Empty : s.CheckOwner.Color;
                    Squares[i].Marked = s.Marked;
                    Squares[i].Counters = s.Counters;
                    Squares[i].Invalidate();
                }
            }
            if (InvokeRequired)
            {
                BeginInvoke(update);
                return;
            }
            update();
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
                    }
                }
                _boardStatusLabel.Text = _boardStatusStrings[(int)_boardStatus];
                _boardStatusLabel.Visible = _boardStatus < BoardStatusEnum.BoardRevealed;
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
            private bool _marked;
            private ColorCounter[] _counters;

            private bool _mouseOver;
            private float _fontSize;

            public BingoSquareControl(int index, string text, string tooltip)
            {
                Index = index;
                BackColor = BgColor;
                Text = text;
                _toolTip = new ToolTip();
                ToolTip = tooltip;
                _counters = new ColorCounter[0];
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

            public int Index { get; init; }

            public string ToolTip
            {
                get { return _toolTip.GetToolTip(this); }
                set {
                    _toolTip.AutoPopDelay = 32766;
                    _toolTip.SetToolTip(this, string.IsNullOrWhiteSpace(value) ? null : value);
                }
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

            public ColorCounter[] Counters
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
                        for(int i = 0; i < value.Length; ++i)
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

            private void drawRectangle(PaintEventArgs e)
            {
                var g = e.Graphics;
                bool isChecked = _color.A == 255;
                var color = isChecked ? _color : BgColor;

                if (_mouseOver)
                {
                    const float brightenFactor = 0.07f;
                    int brighten(int c, float factor)
                    {
                        return Math.Min(255, c + Convert.ToInt32((255 - c) * factor));
                    }
                    color = Color.FromArgb(
                        brighten(color.R, brightenFactor),
                        brighten(color.G, brightenFactor),
                        brighten(color.B, brightenFactor)
                    );
                }
                var brush = new SolidBrush(color);
                g.FillRectangle(brush, new Rectangle(0, 0, Width, Height));
                var h = Convert.ToInt32(Height * 0.4f); //Gradient in bottom 40%
                if (h > 0)
                {
                    var gradientColor = Color.FromArgb(52, 0, 0, 0);
                    var gBrush = new LinearGradientBrush(new Point(0, Height - h - 1), new Point(0, Height), Color.Transparent, gradientColor);
                    g.FillRectangle(gBrush, new Rectangle(0, Height - h, Width, h));
                }
            }

            private void drawBingoText(PaintEventArgs e)
            {
                var flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak;
                var shadowRect = new Rectangle(ClientRectangle.X + 1, ClientRectangle.Y + 1, ClientRectangle.Width, ClientRectangle.Height);
                var shadowColor = Color.FromArgb(128, 0, 0, 0);
                TextRenderer.DrawText(e, Text, Font, shadowRect, shadowColor, flags: flags);
                TextRenderer.DrawText(e, Text, Font, ClientRectangle, TextColor, flags: flags);
            }

            private void drawMarkedStar(PaintEventArgs e)
            {
                e.Graphics.DrawImage(Properties.Resources.tinystar, new Point(3, 3));
            }

            private void drawCounters(PaintEventArgs e)
            {
                var counterFont = new Font(Font.FontFamily, Font.Size * 1.2f);
                var counterFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
                var shadowColor = Color.FromArgb(128, 0, 0, 0);
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
                    TextRenderer.DrawText(e, c.Counter.ToString(), counterFont, new Rectangle(leftXPos + 1, yPos + 1, size.Width, size.Height), shadowColor, flags: counterFlags);
                    TextRenderer.DrawText(e, c.Counter.ToString(), counterFont, new Rectangle(leftXPos, yPos, size.Width, size.Height), c.Color, flags: counterFlags);
                }
            }
        }

        private enum BoardStatusEnum
        {
            NoBoard,
            BoardSetNotRevealed,
            MatchStarting,
            BoardRevealed,
        }

        private void _boardStatusLabel_Click(object sender, EventArgs e)
        {
            if(_boardStatus == BoardStatusEnum.BoardSetNotRevealed)
            {
                _boardStatus = BoardStatusEnum.BoardRevealed;
                _boardStatusLabel.Text = _boardStatusStrings[(int)_boardStatus];
                _boardStatusLabel.Visible = false;
            }
        }
    }
}
