using EldenBingoCommon;

namespace EldenBingo.UI
{
    public partial class PopoutBoardForm : Form
    {
        enum MouseAction { None, Move, Resize, Opacity, Close }
        private System.Windows.Forms.Timer _mouseHoverTimer;
        private bool _lastMouseInside = false;
        private MouseAction _mouseAction;
        private Point _previousMousePos;
        private Client? _client;

        internal BingoControl BingoControl => bingoControl1;

        private Color ChromaKey = Color.FromArgb(36, 31, 28);

        public PopoutBoardForm()
        {
            InitializeComponent();
            BackColor = ChromaKey;
            TransparencyKey = ChromaKey;
            TopMost = true;
            setButtonsVisible(false);
            updateBingoControlSize();
            updateScoreboardFont();
            SizeChanged += (o, e) => updateBingoControlSize();
            Properties.Settings.Default.PropertyChanged += default_PropertyChanged;
            if (Properties.Settings.Default.PopoutSizeX > -1 && Properties.Settings.Default.PopoutSizeY > -1)
            {
                Size = new Size(Properties.Settings.Default.PopoutSizeX, Properties.Settings.Default.PopoutSizeY);
            }
            if (Properties.Settings.Default.PopoutLocationX > -1 && Properties.Settings.Default.PopoutLocationY > -1)
            {
                Location = new Point(Properties.Settings.Default.PopoutLocationX, Properties.Settings.Default.PopoutLocationY);
            }
            else
            {
                var mf = MainForm.Instance;
                if (mf != null)
                {
                    Location = new Point(mf.Location.X + (mf.Size.Width - Size.Width) / 2, mf.Location.Y + (mf.Size.Height - Size.Height) / 2);
                }
            }
            if (Properties.Settings.Default.PopoutOpacity < 1.0f)
            {
                Opacity = Properties.Settings.Default.PopoutOpacity;
            }
        }

        public Client? Client
        {
            get { return _client; }
            set
            {
                _client = value;
                bingoControl1.Client = value;
                scoreboardControl1.Client = value;
            }
        }

        private void OnMouseEnteredForm()
        {
            setButtonsVisible(true);
        }

        private void OnMouseLeftForm()
        {
            setButtonsVisible(false);
        }

        private void setButtonsVisible(bool visible)
        {
            foreach (Control c in panel1.Controls)
            {
                c.Visible = visible;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _mouseHoverTimer = new System.Windows.Forms.Timer();
            _mouseHoverTimer.Interval = 50; // 10 times per second
            _mouseHoverTimer.Tick += mouseHoverTimer_Tick;
            _mouseHoverTimer.Start();
        }

        private void mouseHoverTimer_Tick(object? sender, EventArgs e)
        {
            if (_mouseAction > MouseAction.None)
            {
                var pos = Cursor.Position;
                var diff = new Point(pos.X - _previousMousePos.X, pos.Y - _previousMousePos.Y);
                if (diff.X != 0 || diff.Y != 0)
                {
                    switch (_mouseAction)
                    {
                        case MouseAction.Move:
                            Location = new Point(Location.X + diff.X, Location.Y + diff.Y);
                            Properties.Settings.Default.PopoutLocationX = Location.X;
                            Properties.Settings.Default.PopoutLocationY = Location.Y;
                            break;

                        case MouseAction.Resize:
                            var new_size = new Size(Size.Width + diff.X, Size.Height + diff.Y);
                            Size = new Size(Math.Max(160, new_size.Width), Math.Max(160, new_size.Height));
                            Properties.Settings.Default.PopoutSizeX = Size.Width;
                            Properties.Settings.Default.PopoutSizeY = Size.Height;
                            updateBingoControlSize();
                            break;

                        case MouseAction.Opacity:
                            Opacity = Math.Clamp(Opacity + diff.X * 0.005, 0.05, 1.0);
                            Properties.Settings.Default.PopoutOpacity = (float)Opacity;
                            break;
                    }
                }
                _previousMousePos = pos;
                return;
            }
            // Get mouse pointer position in client coordinates
            var mousePos = PointToClient(Cursor.Position);
            bool mouseInside = ClientRectangle.Contains(mousePos);

            if (mouseInside && !_lastMouseInside)
            {
                OnMouseEnteredForm();
            }
            else if (!mouseInside && _lastMouseInside)
            {
                OnMouseLeftForm();
            }
            _lastMouseInside = mouseInside;
        }

        private void updateBingoControlSize()
        {
            var board_max_x = Size.Width;
            var board_max_y = Size.Height - 32 - Math.Max(50, scoreboardControl1.Height + scoreboardControl1.Location.Y);
            if (board_max_y > board_max_x / BingoControl.AspectRatio)
            {
                board_max_y = Convert.ToInt32(board_max_x / BingoControl.AspectRatio);
            }
            else
            {
                board_max_x = Convert.ToInt32(board_max_y * BingoControl.AspectRatio);
            }
            bingoControl1.Size = new Size(board_max_x, board_max_y);
        }

        private void PopoutBoardForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_mouseHoverTimer != null)
            {
                _mouseHoverTimer.Stop();
                _mouseHoverTimer.Dispose();
            }
            Properties.Settings.Default.PropertyChanged -= default_PropertyChanged;
        }

        private void _button_MouseUp(object sender, MouseEventArgs e)
        {
            _mouseAction = MouseAction.None;
            BackColor = ChromaKey;
        }

        private void _moveButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (_mouseAction == MouseAction.None)
            {
                _mouseAction = MouseAction.Move;
                _previousMousePos = Cursor.Position;
            }
        }

        private void _resizeButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (_mouseAction == MouseAction.None)
            {
                _mouseAction = MouseAction.Resize;
                BackColor = Color.FromArgb(0, 0, 30);
                _previousMousePos = Cursor.Position;
            }
        }

        private void _opacityButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (_mouseAction == MouseAction.None)
            {
                _mouseAction = MouseAction.Opacity;
                _previousMousePos = Cursor.Position;
            }
        }

        private void _closeButton_MouseDown(object sender, MouseEventArgs e)
        {
            Close();
        }


        private void default_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Properties.Settings.Default.BingoFont) ||
                e.PropertyName == nameof(Properties.Settings.Default.BingoFontSize) ||
                e.PropertyName == nameof(Properties.Settings.Default.BingoFontStyle))
            {
                updateScoreboardFont();
            }
        }

        private void updateScoreboardFont()
        {
            void update()
            {
                var font = MainForm.GetFontFromSettings(scoreboardControl1.Font, 11f);
                if (font != scoreboardControl1.Font)
                {
                    scoreboardControl1.Font = font;
                }
            }
            if (InvokeRequired)
            {
                BeginInvoke(update);
                return;
            }
            update();
        }


        public void SetTimerLabel(string text) 
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
    }
}