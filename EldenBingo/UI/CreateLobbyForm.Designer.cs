namespace EldenBingo.UI
{
    partial class CreateLobbyForm
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
            components = new System.ComponentModel.Container();
            _roomNameTextBox = new TextBox();
            label1 = new Label();
            label2 = new Label();
            _nicknameTextBox = new TextBox();
            _adminPasswordTextBox = new TextBox();
            label3 = new Label();
            label5 = new Label();
            groupBox1 = new GroupBox();
            _colorPanel = new Panel();
            _teamComboBox = new ComboBox();
            groupBox2 = new GroupBox();
            _cancelButton = new Button();
            _createButton = new Button();
            errorProvider1 = new ErrorProvider(components);
            _lobbySettingsButton = new Button();
            _seedLabel = new Label();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
            SuspendLayout();
            // 
            // _roomNameTextBox
            // 
            _roomNameTextBox.Location = new Point(111, 23);
            _roomNameTextBox.Name = "_roomNameTextBox";
            _roomNameTextBox.Size = new Size(113, 23);
            _roomNameTextBox.TabIndex = 7;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 26);
            label1.Name = "label1";
            label1.Size = new Size(75, 15);
            label1.TabIndex = 6;
            label1.Text = "Room name:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 25);
            label2.Name = "label2";
            label2.Size = new Size(64, 15);
            label2.TabIndex = 0;
            label2.Text = "Nickname:";
            // 
            // _nicknameTextBox
            // 
            _nicknameTextBox.Location = new Point(111, 22);
            _nicknameTextBox.Name = "_nicknameTextBox";
            _nicknameTextBox.Size = new Size(113, 23);
            _nicknameTextBox.TabIndex = 1;
            // 
            // _adminPasswordTextBox
            // 
            _adminPasswordTextBox.Location = new Point(111, 53);
            _adminPasswordTextBox.Name = "_adminPasswordTextBox";
            _adminPasswordTextBox.PasswordChar = '*';
            _adminPasswordTextBox.PlaceholderText = "(optional)";
            _adminPasswordTextBox.Size = new Size(113, 23);
            _adminPasswordTextBox.TabIndex = 9;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 56);
            label3.Name = "label3";
            label3.Size = new Size(99, 15);
            label3.TabIndex = 8;
            label3.Text = "Admin Password:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(6, 57);
            label5.Name = "label5";
            label5.Size = new Size(38, 15);
            label5.TabIndex = 4;
            label5.Text = "Team:";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(_colorPanel);
            groupBox1.Controls.Add(_teamComboBox);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(_nicknameTextBox);
            groupBox1.Location = new Point(12, 9);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(235, 86);
            groupBox1.TabIndex = 5;
            groupBox1.TabStop = false;
            groupBox1.Text = "User Info";
            // 
            // _colorPanel
            // 
            _colorPanel.BorderStyle = BorderStyle.FixedSingle;
            _colorPanel.Location = new Point(80, 53);
            _colorPanel.Name = "_colorPanel";
            _colorPanel.Size = new Size(26, 25);
            _colorPanel.TabIndex = 3;
            // 
            // _teamComboBox
            // 
            _teamComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _teamComboBox.FormattingEnabled = true;
            _teamComboBox.Location = new Point(111, 54);
            _teamComboBox.Name = "_teamComboBox";
            _teamComboBox.Size = new Size(113, 23);
            _teamComboBox.TabIndex = 5;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label1);
            groupBox2.Controls.Add(_roomNameTextBox);
            groupBox2.Controls.Add(_adminPasswordTextBox);
            groupBox2.Controls.Add(label3);
            groupBox2.Location = new Point(12, 101);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(235, 87);
            groupBox2.TabIndex = 0;
            groupBox2.TabStop = false;
            groupBox2.Text = "Room Info";
            // 
            // _cancelButton
            // 
            _cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _cancelButton.Location = new Point(172, 223);
            _cancelButton.Name = "_cancelButton";
            _cancelButton.Size = new Size(75, 23);
            _cancelButton.TabIndex = 11;
            _cancelButton.Text = "Cancel";
            _cancelButton.UseVisualStyleBackColor = true;
            _cancelButton.Click += _cancelButton_Click;
            // 
            // _createButton
            // 
            _createButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _createButton.Location = new Point(91, 223);
            _createButton.Name = "_createButton";
            _createButton.Size = new Size(75, 23);
            _createButton.TabIndex = 10;
            _createButton.Text = "OK";
            _createButton.UseVisualStyleBackColor = true;
            _createButton.Click += _createButton_Click;
            // 
            // errorProvider1
            // 
            errorProvider1.ContainerControl = this;
            // 
            // _lobbySettingsButton
            // 
            _lobbySettingsButton.Location = new Point(92, 193);
            _lobbySettingsButton.Name = "_lobbySettingsButton";
            _lobbySettingsButton.Size = new Size(155, 23);
            _lobbySettingsButton.TabIndex = 12;
            _lobbySettingsButton.Text = "Lobby Settings >>";
            _lobbySettingsButton.UseVisualStyleBackColor = true;
            _lobbySettingsButton.Click += _lobbySettingsButton_Click;
            // 
            // _seedLabel
            // 
            _seedLabel.AutoSize = true;
            _seedLabel.ForeColor = Color.DarkGreen;
            _seedLabel.Location = new Point(12, 197);
            _seedLabel.Name = "_seedLabel";
            _seedLabel.Size = new Size(44, 15);
            _seedLabel.TabIndex = 13;
            _seedLabel.Text = "Seed: 0";
            _seedLabel.Visible = false;
            // 
            // CreateLobbyForm
            // 
            AcceptButton = _createButton;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = _cancelButton;
            ClientSize = new Size(258, 256);
            Controls.Add(_seedLabel);
            Controls.Add(_lobbySettingsButton);
            Controls.Add(_createButton);
            Controls.Add(_cancelButton);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "CreateLobbyForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Create Lobby";
            FormClosing += CreateLobbyForm_FormClosing;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox _roomNameTextBox;
        private Label label1;
        private Label label2;
        private TextBox _nicknameTextBox;
        private TextBox _adminPasswordTextBox;
        private Label label3;
        private Label label5;
        private GroupBox groupBox1;
        private ComboBox _teamComboBox;
        private GroupBox groupBox2;
        private Button _cancelButton;
        private Button _createButton;
        private ErrorProvider errorProvider1;
        private Panel _colorPanel;
        private Button _lobbySettingsButton;
        private Label _seedLabel;
    }
}