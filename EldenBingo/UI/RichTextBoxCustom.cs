using System.ComponentModel;
using System.Runtime.InteropServices;

namespace EldenBingo.UI
{
    internal class RichTextBoxCustom : RichTextBox
    {
        public RichTextBoxCustom() : base()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        private object _lock = new object();

        [Browsable(true)]
        [Category("Border Style")]
        public Color BorderColor { get; set; }

        private bool mustHideCaret;

        [DefaultValue(false)]
        public bool MustHideCaret
        {
            get
            {
                lock (_lock)
                    return this.mustHideCaret;
            }
            set
            {
                TabStop = false;
                if (value)
                    SetHideCaret();
                else
                    SetShowCaret();
            }
        }

        [DllImport("user32.dll")]
        private static extern int HideCaret(IntPtr hwnd);
        [DllImport("user32.dll", EntryPoint = "ShowCaret")]
        public static extern long ShowCaret(IntPtr hwnd);

        private void SetHideCaret()
        {
            MouseDown += new MouseEventHandler(ReadOnlyRichTextBox_Mouse);
            MouseUp += new MouseEventHandler(ReadOnlyRichTextBox_Mouse);
            Resize += new EventHandler(ReadOnlyRichTextBox_Resize);
            hideCaret();
            lock (_lock)
                this.mustHideCaret = true;
        }

        private void SetShowCaret()
        {
            try
            {
                MouseDown -= new MouseEventHandler(ReadOnlyRichTextBox_Mouse);
                MouseUp -= new MouseEventHandler(ReadOnlyRichTextBox_Mouse);
                Resize -= new EventHandler(ReadOnlyRichTextBox_Resize);
            }
            catch
            {
            }
            showCaret();
            lock (_lock)
                this.mustHideCaret = false;
        }

        protected override void OnGotFocus(EventArgs e)
        {
            hideCaret();
        }

        protected override void OnEnter(EventArgs e)
        {
            hideCaret();
        }

        private void ReadOnlyRichTextBox_Mouse(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            hideCaret();
        }

        private void ReadOnlyRichTextBox_Resize(object sender, System.EventArgs e)
        {
            hideCaret();
        }

        private void hideCaret()
        {
            void update()
            {
                if (MustHideCaret)
                    HideCaret(Handle);
            }
            if (InvokeRequired)
            {
                BeginInvoke(update);
                return;
            }
            update();
        }

        private void showCaret()
        {
            void update()
            {
                if (MustHideCaret)
                    ShowCaret(Handle);
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