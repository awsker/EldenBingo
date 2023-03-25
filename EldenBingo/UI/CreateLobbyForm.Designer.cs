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
            this.components = new System.ComponentModel.Container();
            this._roomNameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._nicknameTextBox = new System.Windows.Forms.TextBox();
            this._adminPasswordTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._colorPanel = new System.Windows.Forms.Panel();
            this._teamComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this._cancelButton = new System.Windows.Forms.Button();
            this._createButton = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // _roomNameTextBox
            // 
            this._roomNameTextBox.Location = new System.Drawing.Point(111, 23);
            this._roomNameTextBox.Name = "_roomNameTextBox";
            this._roomNameTextBox.Size = new System.Drawing.Size(113, 23);
            this._roomNameTextBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Room name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "Nickname:";
            // 
            // _nicknameTextBox
            // 
            this._nicknameTextBox.Location = new System.Drawing.Point(111, 22);
            this._nicknameTextBox.Name = "_nicknameTextBox";
            this._nicknameTextBox.Size = new System.Drawing.Size(113, 23);
            this._nicknameTextBox.TabIndex = 7;
            // 
            // _adminPasswordTextBox
            // 
            this._adminPasswordTextBox.Location = new System.Drawing.Point(111, 53);
            this._adminPasswordTextBox.Name = "_adminPasswordTextBox";
            this._adminPasswordTextBox.PasswordChar = '*';
            this._adminPasswordTextBox.PlaceholderText = "(optional)";
            this._adminPasswordTextBox.Size = new System.Drawing.Size(113, 23);
            this._adminPasswordTextBox.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 15);
            this.label3.TabIndex = 3;
            this.label3.Text = "Admin Password:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 15);
            this.label4.TabIndex = 8;
            this.label4.Text = "Color:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 81);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 15);
            this.label5.TabIndex = 10;
            this.label5.Text = "Team:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._colorPanel);
            this.groupBox1.Controls.Add(this._teamComboBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this._nicknameTextBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(12, 108);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(235, 112);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "User Info";
            // 
            // _colorPanel
            // 
            this._colorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._colorPanel.Location = new System.Drawing.Point(111, 49);
            this._colorPanel.Name = "_colorPanel";
            this._colorPanel.Size = new System.Drawing.Size(25, 25);
            this._colorPanel.TabIndex = 9;
            this._colorPanel.Click += new System.EventHandler(this._colorPanel_Click);
            // 
            // _teamComboBox
            // 
            this._teamComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._teamComboBox.FormattingEnabled = true;
            this._teamComboBox.Location = new System.Drawing.Point(111, 78);
            this._teamComboBox.Name = "_teamComboBox";
            this._teamComboBox.Size = new System.Drawing.Size(113, 23);
            this._teamComboBox.TabIndex = 11;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this._roomNameTextBox);
            this.groupBox2.Controls.Add(this._adminPasswordTextBox);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(12, 11);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(235, 87);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Room Info";
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.Location = new System.Drawing.Point(172, 231);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 13;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _createButton
            // 
            this._createButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._createButton.Location = new System.Drawing.Point(91, 231);
            this._createButton.Name = "_createButton";
            this._createButton.Size = new System.Drawing.Size(75, 23);
            this._createButton.TabIndex = 12;
            this._createButton.Text = "OK";
            this._createButton.UseVisualStyleBackColor = true;
            this._createButton.Click += new System.EventHandler(this._createButton_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // CreateLobbyForm
            // 
            this.AcceptButton = this._createButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(263, 264);
            this.Controls.Add(this._createButton);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CreateLobbyForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create Lobby";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CreateLobbyForm_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private TextBox _roomNameTextBox;
        private Label label1;
        private Label label2;
        private TextBox _nicknameTextBox;
        private TextBox _adminPasswordTextBox;
        private Label label3;
        private Label label4;
        private Label label5;
        private GroupBox groupBox1;
        private ComboBox _teamComboBox;
        private GroupBox groupBox2;
        private Button _cancelButton;
        private Button _createButton;
        private ErrorProvider errorProvider1;
        private Panel _colorPanel;
    }
}