namespace EldenBingo.UI
{
    partial class SettingsDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _okButton = new Button();
            _cancelButton = new Button();
            groupBox1 = new GroupBox();
            _mapSizeCustomYTextBox = new TextBox();
            label2 = new Label();
            _mapSizeCustomRadioButton = new RadioButton();
            _mapSizeRememberLastRadioButton = new RadioButton();
            _mapSizeCustomXTextBox = new TextBox();
            label1 = new Label();
            groupBox2 = new GroupBox();
            _bingoMaxYTextBox = new TextBox();
            label3 = new Label();
            _bingoCustomMaxSizeRadioButton = new RadioButton();
            _bingoNoMaxSizeRadioButton = new RadioButton();
            _bingoMaxXTextBox = new TextBox();
            label4 = new Label();
            groupBox3 = new GroupBox();
            _alwaysOnTopCheckbox = new CheckBox();
            _colorPanel = new Panel();
            label5 = new Label();
            _fontLinkLabel = new LinkLabel();
            label8 = new Label();
            colorDialog1 = new ColorDialog();
            groupBox4 = new GroupBox();
            _mapPositionYTextBox = new TextBox();
            label6 = new Label();
            _mapPositionCustomRadioButton = new RadioButton();
            _mapPositionRelativeRadioButton = new RadioButton();
            _mapPositionXTextBox = new TextBox();
            label7 = new Label();
            groupBox5 = new GroupBox();
            _portTextBox = new TextBox();
            label9 = new Label();
            _hostServerCheckBox = new CheckBox();
            groupBox6 = new GroupBox();
            _showClassesCheckBox = new CheckBox();
            _swapMouseButtons = new CheckBox();
            groupBox7 = new GroupBox();
            _keywordColorAlphaLabel = new Label();
            _keywordColorAlphaTrackBar = new TrackBar();
            _numKeywordsLabel = new Label();
            _keywordColorsButton = new Button();
            label10 = new Label();
            _highlightBingoCheckBox = new CheckBox();
            _highlightMarkedCheckBox = new CheckBox();
            _shadowLabel = new Label();
            _shadowTrackBar = new TrackBar();
            _outOfFocusClickTextBox = new TextBox();
            label11 = new Label();
            groupBox8 = new GroupBox();
            _testSoundButton = new Button();
            label14 = new Label();
            _soundOutputDeviceComboBox = new ComboBox();
            _volumeLabel = new Label();
            _volumeTrackBar = new TrackBar();
            _soundCheckBox = new CheckBox();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            groupBox10 = new GroupBox();
            _checkUpdatesCheckBox = new CheckBox();
            tabPage2 = new TabPage();
            groupBox11 = new GroupBox();
            _numpadNavigationCheckBox = new CheckBox();
            groupBox9 = new GroupBox();
            panel2 = new Panel();
            label12 = new Label();
            _delayMatchEventsTextBox = new TextBox();
            label13 = new Label();
            tabPage3 = new TabPage();
            panel1 = new Panel();
            _arrowNavigationCheckBox = new CheckBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox5.SuspendLayout();
            groupBox6.SuspendLayout();
            groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_keywordColorAlphaTrackBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_shadowTrackBar).BeginInit();
            groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_volumeTrackBar).BeginInit();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            groupBox10.SuspendLayout();
            tabPage2.SuspendLayout();
            groupBox11.SuspendLayout();
            groupBox9.SuspendLayout();
            panel2.SuspendLayout();
            tabPage3.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // _okButton
            // 
            _okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _okButton.Location = new Point(361, 3);
            _okButton.Name = "_okButton";
            _okButton.Size = new Size(75, 23);
            _okButton.TabIndex = 62;
            _okButton.Text = "OK";
            _okButton.UseVisualStyleBackColor = true;
            _okButton.Click += _okButton_Click;
            // 
            // _cancelButton
            // 
            _cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _cancelButton.Location = new Point(442, 3);
            _cancelButton.Name = "_cancelButton";
            _cancelButton.Size = new Size(75, 23);
            _cancelButton.TabIndex = 63;
            _cancelButton.Text = "Cancel";
            _cancelButton.UseVisualStyleBackColor = true;
            _cancelButton.Click += _cancelButton_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(_mapSizeCustomYTextBox);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(_mapSizeCustomRadioButton);
            groupBox1.Controls.Add(_mapSizeRememberLastRadioButton);
            groupBox1.Controls.Add(_mapSizeCustomXTextBox);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(8, 113);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(240, 107);
            groupBox1.TabIndex = 53;
            groupBox1.TabStop = false;
            groupBox1.Text = "Map Initial Size";
            // 
            // _mapSizeCustomYTextBox
            // 
            _mapSizeCustomYTextBox.Location = new Point(117, 74);
            _mapSizeCustomYTextBox.Name = "_mapSizeCustomYTextBox";
            _mapSizeCustomYTextBox.Size = new Size(54, 23);
            _mapSizeCustomYTextBox.TabIndex = 59;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(94, 77);
            label2.Name = "label2";
            label2.Size = new Size(17, 15);
            label2.TabIndex = 58;
            label2.Text = "Y:";
            // 
            // _mapSizeCustomRadioButton
            // 
            _mapSizeCustomRadioButton.AutoSize = true;
            _mapSizeCustomRadioButton.Location = new Point(9, 46);
            _mapSizeCustomRadioButton.Name = "_mapSizeCustomRadioButton";
            _mapSizeCustomRadioButton.Size = new Size(90, 19);
            _mapSizeCustomRadioButton.TabIndex = 55;
            _mapSizeCustomRadioButton.TabStop = true;
            _mapSizeCustomRadioButton.Text = "Custom Size";
            _mapSizeCustomRadioButton.UseVisualStyleBackColor = true;
            // 
            // _mapSizeRememberLastRadioButton
            // 
            _mapSizeRememberLastRadioButton.AutoSize = true;
            _mapSizeRememberLastRadioButton.Location = new Point(9, 22);
            _mapSizeRememberLastRadioButton.Name = "_mapSizeRememberLastRadioButton";
            _mapSizeRememberLastRadioButton.Size = new Size(130, 19);
            _mapSizeRememberLastRadioButton.TabIndex = 54;
            _mapSizeRememberLastRadioButton.TabStop = true;
            _mapSizeRememberLastRadioButton.Text = "Remember Last Size";
            _mapSizeRememberLastRadioButton.UseVisualStyleBackColor = true;
            // 
            // _mapSizeCustomXTextBox
            // 
            _mapSizeCustomXTextBox.Location = new Point(30, 74);
            _mapSizeCustomXTextBox.Name = "_mapSizeCustomXTextBox";
            _mapSizeCustomXTextBox.Size = new Size(54, 23);
            _mapSizeCustomXTextBox.TabIndex = 57;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(7, 77);
            label1.Name = "label1";
            label1.Size = new Size(17, 15);
            label1.TabIndex = 56;
            label1.Text = "X:";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(_bingoMaxYTextBox);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(_bingoCustomMaxSizeRadioButton);
            groupBox2.Controls.Add(_bingoNoMaxSizeRadioButton);
            groupBox2.Controls.Add(_bingoMaxXTextBox);
            groupBox2.Controls.Add(label4);
            groupBox2.Location = new Point(257, 6);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(240, 107);
            groupBox2.TabIndex = 30;
            groupBox2.TabStop = false;
            groupBox2.Text = "Bingo Board Max Size";
            // 
            // _bingoMaxYTextBox
            // 
            _bingoMaxYTextBox.Location = new Point(117, 72);
            _bingoMaxYTextBox.Name = "_bingoMaxYTextBox";
            _bingoMaxYTextBox.Size = new Size(54, 23);
            _bingoMaxYTextBox.TabIndex = 36;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(94, 75);
            label3.Name = "label3";
            label3.Size = new Size(17, 15);
            label3.TabIndex = 35;
            label3.Text = "Y:";
            // 
            // _bingoCustomMaxSizeRadioButton
            // 
            _bingoCustomMaxSizeRadioButton.AutoSize = true;
            _bingoCustomMaxSizeRadioButton.Location = new Point(9, 46);
            _bingoCustomMaxSizeRadioButton.Name = "_bingoCustomMaxSizeRadioButton";
            _bingoCustomMaxSizeRadioButton.Size = new Size(116, 19);
            _bingoCustomMaxSizeRadioButton.TabIndex = 32;
            _bingoCustomMaxSizeRadioButton.TabStop = true;
            _bingoCustomMaxSizeRadioButton.Text = "Custom Max Size";
            _bingoCustomMaxSizeRadioButton.UseVisualStyleBackColor = false;
            // 
            // _bingoNoMaxSizeRadioButton
            // 
            _bingoNoMaxSizeRadioButton.AutoSize = true;
            _bingoNoMaxSizeRadioButton.Location = new Point(9, 22);
            _bingoNoMaxSizeRadioButton.Name = "_bingoNoMaxSizeRadioButton";
            _bingoNoMaxSizeRadioButton.Size = new Size(122, 19);
            _bingoNoMaxSizeRadioButton.TabIndex = 31;
            _bingoNoMaxSizeRadioButton.TabStop = true;
            _bingoNoMaxSizeRadioButton.Text = "No Maximum Size";
            _bingoNoMaxSizeRadioButton.UseVisualStyleBackColor = true;
            // 
            // _bingoMaxXTextBox
            // 
            _bingoMaxXTextBox.Location = new Point(30, 72);
            _bingoMaxXTextBox.Name = "_bingoMaxXTextBox";
            _bingoMaxXTextBox.Size = new Size(54, 23);
            _bingoMaxXTextBox.TabIndex = 34;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(7, 75);
            label4.Name = "label4";
            label4.Size = new Size(17, 15);
            label4.TabIndex = 33;
            label4.Text = "X:";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(_alwaysOnTopCheckbox);
            groupBox3.Controls.Add(_colorPanel);
            groupBox3.Controls.Add(label5);
            groupBox3.Location = new Point(8, 6);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(240, 90);
            groupBox3.TabIndex = 1;
            groupBox3.TabStop = false;
            groupBox3.Text = "Appearance";
            // 
            // _alwaysOnTopCheckbox
            // 
            _alwaysOnTopCheckbox.AutoSize = true;
            _alwaysOnTopCheckbox.Location = new Point(12, 58);
            _alwaysOnTopCheckbox.Name = "_alwaysOnTopCheckbox";
            _alwaysOnTopCheckbox.Size = new Size(102, 19);
            _alwaysOnTopCheckbox.TabIndex = 4;
            _alwaysOnTopCheckbox.Text = "Always on Top";
            _alwaysOnTopCheckbox.UseVisualStyleBackColor = true;
            // 
            // _colorPanel
            // 
            _colorPanel.BorderStyle = BorderStyle.FixedSingle;
            _colorPanel.Location = new Point(168, 22);
            _colorPanel.Name = "_colorPanel";
            _colorPanel.Size = new Size(25, 25);
            _colorPanel.TabIndex = 3;
            _colorPanel.Click += _colorPanel_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(9, 26);
            label5.Name = "label5";
            label5.Size = new Size(153, 15);
            label5.TabIndex = 2;
            label5.Text = "Window Background Color:";
            // 
            // _fontLinkLabel
            // 
            _fontLinkLabel.AutoSize = true;
            _fontLinkLabel.Font = new Font("Lucida Sans Unicode", 12F, FontStyle.Regular, GraphicsUnit.Point);
            _fontLinkLabel.Location = new Point(8, 43);
            _fontLinkLabel.Name = "_fontLinkLabel";
            _fontLinkLabel.Size = new Size(90, 20);
            _fontLinkLabel.TabIndex = 20;
            _fontLinkLabel.TabStop = true;
            _fontLinkLabel.Text = "FontName";
            _fontLinkLabel.LinkClicked += _fontLinkLabel_LinkClicked;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(8, 23);
            label8.Name = "label8";
            label8.Size = new Size(80, 15);
            label8.TabIndex = 19;
            label8.Text = "Font and Size:";
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(_mapPositionYTextBox);
            groupBox4.Controls.Add(label6);
            groupBox4.Controls.Add(_mapPositionCustomRadioButton);
            groupBox4.Controls.Add(_mapPositionRelativeRadioButton);
            groupBox4.Controls.Add(_mapPositionXTextBox);
            groupBox4.Controls.Add(label7);
            groupBox4.Location = new Point(8, 6);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(240, 107);
            groupBox4.TabIndex = 46;
            groupBox4.TabStop = false;
            groupBox4.Text = "Map Initial Position";
            // 
            // _mapPositionYTextBox
            // 
            _mapPositionYTextBox.Location = new Point(117, 72);
            _mapPositionYTextBox.Name = "_mapPositionYTextBox";
            _mapPositionYTextBox.Size = new Size(54, 23);
            _mapPositionYTextBox.TabIndex = 52;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(94, 75);
            label6.Name = "label6";
            label6.Size = new Size(17, 15);
            label6.TabIndex = 51;
            label6.Text = "Y:";
            // 
            // _mapPositionCustomRadioButton
            // 
            _mapPositionCustomRadioButton.AutoSize = true;
            _mapPositionCustomRadioButton.Location = new Point(9, 46);
            _mapPositionCustomRadioButton.Name = "_mapPositionCustomRadioButton";
            _mapPositionCustomRadioButton.Size = new Size(113, 19);
            _mapPositionCustomRadioButton.TabIndex = 48;
            _mapPositionCustomRadioButton.TabStop = true;
            _mapPositionCustomRadioButton.Text = "Custom Position";
            _mapPositionCustomRadioButton.UseVisualStyleBackColor = true;
            // 
            // _mapPositionRelativeRadioButton
            // 
            _mapPositionRelativeRadioButton.AutoSize = true;
            _mapPositionRelativeRadioButton.Location = new Point(9, 22);
            _mapPositionRelativeRadioButton.Name = "_mapPositionRelativeRadioButton";
            _mapPositionRelativeRadioButton.Size = new Size(127, 19);
            _mapPositionRelativeRadioButton.TabIndex = 47;
            _mapPositionRelativeRadioButton.TabStop = true;
            _mapPositionRelativeRadioButton.Text = "Relative to Window";
            _mapPositionRelativeRadioButton.UseVisualStyleBackColor = true;
            // 
            // _mapPositionXTextBox
            // 
            _mapPositionXTextBox.Location = new Point(30, 72);
            _mapPositionXTextBox.Name = "_mapPositionXTextBox";
            _mapPositionXTextBox.Size = new Size(54, 23);
            _mapPositionXTextBox.TabIndex = 50;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(7, 75);
            label7.Name = "label7";
            label7.Size = new Size(17, 15);
            label7.TabIndex = 49;
            label7.Text = "X:";
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(_portTextBox);
            groupBox5.Controls.Add(label9);
            groupBox5.Controls.Add(_hostServerCheckBox);
            groupBox5.Location = new Point(257, 6);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(240, 90);
            groupBox5.TabIndex = 12;
            groupBox5.TabStop = false;
            groupBox5.Text = "Server Hosting";
            // 
            // _portTextBox
            // 
            _portTextBox.Location = new Point(50, 55);
            _portTextBox.Name = "_portTextBox";
            _portTextBox.Size = new Size(61, 23);
            _portTextBox.TabIndex = 15;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(12, 58);
            label9.Name = "label9";
            label9.Size = new Size(32, 15);
            label9.TabIndex = 14;
            label9.Text = "Port:";
            // 
            // _hostServerCheckBox
            // 
            _hostServerCheckBox.AutoSize = true;
            _hostServerCheckBox.Location = new Point(12, 28);
            _hostServerCheckBox.Name = "_hostServerCheckBox";
            _hostServerCheckBox.Size = new Size(184, 19);
            _hostServerCheckBox.TabIndex = 13;
            _hostServerCheckBox.Text = "Host a bingo server on launch";
            _hostServerCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            groupBox6.Controls.Add(_showClassesCheckBox);
            groupBox6.Controls.Add(_swapMouseButtons);
            groupBox6.Location = new Point(8, 226);
            groupBox6.Name = "groupBox6";
            groupBox6.Size = new Size(240, 114);
            groupBox6.TabIndex = 60;
            groupBox6.TabStop = false;
            groupBox6.Text = "Misc.";
            // 
            // _showClassesCheckBox
            // 
            _showClassesCheckBox.Location = new Point(9, 65);
            _showClassesCheckBox.Name = "_showClassesCheckBox";
            _showClassesCheckBox.Size = new Size(218, 41);
            _showClassesCheckBox.TabIndex = 62;
            _showClassesCheckBox.Text = "Show available classes in an overlay on the map (for streaming)";
            _showClassesCheckBox.UseVisualStyleBackColor = true;
            // 
            // _swapMouseButtons
            // 
            _swapMouseButtons.Location = new Point(9, 19);
            _swapMouseButtons.Name = "_swapMouseButtons";
            _swapMouseButtons.Size = new Size(218, 40);
            _swapMouseButtons.TabIndex = 61;
            _swapMouseButtons.Text = "Swap mouse buttons***(Left = Draw, Right = Pan)";
            _swapMouseButtons.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            groupBox7.Controls.Add(_keywordColorAlphaLabel);
            groupBox7.Controls.Add(_keywordColorAlphaTrackBar);
            groupBox7.Controls.Add(_numKeywordsLabel);
            groupBox7.Controls.Add(_keywordColorsButton);
            groupBox7.Controls.Add(label10);
            groupBox7.Controls.Add(_highlightBingoCheckBox);
            groupBox7.Controls.Add(_highlightMarkedCheckBox);
            groupBox7.Controls.Add(_shadowLabel);
            groupBox7.Controls.Add(_shadowTrackBar);
            groupBox7.Controls.Add(_fontLinkLabel);
            groupBox7.Controls.Add(label8);
            groupBox7.Location = new Point(8, 6);
            groupBox7.Name = "groupBox7";
            groupBox7.Size = new Size(240, 304);
            groupBox7.TabIndex = 18;
            groupBox7.TabStop = false;
            groupBox7.Text = "Appearance";
            // 
            // _keywordColorAlphaLabel
            // 
            _keywordColorAlphaLabel.AutoSize = true;
            _keywordColorAlphaLabel.Location = new Point(12, 242);
            _keywordColorAlphaLabel.Name = "_keywordColorAlphaLabel";
            _keywordColorAlphaLabel.Size = new Size(133, 15);
            _keywordColorAlphaLabel.TabIndex = 28;
            _keywordColorAlphaLabel.Text = "Keyword Color Intensity";
            // 
            // _keywordColorAlphaTrackBar
            // 
            _keywordColorAlphaTrackBar.AutoSize = false;
            _keywordColorAlphaTrackBar.LargeChange = 10;
            _keywordColorAlphaTrackBar.Location = new Point(6, 263);
            _keywordColorAlphaTrackBar.Maximum = 100;
            _keywordColorAlphaTrackBar.Name = "_keywordColorAlphaTrackBar";
            _keywordColorAlphaTrackBar.Size = new Size(228, 35);
            _keywordColorAlphaTrackBar.TabIndex = 29;
            _keywordColorAlphaTrackBar.TickFrequency = 10;
            // 
            // _numKeywordsLabel
            // 
            _numKeywordsLabel.AutoSize = true;
            _numKeywordsLabel.Location = new Point(89, 217);
            _numKeywordsLabel.Name = "_numKeywordsLabel";
            _numKeywordsLabel.Size = new Size(103, 15);
            _numKeywordsLabel.TabIndex = 27;
            _numKeywordsLabel.Text = "(%x% active rules)";
            // 
            // _keywordColorsButton
            // 
            _keywordColorsButton.Location = new Point(8, 213);
            _keywordColorsButton.Name = "_keywordColorsButton";
            _keywordColorsButton.Size = new Size(75, 23);
            _keywordColorsButton.TabIndex = 26;
            _keywordColorsButton.Text = "Edit...";
            _keywordColorsButton.UseVisualStyleBackColor = true;
            _keywordColorsButton.Click += _keywordColorsButton_Click;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(8, 192);
            label10.Name = "label10";
            label10.Size = new Size(90, 15);
            label10.TabIndex = 25;
            label10.Text = "Keyword Colors";
            // 
            // _highlightBingoCheckBox
            // 
            _highlightBingoCheckBox.AutoSize = true;
            _highlightBingoCheckBox.Location = new Point(12, 165);
            _highlightBingoCheckBox.Name = "_highlightBingoCheckBox";
            _highlightBingoCheckBox.Size = new Size(140, 19);
            _highlightBingoCheckBox.TabIndex = 24;
            _highlightBingoCheckBox.Text = "Highlight Bingo Lines";
            _highlightBingoCheckBox.UseVisualStyleBackColor = true;
            // 
            // _highlightMarkedCheckBox
            // 
            _highlightMarkedCheckBox.AutoSize = true;
            _highlightMarkedCheckBox.Location = new Point(12, 136);
            _highlightMarkedCheckBox.Name = "_highlightMarkedCheckBox";
            _highlightMarkedCheckBox.Size = new Size(163, 19);
            _highlightMarkedCheckBox.TabIndex = 23;
            _highlightMarkedCheckBox.Text = "Highlight Marked Squares";
            _highlightMarkedCheckBox.UseVisualStyleBackColor = true;
            // 
            // _shadowLabel
            // 
            _shadowLabel.AutoSize = true;
            _shadowLabel.Location = new Point(12, 74);
            _shadowLabel.Name = "_shadowLabel";
            _shadowLabel.Size = new Size(132, 15);
            _shadowLabel.TabIndex = 21;
            _shadowLabel.Text = "Square Shadow Opacity";
            // 
            // _shadowTrackBar
            // 
            _shadowTrackBar.AutoSize = false;
            _shadowTrackBar.Location = new Point(6, 95);
            _shadowTrackBar.Name = "_shadowTrackBar";
            _shadowTrackBar.Size = new Size(228, 35);
            _shadowTrackBar.TabIndex = 22;
            // 
            // _outOfFocusClickTextBox
            // 
            _outOfFocusClickTextBox.Location = new Point(7, 43);
            _outOfFocusClickTextBox.Name = "_outOfFocusClickTextBox";
            _outOfFocusClickTextBox.ReadOnly = true;
            _outOfFocusClickTextBox.Size = new Size(105, 23);
            _outOfFocusClickTextBox.TabIndex = 43;
            _outOfFocusClickTextBox.Enter += _outOfFocusClickTextBox_Enter;
            _outOfFocusClickTextBox.KeyDown += _outOfFocusClickTextBox_KeyDown;
            _outOfFocusClickTextBox.Leave += _outOfFocusClickTextBox_Leave;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(6, 19);
            label11.Name = "label11";
            label11.Size = new Size(189, 15);
            label11.TabIndex = 42;
            label11.Text = "Out-of-focus click key (Keyboard):";
            // 
            // groupBox8
            // 
            groupBox8.Controls.Add(_testSoundButton);
            groupBox8.Controls.Add(label14);
            groupBox8.Controls.Add(_soundOutputDeviceComboBox);
            groupBox8.Controls.Add(_volumeLabel);
            groupBox8.Controls.Add(_volumeTrackBar);
            groupBox8.Controls.Add(_soundCheckBox);
            groupBox8.Location = new Point(8, 102);
            groupBox8.Name = "groupBox8";
            groupBox8.Size = new Size(240, 198);
            groupBox8.TabIndex = 5;
            groupBox8.TabStop = false;
            groupBox8.Text = "Sounds";
            // 
            // _testSoundButton
            // 
            _testSoundButton.Location = new Point(7, 163);
            _testSoundButton.Name = "_testSoundButton";
            _testSoundButton.Size = new Size(107, 23);
            _testSoundButton.TabIndex = 11;
            _testSoundButton.Text = "Play Test Sfx";
            _testSoundButton.UseVisualStyleBackColor = true;
            _testSoundButton.Click += _testSoundButton_Click;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(9, 111);
            label14.Name = "label14";
            label14.Size = new Size(86, 15);
            label14.TabIndex = 9;
            label14.Text = "Output Device:";
            // 
            // _soundOutputDeviceComboBox
            // 
            _soundOutputDeviceComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _soundOutputDeviceComboBox.FormattingEnabled = true;
            _soundOutputDeviceComboBox.Location = new Point(8, 132);
            _soundOutputDeviceComboBox.Name = "_soundOutputDeviceComboBox";
            _soundOutputDeviceComboBox.Size = new Size(225, 23);
            _soundOutputDeviceComboBox.TabIndex = 10;
            // 
            // _volumeLabel
            // 
            _volumeLabel.AutoSize = true;
            _volumeLabel.Location = new Point(12, 52);
            _volumeLabel.Name = "_volumeLabel";
            _volumeLabel.Size = new Size(47, 15);
            _volumeLabel.TabIndex = 7;
            _volumeLabel.Text = "Volume";
            // 
            // _volumeTrackBar
            // 
            _volumeTrackBar.AutoSize = false;
            _volumeTrackBar.Location = new Point(6, 73);
            _volumeTrackBar.Name = "_volumeTrackBar";
            _volumeTrackBar.Size = new Size(228, 35);
            _volumeTrackBar.TabIndex = 8;
            // 
            // _soundCheckBox
            // 
            _soundCheckBox.AutoSize = true;
            _soundCheckBox.Location = new Point(12, 25);
            _soundCheckBox.Name = "_soundCheckBox";
            _soundCheckBox.Size = new Size(128, 19);
            _soundCheckBox.TabIndex = 6;
            _soundCheckBox.Text = "Enable alert sounds";
            _soundCheckBox.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(521, 396);
            tabControl1.TabIndex = 34;
            // 
            // tabPage1
            // 
            tabPage1.BackColor = SystemColors.Control;
            tabPage1.Controls.Add(groupBox10);
            tabPage1.Controls.Add(groupBox3);
            tabPage1.Controls.Add(groupBox8);
            tabPage1.Controls.Add(groupBox5);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(513, 368);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "General";
            // 
            // groupBox10
            // 
            groupBox10.Controls.Add(_checkUpdatesCheckBox);
            groupBox10.Location = new Point(257, 102);
            groupBox10.Name = "groupBox10";
            groupBox10.Size = new Size(240, 59);
            groupBox10.TabIndex = 16;
            groupBox10.TabStop = false;
            groupBox10.Text = "Application Updates";
            // 
            // _checkUpdatesCheckBox
            // 
            _checkUpdatesCheckBox.AutoSize = true;
            _checkUpdatesCheckBox.Location = new Point(12, 26);
            _checkUpdatesCheckBox.Name = "_checkUpdatesCheckBox";
            _checkUpdatesCheckBox.Size = new Size(179, 19);
            _checkUpdatesCheckBox.TabIndex = 17;
            _checkUpdatesCheckBox.Text = "Check for updates on startup";
            _checkUpdatesCheckBox.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.BackColor = SystemColors.Control;
            tabPage2.Controls.Add(groupBox11);
            tabPage2.Controls.Add(groupBox9);
            tabPage2.Controls.Add(groupBox2);
            tabPage2.Controls.Add(groupBox7);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(513, 368);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Bingo Board";
            // 
            // groupBox11
            // 
            groupBox11.Controls.Add(_arrowNavigationCheckBox);
            groupBox11.Controls.Add(_numpadNavigationCheckBox);
            groupBox11.Controls.Add(_outOfFocusClickTextBox);
            groupBox11.Controls.Add(label11);
            groupBox11.Location = new Point(257, 232);
            groupBox11.Name = "groupBox11";
            groupBox11.Size = new Size(240, 130);
            groupBox11.TabIndex = 41;
            groupBox11.TabStop = false;
            groupBox11.Text = "Hotkey";
            // 
            // _numpadNavigationCheckBox
            // 
            _numpadNavigationCheckBox.AutoSize = true;
            _numpadNavigationCheckBox.Location = new Point(9, 76);
            _numpadNavigationCheckBox.Name = "_numpadNavigationCheckBox";
            _numpadNavigationCheckBox.Size = new Size(134, 19);
            _numpadNavigationCheckBox.TabIndex = 44;
            _numpadNavigationCheckBox.Text = "Numpad Navigation";
            _numpadNavigationCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            groupBox9.Controls.Add(panel2);
            groupBox9.Controls.Add(label13);
            groupBox9.Location = new Point(257, 119);
            groupBox9.Name = "groupBox9";
            groupBox9.Size = new Size(240, 107);
            groupBox9.TabIndex = 37;
            groupBox9.TabStop = false;
            groupBox9.Text = "Spectator Settings";
            // 
            // panel2
            // 
            panel2.Controls.Add(label12);
            panel2.Controls.Add(_delayMatchEventsTextBox);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(3, 74);
            panel2.Name = "panel2";
            panel2.Size = new Size(234, 30);
            panel2.TabIndex = 2;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(93, 6);
            label12.Name = "label12";
            label12.Size = new Size(73, 15);
            label12.TabIndex = 40;
            label12.Text = "milliseconds";
            // 
            // _delayMatchEventsTextBox
            // 
            _delayMatchEventsTextBox.Location = new Point(6, 3);
            _delayMatchEventsTextBox.MaximumSize = new Size(100, 0);
            _delayMatchEventsTextBox.Name = "_delayMatchEventsTextBox";
            _delayMatchEventsTextBox.Size = new Size(81, 23);
            _delayMatchEventsTextBox.TabIndex = 39;
            // 
            // label13
            // 
            label13.Dock = DockStyle.Top;
            label13.Location = new Point(3, 19);
            label13.Name = "label13";
            label13.Padding = new Padding(3, 3, 3, 0);
            label13.Size = new Size(234, 55);
            label13.TabIndex = 38;
            label13.Text = "When spectating, delay all match events (this includes square checks, counters, match status changes, timer etc.):";
            // 
            // tabPage3
            // 
            tabPage3.BackColor = SystemColors.Control;
            tabPage3.Controls.Add(groupBox4);
            tabPage3.Controls.Add(groupBox1);
            tabPage3.Controls.Add(groupBox6);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(513, 368);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Map";
            // 
            // panel1
            // 
            panel1.Controls.Add(_okButton);
            panel1.Controls.Add(_cancelButton);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 396);
            panel1.Name = "panel1";
            panel1.Size = new Size(521, 29);
            panel1.TabIndex = 34;
            // 
            // _arrowNavigationCheckBox
            // 
            _arrowNavigationCheckBox.AutoSize = true;
            _arrowNavigationCheckBox.Location = new Point(9, 102);
            _arrowNavigationCheckBox.Name = "_arrowNavigationCheckBox";
            _arrowNavigationCheckBox.Size = new Size(119, 19);
            _arrowNavigationCheckBox.TabIndex = 45;
            _arrowNavigationCheckBox.Text = "Arrow Navigation";
            _arrowNavigationCheckBox.UseVisualStyleBackColor = true;
            // 
            // SettingsDialog
            // 
            AcceptButton = _okButton;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = _cancelButton;
            ClientSize = new Size(521, 425);
            Controls.Add(tabControl1);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Settings";
            Load += SettingsDialog_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            groupBox6.ResumeLayout(false);
            groupBox7.ResumeLayout(false);
            groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_keywordColorAlphaTrackBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)_shadowTrackBar).EndInit();
            groupBox8.ResumeLayout(false);
            groupBox8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_volumeTrackBar).EndInit();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            groupBox10.ResumeLayout(false);
            groupBox10.PerformLayout();
            tabPage2.ResumeLayout(false);
            groupBox11.ResumeLayout(false);
            groupBox11.PerformLayout();
            groupBox9.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            tabPage3.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button _okButton;
        private Button _cancelButton;
        private GroupBox groupBox1;
        private TextBox _mapSizeCustomYTextBox;
        private Label label2;
        private RadioButton _mapSizeCustomRadioButton;
        private RadioButton _mapSizeRememberLastRadioButton;
        private TextBox _mapSizeCustomXTextBox;
        private Label label1;
        private GroupBox groupBox2;
        private TextBox _bingoMaxYTextBox;
        private Label label3;
        private RadioButton _bingoCustomMaxSizeRadioButton;
        private RadioButton _bingoNoMaxSizeRadioButton;
        private TextBox _bingoMaxXTextBox;
        private Label label4;
        private GroupBox groupBox3;
        private Label label5;
        private ColorDialog colorDialog1;
        private Panel _colorPanel;
        private GroupBox groupBox4;
        private TextBox _mapPositionYTextBox;
        private Label label6;
        private RadioButton _mapPositionCustomRadioButton;
        private RadioButton _mapPositionRelativeRadioButton;
        private TextBox _mapPositionXTextBox;
        private Label label7;
        private LinkLabel _fontLinkLabel;
        private Label label8;
        private GroupBox groupBox5;
        private TextBox _portTextBox;
        private Label label9;
        private CheckBox _hostServerCheckBox;
        private GroupBox groupBox6;
        private CheckBox _swapMouseButtons;
        private CheckBox _showClassesCheckBox;
        private GroupBox groupBox7;
        private GroupBox groupBox8;
        private CheckBox _soundCheckBox;
        private TrackBar _volumeTrackBar;
        private Label _volumeLabel;
        private TextBox _outOfFocusClickTextBox;
        private Label label11;
        private CheckBox _alwaysOnTopCheckbox;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private Panel panel1;
        private GroupBox groupBox9;
        private Panel panel2;
        private Label label12;
        private TextBox _delayMatchEventsTextBox;
        private Label label13;
        private Label label14;
        private ComboBox _soundOutputDeviceComboBox;
        private Button _testSoundButton;
        private GroupBox groupBox10;
        private CheckBox _checkUpdatesCheckBox;
        private GroupBox groupBox11;
        private CheckBox _highlightBingoCheckBox;
        private CheckBox _highlightMarkedCheckBox;
        private Label _shadowLabel;
        private TrackBar _shadowTrackBar;
        private Button _keywordColorsButton;
        private Label label10;
        private Label _numKeywordsLabel;
        private Label _keywordColorAlphaLabel;
        private TrackBar _keywordColorAlphaTrackBar;
        private CheckBox _numpadNavigationCheckBox;
        private CheckBox _arrowNavigationCheckBox;
    }
}