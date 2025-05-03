using EldenBingo.Settings;
using EldenBingo.Sfx;

namespace EldenBingo.UI
{
    public partial class SettingsDialog : Form
    {
        private const float fontSize = 12f;

        private Keys _outOfFocusKey;
        private bool _rebindingKey = false;

        private string _volumeLabelInitial;
        private string _shadowLabelInitial;

        private string _numKeywordsLabelInitial;
        private string _keywordAlphaLabelInitial;

        private List<KeywordSquareColor> _keywordColors;

        public SettingsDialog()
        {
            InitializeComponent();

            _volumeLabelInitial = _volumeLabel.Text;
            _shadowLabelInitial = _shadowLabel.Text;
            _numKeywordsLabelInitial = _numKeywordsLabel.Text;
            _keywordAlphaLabelInitial = _keywordColorAlphaLabel.Text;

            _keywordColors = new List<KeywordSquareColor>(SquareColorsJsonHelper.Colors);

            initControls();

            tabControl1.SelectedIndex = Properties.Settings.Default.LastSettingsTab;
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
            Properties.Settings.Default.LastSettingsTab = tabControl1.SelectedIndex;
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

            _shadowTrackBar.Value = Properties.Settings.Default.SquareShadows / 10;
            _shadowTrackBar.ValueChanged += (o, e) => updateShadowText();
            _highlightMarkedCheckBox.Checked = Properties.Settings.Default.MarkHighlight;
            _highlightBingoCheckBox.Checked = Properties.Settings.Default.BingoHighlight;

            _outOfFocusKey = (Keys)Properties.Settings.Default.ClickHotkey;

            _hostServerCheckBox.Checked = Properties.Settings.Default.HostServerOnLaunch;
            _portTextBox.Text = Properties.Settings.Default.Port.ToString();

            _mapSizeCustomRadioButton.CheckedChanged += (_, _) => updateSizeEnable();
            _mapPositionCustomRadioButton.CheckedChanged += (_, _) => updatePositionEnable();
            _bingoCustomMaxSizeRadioButton.CheckedChanged += (_, _) => updateMaxSizeEnable();

            _swapMouseButtons.Checked = Properties.Settings.Default.FlipMouseButtons;
            _showClassesCheckBox.Checked = Properties.Settings.Default.ShowClassesOnMap;

            _soundCheckBox.Checked = Properties.Settings.Default.PlaySounds;
            _volumeTrackBar.Value = Properties.Settings.Default.SoundVolume / 10;
            _volumeTrackBar.ValueChanged += (o, e) => updateVolumeText();

            _colorPanel.BackColor = Properties.Settings.Default.ControlBackColor;
            _alwaysOnTopCheckbox.Checked = Properties.Settings.Default.AlwaysOnTop;

            _delayMatchEventsTextBox.Text = Properties.Settings.Default.DelayMatchEvents.ToString();

            _checkUpdatesCheckBox.Checked = Properties.Settings.Default.CheckForUpdates;

            _keywordColorAlphaTrackBar.Value = Properties.Settings.Default.SquareColorsAlpha;
            _keywordColorAlphaTrackBar.ValueChanged += (o, e) => updateKeywordColorText();

            updateSizeEnable();
            updatePositionEnable();
            updateMaxSizeEnable();
            updateOutOfFocusText();
            updateVolumeText();
            updateShadowText();
            updateKeywordColorText();
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

            Properties.Settings.Default.PlaySounds = _soundCheckBox.Checked;
            Properties.Settings.Default.SoundVolume = Math.Clamp(_volumeTrackBar.Value * 10, 0, 100);
            Properties.Settings.Default.OutputDevice = (_soundOutputDeviceComboBox.SelectedItem as AudioDevice)?.Id ?? string.Empty;

            Properties.Settings.Default.SquareShadows = Math.Clamp(_shadowTrackBar.Value * 10, 0, 100);
            Properties.Settings.Default.MarkHighlight = _highlightMarkedCheckBox.Checked;
            Properties.Settings.Default.BingoHighlight = _highlightBingoCheckBox.Checked;
            Properties.Settings.Default.ClickHotkey = (int)_outOfFocusKey;

            Properties.Settings.Default.AlwaysOnTop = _alwaysOnTopCheckbox.Checked;

            Properties.Settings.Default.DelayMatchEvents = delayMatchEvents;

            Properties.Settings.Default.CheckForUpdates = _checkUpdatesCheckBox.Checked;

            SquareColorsJsonHelper.Colors = _keywordColors.ToArray();
            Properties.Settings.Default.SquareColorsAlpha = _keywordColorAlphaTrackBar.Value;

            Properties.Settings.Default.LastSettingsTab = tabControl1.SelectedIndex;

            Properties.Settings.Default.Save();
            return true;
        }

        private void SettingsDialog_Load(object sender, EventArgs e)
        {
            _swapMouseButtons.Text = _swapMouseButtons.Text.Replace("***", "\r\n");

            initOutputDeviceComboBox();
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

        private void initOutputDeviceComboBox()
        {
            var devices = MainForm.Instance?.SoundPlayer?.GetAudioDevices() ?? null;
            _soundOutputDeviceComboBox.DataSource = devices;
            _soundOutputDeviceComboBox.SelectedIndex = indexOfDevice(devices, Properties.Settings.Default.OutputDevice);
        }

        private int indexOfDevice(IList<AudioDevice>? devices, string id)
        {
            if (devices == null)
                return 0;
            for (int i = 0; i < devices.Count; i++)
            {
                if (devices[i].Id == id)
                    return i;
            }
            return 0;
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

        private void updateVolumeText()
        {
            _volumeLabel.Text = $"{_volumeLabelInitial} {valToPercent(_volumeTrackBar.Value * 10)}";
        }

        private void updateShadowText()
        {
            _shadowLabel.Text = $"{_shadowLabelInitial} {valToPercent(_shadowTrackBar.Value * 10)}";
        }

        private void updateKeywordColorText()
        {
            _numKeywordsLabel.Text = _numKeywordsLabelInitial.Replace("%x%", _keywordColors.Count.ToString());
            _keywordColorAlphaLabel.Text = $"{_keywordAlphaLabelInitial} {valToPercent(_keywordColorAlphaTrackBar.Value)}";
        }

        private string valToPercent(int val)
        {
            return $"({val}%)";
        }

        private void _testSoundButton_Click(object sender, EventArgs e)
        {
            var player = MainForm.Instance?.SoundPlayer;
            if (player != null)
            {
                var prev = Properties.Settings.Default.OutputDevice;
                var selected = _soundOutputDeviceComboBox.SelectedItem as AudioDevice;
                if (selected != null)
                {
                    player.SetAudioDevice(selected.Id);
                    player.PlaySound(SoundType.SquareClaimedOther, Math.Clamp(_volumeTrackBar.Value * 10, 0, 100));
                }
                player.SetAudioDevice(prev);
            }
        }

        private void _keywordColorsButton_Click(object sender, EventArgs e)
        {
            var dialog = new KeywordSquareColorEditorForm();
            dialog.TopMost = true;
            dialog.Colors = _keywordColors;
            var res = dialog.ShowDialog(this);
            if (res == DialogResult.OK)
            {
                _keywordColors = dialog.Colors;
                updateKeywordColorText();
            }
        }
    }
}