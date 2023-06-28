using EldenBingoCommon;

namespace EldenBingo.UI
{
    public partial class GameSettingsForm : Form
    {
        public GameSettingsForm()
        {
            InitializeComponent();
        }

        public BingoGameSettings Settings
        {
            get { return _gameSettingsControl.Settings; }
            set
            {
                _gameSettingsControl.Settings = value;
            }
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
