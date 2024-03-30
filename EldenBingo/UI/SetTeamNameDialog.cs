namespace EldenBingo.UI
{
    public partial class SetTeamNameDialog : Form
    {
        public SetTeamNameDialog()
        {
            InitializeComponent();
            _teamNameTextBox.SelectAll();
        }

        public string TeamName
        {
            get
            {
                return _teamNameTextBox.Text;
            }
            set
            {
                _teamNameTextBox.Text = value;
            }
        }

        private void _teamNameTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                DialogResult = DialogResult.OK;
                Close();
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
