namespace EldenBingo.UI
{
    public partial class SettingsDialog : Form
    {
        private const float fontSize = 12f;

        public SettingsDialog()
        {
            InitializeComponent();
            initControls();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void _colorPanel_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = _colorPanel.BackColor;
            if (colorDialog1.ShowDialog(this) == DialogResult.OK)
            {
                _colorPanel.BackColor = colorDialog1.Color;
            }
        }

        private void _fontLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var dialog = new FontDialog();
                dialog.FontMustExist = true;
                dialog.Font = _fontLinkLabel.Font;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    _fontLinkLabel.Font = dialog.Font;
                    _fontLinkLabel.Text = _fontLinkLabel.Font.FontFamily.Name;
                }
            } 
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            if (saveSettings())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
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

            _fontLinkLabel.Font = MainForm.GetFontFromSettings(_fontLinkLabel.Font, fontSize);
            _fontLinkLabel.Text = _fontLinkLabel.Font.FontFamily.Name;
            _colorPanel.BackColor = Properties.Settings.Default.ControlBackColor;

            _hostServerCheckBox.Checked = Properties.Settings.Default.HostServerOnLaunch;
            _portTextBox.Text = Properties.Settings.Default.Port.ToString();

            _mapSizeCustomRadioButton.CheckedChanged += (_, _) => updateSizeEnable();
            _mapPositionCustomRadioButton.CheckedChanged += (_, _) => updatePositionEnable();
            _bingoCustomMaxSizeRadioButton.CheckedChanged += (_, _) => updateMaxSizeEnable();

            _swapMouseButtons.Checked = Properties.Settings.Default.FlipMouseButtons;
            _showClassesCheckBox.Checked = Properties.Settings.Default.ShowClassesOnMap;
            _clickIncrementsCountCheckbox.Checked = Properties.Settings.Default.ClickIncrementsCountedSquares;

            _soundCheckBox.Checked = Properties.Settings.Default.PlaySounds;

            updateSizeEnable();
            updatePositionEnable();
            updateMaxSizeEnable();
        }

        private void updateSizeEnable()
        {
            _mapSizeCustomXTextBox.Enabled = _mapSizeCustomRadioButton.Checked;
            _mapSizeCustomYTextBox.Enabled = _mapSizeCustomRadioButton.Checked;
        }

        private void updatePositionEnable()
        {
            _mapPositionXTextBox.Enabled = _mapPositionCustomRadioButton.Checked;
            _mapPositionYTextBox.Enabled = _mapPositionCustomRadioButton.Checked;
        }

        private void updateMaxSizeEnable()
        {
            _bingoMaxXTextBox.Enabled = _bingoCustomMaxSizeRadioButton.Checked;
            _bingoMaxYTextBox.Enabled = _bingoCustomMaxSizeRadioButton.Checked;
        }

        private bool saveSettings()
        {
            Properties.Settings.Default.MapWindowCustomSize = _mapSizeCustomRadioButton.Checked;
            if (int.TryParse(_mapSizeCustomXTextBox.Text, out var x))
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

            Properties.Settings.Default.BingoFont = _fontLinkLabel.Font.FontFamily.Name;
            Properties.Settings.Default.BingoFontStyle = (int)_fontLinkLabel.Font.Style;
            Properties.Settings.Default.BingoFontSize = _fontLinkLabel.Font.Size;

            Properties.Settings.Default.HostServerOnLaunch = _hostServerCheckBox.Checked;
            if (int.TryParse(_portTextBox.Text, out int port))
            {
                Properties.Settings.Default.Port = port;
            }
            else
            {
                //Invalid port
                return false;
            }

            Properties.Settings.Default.FlipMouseButtons = _swapMouseButtons.Checked;
            Properties.Settings.Default.ShowClassesOnMap = _showClassesCheckBox.Checked;
            Properties.Settings.Default.ClickIncrementsCountedSquares = _clickIncrementsCountCheckbox.Checked;

            Properties.Settings.Default.PlaySounds = _soundCheckBox.Checked;

            Properties.Settings.Default.Save();
            return true;
        }

        private void SettingsDialog_Load(object sender, EventArgs e)
        {
            _swapMouseButtons.Text = _swapMouseButtons.Text.Replace("***", "\r\n");
        }
    }
}