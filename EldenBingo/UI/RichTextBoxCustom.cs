using System.ComponentModel;

namespace EldenBingo.UI
{
    internal class RichTextBoxCustom : RichTextBox
    {
        [Browsable(true)]
        [Category("Border Style")]
        public Color BorderColor { get; set; }

        public RichTextBoxCustom():base()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }
    }
}