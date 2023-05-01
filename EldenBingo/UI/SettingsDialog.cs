namespace EldenBingo.UI
{
    public partial class SettingsDialog : Form
    {
        public SettingsDialog()
        {
            InitializeComponent();
            initControls();
        }

        private void initControls()
        {
            _mapSizeRememberLastRadioButton.Checked = !Properties.Settings.Default.MapWindowCustomSize;
            _mapSizeCustomRadioButton.Checked = Properties.Settings.Default.MapWindowCustomSize;

            _mapSizeCustomXTextBox.Text = Properties.Settings.Default.MapWindowWidth.ToString();
            _mapSizeCustomYTextBox.Text = Properties.Settings.Default.MapWindowHeight.ToString();

            _mapPositionRelativeRadioButton.Checked = !Properties.Settings.Default.MapWindowCustomPosition;
            _mapPositionCustomRadioButton.Checked = Properties.Settings.Default.MapWindowCustomPosition;

            _mapPositionXTextBox.Text = Properties.Settings.Default.MapWindowX.ToString();
            _mapPositionYTextBox.Text = Properties.Settings.Default.MapWindowY.ToString();

            _bingoNoMaxSizeRadioButton.Checked = !Properties.Settings.Default.BingoBoardMaximumSize;
            _bingoCustomMaxSizeRadioButton.Checked = Properties.Settings.Default.BingoBoardMaximumSize;

            _bingoMaxXTextBox.Text = Properties.Settings.Default.BingoMaxSizeX.ToString();
            _bingoMaxYTextBox.Text = Properties.Settings.Default.BingoMaxSizeY.ToString();

            _colorPanel.BackColor = Properties.Settings.Default.ControlBackColor;
        }

        private bool saveSettings()
        {
            Properties.Settings.Default.MapWindowCustomSize = _mapSizeCustomRadioButton.Checked;
            if(int.TryParse(_mapSizeCustomXTextBox.Text, out var x))
            {
                Properties.Settings.Default.MapWindowWidth = x;
            } 
            else
            {
                //Invalid x size
                return false;
            }
            if (int.TryParse(_mapSizeCustomYTextBox.Text, out var y))
            {
                Properties.Settings.Default.MapWindowHeight = y;
            }
            else
            {
                //Invalid y size
                return false;
            }

            Properties.Settings.Default.MapWindowCustomPosition = _mapPositionCustomRadioButton.Checked;
            if (int.TryParse(_mapPositionXTextBox.Text, out x))
            {
                Properties.Settings.Default.MapWindowX = x;
            }
            else
            {
                //Invalid x pos
                return false;
            }
            if (int.TryParse(_mapPositionYTextBox.Text, out y))
            {
                Properties.Settings.Default.MapWindowY = y;
            }
            else
            {
                //Invalid y pos
                return false;
            }

            Properties.Settings.Default.BingoBoardMaximumSize = _bingoCustomMaxSizeRadioButton.Checked;
            if (int.TryParse(_bingoMaxXTextBox.Text, out x))
            {
                Properties.Settings.Default.BingoMaxSizeX = x;
            }
            else
            {
                //Invalid x size
                return false;
            }
            if (int.TryParse(_bingoMaxYTextBox.Text, out y))
            {
                Properties.Settings.Default.BingoMaxSizeY = y;
            }
            else
            {
                //Invalid y size
                return false;
            }

            Properties.Settings.Default.ControlBackColor = _colorPanel.BackColor;

            Properties.Settings.Default.Save();
            return true;
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            if(saveSettings())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void _colorPanel_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = _colorPanel.BackColor;
            if(colorDialog1.ShowDialog(this) == DialogResult.OK)
            {
                _colorPanel.BackColor = colorDialog1.Color;
            }
        }
    }
}
