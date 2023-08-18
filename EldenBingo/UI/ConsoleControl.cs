namespace EldenBingo.UI
{
    internal partial class ConsoleControl : UserControl
    {
        public ConsoleControl() : base()
        {
            InitializeComponent();
        }

        public override Color BackColor
        {
            get => base.BackColor;
            set
            {
                base.BackColor = value;
                _consoleTextBox.BackColor = value;
            }
        }

        public void PrintToConsole(string text, Color color, bool timestamp = true)
        {
            void printAction()
            {
                if (Disposing || IsDisposed)
                    return;
                _consoleTextBox.SelectionStart = _consoleTextBox.TextLength;
                _consoleTextBox.SelectionLength = 0;

                _consoleTextBox.SelectionColor = color;
                if (timestamp)
                {
                    text = $"[{DateTime.Now:HH:mm}] {text}";
                }
                _consoleTextBox.AppendText(text + "\r\n");
                _consoleTextBox.SelectionColor = _consoleTextBox.ForeColor;

                _consoleTextBox.ScrollToCaret();
            }
            if (InvokeRequired)
            {
                BeginInvoke(printAction);
                return;
            }
            printAction();
        }
    }
}