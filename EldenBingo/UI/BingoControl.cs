using EldenBingo.Net;
using EldenBingo.Properties;
using EldenBingo.Settings;
using EldenBingoCommon;
using Neto.Shared;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Timers;

namespace EldenBingo.UI
{
    internal partial class BingoControl : ClientUserControl
    {
        private const float CheckAnimationTimerMax = 5.0f;
        private const float BingoAnimationTimerMax = 3.0f;
        private const int AnimationFPS = 30;

        private readonly object _concLock = new object();

        private static readonly Color BgColor = Color.FromArgb(18, 20, 20);
        private static readonly Color TextColor = Color.FromArgb(232, 230, 227);

        public Font SquareFont { get; private set; }
        public Font LabelFont { get; private set; }

        private const string NO_BOARD_WAITING = "Waiting for match to start...";
        private const string BOARD_NOT_REVEALED = "Click to reveal...";
        private const string NO_BOARD_STARTING = "Match Starting...";
        
        private string _statusString = string.Empty;
        private bool _statusStringVisible = false;
        private bool _revealed = false;

        private string StatusString
        {
            get { return _statusString; }
            set
            {
                if (_statusString != value)
                {
                    _statusString = value;
                    Invalidate();
                }
            }
        }

        private bool StatusStringVisible
        {
            get { return _statusStringVisible; }
            set
            {
                if (_statusStringVisible != value)
                {
                    _statusStringVisible = value;
                    Invalidate();
                }
            }
        }
        
        private bool Revealed
        {
            get { return _revealed; }
            set
            {
                if (_revealed != value)
                {
                    _revealed = value;
                    Invalidate();
                }
            }
        }

        private IList<BingoSquareControl> Squares;

        private System.Timers.Timer? _timer;

        private int _selectedSquareIndex = -1;
        private int _lastNavigationSelection = 0;
        private int _mouseHoverSquare = -1;

        private float _aspectRatio = 1.1f;
        private int _lineWidth = 2;

        private BingoBoard? _bingoBoard;

        private static Bitmap _starImage;
        private static Bitmap _squareGradient;

        public Color GridColor { get; set; } = Color.FromArgb(118, 110, 97);

        public BingoBoard? BingoBoard
        {
            get { return _bingoBoard; }
            set
            {
                _bingoBoard = value;
                
                if (Client != null && Client.Room != null && Client.Room.Match != null)
                {
                    updateBoardStatus(Client.Room.Match.MatchStatus);
                }
                else
                {
                    updateBoardStatus(MatchStatus.NotRunning);
                }
                updateSquaresArray();
                redrawAllSquares();
            }
        }
        public bool MaintainAspectRatio { get; set; } = true;

        private int[] _activeTeams;
        public int[] ActiveTeams
        {
            get { return _activeTeams; }
            set
            {
                var oldSet = new HashSet<int>(_activeTeams);
                var newSet = new HashSet<int>(value);
                if (!oldSet.SetEquals(newSet))
                {
                    _activeTeams = value;
                    redrawAllSquares();
                }
            }
        }

        public bool Lockout { get; private set; }

        public float AspectRatio
        {
            get { return _aspectRatio; }
            set
            {
                if (_aspectRatio != value)
                {
                    _aspectRatio = value;
                    fixAspectRatio();
                }
            }
        }

        public int LineWidth
        {
            get { return _lineWidth; }
            set
            {
                if (_lineWidth != value)
                {
                    _lineWidth = value;
                    repositionSquares();
                }
            }
        }

        public int BoardSize => BingoBoard?.Size ?? 0;
        public int SquareCount => BingoBoard?.SquareCount ?? 0;

        static BingoControl()
        {
            _starImage = Resources.tinystar;
            _squareGradient = Resources.square_gradient;
        }

        public BingoControl() : base()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            AutoScaleMode = AutoScaleMode.None;
            Squares = new List<BingoSquareControl>();
            _activeTeams = Array.Empty<int>();
            setBoard(null);
            updateSquaresArray();
            initFonts();
            Load += bingoControl_Load;
            Disposed += (o, e) =>
            {
                DisconnectHotkeys();
                disconnectMouseWheel();
            };
            Resize += bingoControl_SizeChanged;
            MouseClick += bingoControl_OnMouseClick;
            MouseMove += bingoControl_OnMouseMove;
            MouseLeave += bingoControl_OnMouseLeave;
            Properties.Settings.Default.PropertyChanged += default_PropertyChanged;
        }

        private void bingoControl_OnMouseMove(object? sender, MouseEventArgs e)
        {
            var sq = getSquareAtPosition(e.Location);
            if (sq == null)
            {
                if (_selectedSquareIndex == _mouseHoverSquare)
                    setSelectedSquare(-1);
                _mouseHoverSquare = -1;
            }
            else
            {
                setSelectedSquare(sq.Index);
                _mouseHoverSquare = sq.Index;
            }
        }

        private void bingoControl_OnMouseLeave(object? sender, EventArgs e)
        {
            if (_selectedSquareIndex == _mouseHoverSquare)
                setSelectedSquare(-1);
            _mouseHoverSquare = -1;
        }

        private void updateSquaresArray()
        {
            lock (_concLock)
            {
                var squareCount = SquareCount;
                if (Squares.Count == squareCount)
                    return;
                while (Squares.Count > squareCount)
                {
                    Squares.RemoveAt(Squares.Count - 1);
                }
                while (Squares.Count < squareCount)
                {
                    var c = new BingoSquareControl(this, Squares.Count);
                    c.OnChanged += bingoSquareChanged;
                    Squares.Add(c);
                }
            }
        }

        private void bingoSquareChanged(object? sender, EventArgs e)
        {
            if (sender is BingoSquareControl c)
                redrawSquare(c);
        }

        private void repositionSquares()
        {
            updateSquaresArray();
            lock (_concLock)
            {
                var w = Width;
                var h = Height;

                var boardSize = BoardSize;
                var squareCount = SquareCount;

                if (w == 0 || h == 0 || boardSize == 0)
                    return;
                var squaresTotalWidth = (w - LineWidth * (boardSize - 1) - 2 * LineWidth);
                var squaresTotalHeight = (h - LineWidth * (boardSize - 1) - 2 * LineWidth);
                var sqrWidth = squaresTotalWidth / boardSize;
                var sqrHeight = squaresTotalHeight / boardSize;

                for (int i = 0; i < squareCount; ++i)
                {
                    var square = Squares[i];
                    var x = i % boardSize;
                    var y = i / boardSize;
                    int xrest = x < squaresTotalWidth % boardSize ? 1 : 0;
                    int yrest = y < squaresTotalHeight % boardSize ? 1 : 0;

                    var squareLocation = new Point(LineWidth + x * sqrWidth + Math.Min(squaresTotalWidth % boardSize, x) + LineWidth * x, LineWidth + y * sqrHeight + Math.Min(squaresTotalHeight % boardSize, y) + LineWidth * y);
                    var squareSize = new Size(sqrWidth + xrest, sqrHeight + yrest);

                    square.Rect = new Rectangle(squareLocation, squareSize);
                }
                initFonts();
                redrawAllSquares();
            }
        }

        private void initFonts()
        {
            var labelFontSize = Height / 30f;
            LabelFont = MainForm.GetFontFromSettings(Font, labelFontSize);

            if (Squares == null || Squares.Count == 0)
                return;
            var squareHeight = Squares[0].Rect.Height;

            var bingoFontSize = squareHeight / 10f;
            SquareFont = MainForm.GetFontFromSettings(Font, bingoFontSize);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var brush = new SolidBrush(GridColor);

            var drawRect = new Rectangle(0, 0, Width, Height);
            drawRect.Intersect(e.ClipRectangle);
            
            g.FillRectangle(brush, drawRect);

            if (StatusStringVisible)
            {
                g.SetClip(new Rectangle(0, 0, Width, Height));
                var innerRect = new Rectangle(LineWidth, LineWidth, Width - LineWidth * 2, Height - LineWidth * 2);
                
                g.FillRectangle(new SolidBrush(BgColor), innerRect);
                var shadows = Properties.Settings.Default.SquareShadows * 0.01f;

                //Draw subtle dark shadow around the edges
                if (Properties.Settings.Default.SquareShadows > 0)
                {
                    var colorMatrix = new ColorMatrix();
                    colorMatrix.Matrix00 = -1f;
                    colorMatrix.Matrix11 = -1f;
                    colorMatrix.Matrix22 = -1f;
                    colorMatrix.Matrix33 = 0.75f * shadows;

                    var imageAttributes = new ImageAttributes();
                    imageAttributes.SetColorMatrix(colorMatrix);

                    g.DrawImage(_squareGradient, innerRect, 0, 0, _squareGradient.Width, _squareGradient.Height, GraphicsUnit.Pixel, imageAttributes);
                }
                if (!string.IsNullOrWhiteSpace(StatusString))
                {
                    var flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak;
                    TextRenderer.DrawText(g, StatusString, LabelFont, innerRect, TextColor, flags);
                }
                return;
            }
            var clipBounds = g.ClipBounds;
            foreach (var square in Squares.Where(s => s.Changed || s.Rect.IntersectsWith(e.ClipRectangle)))
            {
                g.SetClip(square.Rect);
                square.Paint(g);
            }
            g.SetClip(clipBounds);
        }

        private bool fixAspectRatio()
        {
            if (Width == 0 || Height == 0 || !MaintainAspectRatio)
                return true;

            var sizeBefore = Size;
            var actualAsp = (float)Width / (float)Height;
            int targetWidth = Width;
            int targetHeight = Height;
            if (actualAsp > _aspectRatio)
            {
                //Too wide
                targetWidth = Convert.ToInt32(Height * _aspectRatio);
            }
            else if (actualAsp < _aspectRatio)
            {
                //Too tall
                targetHeight = Convert.ToInt32(Width / _aspectRatio);
            }
            Size = new Size(targetWidth, targetHeight);
            bool changed = Size != sizeBefore;
            if (changed)
                redrawAllSquares();

            return changed;
        }

        public void FlashBingo(BingoLine bingo)
        {
            var boardWidth = BingoBoard?.Size ?? 0;
            var squareCount = BingoBoard?.SquareCount ?? 0;

            void flashSquares(int startx, int starty, int dx, int dy)
            {
                var x = startx;
                var y = starty;
                for (int i = 0; i < boardWidth; ++i)
                {
                    Squares[y * boardWidth + x].BingoAnimationTimer = BingoAnimationTimerMax;
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
                    flashSquares(0, boardWidth - 1, 1, -1);
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

        public int GetSelectedSquareIndex()
        {
            return _selectedSquareIndex;
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
            Client.AddListener<ServerScoreboardUpdate>(scoreBoardUpdate);
        }

        private void scoreBoardUpdate(ClientModel? model, ServerScoreboardUpdate update)
        {
            ActiveTeams = update.Scoreboard.Select(t => t.Team).ToArray();
        }

        protected override void ClientChanged()
        {
            if (Client?.Room?.Match == null)
                return;

            setBoard(Client.Room.Match.Board);
            updateBoardStatus(Client.Room.Match.MatchStatus);
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
                    maxTimer = Math.Max(maxTimer, square.CheckAnimationTimer);
                }
                if (square.BingoAnimationTimer > 0)
                {
                    square.BingoAnimationTimer -= delta;
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
            Revealed = false;
            if (Client?.BingoBoard == null)
            {
                setBoard(null);
            }
            if (Client?.Room?.Match != null)
            {
                updateBoardStatus(Client.Room.Match.MatchStatus);
            }
        }

        private void squareUpdate(ClientModel? _, ServerSquareUpdate update)
        {
            var boardSize = BingoBoard?.Size ?? 0;
            if (BingoBoard != null && update.Index >= 0 && update.Index < boardSize * boardSize)
            {
                BingoBoard.Squares[update.Index] = update.Square;
                updateSquareStatus(update.Index, Properties.Settings.Default.CheckHighlight);
            }
        }

        private void matchStatusUpdate(ClientModel? _, ServerMatchStatusUpdate matchStatus)
        {
            updateBoardStatus(matchStatus.MatchStatus);
        }

        private void entireBingoBoardUpdate(ClientModel? _, ServerEntireBingoBoardUpdate boardUpdate)
        {
            if (Client?.Room == null)
                return;

            setBoard(Client.Room.Match.Board);
        }

        private void bingoUpdate(ClientModel? _, ServerBingoAchievedUpdate update)
        {
            if(Properties.Settings.Default.BingoHighlight)
                FlashBingo(update.Bingo);
        }

        private async void bingoControl_OnMouseClick(object? sender, MouseEventArgs e)
        {
            // Clicking does nothing without a board set
            if (BingoBoard == null)
                return;

            // Clicking with a board set but not revealed, reveal the board 
            if (e.Button.HasFlag(MouseButtons.Left) && _statusStringVisible && !_revealed)
            {
                Revealed = true;
                StatusStringVisible = false;
                return;
            }

            var sq = getSquareAtPosition(e.Location);
            if (sq == null)
                return;

            if (e.Button.HasFlag(MouseButtons.Left))
                await checkSquare(sq);

            if (e.Button.HasFlag(MouseButtons.Right))
                await markSquare(sq);
        }

        private void bingoControl_Load(object? sender, EventArgs e)
        {
            if (Parent != null)
            {
                Parent.SizeChanged += Parent_SizeChanged;
            }
            updateSize();
            updateBingoMaximumSize();
            ConnectHotkeys();
            connectMouseWheel();
        }

        private void Parent_SizeChanged(object? sender, EventArgs e)
        {
            updateSize();
        }

        private void updateSize()
        {
            if (Parent == null)
                return;

            try
            {
                SuspendLayout();
                var parentSize = Parent.Size;
                Size = new Size(parentSize.Width - Location.X - 3, parentSize.Height - Location.Y - 3);
                fixAspectRatio();
            }
            finally
            {
                ResumeLayout();
            }
        }

        private async void keyPressed(object? sender, KeyEventArgs e)
        {
            if (Squares == null ||e.KeyCode == Keys.None)
                return;

            var mods = (Keys)Properties.Settings.Default.Hotkey_ModifierKeys;

            // If all defined modifiers are not pressed, ignore the input
            if ((mods & e.Modifiers) != mods)
                return;

            // Handle arrow keys and numpad keys to move selection cursor
            bool handled = false;
            int size = BoardSize;
            if (size > 0 && Squares != null && Squares.Count > 0)
            {
                bool dontMoveCursor = false;
                int selected = _selectedSquareIndex;
                if (selected <= -1)
                {
                    // No square was selected, so highlight the first square (so just ensure a numpad key is pressed, but don't actually move the cursor)
                    dontMoveCursor = true;
                    // And set the selection to the square that was previously selected
                    selected = _lastNavigationSelection;
                }
                int row = selected / size;
                int col = selected % size;
                void moveLeft()
                {
                    col = Math.Clamp(col - 1, 0, size - 1);
                    handled = true;
                }
                void moveRight()
                {
                    col = Math.Clamp(col + 1, 0, size - 1);
                    handled = true;
                }
                void moveUp()
                {
                    row = Math.Clamp(row - 1, 0, size - 1);
                    handled = true;
                }
                void moveDown()
                {
                    row = Math.Clamp(row + 1, 0, size - 1);
                    handled = true;
                }
                void check()
                {
                    var square = getSelectedSquare();
                    if (square != null)
                        _ = checkSquare(square);
                }
                void increment()
                {
                    var square = getSelectedSquare();
                    if (square != null)
                        _ = changeSquareCounter(square, 1);
                }
                void decrement()
                {
                    var square = getSelectedSquare();
                    if (square != null)
                        _ = changeSquareCounter(square, -1);
                }
                void star()
                {
                    var square = getSelectedSquare();
                    if (square != null)
                        _ = markSquare(square);
                }
                var key = e.KeyValue;
                if (key == Properties.Settings.Default.Hotkey_Check) check();
                if (key == Properties.Settings.Default.Hotkey_CountIncrease) increment();
                if (key == Properties.Settings.Default.Hotkey_CountDecrease) decrement();
                if (key == Properties.Settings.Default.Hotkey_Star) star();
                if (key == Properties.Settings.Default.Hotkey_Up) moveUp();
                if (key == Properties.Settings.Default.Hotkey_Down) moveDown();
                if (key == Properties.Settings.Default.Hotkey_Left) moveLeft();
                if (key == Properties.Settings.Default.Hotkey_Right) moveRight();
                if (key == Properties.Settings.Default.Hotkey_UpLeft)
                {
                    moveUp();
                    moveLeft();
                }
                if (key == Properties.Settings.Default.Hotkey_UpRight)
                {
                    moveUp();
                    moveRight();
                }
                if (key == Properties.Settings.Default.Hotkey_DownLeft)
                {
                    moveDown();
                    moveLeft();
                }
                if (key == Properties.Settings.Default.Hotkey_DownRight)
                {
                    moveDown();
                    moveRight();
                }
                if (handled)
                {
                    if (dontMoveCursor)
                    {
                        row = selected / size;
                        col = selected % size;
                    }
                    int newIndex = row * size + col;
                    _lastNavigationSelection = newIndex;
                    setSelectedSquare(newIndex);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
        }

        private BingoSquareControl? getSquareAtPosition(Point position)
        {
            return Squares.FirstOrDefault(sq => sq.Rect.Contains(position));
        }

        private BingoSquareControl? getSelectedSquare()
        {
            return _selectedSquareIndex >= 0 && _selectedSquareIndex < Squares.Count ? Squares[_selectedSquareIndex] : null;
        }

        private void setSelectedSquare(int newIndex)
        {
            if (_selectedSquareIndex == newIndex) return;
            // Deselect old square
            if (_selectedSquareIndex >= 0 && _selectedSquareIndex < Squares.Count)
                Squares[_selectedSquareIndex].Selected = false;
            _selectedSquareIndex = newIndex;
            // Select new square
            if (_selectedSquareIndex >= 0 && _selectedSquareIndex < Squares.Count)
                Squares[_selectedSquareIndex].Selected = true;
        }

        private async void mouseWheel(object? sender, MouseEventArgs e)
        {
            if (Squares == null || e.Delta == 0 || _mouseHoverSquare == -1)
                return;

            if (_mouseHoverSquare >= 0 && _mouseHoverSquare < Squares.Count)
            {
                var square = Squares[_mouseHoverSquare];
                await changeSquareCounter(square, Math.Clamp(e.Delta, -1, 1));
            }
        }

        private void bingoControl_SizeChanged(object? sender, EventArgs e)
        {
            repositionSquares();
            initFonts();
        }

        private void default_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Properties.Settings.Default.BingoMaxSizeX) ||
                e.PropertyName == nameof(Properties.Settings.Default.BingoMaxSizeY) ||
                e.PropertyName == nameof(Properties.Settings.Default.BingoBoardMaximumSize))
            {
                updateBingoMaximumSize();
            }
            if (e.PropertyName == nameof(Properties.Settings.Default.BingoFont) ||
                e.PropertyName == nameof(Properties.Settings.Default.BingoFontStyle) ||
                e.PropertyName == nameof(Properties.Settings.Default.BingoFontSize))
            {
                initFonts();
                redrawAllSquares();
            }
            if(e.PropertyName == nameof(Properties.Settings.SquareShadows))
            {
                redrawAllSquares();
            }
            if (e.PropertyName == nameof(Properties.Settings.KeywordColorsJson) || e.PropertyName == nameof(Properties.Settings.KeywordColorsAlpha))
            {
                foreach (var square in Squares)
                {
                    square.UpdateKeywordColor();
                }
                redrawAllSquares();
            }
        }

        private void updateBingoMaximumSize()
        {
            var sizeBefore = Size;
            if (Properties.Settings.Default.BingoBoardMaximumSize && Properties.Settings.Default.BingoMaxSizeX > 0 && Properties.Settings.Default.BingoMaxSizeY > 0)
            {
                MaximumSize = new Size(Properties.Settings.Default.BingoMaxSizeX, Properties.Settings.Default.BingoMaxSizeY);
            }
            else
            {
                MaximumSize = new Size();
            }
            if (sizeBefore != Size)
            {
                updateSize();
            }
        }

        private void redrawSquare(BingoSquareControl sq)
        {
            sq.Changed = true;
            Invalidate(sq.Rect);
        }

        private void redrawAllSquares()
        {
            foreach (var square in Squares)
            {
                square.Changed = true;
            }
            Invalidate();
        }

        private void connectMouseWheel()
        {
            var mainForm = MainForm.GetMainForm(this);
            if (mainForm != null)
            {
                mainForm.RawInput.MouseWheel += mouseWheel;
            }
        }

        private void disconnectMouseWheel()
        {
            var mainForm = MainForm.GetMainForm(this);
            if (mainForm != null)
            {
                mainForm.RawInput.MouseWheel -= mouseWheel;
            }
        }

        public void ConnectHotkeys()
        {
            var mainForm = MainForm.GetMainForm(this);
            if (mainForm != null)
            {                
                mainForm.RawInput.KeyPressed += keyPressed;
            }
        }

        public void DisconnectHotkeys()
        {
            var mainForm = MainForm.GetMainForm(this);
            if (mainForm != null)
            {
                mainForm.RawInput.KeyPressed -= keyPressed;
            }
        }

        private UserInRoom? getUserToSetFor()
        {
            return LobbyControl.CurrentlyOnBehalfOfUser;
        }

        private void setBoard(BingoBoard? board)
        {
            void update()
            {
                BingoBoard = board;
                if (board == null)
                {
                    if (Client?.Room == null)
                    {
                        StatusString = string.Empty;
                        StatusStringVisible = true;
                        Revealed = false;
                    }
                    return;
                }
                var squareCount = board.SquareCount;
                for (int i = 0; i < squareCount; ++i)
                {
                    updateSquare(i, false);
                }
                _selectedSquareIndex = -1;
                _lastNavigationSelection = 0;
                repositionSquares();
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

        private void updateSquare(int index, bool highlightNewSquares)
        {
            if (BingoBoard == null || index < 0 || index >= BingoBoard.SquareCount)
                return;
            var s = BingoBoard.Squares[index];
            Squares[index].Text = escapeText(s.Text);
            updateSquareStatus(index, highlightNewSquares);
        }

        private void updateSquareStatus(int index, bool highlightNewSquares)
        {
            if (BingoBoard == null || index < 0 || index >= BingoBoard.SquareCount)
                return;

            var s = BingoBoard.Squares[index];
            var teamsBefore = Squares[index].Teams;
            var square = Squares[index];
            square.Teams = s.Team;
            square.Marked = s.Marked;
            square.Counters = s.Counters;
            if (highlightNewSquares)
            {
                //If a new team checked the square (more teams present than before)
                if (s.Team.Length > teamsBefore.Length)
                {
                    //Start check animation
                    Squares[index].CheckAnimationTimer = CheckAnimationTimerMax;
                    startTimer();
                }
                //If a team check was removed (less teams present than before)
                else if (s.Team.Length < teamsBefore.Length)
                {
                    //Stop check animation
                    Squares[index].CheckAnimationTimer = 0f;
                }
            }
            redrawSquare(square);
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
                await checkSquare(c);
            }
            if (e.Button == MouseButtons.Right && Client.Room.Match.MatchStatus >= MatchStatus.Preparation)
            {
                await markSquare(c);
            }
        }

        private void square_MouseEntered(object? sender, EventArgs e)
        {
            // When a square is entered, deselect all other squares
            if(sender is BingoSquareControl square)
            {
                setSelectedSquare(square.Index);
                _mouseHoverSquare = square.Index;
            }
        }

        private void square_MouseLeave(object? sender, EventArgs e)
        {
            // When a square is entered, deselect all other squares
            if (sender is BingoSquareControl square)
            {
                if (_selectedSquareIndex == square.Index)
                    setSelectedSquare(-1);
                if (_mouseHoverSquare == square.Index)
                    _mouseHoverSquare = -1;
            }
        }

        private async Task checkSquare(BingoSquareControl c)
        {
            //No room or no board set in room
            if (Client?.Room?.Match?.Board == null)
                return;

            var userToSetFor = getUserToSetFor();
            if (userToSetFor == null)
                return;

            var square = Client.Room.Match.Board.Squares[c.Index];
            var p = new Packet(new ClientTryCheck(c.Index, userToSetFor.Guid));
            await Client.SendPacketToServer(p);
        }

        private async Task markSquare(BingoSquareControl c)
        {
            //No room or no board set in room
            if (Client?.Room?.Match?.Board == null)
                return;

            var p = new Packet(new ClientTryMark(c.Index));
            await Client.SendPacketToServer(p);
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

        private void updateBoardStatus(MatchStatus status)
        {
            void update()
            {
                if (_bingoBoard == null)
                {
                    StatusString = status == MatchStatus.Starting ? NO_BOARD_STARTING : NO_BOARD_WAITING;
                    StatusStringVisible = true;
                }
                else
                {
                    if (!Revealed && status < MatchStatus.Preparation)
                    {
                        StatusString = BOARD_NOT_REVEALED;
                        StatusStringVisible = true;
                    }
                    else
                    {
                        StatusString = string.Empty;
                        StatusStringVisible = false;
                        // If match has started proper, reset the Revealed status in preparation for next game
                        if (status >= MatchStatus.Preparation)
                            Revealed = false;
                        redrawAllSquares();
                    }
                }
            }
            if (InvokeRequired)
            {
                BeginInvoke(update);
                return;
            }
            update();
        }

        private class BingoSquareControl
        {
            private BingoControl _parent;

            // This is always set by parent so don't bother notifying change, the parent will know
            private Rectangle _rect;
            public Rectangle Rect
            {
                get { return _rect; }
                set
                {
                    _rect = value;
                    if (_rect.Height > 0)
                    {
                        _gradientBrush = new LinearGradientBrush(new Point(0, _rect.Y), new Point(0, _rect.Y + _rect.Height), Color.Transparent, Color.Transparent);
                    };
                }
            }

            private float _checkAnimationTimer;
            private float _bingoAnimationTimer;

            public float CheckAnimationTimer
            {
                get { return _checkAnimationTimer; }
                set
                {
                    _checkAnimationTimer = value;
                    onChanged();
                }
            }
            public float BingoAnimationTimer
            {
                get { return _bingoAnimationTimer; }
                set
                {
                    _bingoAnimationTimer = value;
                    onChanged();
                }
            }
            private int[] _teams;
            private SquareCounter[] _counters;
            private bool _marked;
            private SolidBrush _brush;
            private LinearGradientBrush _gradientBrush;
            private ImageAttributes _imageAttributes;
            private ColorMatrix _colorMatrix;

            private Color? _keywordColor;

            private bool _mouseOver;
            public bool Selected
            {
                get { return _mouseOver; }
                set
                {
                    if (_mouseOver != value)
                    {
                        _mouseOver = value;
                        onChanged();
                    }
                }
            }
            
            public bool Changed;

            public event EventHandler OnChanged;

            public string _text;
            public string Text
            {
                get { return _text; }
                set
                {
                    if (_text != value)
                    {
                        _text = value;
                        UpdateKeywordColor();
                        onChanged();
                    }
                }
            }

            public BingoSquareControl(BingoControl parent, int index)
            {
                _parent = parent;
                Index = index;
                _teams = Array.Empty<int>();
                _counters = new SquareCounter[0];
                _brush = new SolidBrush(Color.White);
                _imageAttributes = new ImageAttributes();
                _colorMatrix = new ColorMatrix();
                _imageAttributes.SetColorMatrix(_colorMatrix);
                
            }

            private void onChanged()
            {
                Changed = true;
                OnChanged?.Invoke(this, EventArgs.Empty);
            }

            private void onTextChanged(object? sender, EventArgs e)
            {
                UpdateKeywordColor();
            }

            public void UpdateKeywordColor()
            {
                var oldValue = _keywordColor;
                _keywordColor = KeywordColorsJsonHelper.GetColor(Text)?.Color ?? null;
            }

            private float keywordColorAlpha()
            {
                return Math.Clamp(Properties.Settings.Default.KeywordColorsAlpha * 0.01f, 0f, 1f);
            }

            public int[] Teams
            {
                get { return _teams; }
                set
                {
                    var oldSet = new HashSet<int>(_teams);
                    var newSet = new HashSet<int>(value);
                    _teams = value;
                    if (!oldSet.SetEquals(newSet))
                    {
                        onChanged();
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
                        onChanged();
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
                            onChanged();
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
                        onChanged();
                    }
                }
            }

            public void Paint(Graphics g)
            {
                Changed = false;
                drawRectangle(g, Rect);

                if (_parent == null) return;
                
                drawBingoText(g, Rect);

                if (Marked)
                    drawMarkedStar(g, Rect);

                //Don't draw counters in lockout mode if a square is claimed
                if (!_parent.Lockout || Teams.Length == 0)
                    drawCounters(g, Rect);
            }

            private void drawRectangle(Graphics g, Rectangle rect)
            {
                if (_parent == null) return;

                bool isChecked = _teams.Length > 0;
                //Draw empty background
                void drawBackground()
                {
                    Color color = BgColor;
                    if (Selected)
                    {
                        color = color.Brighten(0.14f);
                    }
                    _brush.Color = color;
                    g.FillRectangle(_brush, rect);
                }
                if (_teams.Length == 0)
                {
                    drawBackground();
                }
                else
                {
                    Dictionary<int, int> teamIndex = new Dictionary<int, int>();
                    //In lockout, or if no active teams are set, draw the teams in the order they appear in the "selected" list
                    if (_parent.Lockout || _parent.ActiveTeams.Length == 0)
                    {
                        for (int i = 0; i < _teams.Length; ++i)
                        {
                            teamIndex[_teams[i]] = i;
                        }
                    }
                    //In non-lockout, order the teams according to the active teams list, so we get gaps for teams that have not checked the square
                    else
                    {
                        var teamArray = _parent.ActiveTeams.ToArray();
                        for (int i = 0; i < teamArray.Length; ++i)
                        {
                            teamIndex[teamArray[i]] = i;
                        }
                    }
                    var numTeams = teamIndex.Count;
                    if (numTeams > 0)
                    {
                        // If not all teams have checked this square, draw the background so it shows behind the pie
                        if (_teams.Length < _parent.ActiveTeams.Length)
                            drawBackground();

                        if (numTeams > 1)
                            g.SmoothingMode = SmoothingMode.AntiAlias;
                        foreach (var team in _teams)
                        {
                            //Find the index of the team, if not present (should never happen) continue to next team
                            if (!teamIndex.TryGetValue(team, out var i))
                                continue;

                            Color color = BingoConstants.GetTeamColor(team);
                            if (Selected)
                            {
                                color = color.Brighten(0.14f);
                            }
                            _brush.Color = color;

                            if (numTeams == 1)
                            {
                                g.FillRectangle(_brush, rect);
                            }
                            else
                            {
                                var angleAdd = 360f / numTeams;
                                var angleStart = numTeams % 2 == 0 ? 270f - (numTeams == 2 ? 45f : angleAdd / 2f) : 270f;
                                g.SmoothingMode = SmoothingMode.AntiAlias;
                                g.FillPie(_brush, new Rectangle(rect.X - rect.Width, rect.Y - rect.Height, rect.Width * 3, rect.Height * 3), angleStart + angleAdd * i, angleAdd);
                            }
                        }
                        g.SmoothingMode = SmoothingMode.None;
                    }
                }
                var shadows = Properties.Settings.Default.SquareShadows * 0.01f;
                var gradientColor = isChecked ? Color.FromArgb(Convert.ToInt32(120 * shadows), 0, 0, 0) : Color.FromArgb(Convert.ToInt32(150 * shadows), 0, 0, 0);
                _gradientBrush.LinearColors = new[] { gradientColor, Color.Transparent };
                g.FillRectangle(_gradientBrush, rect);

                //Draw subtle dark shadow around the edges
                if (Properties.Settings.Default.SquareShadows > 0)
                {
                    _colorMatrix.Matrix00 = -1f;
                    _colorMatrix.Matrix11 = -1f;
                    _colorMatrix.Matrix22 = -1f;
                    _colorMatrix.Matrix33 = 0.75f * shadows;
                    _imageAttributes.SetColorMatrix(_colorMatrix);
                    g.DrawImage(_squareGradient, rect, 0, 0, _squareGradient.Width, _squareGradient.Height, GraphicsUnit.Pixel, _imageAttributes);
                }

                if (Properties.Settings.Default.CheckHighlight && CheckAnimationTimer > 0)
                {
                    var alpha = (1.0f - MathF.Sin(CheckAnimationTimer * 8f) * 0.2f) * invLerp(0.0f, 0.8f, CheckAnimationTimer);
                    _colorMatrix.Matrix00 = 1f;
                    _colorMatrix.Matrix11 = 1f;
                    _colorMatrix.Matrix22 = 1f;
                    _colorMatrix.Matrix33 = alpha;
                    _imageAttributes.SetColorMatrix(_colorMatrix);
                    g.DrawImage(_squareGradient, rect, 0, 0, _squareGradient.Width, _squareGradient.Height, GraphicsUnit.Pixel, _imageAttributes);
                }

                //White out the entire tile
                if (BingoAnimationTimer > 0 || CheckAnimationTimer > 0)
                {
                    //Slow fading flash when square is involved in a bingo (but only if enabled in settings)
                    var frac = Properties.Settings.Default.BingoHighlight ? BingoAnimationTimer / BingoAnimationTimerMax : 0f;
                    //Quick flash if just checked (but only if enabled in settings)
                    var frac2 = Properties.Settings.Default.CheckHighlight ? invLerp(CheckAnimationTimerMax - 0.3f, CheckAnimationTimerMax, CheckAnimationTimer) * 0.4f : 0f;
                    _brush.Color = Color.FromArgb(Convert.ToInt32(255 * Math.Max(frac, frac2)), 255, 255, 230);
                    g.FillRectangle(_brush, rect);
                }
            }


            private void drawBingoText(Graphics g, Rectangle rect)
            {
                if (_parent == null) return;

                const int TextOffsetX = 0;
                const int TextOffsetY = -2;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                var flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak;
                var f = _parent.SquareFont;
                Size size = TextRenderer.MeasureText(Text, f, rect.Size, flags);
                while (size.Width > rect.Width || size.Height > rect.Height + TextOffsetY)
                {
                    f = new Font(f.FontFamily, f.Size - .2f, f.Style);
                    size = TextRenderer.MeasureText(Text, f, rect.Size, flags);
                }
                int textShadowOffset = Math.Max(1, 1 + rect.Height / 90);
                var textRect = new Rectangle(rect.X + TextOffsetX, rect.Y + TextOffsetY, rect.Width, rect.Height);
                var shadowRect = new Rectangle(rect.X + TextOffsetX + textShadowOffset, rect.Y + TextOffsetY + textShadowOffset, rect.Width, rect.Height);
                var shadowColor = Color.FromArgb(96, 0, 0, 0);
                TextRenderer.DrawText(g, Text, f, shadowRect, shadowColor, flags);
                //If a keyword color is set for this square (meaning a matching keyword rule was found)
                //and no team has claimed the square yet:
                //Color the text with the keyword color
                var tc = TextColor;
                if (_keywordColor != null && _teams.Length == 0)
                {
                    tc = blend(_keywordColor.Value, TextColor, keywordColorAlpha());
                }
                TextRenderer.DrawText(g, Text, f, textRect, tc, flags);
            }

            private static Color blend(Color color, Color backColor, double amount)
            {
                byte r = (byte)(color.R * amount + backColor.R * (1 - amount));
                byte g = (byte)(color.G * amount + backColor.G * (1 - amount));
                byte b = (byte)(color.B * amount + backColor.B * (1 - amount));
                return Color.FromArgb(r, g, b);
            }

            private void drawCounters(Graphics g, Rectangle rect)
            {
                if (_parent == null) return;

                var f = _parent.SquareFont;
                var spectator = _parent.Client?.LocalUser?.IsSpectator ?? false;

                var counterFont = new Font(f.FontFamily, f.Size * 1.2f);
                var counterFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
                var shadowColor = Color.FromArgb(96, 0, 0, 0);

                var activeTeams = _parent.ActiveTeams;
                var numTeams = activeTeams.Length;
                var counters = _counters.ToDictionary(c => c.Team, c => c.Counter);
                for(int i = 0; i < numTeams; ++i)
                {
                    var team = activeTeams[i];
                    var count = counters.GetValueOrDefault(team, 0);

                    // Don't draw 0 counters or counter for teams that have claimed the square
                    if (count == 0 || Teams.Contains(team))
                        continue;

                    var size = TextRenderer.MeasureText(count.ToString(), counterFont);
                    int leftXPos;
                    if (i == 0)
                        leftXPos = rect.X;
                    else if (i == numTeams - 1)
                        leftXPos = rect.X + rect.Width - size.Width;
                    else
                    {
                        var widthBetweenLeftAndRight = rect.Width - size.Width;
                        leftXPos = Convert.ToInt32(rect.X + i * (widthBetweenLeftAndRight / (numTeams - 1f)));
                    }
                    int yPos = rect.Y + rect.Height - size.Height;
                    var color = BingoConstants.GetTeamColorBright(team);
                    TextRenderer.DrawText(g, count.ToString(), counterFont, new Rectangle(leftXPos + 1, yPos + 1, size.Width, size.Height), shadowColor, flags: counterFlags);
                    TextRenderer.DrawText(g, count.ToString(), counterFont, new Rectangle(leftXPos, yPos, size.Width, size.Height), color, flags: counterFlags);
                }
            }

            private void drawMarkedStar(Graphics g, Rectangle rect)
            {
                if (_parent == null) return;

                var scale = rect.Width / 96f;
                var x = rect.X + 3f * scale;
                var y = rect.Y + 3f * scale;
                var width = _starImage.Width * scale * 0.7f;
                var height = _starImage.Height * scale * 0.7f;
                g.DrawImage(_starImage, x, y, width, height);
            }

            private float invLerp(float a, float b, float v)
            {
                return Math.Clamp((v - a) / (b - a), 0f, 1f);
            }
        }
    }
}