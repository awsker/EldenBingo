namespace EldenBingo.UI
{
    public partial class SettingsDialog : Form
    {
        const float fontSize = 12f;
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

            var ffName = Properties.Settings.Default.BingoFont;
            if (!string.IsNullOrWhiteSpace(ffName))
            {
                var ff2 = new FontFamily(ffName);
                var font = new Font(ff2, fontSize, (FontStyle)Properties.Settings.Default.BingoFontStyle);
                if (font.Name == ffName)
                    _fontLinkLabel.Font = font;
            }
            _fontLinkLabel.Text = _fontLinkLabel.Font.FontFamily.Name;
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

            Properties.Settings.Default.BingoFont = _fontLinkLabel.Font.FontFamily.Name;
            Properties.Settings.Default.BingoFontStyle = (int)_fontLinkLabel.Font.Style;

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

        private void _fontLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var dialog= new FontDialog();
            dialog.FontMustExist = true;
            dialog.Font = _fontLinkLabel.Font;
            if(dialog.ShowDialog(this) == DialogResult.OK)
            {
                _fontLinkLabel.Font = new Font(dialog.Font.FontFamily, fontSize, dialog.Font.Style);
                _fontLinkLabel.Text = _fontLinkLabel.Font.FontFamily.Name;
            }
        }
    }
}
