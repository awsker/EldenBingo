using EldenBingo.Net.DataContainers;
using EldenBingoCommon;

namespace EldenBingo.UI
{
    internal partial class BingoControl : ClientUserControl
    {
        private BingoSquareControl[] Squares;
        public BingoControl() : base()
        {
            InitializeComponent();
            _gridControl.SetAspectRatio(1.1f);
            _gridControl.MaintainAspectRatio = true;
            Squares = new BingoSquareControl[25];
            for (int i = 0; i < 25; ++i)
            {
                var c = new BingoSquareControl(i, string.Empty, string.Empty);
                Squares[i] = c;
                _gridControl.Controls.Add(c);
                c.MouseDown += square_MouseDown;
            }
            Load += bingoControl_Load;
        }

        private void bingoControl_Load(object? sender, EventArgs e)
        {
            _gridControl.UpdateGrid();
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
                    var p = new Packet(NetConstants.PacketTypes.ClientTryCheck, new byte[] { (byte)c.Index });
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

        protected override void ClientChanged()
        {
            //
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
                e.PacketType == NetConstants.PacketTypes.ServerBingoBoardMarkChanged) &&
                e.Object is CheckChangedData checkData)
            {
                if (checkData.Room.Match.Board != null)
                {
                    setBoard(checkData.Room.Match.Board);
                }
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
                    Squares[i].Color = s.Color;
                    Squares[i].Marked = s.Marked;
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

        private class BingoSquareControl : Control
        {
            private readonly Label _label;
            private readonly ToolTip _toolTip;
            private Color _color;
            private bool _marked;
            private Image _starImage;

            private static readonly Color BgColor = Color.FromArgb(18, 20, 20);
            private static readonly Color TextColor = Color.FromArgb(232, 230, 227);

            private bool _mouseOver;

            public int Index { get; init; }
            /*
            public new string Text
            {
                get { return _label.Text; }
                set
                {
                    _label.Text = value;
                }
            }

            public new Font Font
            {
                get { return _label.Font; }
                set { _label.Font = value; }
            }
            */
            public string ToolTip
            {
                get { return _toolTip.GetToolTip(this); }
                set { _toolTip.SetToolTip(this, string.IsNullOrWhiteSpace(value) ? null : value); }
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

            public BingoSquareControl(int index, string text, string tooltip)
            {
                Index = index;
                BackColor = BgColor;
                Text = text;
                _toolTip = new ToolTip();
                ToolTip = tooltip + "Test test test";
                Controls.Add(_label);
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

            protected override void OnPaint(PaintEventArgs e)
            {
                // Call the OnPaint method of the base class.  
                base.OnPaint(e);
                // Call methods of the System.Drawing.Graphics object.
                var g = e.Graphics;
                var color = _color.A < 255 ? BgColor : _color;
                
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
                const float minWidth = 88f;
                const float maxWidth = 118f;
                const float minFont = 8f;
                const float maxFont = 12f;
                var flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak;
                var frac = Math.Min(Math.Max(0f, (Width - minWidth) / (maxWidth - minWidth)), 1f);
                var fontSize = minFont + (maxFont - minFont) * frac;
                var f = new Font(Font.FontFamily, fontSize);
                TextRenderer.DrawText(e, Text, f, ClientRectangle, Color.White, flags: flags);
                if(Marked)
                {
                    g.DrawImage(Properties.Resources.tinystar, new Point(0, 0));
                }
            }
        }
    }
}
