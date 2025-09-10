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
            components = new System.ComponentModel.Container();
            label1 = new Label();
            _bingoJsonTextBox = new TextBox();
            _browseJsonButton = new Button();
            _uploadJsonButton = new Button();
            _generateNewBoardButton = new Button();
            _stopMatchButton = new Button();
            _pauseMatchButton = new Button();
            _startMatchButton = new Button();
            errorProvider1 = new ErrorProvider(components);
            _adminStatusLabel = new Label();
            label3 = new Label();
            _lobbySettingsButton = new Button();
            ((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label1.ForeColor = Color.White;
            label1.Location = new Point(14, 41);
            label1.Name = "label1";
            label1.Size = new Size(143, 20);
            label1.TabIndex = 1;
            label1.Text = "Upload Bingo JSON:";
            // 
            // _bingoJsonTextBox
            // 
            _bingoJsonTextBox.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            _bingoJsonTextBox.HideSelection = false;
            _bingoJsonTextBox.Location = new Point(163, 38);
            _bingoJsonTextBox.Name = "_bingoJsonTextBox";
            _bingoJsonTextBox.Size = new Size(263, 27);
            _bingoJsonTextBox.TabIndex = 2;
            // 
            // _browseJsonButton
            // 
            _browseJsonButton.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            _browseJsonButton.Location = new Point(432, 38);
            _browseJsonButton.Name = "_browseJsonButton";
            _browseJsonButton.Size = new Size(85, 29);
            _browseJsonButton.TabIndex = 3;
            _browseJsonButton.Text = "Browse";
            _browseJsonButton.UseVisualStyleBackColor = true;
            _browseJsonButton.Click += _browseJsonButton_Click;
            // 
            // _uploadJsonButton
            // 
            _uploadJsonButton.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            _uploadJsonButton.Location = new Point(523, 38);
            _uploadJsonButton.Name = "_uploadJsonButton";
            _uploadJsonButton.Size = new Size(85, 29);
            _uploadJsonButton.TabIndex = 4;
            _uploadJsonButton.Text = "Upload";
            _uploadJsonButton.UseVisualStyleBackColor = true;
            _uploadJsonButton.Click += _uploadJsonButton_Click;
            // 
            // _generateNewBoardButton
            // 
            _generateNewBoardButton.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            _generateNewBoardButton.Location = new Point(193, 71);
            _generateNewBoardButton.Name = "_generateNewBoardButton";
            _generateNewBoardButton.Size = new Size(185, 29);
            _generateNewBoardButton.TabIndex = 6;
            _generateNewBoardButton.Text = "Randomize New Board";
            _generateNewBoardButton.UseVisualStyleBackColor = true;
            _generateNewBoardButton.Click += _generateNewBoardButton_Click;
            // 
            // _stopMatchButton
            // 
            _stopMatchButton.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            _stopMatchButton.Location = new Point(263, 108);
            _stopMatchButton.Name = "_stopMatchButton";
            _stopMatchButton.Size = new Size(115, 29);
            _stopMatchButton.TabIndex = 9;
            _stopMatchButton.Text = "Stop Match";
            _stopMatchButton.UseVisualStyleBackColor = true;
            _stopMatchButton.Click += _stopMatchButton_Click;
            // 
            // _pauseMatchButton
            // 
            _pauseMatchButton.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            _pauseMatchButton.Location = new Point(138, 108);
            _pauseMatchButton.Name = "_pauseMatchButton";
            _pauseMatchButton.Size = new Size(115, 29);
            _pauseMatchButton.TabIndex = 8;
            _pauseMatchButton.Text = "Pause Match";
            _pauseMatchButton.UseVisualStyleBackColor = true;
            _pauseMatchButton.Click += _pauseMatchButton_Click;
            // 
            // _startMatchButton
            // 
            _startMatchButton.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            _startMatchButton.Location = new Point(14, 108);
            _startMatchButton.Name = "_startMatchButton";
            _startMatchButton.Size = new Size(115, 29);
            _startMatchButton.TabIndex = 7;
            _startMatchButton.Text = "Start Match";
            _startMatchButton.UseVisualStyleBackColor = true;
            _startMatchButton.Click += _startMatchButton_Click;
            // 
            // errorProvider1
            // 
            errorProvider1.ContainerControl = this;
            // 
            // _adminStatusLabel
            // 
            _adminStatusLabel.AutoSize = true;
            _adminStatusLabel.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            _adminStatusLabel.Location = new Point(379, 110);
            _adminStatusLabel.Name = "_adminStatusLabel";
            _adminStatusLabel.Size = new Size(0, 20);
            _adminStatusLabel.TabIndex = 6;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label3.ForeColor = Color.White;
            label3.Location = new Point(8, 9);
            label3.Name = "label3";
            label3.Size = new Size(112, 20);
            label3.TabIndex = 0;
            label3.Text = "Admin Controls";
            // 
            // _lobbySettingsButton
            // 
            _lobbySettingsButton.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            _lobbySettingsButton.Location = new Point(14, 71);
            _lobbySettingsButton.Name = "_lobbySettingsButton";
            _lobbySettingsButton.Size = new Size(173, 29);
            _lobbySettingsButton.TabIndex = 5;
            _lobbySettingsButton.Text = "Edit Lobby Settings";
            _lobbySettingsButton.UseVisualStyleBackColor = true;
            _lobbySettingsButton.Click += _lobbySettingsButton_Click;
            // 
            // AdminControl
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(_lobbySettingsButton);
            Controls.Add(_adminStatusLabel);
            Controls.Add(label3);
            Controls.Add(_generateNewBoardButton);
            Controls.Add(_stopMatchButton);
            Controls.Add(label1);
            Controls.Add(_startMatchButton);
            Controls.Add(_browseJsonButton);
            Controls.Add(_pauseMatchButton);
            Controls.Add(_bingoJsonTextBox);
            Controls.Add(_uploadJsonButton);
            Name = "AdminControl";
            Size = new Size(639, 143);
            Load += AdminControl_Load;
            ((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
            ResumeLayout(false);
            PerformLayout();
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
