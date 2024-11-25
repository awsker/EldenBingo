namespace EldenBingo
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            tabControl1 = new TabControl();
            _consolePage = new TabPage();
            _consoleControl = new UI.ConsoleControl();
            _lobbyPage = new TabPage();
            _lobbyControl = new UI.LobbyControl();
            _processMonitorStatusTextBox = new TextBox();
            toolStrip1 = new ToolStrip();
            _connectButton = new ToolStripButton();
            _disconnectButton = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            _createLobbyButton = new ToolStripButton();
            _joinLobbyButton = new ToolStripButton();
            _leaveRoomButton = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            _openMapButton = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            _settingsButton = new ToolStripButton();
            _startGameButton = new ToolStripButton();
            splitContainer1 = new SplitContainer();
            _usersListBox = new UI.RichListBox();
            panel1 = new Panel();
            _clientStatusTextBox = new TextBox();
            _changeTeamButton = new ToolStripButton();
            tabControl1.SuspendLayout();
            _consolePage.SuspendLayout();
            _lobbyPage.SuspendLayout();
            toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Alignment = TabAlignment.Left;
            tabControl1.Controls.Add(_consolePage);
            tabControl1.Controls.Add(_lobbyPage);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            tabControl1.Location = new Point(0, 70);
            tabControl1.Multiline = true;
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(947, 591);
            tabControl1.TabIndex = 1;
            // 
            // _consolePage
            // 
            _consolePage.Controls.Add(_consoleControl);
            _consolePage.Location = new Point(30, 4);
            _consolePage.Margin = new Padding(0);
            _consolePage.Name = "_consolePage";
            _consolePage.Size = new Size(913, 583);
            _consolePage.TabIndex = 0;
            _consolePage.Text = "Console";
            _consolePage.UseVisualStyleBackColor = true;
            _consolePage.Click += _createLobbyButton_Click;
            // 
            // _consoleControl
            // 
            _consoleControl.Dock = DockStyle.Fill;
            _consoleControl.Location = new Point(0, 0);
            _consoleControl.Name = "_consoleControl";
            _consoleControl.Size = new Size(913, 583);
            _consoleControl.TabIndex = 0;
            // 
            // _lobbyPage
            // 
            _lobbyPage.Controls.Add(_lobbyControl);
            _lobbyPage.Location = new Point(30, 4);
            _lobbyPage.Name = "_lobbyPage";
            _lobbyPage.Size = new Size(913, 583);
            _lobbyPage.TabIndex = 1;
            _lobbyPage.Text = "Lobby";
            _lobbyPage.UseVisualStyleBackColor = true;
            // 
            // _lobbyControl
            // 
            _lobbyControl.Client = null;
            _lobbyControl.Dock = DockStyle.Fill;
            _lobbyControl.Location = new Point(0, 0);
            _lobbyControl.Name = "_lobbyControl";
            _lobbyControl.Size = new Size(913, 583);
            _lobbyControl.TabIndex = 0;
            // 
            // _processMonitorStatusTextBox
            // 
            _processMonitorStatusTextBox.BackColor = SystemColors.Control;
            _processMonitorStatusTextBox.Dock = DockStyle.Right;
            _processMonitorStatusTextBox.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            _processMonitorStatusTextBox.Location = new Point(753, 0);
            _processMonitorStatusTextBox.Name = "_processMonitorStatusTextBox";
            _processMonitorStatusTextBox.ReadOnly = true;
            _processMonitorStatusTextBox.Size = new Size(194, 27);
            _processMonitorStatusTextBox.TabIndex = 3;
            _processMonitorStatusTextBox.TextAlign = HorizontalAlignment.Center;
            // 
            // toolStrip1
            // 
            toolStrip1.AutoSize = false;
            toolStrip1.Items.AddRange(new ToolStripItem[] { _connectButton, _disconnectButton, toolStripSeparator1, _createLobbyButton, _joinLobbyButton, _leaveRoomButton, _changeTeamButton, toolStripSeparator2, _openMapButton, toolStripSeparator3, _settingsButton, _startGameButton });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(947, 70);
            toolStrip1.TabIndex = 4;
            toolStrip1.Text = "toolStrip1";
            // 
            // _connectButton
            // 
            _connectButton.Image = (Image)resources.GetObject("_connectButton.Image");
            _connectButton.ImageScaling = ToolStripItemImageScaling.None;
            _connectButton.ImageTransparentColor = Color.Magenta;
            _connectButton.Name = "_connectButton";
            _connectButton.Size = new Size(56, 67);
            _connectButton.Text = "Connect";
            _connectButton.TextAlign = ContentAlignment.BottomCenter;
            _connectButton.TextImageRelation = TextImageRelation.ImageAboveText;
            _connectButton.Click += _connectButton_Click;
            // 
            // _disconnectButton
            // 
            _disconnectButton.Image = (Image)resources.GetObject("_disconnectButton.Image");
            _disconnectButton.ImageScaling = ToolStripItemImageScaling.None;
            _disconnectButton.ImageTransparentColor = Color.Magenta;
            _disconnectButton.Name = "_disconnectButton";
            _disconnectButton.Size = new Size(70, 67);
            _disconnectButton.Text = "Disconnect";
            _disconnectButton.TextAlign = ContentAlignment.BottomCenter;
            _disconnectButton.TextImageRelation = TextImageRelation.ImageAboveText;
            _disconnectButton.Click += _disconnectButton_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 70);
            // 
            // _createLobbyButton
            // 
            _createLobbyButton.Image = (Image)resources.GetObject("_createLobbyButton.Image");
            _createLobbyButton.ImageScaling = ToolStripItemImageScaling.None;
            _createLobbyButton.ImageTransparentColor = Color.Magenta;
            _createLobbyButton.Name = "_createLobbyButton";
            _createLobbyButton.Size = new Size(81, 67);
            _createLobbyButton.Text = "Create Lobby";
            _createLobbyButton.TextAlign = ContentAlignment.BottomCenter;
            _createLobbyButton.TextImageRelation = TextImageRelation.ImageAboveText;
            _createLobbyButton.Click += _createLobbyButton_Click;
            // 
            // _joinLobbyButton
            // 
            _joinLobbyButton.Image = (Image)resources.GetObject("_joinLobbyButton.Image");
            _joinLobbyButton.ImageScaling = ToolStripItemImageScaling.None;
            _joinLobbyButton.ImageTransparentColor = Color.Magenta;
            _joinLobbyButton.Name = "_joinLobbyButton";
            _joinLobbyButton.Size = new Size(68, 67);
            _joinLobbyButton.Text = "Join Lobby";
            _joinLobbyButton.TextAlign = ContentAlignment.BottomCenter;
            _joinLobbyButton.TextImageRelation = TextImageRelation.ImageAboveText;
            _joinLobbyButton.Click += _joinLobbyButton_Click;
            // 
            // _leaveRoomButton
            // 
            _leaveRoomButton.Image = (Image)resources.GetObject("_leaveRoomButton.Image");
            _leaveRoomButton.ImageScaling = ToolStripItemImageScaling.None;
            _leaveRoomButton.ImageTransparentColor = Color.Magenta;
            _leaveRoomButton.Name = "_leaveRoomButton";
            _leaveRoomButton.Size = new Size(77, 67);
            _leaveRoomButton.Text = "Leave Lobby";
            _leaveRoomButton.TextAlign = ContentAlignment.BottomCenter;
            _leaveRoomButton.TextImageRelation = TextImageRelation.ImageAboveText;
            _leaveRoomButton.Click += _leaveRoomButton_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 70);
            // 
            // _openMapButton
            // 
            _openMapButton.Image = (Image)resources.GetObject("_openMapButton.Image");
            _openMapButton.ImageScaling = ToolStripItemImageScaling.None;
            _openMapButton.ImageTransparentColor = Color.Magenta;
            _openMapButton.Name = "_openMapButton";
            _openMapButton.Size = new Size(67, 67);
            _openMapButton.Text = "Open Map";
            _openMapButton.TextAlign = ContentAlignment.BottomCenter;
            _openMapButton.TextImageRelation = TextImageRelation.ImageAboveText;
            _openMapButton.ToolTipText = "Open Map";
            _openMapButton.Click += _openMapButton_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 70);
            // 
            // _settingsButton
            // 
            _settingsButton.Image = (Image)resources.GetObject("_settingsButton.Image");
            _settingsButton.ImageScaling = ToolStripItemImageScaling.None;
            _settingsButton.ImageTransparentColor = Color.Magenta;
            _settingsButton.Name = "_settingsButton";
            _settingsButton.Size = new Size(53, 67);
            _settingsButton.Text = "Settings";
            _settingsButton.TextAlign = ContentAlignment.BottomCenter;
            _settingsButton.TextImageRelation = TextImageRelation.ImageAboveText;
            _settingsButton.Click += _settingsButton_Click;
            // 
            // _startGameButton
            // 
            _startGameButton.Alignment = ToolStripItemAlignment.Right;
            _startGameButton.Image = (Image)resources.GetObject("_startGameButton.Image");
            _startGameButton.ImageScaling = ToolStripItemImageScaling.None;
            _startGameButton.ImageTransparentColor = Color.Magenta;
            _startGameButton.Name = "_startGameButton";
            _startGameButton.Size = new Size(94, 67);
            _startGameButton.Text = "Start Elden Ring";
            _startGameButton.TextAlign = ContentAlignment.BottomCenter;
            _startGameButton.TextImageRelation = TextImageRelation.ImageAboveText;
            _startGameButton.Click += _startGameButton_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 70);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(_usersListBox);
            splitContainer1.Size = new Size(947, 591);
            splitContainer1.SplitterDistance = 750;
            splitContainer1.SplitterWidth = 3;
            splitContainer1.TabIndex = 0;
            // 
            // _usersListBox
            // 
            _usersListBox.BackColor = SystemColors.ControlDark;
            _usersListBox.Dock = DockStyle.Fill;
            _usersListBox.DrawMode = DrawMode.OwnerDrawFixed;
            _usersListBox.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            _usersListBox.FormattingEnabled = true;
            _usersListBox.IntegralHeight = false;
            _usersListBox.ItemHeight = 20;
            _usersListBox.Location = new Point(0, 0);
            _usersListBox.Name = "_usersListBox";
            _usersListBox.Size = new Size(194, 591);
            _usersListBox.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Controls.Add(_clientStatusTextBox);
            panel1.Controls.Add(_processMonitorStatusTextBox);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 661);
            panel1.Name = "panel1";
            panel1.Size = new Size(947, 27);
            panel1.TabIndex = 0;
            // 
            // _clientStatusTextBox
            // 
            _clientStatusTextBox.BackColor = SystemColors.Control;
            _clientStatusTextBox.Dock = DockStyle.Fill;
            _clientStatusTextBox.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            _clientStatusTextBox.Location = new Point(0, 0);
            _clientStatusTextBox.Name = "_clientStatusTextBox";
            _clientStatusTextBox.ReadOnly = true;
            _clientStatusTextBox.Size = new Size(753, 27);
            _clientStatusTextBox.TabIndex = 4;
            _clientStatusTextBox.TextAlign = HorizontalAlignment.Center;
            // 
            // _changeTeamButton
            // 
            _changeTeamButton.Image = (Image)resources.GetObject("_changeTeamButton.Image");
            _changeTeamButton.ImageScaling = ToolStripItemImageScaling.None;
            _changeTeamButton.ImageTransparentColor = Color.Magenta;
            _changeTeamButton.Name = "_changeTeamButton";
            _changeTeamButton.Size = new Size(83, 67);
            _changeTeamButton.Text = "Change Team";
            _changeTeamButton.TextAlign = ContentAlignment.BottomCenter;
            _changeTeamButton.TextImageRelation = TextImageRelation.ImageAboveText;
            _changeTeamButton.Click += _changeTeamButton_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(947, 688);
            Controls.Add(tabControl1);
            Controls.Add(splitContainer1);
            Controls.Add(panel1);
            Controls.Add(toolStrip1);
            MinimumSize = new Size(954, 572);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Elden Bingo";
            Load += MainForm_Load;
            tabControl1.ResumeLayout(false);
            _consolePage.ResumeLayout(false);
            _lobbyPage.ResumeLayout(false);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage _consolePage;
        private TextBox _processMonitorStatusTextBox;
        private ToolStrip toolStrip1;
        private ToolStripButton _connectButton;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton _createLobbyButton;
        private ToolStripButton _joinLobbyButton;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton _settingsButton;
        private ToolStripButton _startGameButton;
        private ToolStripButton _disconnectButton;
        private ToolStripButton _leaveRoomButton;
        private SplitContainer splitContainer1;
        private UI.RichListBox _usersListBox;
        private Panel panel1;
        private TextBox _clientStatusTextBox;
        private UI.ConsoleControl _consoleControl;
        private TabPage _lobbyPage;
        private UI.LobbyControl _lobbyControl;
        private ToolStripButton _openMapButton;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton _changeTeamButton;
    }
}