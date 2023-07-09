using System.ComponentModel;
using System.Runtime.InteropServices;

namespace EldenBingo.UI
{
    internal class RichTextBoxCustom : RichTextBox
    {
        private object _lock = new object();

        private bool _mustHideCaret;

        public RichTextBoxCustom() : base()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        [Browsable(true)]
        [Category("Border Style")]
        public Color BorderColor { get; set; }

        [DefaultValue(false)]
        public bool MustHideCaret
        {
            get
            {
                lock (_lock)
                    return _mustHideCaret;
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

        [DllImport("user32.dll", EntryPoint = "ShowCaret")]
        public static extern long ShowCaret(IntPtr hwnd);

        protected override void OnGotFocus(EventArgs e)
        {
            hideCaret();
        }

        protected override void OnEnter(EventArgs e)
        {
            hideCaret();
        }

        [DllImport("user32.dll")]
        private static extern int HideCaret(IntPtr hwnd);

        private void SetHideCaret()
        {
            MouseDown += onMouse;
            MouseUp += onMouse;
            Resize += onResize;
            hideCaret();
            lock (_lock)
                _mustHideCaret = true;
        }

        private void SetShowCaret()
        {
            try
            {
                MouseDown -= onMouse;
                MouseUp -= onMouse;
                Resize -= onResize;
            }
            catch
            {
            }
            showCaret();
            lock (_lock)
                _mustHideCaret = false;
        }

        private void onMouse(object? sender, MouseEventArgs e)
        {
            hideCaret();
        }

        private void onResize(object? sender, System.EventArgs e)
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