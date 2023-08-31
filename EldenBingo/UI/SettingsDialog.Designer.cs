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
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._mapSizeCustomYTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._mapSizeCustomRadioButton = new System.Windows.Forms.RadioButton();
            this._mapSizeRememberLastRadioButton = new System.Windows.Forms.RadioButton();
            this._mapSizeCustomXTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this._bingoMaxYTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this._bingoCustomMaxSizeRadioButton = new System.Windows.Forms.RadioButton();
            this._bingoNoMaxSizeRadioButton = new System.Windows.Forms.RadioButton();
            this._bingoMaxXTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this._colorPanel = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this._fontLinkLabel = new System.Windows.Forms.LinkLabel();
            this.label8 = new System.Windows.Forms.Label();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this._mapPositionYTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this._mapPositionCustomRadioButton = new System.Windows.Forms.RadioButton();
            this._mapPositionRelativeRadioButton = new System.Windows.Forms.RadioButton();
            this._mapPositionXTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this._portTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this._hostServerCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this._showClassesCheckBox = new System.Windows.Forms.CheckBox();
            this._swapMouseButtons = new System.Windows.Forms.CheckBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this._clickIncrementsCountCheckbox = new System.Windows.Forms.CheckBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this._volumeTrackBar = new System.Windows.Forms.TrackBar();
            this._soundCheckBox = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._volumeTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // _okButton
            // 
            this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._okButton.Location = new System.Drawing.Point(548, 356);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 12;
            this._okButton.Text = "OK";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.Location = new System.Drawing.Point(629, 356);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 13;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._mapSizeCustomYTextBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this._mapSizeCustomRadioButton);
            this.groupBox1.Controls.Add(this._mapSizeRememberLastRadioButton);
            this.groupBox1.Controls.Add(this._mapSizeCustomXTextBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 107);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Map Initial Size";
            // 
            // _mapSizeCustomYTextBox
            // 
            this._mapSizeCustomYTextBox.Location = new System.Drawing.Point(117, 74);
            this._mapSizeCustomYTextBox.Name = "_mapSizeCustomYTextBox";
            this._mapSizeCustomYTextBox.Size = new System.Drawing.Size(54, 23);
            this._mapSizeCustomYTextBox.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(94, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Y:";
            // 
            // _mapSizeCustomRadioButton
            // 
            this._mapSizeCustomRadioButton.AutoSize = true;
            this._mapSizeCustomRadioButton.Location = new System.Drawing.Point(9, 46);
            this._mapSizeCustomRadioButton.Name = "_mapSizeCustomRadioButton";
            this._mapSizeCustomRadioButton.Size = new System.Drawing.Size(90, 19);
            this._mapSizeCustomRadioButton.TabIndex = 3;
            this._mapSizeCustomRadioButton.TabStop = true;
            this._mapSizeCustomRadioButton.Text = "Custom Size";
            this._mapSizeCustomRadioButton.UseVisualStyleBackColor = true;
            // 
            // _mapSizeRememberLastRadioButton
            // 
            this._mapSizeRememberLastRadioButton.AutoSize = true;
            this._mapSizeRememberLastRadioButton.Location = new System.Drawing.Point(9, 22);
            this._mapSizeRememberLastRadioButton.Name = "_mapSizeRememberLastRadioButton";
            this._mapSizeRememberLastRadioButton.Size = new System.Drawing.Size(130, 19);
            this._mapSizeRememberLastRadioButton.TabIndex = 2;
            this._mapSizeRememberLastRadioButton.TabStop = true;
            this._mapSizeRememberLastRadioButton.Text = "Remember Last Size";
            this._mapSizeRememberLastRadioButton.UseVisualStyleBackColor = true;
            // 
            // _mapSizeCustomXTextBox
            // 
            this._mapSizeCustomXTextBox.Location = new System.Drawing.Point(30, 74);
            this._mapSizeCustomXTextBox.Name = "_mapSizeCustomXTextBox";
            this._mapSizeCustomXTextBox.Size = new System.Drawing.Size(54, 23);
            this._mapSizeCustomXTextBox.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "X:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this._bingoMaxYTextBox);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this._bingoCustomMaxSizeRadioButton);
            this.groupBox2.Controls.Add(this._bingoNoMaxSizeRadioButton);
            this.groupBox2.Controls.Add(this._bingoMaxXTextBox);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(12, 239);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 107);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Bingo Board Max Size";
            // 
            // _bingoMaxYTextBox
            // 
            this._bingoMaxYTextBox.Location = new System.Drawing.Point(117, 72);
            this._bingoMaxYTextBox.Name = "_bingoMaxYTextBox";
            this._bingoMaxYTextBox.Size = new System.Drawing.Size(54, 23);
            this._bingoMaxYTextBox.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(94, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 15);
            this.label3.TabIndex = 11;
            this.label3.Text = "Y:";
            // 
            // _bingoCustomMaxSizeRadioButton
            // 
            this._bingoCustomMaxSizeRadioButton.AutoSize = true;
            this._bingoCustomMaxSizeRadioButton.Location = new System.Drawing.Point(9, 46);
            this._bingoCustomMaxSizeRadioButton.Name = "_bingoCustomMaxSizeRadioButton";
            this._bingoCustomMaxSizeRadioButton.Size = new System.Drawing.Size(116, 19);
            this._bingoCustomMaxSizeRadioButton.TabIndex = 13;
            this._bingoCustomMaxSizeRadioButton.TabStop = true;
            this._bingoCustomMaxSizeRadioButton.Text = "Custom Max Size";
            this._bingoCustomMaxSizeRadioButton.UseVisualStyleBackColor = false;
            // 
            // _bingoNoMaxSizeRadioButton
            // 
            this._bingoNoMaxSizeRadioButton.AutoSize = true;
            this._bingoNoMaxSizeRadioButton.Location = new System.Drawing.Point(9, 22);
            this._bingoNoMaxSizeRadioButton.Name = "_bingoNoMaxSizeRadioButton";
            this._bingoNoMaxSizeRadioButton.Size = new System.Drawing.Size(122, 19);
            this._bingoNoMaxSizeRadioButton.TabIndex = 12;
            this._bingoNoMaxSizeRadioButton.TabStop = true;
            this._bingoNoMaxSizeRadioButton.Text = "No Maximum Size";
            this._bingoNoMaxSizeRadioButton.UseVisualStyleBackColor = true;
            // 
            // _bingoMaxXTextBox
            // 
            this._bingoMaxXTextBox.Location = new System.Drawing.Point(30, 72);
            this._bingoMaxXTextBox.Name = "_bingoMaxXTextBox";
            this._bingoMaxXTextBox.Size = new System.Drawing.Size(54, 23);
            this._bingoMaxXTextBox.TabIndex = 14;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "X:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this._colorPanel);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Location = new System.Drawing.Point(237, 15);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(233, 60);
            this.groupBox3.TabIndex = 16;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Appearance";
            // 
            // _colorPanel
            // 
            this._colorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._colorPanel.Location = new System.Drawing.Point(168, 22);
            this._colorPanel.Name = "_colorPanel";
            this._colorPanel.Size = new System.Drawing.Size(25, 25);
            this._colorPanel.TabIndex = 4;
            this._colorPanel.Click += new System.EventHandler(this._colorPanel_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(153, 15);
            this.label5.TabIndex = 0;
            this.label5.Text = "Window Background Color:";
            // 
            // _fontLinkLabel
            // 
            this._fontLinkLabel.AutoSize = true;
            this._fontLinkLabel.Font = new System.Drawing.Font("Lucida Sans Unicode", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._fontLinkLabel.Location = new System.Drawing.Point(8, 43);
            this._fontLinkLabel.Name = "_fontLinkLabel";
            this._fontLinkLabel.Size = new System.Drawing.Size(90, 20);
            this._fontLinkLabel.TabIndex = 18;
            this._fontLinkLabel.TabStop = true;
            this._fontLinkLabel.Text = "FontName";
            this._fontLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._fontLinkLabel_LinkClicked);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 23);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(80, 15);
            this.label8.TabIndex = 5;
            this.label8.Text = "Font and Size:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this._mapPositionYTextBox);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this._mapPositionCustomRadioButton);
            this.groupBox4.Controls.Add(this._mapPositionRelativeRadioButton);
            this.groupBox4.Controls.Add(this._mapPositionXTextBox);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Location = new System.Drawing.Point(12, 130);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(200, 107);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Map Initial Position";
            // 
            // _mapPositionYTextBox
            // 
            this._mapPositionYTextBox.Location = new System.Drawing.Point(117, 72);
            this._mapPositionYTextBox.Name = "_mapPositionYTextBox";
            this._mapPositionYTextBox.Size = new System.Drawing.Size(54, 23);
            this._mapPositionYTextBox.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(94, 75);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 15);
            this.label6.TabIndex = 5;
            this.label6.Text = "Y:";
            // 
            // _mapPositionCustomRadioButton
            // 
            this._mapPositionCustomRadioButton.AutoSize = true;
            this._mapPositionCustomRadioButton.Location = new System.Drawing.Point(9, 46);
            this._mapPositionCustomRadioButton.Name = "_mapPositionCustomRadioButton";
            this._mapPositionCustomRadioButton.Size = new System.Drawing.Size(113, 19);
            this._mapPositionCustomRadioButton.TabIndex = 8;
            this._mapPositionCustomRadioButton.TabStop = true;
            this._mapPositionCustomRadioButton.Text = "Custom Position";
            this._mapPositionCustomRadioButton.UseVisualStyleBackColor = true;
            // 
            // _mapPositionRelativeRadioButton
            // 
            this._mapPositionRelativeRadioButton.AutoSize = true;
            this._mapPositionRelativeRadioButton.Location = new System.Drawing.Point(9, 22);
            this._mapPositionRelativeRadioButton.Name = "_mapPositionRelativeRadioButton";
            this._mapPositionRelativeRadioButton.Size = new System.Drawing.Size(127, 19);
            this._mapPositionRelativeRadioButton.TabIndex = 7;
            this._mapPositionRelativeRadioButton.TabStop = true;
            this._mapPositionRelativeRadioButton.Text = "Relative to Window";
            this._mapPositionRelativeRadioButton.UseVisualStyleBackColor = true;
            // 
            // _mapPositionXTextBox
            // 
            this._mapPositionXTextBox.Location = new System.Drawing.Point(30, 72);
            this._mapPositionXTextBox.Name = "_mapPositionXTextBox";
            this._mapPositionXTextBox.Size = new System.Drawing.Size(54, 23);
            this._mapPositionXTextBox.TabIndex = 9;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 75);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 15);
            this.label7.TabIndex = 1;
            this.label7.Text = "X:";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this._portTextBox);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this._hostServerCheckBox);
            this.groupBox5.Location = new System.Drawing.Point(495, 15);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(209, 90);
            this.groupBox5.TabIndex = 17;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Server";
            // 
            // _portTextBox
            // 
            this._portTextBox.Location = new System.Drawing.Point(50, 59);
            this._portTextBox.Name = "_portTextBox";
            this._portTextBox.Size = new System.Drawing.Size(61, 23);
            this._portTextBox.TabIndex = 19;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 62);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(32, 15);
            this.label9.TabIndex = 1;
            this.label9.Text = "Port:";
            // 
            // _hostServerCheckBox
            // 
            this._hostServerCheckBox.AutoSize = true;
            this._hostServerCheckBox.Location = new System.Drawing.Point(12, 28);
            this._hostServerCheckBox.Name = "_hostServerCheckBox";
            this._hostServerCheckBox.Size = new System.Drawing.Size(184, 19);
            this._hostServerCheckBox.TabIndex = 18;
            this._hostServerCheckBox.Text = "Host a bingo server on launch";
            this._hostServerCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this._showClassesCheckBox);
            this.groupBox6.Controls.Add(this._swapMouseButtons);
            this.groupBox6.Location = new System.Drawing.Point(237, 228);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(233, 118);
            this.groupBox6.TabIndex = 20;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Map";
            // 
            // _showClassesCheckBox
            // 
            this._showClassesCheckBox.Location = new System.Drawing.Point(9, 65);
            this._showClassesCheckBox.Name = "_showClassesCheckBox";
            this._showClassesCheckBox.Size = new System.Drawing.Size(218, 41);
            this._showClassesCheckBox.TabIndex = 2;
            this._showClassesCheckBox.Text = "Show available classes in an overlay on the map (for streaming)";
            this._showClassesCheckBox.UseVisualStyleBackColor = true;
            // 
            // _swapMouseButtons
            // 
            this._swapMouseButtons.Location = new System.Drawing.Point(9, 19);
            this._swapMouseButtons.Name = "_swapMouseButtons";
            this._swapMouseButtons.Size = new System.Drawing.Size(218, 40);
            this._swapMouseButtons.TabIndex = 1;
            this._swapMouseButtons.Text = "Swap mouse buttons***(Left = Draw, Right = Pan)";
            this._swapMouseButtons.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this._clickIncrementsCountCheckbox);
            this.groupBox7.Controls.Add(this._fontLinkLabel);
            this.groupBox7.Controls.Add(this.label8);
            this.groupBox7.Location = new System.Drawing.Point(237, 85);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(233, 135);
            this.groupBox7.TabIndex = 17;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Bingo Board";
            // 
            // _clickIncrementsCountCheckbox
            // 
            this._clickIncrementsCountCheckbox.Location = new System.Drawing.Point(9, 71);
            this._clickIncrementsCountCheckbox.Name = "_clickIncrementsCountCheckbox";
            this._clickIncrementsCountCheckbox.Size = new System.Drawing.Size(219, 54);
            this._clickIncrementsCountCheckbox.TabIndex = 19;
            this._clickIncrementsCountCheckbox.Text = "Clicking on \"Counted\" square increments the counter instead of marking the square" +
    "";
            this._clickIncrementsCountCheckbox.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.label10);
            this.groupBox8.Controls.Add(this._volumeTrackBar);
            this.groupBox8.Controls.Add(this._soundCheckBox);
            this.groupBox8.Location = new System.Drawing.Point(495, 116);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(209, 121);
            this.groupBox8.TabIndex = 21;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Sounds";
            // 
            // _volumeTrackBar
            // 
            this._volumeTrackBar.Location = new System.Drawing.Point(6, 76);
            this._volumeTrackBar.Name = "_volumeTrackBar";
            this._volumeTrackBar.Size = new System.Drawing.Size(197, 45);
            this._volumeTrackBar.TabIndex = 21;
            // 
            // _soundCheckBox
            // 
            this._soundCheckBox.AutoSize = true;
            this._soundCheckBox.Location = new System.Drawing.Point(12, 25);
            this._soundCheckBox.Name = "_soundCheckBox";
            this._soundCheckBox.Size = new System.Drawing.Size(128, 19);
            this._soundCheckBox.TabIndex = 20;
            this._soundCheckBox.Text = "Enable alert sounds";
            this._soundCheckBox.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 55);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(47, 15);
            this.label10.TabIndex = 22;
            this.label10.Text = "Volume";
            // 
            // SettingsDialog
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(716, 391);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._volumeTrackBar)).EndInit();
            this.ResumeLayout(false);

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
        private CheckBox _clickIncrementsCountCheckbox;
        private GroupBox groupBox8;
        private CheckBox _soundCheckBox;
        private TrackBar _volumeTrackBar;
        private Label label10;
    }
}