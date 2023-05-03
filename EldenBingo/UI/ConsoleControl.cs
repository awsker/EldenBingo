using EldenBingoCommon;

namespace EldenBingo.UI
{
    internal partial class ConsoleControl : ClientUserControl
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

        protected override void AddClientListeners()
        {
            Client.StatusChanged += client_StatusChanged;
        }

        protected override void RemoveClientListeners()
        {
            Client.StatusChanged -= client_StatusChanged;
        }

        private void client_StatusChanged(object? sender, StatusEventArgs e)
        {
            printToConsole(e.Status, e.Color, true);
        }

        private void printToConsole(string text, Color color, bool timestamp = true)
        {
            void printAction()
            {
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