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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this._consolePage = new System.Windows.Forms.TabPage();
            this._consoleControl = new EldenBingo.UI.ConsoleControl();
            this._lobbyPage = new System.Windows.Forms.TabPage();
            this._lobbyControl = new EldenBingo.UI.LobbyControl();
            this._processMonitorStatusTextBox = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this._connectButton = new System.Windows.Forms.ToolStripButton();
            this._disconnectButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._createLobbyButton = new System.Windows.Forms.ToolStripButton();
            this._joinLobbyButton = new System.Windows.Forms.ToolStripButton();
            this._leaveRoomButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this._openMapButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this._settingsButton = new System.Windows.Forms.ToolStripButton();
            this._startGameButton = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._usersListBox = new EldenBingo.UI.RichListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this._clientStatusTextBox = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this._consolePage.SuspendLayout();
            this._lobbyPage.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tabControl1.Controls.Add(this._consolePage);
            this.tabControl1.Controls.Add(this._lobbyPage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tabControl1.Location = new System.Drawing.Point(0, 70);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(947, 591);
            this.tabControl1.TabIndex = 1;
            // 
            // _consolePage
            // 
            this._consolePage.Controls.Add(this._consoleControl);
            this._consolePage.Location = new System.Drawing.Point(30, 4);
            this._consolePage.Margin = new System.Windows.Forms.Padding(0);
            this._consolePage.Name = "_consolePage";
            this._consolePage.Size = new System.Drawing.Size(913, 583);
            this._consolePage.TabIndex = 0;
            this._consolePage.Text = "Console";
            this._consolePage.UseVisualStyleBackColor = true;
            this._consolePage.Click += new System.EventHandler(this._createLobbyButton_Click);
            // 
            // _consoleControl
            // 
            this._consoleControl.Client = null;
            this._consoleControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._consoleControl.Location = new System.Drawing.Point(0, 0);
            this._consoleControl.Name = "_consoleControl";
            this._consoleControl.Size = new System.Drawing.Size(913, 583);
            this._consoleControl.TabIndex = 0;
            // 
            // _lobbyPage
            // 
            this._lobbyPage.Controls.Add(this._lobbyControl);
            this._lobbyPage.Location = new System.Drawing.Point(30, 4);
            this._lobbyPage.Name = "_lobbyPage";
            this._lobbyPage.Size = new System.Drawing.Size(913, 583);
            this._lobbyPage.TabIndex = 1;
            this._lobbyPage.Text = "Lobby";
            this._lobbyPage.UseVisualStyleBackColor = true;
            //
            // _lobbyControl
            //
            this._lobbyControl.Client = null;
            this._lobbyControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lobbyControl.Location = new System.Drawing.Point(0, 0);
            this._lobbyControl.Name = "_lobbyControl";
            this._lobbyControl.Size = new System.Drawing.Size(913, 511);
            this._lobbyControl.TabIndex = 0;
            //
            // 
            // _processMonitorStatusTextBox
            // 
            this._processMonitorStatusTextBox.BackColor = System.Drawing.SystemColors.Control;
            this._processMonitorStatusTextBox.Dock = System.Windows.Forms.DockStyle.Right;
            this._processMonitorStatusTextBox.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._processMonitorStatusTextBox.Location = new System.Drawing.Point(753, 0);
            this._processMonitorStatusTextBox.Name = "_processMonitorStatusTextBox";
            this._processMonitorStatusTextBox.ReadOnly = true;
            this._processMonitorStatusTextBox.Size = new System.Drawing.Size(194, 27);
            this._processMonitorStatusTextBox.TabIndex = 3;
            this._processMonitorStatusTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._connectButton,
            this._disconnectButton,
            this.toolStripSeparator1,
            this._createLobbyButton,
            this._joinLobbyButton,
            this._leaveRoomButton,
            this.toolStripSeparator2,
            this._openMapButton,
            this.toolStripSeparator3,
            this._settingsButton,
            this._startGameButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(947, 70);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // _connectButton
            // 
            this._connectButton.Image = ((System.Drawing.Image)(resources.GetObject("_connectButton.Image")));
            this._connectButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this._connectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._connectButton.Name = "_connectButton";
            this._connectButton.Size = new System.Drawing.Size(56, 67);
            this._connectButton.Text = "Connect";
            this._connectButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this._connectButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this._connectButton.Click += new System.EventHandler(this._connectButton_Click);
            // 
            // _disconnectButton
            // 
            this._disconnectButton.Image = ((System.Drawing.Image)(resources.GetObject("_disconnectButton.Image")));
            this._disconnectButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this._disconnectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._disconnectButton.Name = "_disconnectButton";
            this._disconnectButton.Size = new System.Drawing.Size(70, 67);
            this._disconnectButton.Text = "Disconnect";
            this._disconnectButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this._disconnectButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this._disconnectButton.Click += new System.EventHandler(this._disconnectButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 70);
            // 
            // _createLobbyButton
            // 
            this._createLobbyButton.Image = ((System.Drawing.Image)(resources.GetObject("_createLobbyButton.Image")));
            this._createLobbyButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this._createLobbyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._createLobbyButton.Name = "_createLobbyButton";
            this._createLobbyButton.Size = new System.Drawing.Size(81, 67);
            this._createLobbyButton.Text = "Create Lobby";
            this._createLobbyButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this._createLobbyButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this._createLobbyButton.Click += new System.EventHandler(this._createLobbyButton_Click);
            // 
            // _joinLobbyButton
            // 
            this._joinLobbyButton.Image = ((System.Drawing.Image)(resources.GetObject("_joinLobbyButton.Image")));
            this._joinLobbyButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this._joinLobbyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._joinLobbyButton.Name = "_joinLobbyButton";
            this._joinLobbyButton.Size = new System.Drawing.Size(68, 67);
            this._joinLobbyButton.Text = "Join Lobby";
            this._joinLobbyButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this._joinLobbyButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this._joinLobbyButton.Click += new System.EventHandler(this._joinLobbyButton_Click);
            // 
            // _leaveRoomButton
            // 
            this._leaveRoomButton.Image = ((System.Drawing.Image)(resources.GetObject("_leaveRoomButton.Image")));
            this._leaveRoomButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this._leaveRoomButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._leaveRoomButton.Name = "_leaveRoomButton";
            this._leaveRoomButton.Size = new System.Drawing.Size(77, 67);
            this._leaveRoomButton.Text = "Leave Lobby";
            this._leaveRoomButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this._leaveRoomButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this._leaveRoomButton.Click += new System.EventHandler(this._leaveRoomButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 70);
            // 
            // _openMapButton
            // 
            this._openMapButton.Image = ((System.Drawing.Image)(resources.GetObject("_openMapButton.Image")));
            this._openMapButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this._openMapButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._openMapButton.Name = "_openMapButton";
            this._openMapButton.Size = new System.Drawing.Size(67, 67);
            this._openMapButton.Text = "Open Map";
            this._openMapButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this._openMapButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this._openMapButton.ToolTipText = "Open Map";
            this._openMapButton.Click += new System.EventHandler(this._openMapButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 70);
            // 
            // _settingsButton
            // 
            this._settingsButton.Image = ((System.Drawing.Image)(resources.GetObject("_settingsButton.Image")));
            this._settingsButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this._settingsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._settingsButton.Name = "_settingsButton";
            this._settingsButton.Size = new System.Drawing.Size(53, 67);
            this._settingsButton.Text = "Settings";
            this._settingsButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this._settingsButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this._settingsButton.Click += new System.EventHandler(this._settingsButton_Click);
            // 
            // _startGameButton
            // 
            this._startGameButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this._startGameButton.Image = ((System.Drawing.Image)(resources.GetObject("_startGameButton.Image")));
            this._startGameButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this._startGameButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._startGameButton.Name = "_startGameButton";
            this._startGameButton.Size = new System.Drawing.Size(94, 67);
            this._startGameButton.Text = "Start Elden Ring";
            this._startGameButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this._startGameButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this._startGameButton.Click += new System.EventHandler(this._startGameButton_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 70);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._usersListBox);
            this.splitContainer1.Size = new System.Drawing.Size(947, 591);
            this.splitContainer1.SplitterDistance = 750;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 0;
            // 
            // _usersListBox
            // 
            this._usersListBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this._usersListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._usersListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this._usersListBox.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._usersListBox.FormattingEnabled = true;
            this._usersListBox.IntegralHeight = false;
            this._usersListBox.ItemHeight = 20;
            this._usersListBox.Location = new System.Drawing.Point(0, 0);
            this._usersListBox.Name = "_usersListBox";
            this._usersListBox.Size = new System.Drawing.Size(194, 591);
            this._usersListBox.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._clientStatusTextBox);
            this.panel1.Controls.Add(this._processMonitorStatusTextBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 661);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(947, 27);
            this.panel1.TabIndex = 0;
            // 
            // _clientStatusTextBox
            // 
            this._clientStatusTextBox.BackColor = System.Drawing.SystemColors.Control;
            this._clientStatusTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._clientStatusTextBox.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._clientStatusTextBox.Location = new System.Drawing.Point(0, 0);
            this._clientStatusTextBox.Name = "_clientStatusTextBox";
            this._clientStatusTextBox.ReadOnly = true;
            this._clientStatusTextBox.Size = new System.Drawing.Size(753, 27);
            this._clientStatusTextBox.TabIndex = 4;
            this._clientStatusTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(947, 688);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.MinimumSize = new System.Drawing.Size(954, 572);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Elden Bingo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControl1.ResumeLayout(false);
            this._consolePage.ResumeLayout(false);
            this._lobbyPage.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

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
    }
}