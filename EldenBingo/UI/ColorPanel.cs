namespace EldenBingo.UI
{
    public partial class ColorPanel : Control
    {
        private Brush _brush;
        private Pen _mouseHoverPen;
        private bool _mouseOver;

        public ColorPanel(Color c)
        {
            Color = c;
            _brush = new SolidBrush(c);
            _mouseHoverPen = new Pen(Color.White);
            MouseEnter += (o, e) =>
            {
                _mouseOver = true;
                Invalidate();
            };
            MouseLeave += (o, e) =>
            {
                _mouseOver = false;
                Invalidate();
            };
        }

        public Color Color { get; init; }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var rect = new Rectangle(0, 0, Width, Height);
            e.Graphics.FillRectangle(_brush, rect);
            if (_mouseOver)
            {
                e.Graphics.DrawRectangle(_mouseHoverPen, new Rectangle(0, 0, Width - 1, Height - 1));
            }
        }
    }
}