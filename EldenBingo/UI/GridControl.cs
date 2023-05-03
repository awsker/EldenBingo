namespace EldenBingo.UI
{
    internal class GridControl : Control
    {
        private float _aspectRatio;
        private int _borderX = 2;
        private int _borderY = 2;
        private int _gridHeight = 3;
        private int _gridWidth = 3;
        private int _paddingX = 2;
        private int _paddingY = 2;

        public GridControl() : base()
        {
            Resize += gridControl_Resize; ;
            SizeChanged += gridControl_SizeChanged;
            ControlAdded += gridControl_ControlAdded;
            fixAspectRatio();
        }

        public int BorderX
        {
            get
            {
                return _borderX;
            }
            set
            {
                if (value != _borderX)
                {
                    _borderX = value;
                    updateSubControlsPositionAndSize();
                }
            }
        }

        public int BorderY
        {
            get
            {
                return _borderY;
            }
            set
            {
                if (value != _borderY)
                {
                    _borderY = value;
                    updateSubControlsPositionAndSize();
                }
            }
        }

        public int GridHeight
        {
            get
            {
                return _gridHeight;
            }
            set
            {
                if (value != _gridHeight)
                {
                    _gridHeight = value;
                    updateSubControlsPositionAndSize();
                }
            }
        }

        public int GridWidth
        {
            get
            {
                return _gridWidth;
            }
            set
            {
                if (value != _gridWidth)
                {
                    _gridWidth = value;
                    updateSubControlsPositionAndSize();
                }
            }
        }

        public bool MaintainAspectRatio { get; set; } = false;

        public int PaddingX
        {
            get
            {
                return _paddingX;
            }
            set
            {
                if (value != _paddingX)
                {
                    _paddingX = value;
                    updateSubControlsPositionAndSize();
                }
            }
        }

        public int PaddingY
        {
            get
            {
                return _paddingY;
            }
            set
            {
                if (value != _paddingY)
                {
                    _paddingY = value;
                    updateSubControlsPositionAndSize();
                }
            }
        }

        public void SetAspectRatio(float asp)
        {
            _aspectRatio = asp;
        }

        public void UpdateGrid()
        {
            updateSubControlsPositionAndSize();
        }

        //Returns true if done
        private bool fixAspectRatio()
        {
            if (Width == 0 || Height == 0 || !MaintainAspectRatio)
                return true;

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

            if (Width != targetWidth)
            {
                Width = targetWidth;
                return false;
            }
            if (Height != targetHeight)
            {
                Height = targetHeight;
                return false;
            }
            return true;
        }

        private void gridControl_ControlAdded(object? sender, ControlEventArgs e)
        {
            fixAspectRatio();
            updateSubControlsPositionAndSize();
        }

        private void gridControl_Resize(object? sender, EventArgs e)
        {
            if (fixAspectRatio())
                updateSubControlsPositionAndSize();
        }

        private void gridControl_SizeChanged(object? sender, EventArgs e)
        {
            if (fixAspectRatio())
                updateSubControlsPositionAndSize();
        }

        private void updateSubControlsPositionAndSize()
        {
            var w = Width;
            var h = Height;
            if (w == 0 || h == 0)
                return;
            var squaresTotalWidth = (w - PaddingX * (GridWidth - 1) - 2 * BorderX);
            var squaresTotalHeight = (h - PaddingY * (GridHeight - 1) - 2 * BorderY);
            var sqrWidth = squaresTotalWidth / GridWidth;
            var sqrHeight = squaresTotalHeight / GridHeight;

            var totalSquares = GridWidth * GridHeight;
            for (int i = 0; i < Controls.Count; ++i)
            {
                var c = Controls[i];
                c.Dock = DockStyle.None;
                if (i >= totalSquares)
                {
                    c.Location = new Point(0, 0);
                    c.Visible = false;
                    continue;
                }
                var x = i % GridWidth;
                var y = i / GridWidth;
                int xrest = x < squaresTotalWidth % GridWidth ? 1 : 0;
                int yrest = y < squaresTotalHeight % GridHeight ? 1 : 0;

                c.Width = sqrWidth + xrest;
                c.Height = sqrHeight + yrest;
                c.Location = new Point(BorderX + x * sqrWidth + Math.Min(squaresTotalWidth % GridWidth, x) + PaddingX * x, BorderY + y * sqrHeight + Math.Min(squaresTotalHeight % GridHeight, y) + PaddingY * y);
            }
            Invalidate();
        }
    }
}