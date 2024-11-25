using EldenBingoCommon;

namespace EldenBingo.UI
{
    public partial class ChangeTeamForm : Form
    {
        public ChangeTeamForm()
        {
            InitializeComponent();
            initTeamComboBox();
        }

        private void initTeamComboBox()
        {
            for (int i = -1; i < BingoConstants.TeamColors.Length; ++i)
            {
                _teamComboBox.Items.Add(BingoConstants.GetTeamName(i));
            }
            _teamComboBox.SelectedIndex = 0;
            _teamComboBox.SelectedIndexChanged += (o, e) => setPanelColor();
        }

        private void setPanelColor()
        {
            _colorPanel.BackColor = BingoConstants.GetTeamColor(Team);
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

        public int Team
        {
            get { return _teamComboBox.SelectedIndex - 1; }
            set { _teamComboBox.SelectedIndex = (int)value + 1; }
        }
    }
}
