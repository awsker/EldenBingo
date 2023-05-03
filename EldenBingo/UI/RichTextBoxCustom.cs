using System.ComponentModel;

namespace EldenBingo.UI
{
    internal class RichTextBoxCustom : RichTextBox
    {
        public RichTextBoxCustom() : base()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        [Browsable(true)]
        [Category("Border Style")]
        public Color BorderColor { get; set; }
    }
}