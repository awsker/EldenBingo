namespace EldenBingo.UI
{
    partial class AdminControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this._bingoJsonTextBox = new System.Windows.Forms.TextBox();
            this._browseJsonButton = new System.Windows.Forms.Button();
            this._uploadJsonButton = new System.Windows.Forms.Button();
            this._generateNewBoardButton = new System.Windows.Forms.Button();
            this._stopMatchButton = new System.Windows.Forms.Button();
            this._pauseMatchButton = new System.Windows.Forms.Button();
            this._startMatchButton = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this._adminStatusLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._lobbySettingsButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(14, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Upload Bingo JSON:";
            // 
            // _bingoJsonTextBox
            // 
            this._bingoJsonTextBox.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._bingoJsonTextBox.Location = new System.Drawing.Point(163, 38);
            this._bingoJsonTextBox.Name = "_bingoJsonTextBox";
            this._bingoJsonTextBox.Size = new System.Drawing.Size(263, 27);
            this._bingoJsonTextBox.TabIndex = 2;
            // 
            // _browseJsonButton
            // 
            this._browseJsonButton.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._browseJsonButton.Location = new System.Drawing.Point(432, 38);
            this._browseJsonButton.Name = "_browseJsonButton";
            this._browseJsonButton.Size = new System.Drawing.Size(85, 29);
            this._browseJsonButton.TabIndex = 3;
            this._browseJsonButton.Text = "Browse";
            this._browseJsonButton.UseVisualStyleBackColor = true;
            this._browseJsonButton.Click += new System.EventHandler(this._browseJsonButton_Click);
            // 
            // _uploadJsonButton
            // 
            this._uploadJsonButton.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._uploadJsonButton.Location = new System.Drawing.Point(523, 38);
            this._uploadJsonButton.Name = "_uploadJsonButton";
            this._uploadJsonButton.Size = new System.Drawing.Size(85, 29);
            this._uploadJsonButton.TabIndex = 4;
            this._uploadJsonButton.Text = "Upload";
            this._uploadJsonButton.UseVisualStyleBackColor = true;
            this._uploadJsonButton.Click += new System.EventHandler(this._uploadJsonButton_Click);
            // 
            // _generateNewBoardButton
            // 
            this._generateNewBoardButton.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._generateNewBoardButton.Location = new System.Drawing.Point(193, 71);
            this._generateNewBoardButton.Name = "_generateNewBoardButton";
            this._generateNewBoardButton.Size = new System.Drawing.Size(185, 29);
            this._generateNewBoardButton.TabIndex = 6;
            this._generateNewBoardButton.Text = "Randomize New Board";
            this._generateNewBoardButton.UseVisualStyleBackColor = true;
            this._generateNewBoardButton.Click += new System.EventHandler(this._generateNewBoardButton_Click);
            // 
            // _stopMatchButton
            // 
            this._stopMatchButton.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._stopMatchButton.Location = new System.Drawing.Point(263, 108);
            this._stopMatchButton.Name = "_stopMatchButton";
            this._stopMatchButton.Size = new System.Drawing.Size(115, 29);
            this._stopMatchButton.TabIndex = 9;
            this._stopMatchButton.Text = "Stop Match";
            this._stopMatchButton.UseVisualStyleBackColor = true;
            this._stopMatchButton.Click += new System.EventHandler(this._stopMatchButton_Click);
            // 
            // _pauseMatchButton
            // 
            this._pauseMatchButton.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._pauseMatchButton.Location = new System.Drawing.Point(138, 108);
            this._pauseMatchButton.Name = "_pauseMatchButton";
            this._pauseMatchButton.Size = new System.Drawing.Size(115, 29);
            this._pauseMatchButton.TabIndex = 8;
            this._pauseMatchButton.Text = "Pause Match";
            this._pauseMatchButton.UseVisualStyleBackColor = true;
            this._pauseMatchButton.Click += new System.EventHandler(this._pauseMatchButton_Click);
            // 
            // _startMatchButton
            // 
            this._startMatchButton.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._startMatchButton.Location = new System.Drawing.Point(14, 108);
            this._startMatchButton.Name = "_startMatchButton";
            this._startMatchButton.Size = new System.Drawing.Size(115, 29);
            this._startMatchButton.TabIndex = 7;
            this._startMatchButton.Text = "Start Match";
            this._startMatchButton.UseVisualStyleBackColor = true;
            this._startMatchButton.Click += new System.EventHandler(this._startMatchButton_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // _adminStatusLabel
            // 
            this._adminStatusLabel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this._adminStatusLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._adminStatusLabel.Location = new System.Drawing.Point(379, 110);
            this._adminStatusLabel.Name = "_adminStatusLabel";
            this._adminStatusLabel.Size = new System.Drawing.Size(257, 29);
            this._adminStatusLabel.AutoSize = true;
            this._adminStatusLabel.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(8, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 20);
            this.label3.TabIndex = 0;
            this.label3.Text = "Admin Controls";
            // 
            // _lobbySettingsButton
            // 
            this._lobbySettingsButton.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._lobbySettingsButton.Location = new System.Drawing.Point(14, 71);
            this._lobbySettingsButton.Name = "_lobbySettingsButton";
            this._lobbySettingsButton.Size = new System.Drawing.Size(173, 29);
            this._lobbySettingsButton.TabIndex = 5;
            this._lobbySettingsButton.Text = "Edit Lobby Settings";
            this._lobbySettingsButton.UseVisualStyleBackColor = true;
            this._lobbySettingsButton.Click += new System.EventHandler(this._lobbySettingsButton_Click);
            // 
            // AdminControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.Controls.Add(this._lobbySettingsButton);
            this.Controls.Add(this._adminStatusLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._generateNewBoardButton);
            this.Controls.Add(this._stopMatchButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._startMatchButton);
            this.Controls.Add(this._browseJsonButton);
            this.Controls.Add(this._pauseMatchButton);
            this.Controls.Add(this._bingoJsonTextBox);
            this.Controls.Add(this._uploadJsonButton);
            this.Name = "AdminControl";
            this.Size = new System.Drawing.Size(639, 143);
            this.Load += new System.EventHandler(this.AdminControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private TextBox _bingoJsonTextBox;
        private Button _browseJsonButton;
        private Button _uploadJsonButton;
        private Button _generateNewBoardButton;
        private Button _stopMatchButton;
        private Button _pauseMatchButton;
        private Button _startMatchButton;
        private ErrorProvider errorProvider1;
        private Label _adminStatusLabel;
        private Label label3;
        private Button _lobbySettingsButton;
    }
}
