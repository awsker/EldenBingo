namespace EldenBingo.UI
{
    public partial class SettingsDialog : Form
    {
        private const float fontSize = 12f;

        private Keys _outOfFocusKey;
        private bool _rebindingKey = false;

        public SettingsDialog()
        {
            InitializeComponent();
            initControls();
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

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
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

            _outOfFocusKey = (Keys)Properties.Settings.Default.ClickHotkey;

            _hostServerCheckBox.Checked = Properties.Settings.Default.HostServerOnLaunch;
            _portTextBox.Text = Properties.Settings.Default.Port.ToString();

            _mapSizeCustomRadioButton.CheckedChanged += (_, _) => updateSizeEnable();
            _mapPositionCustomRadioButton.CheckedChanged += (_, _) => updatePositionEnable();
            _bingoCustomMaxSizeRadioButton.CheckedChanged += (_, _) => updateMaxSizeEnable();

            _swapMouseButtons.Checked = Properties.Settings.Default.FlipMouseButtons;
            _showClassesCheckBox.Checked = Properties.Settings.Default.ShowClassesOnMap;
            _clickIncrementsCountCheckbox.Checked = Properties.Settings.Default.ClickIncrementsCountedSquares;

            _soundCheckBox.Checked = Properties.Settings.Default.PlaySounds;
            _volumeTrackBar.Value = Convert.ToInt32(Properties.Settings.Default.SoundVolume / 10f);

            _colorPanel.BackColor = Properties.Settings.Default.ControlBackColor;
            _alwaysOnTopCheckbox.Checked = Properties.Settings.Default.AlwaysOnTop;

            _delayMatchEventsTextBox.Text = Properties.Settings.Default.DelayMatchEvents.ToString();

            updateSizeEnable();
            updatePositionEnable();
            updateMaxSizeEnable();
            updateOutOfFocusText();
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
            if (!int.TryParse(_mapSizeCustomXTextBox.Text, out var mapWidth))
            {
                //Invalid x size
                return false; 
            }
            if (!int.TryParse(_mapSizeCustomYTextBox.Text, out var mapHeight))
            {
                //Invalid y size
                return false;
            }
            if (!int.TryParse(_mapPositionXTextBox.Text, out var mapX))
            {
                //Invalid x pos
                return false;
            }
            if (!int.TryParse(_mapPositionYTextBox.Text, out var mapY))
            {
                //Invalid y pos
                return false;
            }
            if (!int.TryParse(_bingoMaxXTextBox.Text, out var bingoMaxWidth))
            {
                //Invalid x size
                return false;
            }
            if (!int.TryParse(_bingoMaxYTextBox.Text, out var bingoMaxHeight))
            {
                //Invalid y size
                return false;
            }
            if (!int.TryParse(_portTextBox.Text, out int port))
            {
                //Invalid port
                return false;
            }
            if (!int.TryParse(_delayMatchEventsTextBox.Text, out int delayMatchEvents))
            {
                //Invalid event delay
                return false;
            }
            Properties.Settings.Default.MapWindowCustomSize = _mapSizeCustomRadioButton.Checked;
            Properties.Settings.Default.MapWindowCustomPosition = _mapPositionCustomRadioButton.Checked;
            Properties.Settings.Default.MapWindowWidth = mapWidth;
            Properties.Settings.Default.MapWindowHeight = mapHeight;
            Properties.Settings.Default.MapWindowX = mapX;
            Properties.Settings.Default.MapWindowY = mapY;
            Properties.Settings.Default.BingoBoardMaximumSize = _bingoCustomMaxSizeRadioButton.Checked;
            Properties.Settings.Default.BingoMaxSizeX = bingoMaxWidth;
            Properties.Settings.Default.BingoMaxSizeY = bingoMaxHeight;

            Properties.Settings.Default.ControlBackColor = _colorPanel.BackColor;

            Properties.Settings.Default.BingoFont = _fontLinkLabel.Font.FontFamily.Name;
            Properties.Settings.Default.BingoFontStyle = (int)_fontLinkLabel.Font.Style;
            Properties.Settings.Default.BingoFontSize = _fontLinkLabel.Font.Size;

            Properties.Settings.Default.Port = port;
            Properties.Settings.Default.HostServerOnLaunch = _hostServerCheckBox.Checked;

            Properties.Settings.Default.FlipMouseButtons = _swapMouseButtons.Checked;
            Properties.Settings.Default.ShowClassesOnMap = _showClassesCheckBox.Checked;
            Properties.Settings.Default.ClickIncrementsCountedSquares = _clickIncrementsCountCheckbox.Checked;

            Properties.Settings.Default.PlaySounds = _soundCheckBox.Checked;
            Properties.Settings.Default.SoundVolume = Math.Clamp(_volumeTrackBar.Value * 10, 0, 100);

            Properties.Settings.Default.ClickHotkey = (int)_outOfFocusKey;

            Properties.Settings.Default.AlwaysOnTop = _alwaysOnTopCheckbox.Checked;

            Properties.Settings.Default.DelayMatchEvents = delayMatchEvents;

            Properties.Settings.Default.Save();
            return true;
        }

        private void SettingsDialog_Load(object sender, EventArgs e)
        {
            _swapMouseButtons.Text = _swapMouseButtons.Text.Replace("***", "\r\n");
        }

        private void _outOfFocusClickTextBox_Enter(object sender, EventArgs e)
        {
            _rebindingKey = true;
            updateOutOfFocusText();
        }

        private void _outOfFocusClickTextBox_Leave(object sender, EventArgs e)
        {
            _rebindingKey = false;
            updateOutOfFocusText();
        }

        private void _outOfFocusClickTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (_rebindingKey)
            {
                _outOfFocusKey = e.KeyCode == Keys.Escape ? Keys.None : e.KeyCode;
                _rebindingKey = false;
                updateOutOfFocusText();
                label11.Focus();
            }
        }

        private void updateOutOfFocusText()
        {
            if (_rebindingKey)
            {
                _outOfFocusClickTextBox.Text = "Press a key...";
            }
            else
            {
                _outOfFocusClickTextBox.Text = _outOfFocusKey.ToString().ToUpper();
            }
        }


    }
}