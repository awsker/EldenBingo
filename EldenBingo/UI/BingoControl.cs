﻿using EldenBingo.Net;
using EldenBingo.Properties;
using EldenBingoCommon;
using Neto.Shared;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Timers;

namespace EldenBingo.UI
{
    internal partial class BingoControl : ClientUserControl
    {
        public const float AspectRatio = 1.1f;
        private const float CheckAnimationTimerMax = 4.0f;
        private const float BingoAnimationTimerMax = 3.0f;
        private const int AnimationFPS = 30;

        private static readonly Color BgColor = Color.FromArgb(18, 20, 20);
        private static readonly Color TextColor = Color.FromArgb(232, 230, 227);
        private BoardStatusEnum _boardStatus;
        private string[] _boardStatusStrings = { "Waiting for match to start...", "Click to reveal...", "Match Starting...", "" };
        private bool _revealed = false;
        private BingoSquareControl[] Squares;
        private System.Timers.Timer? _timer;

        private int _size;

        public BingoControl() : base()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            _boardStatusLabel.ForeColor = TextColor;
            _boardStatusLabel.BackColor = BgColor;
            _gridControl.SetAspectRatio(AspectRatio);
            _gridControl.MaintainAspectRatio = true;
            _size = 0;
            Squares = new BingoSquareControl[0];
            initSquareControls(_size);
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

        private void initSquareControls(int size)
        {
            var targetSquares = size * size;
            _gridControl.GridWidth = size;
            _gridControl.GridHeight = size;
            if (_size > size)
            {
                while(_gridControl.Controls.Count > targetSquares)
                {
                    var lastIndex = _gridControl.Controls.Count - 1;
                    _gridControl.Controls.RemoveAt(lastIndex);
                }
            }
            else if(_size < size)
            {
                int i = _gridControl.Controls.Count;
                while (_gridControl.Controls.Count < targetSquares)
                {
                    var squareControl = new BingoSquareControl(i++, string.Empty, string.Empty);
                    squareControl.MouseDown += square_MouseDown;
                    _gridControl.Controls.Add(squareControl);
                }
            }
            if (_size != size) 
            {
                var squareList = new List<BingoSquareControl>(_gridControl.Controls.OfType<BingoSquareControl>());
                Squares = squareList.ToArray();
                _size = size;
            }
        }

        public void FlashBingo(BingoLine bingo)
        {
            void flashSquares(int startx, int starty, int dx, int dy)
            {
                int index(int x, int y) { return y * _size + x; }
                var x = startx;
                var y = starty;
                for (int i = 0; i < _size; ++i)
                {
                    Squares[index(x, y)].BingoAnimationTimer = BingoAnimationTimerMax;
                    x += dx;
                    y += dy;
                }
            }
            switch (bingo.Type)
            {
                case 0:
                    flashSquares(bingo.BingoIndex, 0, 0, 1);
                    break;

                case 1:
                    flashSquares(0, bingo.BingoIndex, 1, 0);
                    break;

                case 2:
                    flashSquares(0, 0, 1, 1);
                    break;

                case 3:
                    flashSquares(0, _size - 1, 1, -1);
                    break;
            }
            startTimer();
        }

        public void UpdateBoard()
        {
            if (Client?.Room?.Match != null)
            {
                setBoard(Client.Room.Match.Board);
            }
        }

        protected override void AddClientListeners()
        {
            if (Client == null)
                return;

            Client.OnRoomChanged += onRoomChanged;
            Client.AddListener<ServerSquareUpdate>(squareUpdate);
            Client.AddListener<ServerMatchStatusUpdate>(matchStatusUpdate);
            Client.AddListener<ServerEntireBingoBoardUpdate>(entireBingoBoardUpdate);
            Client.AddListener<ServerBingoAchievedUpdate>(bingoUpdate);
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
            Client.RemoveListener<ServerSquareUpdate>(squareUpdate);
            Client.RemoveListener<ServerMatchStatusUpdate>(matchStatusUpdate);
            Client.RemoveListener<ServerEntireBingoBoardUpdate>(entireBingoBoardUpdate);
            Client.RemoveListener<ServerBingoAchievedUpdate>(bingoUpdate);
        }

        private void startTimer()
        {
            if (_timer == null)
            {
                _timer = new System.Timers.Timer();
                _timer.Interval = 1000d / AnimationFPS;
                _timer.Elapsed += onTimerTick;
                _timer.Start();
            }
        }

        private void onTimerTick(object? sender, ElapsedEventArgs e)
        {
            var delta = 1.0f / AnimationFPS;
            var maxTimer = 0f;
            foreach (var square in Squares)
            {
                if (square.CheckAnimationTimer > 0)
                {
                    square.CheckAnimationTimer -= delta;
                    square.Invalidate();
                    maxTimer = Math.Max(maxTimer, square.CheckAnimationTimer);
                }
                if (square.BingoAnimationTimer > 0)
                {
                    square.BingoAnimationTimer -= delta;
                    square.Invalidate();
                    maxTimer = Math.Max(maxTimer, square.BingoAnimationTimer);
                }
            }
            if (maxTimer <= 0 && _timer != null)
            {
                _timer.Stop();
                _timer = null;
            }
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

        private void squareUpdate(ClientModel? _, ServerSquareUpdate update)
        {
            if (Client?.BingoBoard != null && update.Index >= 0 && update.Index < _size * _size)
            {
                Client.BingoBoard.Squares[update.Index] = update.Square;
                updateSquareStatus(Client.BingoBoard, update.Index, true);
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

        private void bingoUpdate(ClientModel? _, ServerBingoAchievedUpdate update)
        {
            FlashBingo(update.Bingo);
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
            setupClickHotkey();
        }

        private async void keyPressed(object? sender, KeyEventArgs e)
        {
            if (Squares == null)
                return;

            var key = Properties.Settings.Default.ClickHotkey;
            if (key != 0 && e.KeyValue == key)
            {
                foreach (var square in Squares)
                {
                    if (square.MouseOver)
                    {
                        await clickSquare(square);
                        return;
                    }
                }
            }
        }

        private async void mouseWheel(object? sender, MouseEventArgs e)
        {
            if (Squares == null || e.Delta == 0)
                return;

            foreach (var square in Squares)
            {
                if (square.MouseOver)
                {
                    await changeSquareCounter(square, Math.Clamp(e.Delta, -1, 1));
                    return;
                }
            }
        }

        private void bingoControl_SizeChanged(object? sender, EventArgs e)
        {
            recalculateFontSizeForSquares();
        }

        private void clearBoard()
        {
            void update()
            {
                initSquareControls(0);
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

        private void setupClickHotkey()
        {
            var mainForm = MainForm.GetMainForm(this);
            if (mainForm != null)
            {                
                mainForm.RawInput.KeyPressed += keyPressed;
                mainForm.RawInput.MouseWheel += mouseWheel;
            }
        }

        private UserInRoom? getUserToSetFor()
        {
            return LobbyControl.CurrentlyOnBehalfOfUser;
        }

        private void recalculateFontSizeForSquares()
        {
            var scale = this.DefaultScaleFactors();

            var labelFontSize = Height / 30f;
            _boardStatusLabel.Font = MainForm.GetFontFromSettings(Font, labelFontSize / scale.Height);

            if (Squares == null || Squares.Length == 0)
                return;
            var squareHeight = Squares[0].Height;

            var bingoFontSize = squareHeight / 10f;
            var font = MainForm.GetFontFromSettings(Font, bingoFontSize / scale.Height);
            
            foreach(var square in Squares)
            {
                square.Font = font;
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

                initSquareControls(board.Size);
                recalculateFontSizeForSquares();
                for (int i = 0; i < board.SquareCount; ++i)
                {
                    updateSquare(board, i, false);
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

        private string escapeText(string text)
        {
            return text.Replace("&", "&&");
        }

        private void updateSquare(BingoBoard board, int index, bool highlightNewSquares)
        {
            var s = board.Squares[index];
            Squares[index].Text = escapeText(s.Text);
            Squares[index].ToolTip = s.Tooltip;
            updateSquareStatus(board, index, highlightNewSquares);
        }

        private void updateSquareStatus(BingoBoard board, int index, bool highlightNewSquares)
        {
            var s = board.Squares[index];
            var colorBefore = Squares[index].Color;
            Squares[index].Color = s.Team.HasValue ? BingoConstants.GetTeamColor(s.Team.Value) : Color.Empty;
            Squares[index].Marked = s.Marked;
            Squares[index].Counters = s.Counters;
            if (highlightNewSquares && colorBefore != Squares[index].Color) 
            {
                if (Squares[index].Color != Color.Empty)
                {
                    Squares[index].CheckAnimationTimer = CheckAnimationTimerMax;
                    startTimer();
                }
                else
                {
                    Squares[index].CheckAnimationTimer = 0f;
                }
            }
            Squares[index].Invalidate();
        }

        private async void square_MouseDown(object? sender, MouseEventArgs e)
        {
            if (Client == null || sender is not BingoSquareControl c)
                return;

            //No room or no board set in room
            if (Client.Room?.Match?.Board == null)
                return;

            BingoBoard board = Client.Room.Match.Board;

            if (e.Button == MouseButtons.Left && Client.Room.Match.MatchStatus >= MatchStatus.Running)
            {
                await clickSquare(c);
            }
            if (e.Button == MouseButtons.Right && Client.Room.Match.MatchStatus >= MatchStatus.Preparation)
            {
                await markSquare(c);
            }
        }

        private async Task clickSquare(BingoSquareControl c)
        {
            //No room or no board set in room
            if (Client?.Room?.Match?.Board == null)
                return;

            var userToSetFor = getUserToSetFor();
            if (userToSetFor == null)
                return;

            var square = Client.Room.Match.Board.Squares[c.Index];
            var p = new Packet(new ClientTryCheck(c.Index, userToSetFor.Guid));
            if (square.MaxCount <= 0 || !Properties.Settings.Default.ClickIncrementsCountedSquares)
            {
                await Client.SendPacketToServer(p);
            }
            else
            {
                var currentTeamCount = c.Counters.FirstOrDefault(t => t.Team == userToSetFor.Team).Counter;
                //Will reach the max count with this click, so include a check packet
                if (currentTeamCount + 1 == square.MaxCount)
                {
                    //Increment 1 and check the square
                    p.AddObject(new ClientTrySetCounter(c.Index, 1, userToSetFor.Guid));
                }
                //Already at max count, so decrease counter and include an uncheck packet
                else if (currentTeamCount == square.MaxCount)
                {
                    //Decrement 1 and uncheck the square - Only when the square is currently owned by this user's team
                    if (square.Team == userToSetFor.Team)
                        p.AddObject(new ClientTrySetCounter(c.Index, -1, userToSetFor.Guid));
                }
                else if (square.Team != userToSetFor.Team)
                {
                    //Increment 1 if the team doesn't already own the square
                    p = new Packet(new ClientTrySetCounter(c.Index, 1, userToSetFor.Guid));
                }
                await Client.SendPacketToServer(p);
            }
        }

        private async Task markSquare(BingoSquareControl c)
        {
            //No room or no board set in room
            if (Client?.Room?.Match?.Board == null)
                return;

            var p = new Packet(new ClientTryMark(c.Index));
            await Client.SendPacketToServer(p);
        }

        private async void square_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (sender is not BingoSquareControl c)
                return;

            if (e.Delta != 0)
            {
                var change = Math.Max(-1, Math.Min(1, e.Delta));
                await changeSquareCounter(c, change);
            }
        }

        private async Task changeSquareCounter(BingoSquareControl c, int change)
        {
            //No room or no board set in room
            if (Client?.Room?.Match?.Board == null)
                return;

            var userToSetFor = getUserToSetFor();
            if (userToSetFor == null || userToSetFor.IsSpectator)
                return;

            if (Client.Room?.Match?.Board != null && Client.Room.Match.MatchStatus >= MatchStatus.Running)
            {
                var p = new Packet(new ClientTrySetCounter(c.Index, change, userToSetFor.Guid));
                await Client.SendPacketToServer(p);
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
                    if (match.MatchStatus < MatchStatus.Preparation)
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
            public bool MouseOver;
            public float CheckAnimationTimer;
            public float BingoAnimationTimer;
            private readonly ToolTip _toolTip;
            private Color _color;
            private SquareCounter[] _counters;
            private bool _marked;
            private SolidBrush _brush;

            public BingoSquareControl(int index, string text, string tooltip)
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
                Index = index;
                BackColor = BgColor;
                Text = text;
                _toolTip = new ToolTip();
                ToolTip = tooltip;
                _counters = new SquareCounter[0];
                _brush = new SolidBrush(Color.White);
                var control = this;
                MouseEnter += (o, e) =>
                {
                    MouseOver = true;
                    control.Invalidate();
                };
                MouseLeave += (o, e) =>
                {
                    MouseOver = false;
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

            public SquareCounter[] Counters
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
                base.OnPaint(e);

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
                const int textUp = 2;
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
                    {
                        var widthBetweenLeftAndRight = Width - size.Width;
                        leftXPos = Convert.ToInt32(i * (widthBetweenLeftAndRight / (_counters.Length - 1f)));
                    }
                    int yPos = Height - size.Height;
                    var color = BingoConstants.GetTeamColorBright(c.Team);
                    TextRenderer.DrawText(e, c.Counter.ToString(), counterFont, new Rectangle(leftXPos + 1, yPos + 1, size.Width, size.Height), shadowColor, flags: counterFlags);
                    TextRenderer.DrawText(e, c.Counter.ToString(), counterFont, new Rectangle(leftXPos, yPos, size.Width, size.Height), color, flags: counterFlags);
                }
            }

            private void drawMarkedStar(PaintEventArgs e)
            {
                var scale = Width / 96f;
                var x = 3f * scale;
                var y = 3f * scale;
                var width = Resources.tinystar.Width * scale * 0.7f;
                var height = Resources.tinystar.Height * scale * 0.7f;
                e.Graphics.DrawImage(Resources.tinystar, x, y, width, height);
            }

            private void drawRectangle(PaintEventArgs e)
            {
                var g = e.Graphics;
                bool isChecked = _color.A == 255;
                var color = isChecked ? _color : BgColor;

                if (MouseOver)
                {
                    color = color.Brighten(0.14f);
                }
                _brush.Color = color;
                g.FillRectangle(_brush, new Rectangle(0, 0, Width, Height));

                var gradientColor = isChecked ? Color.FromArgb(35, 0, 0, 0) : Color.FromArgb(50, 0, 0, 0);
                var gBrush = new LinearGradientBrush(new Point(0, 0), new Point(0, Height), gradientColor, Color.Transparent);
                g.FillRectangle(gBrush, new Rectangle(0, 0, Width, Height));

                if (CheckAnimationTimer > 0)
                {
                    var alpha = (1.0f - MathF.Sin(CheckAnimationTimer * 8f) * 0.2f) * invLerp(0.0f, 0.8f, CheckAnimationTimer);
                    var attr = new ImageAttributes();
                    var cm = new ColorMatrix();
                    cm.Matrix33 = alpha;
                    attr.SetColorMatrix(cm);
                    var image = Resources.square_gradient;
                    g.DrawImage(Resources.square_gradient, new Rectangle(0, 0, Width, Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attr);
                }

                //White out the entire tile
                if (BingoAnimationTimer > 0 || CheckAnimationTimer > 0)
                {
                    //Slow fading flash when square is involved in a bingo
                    var frac = BingoAnimationTimer / BingoAnimationTimerMax;
                    //Quick flash if just checked
                    var frac2 = invLerp(CheckAnimationTimerMax - 0.2f, CheckAnimationTimerMax, CheckAnimationTimer) * 0.3f;
                    _brush.Color = Color.FromArgb(Convert.ToInt32(255 * Math.Max(frac, frac2)), 255, 255, 230);
                    g.FillRectangle(_brush, new Rectangle(0, 0, Width, Height));
                }
            }

            private int round(float f)
            {
                return (int)MathF.Round(f);
            }

            private float mod(float x, float m)
            {
                return x > 0 ? x % m : (x % m + m) % m;
            }

            private float invLerp(float a, float b, float v)
            {
                return Math.Clamp((v - a) / (b - a), 0f, 1f);
            }
        }
    }
}