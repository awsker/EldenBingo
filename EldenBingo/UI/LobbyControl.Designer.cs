namespace EldenBingo.UI
{
    partial class LobbyControl
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
            splitContainer1 = new SplitContainer();
            _lobbyStatusPanel = new Panel();
            _scoreboardControl = new ScoreboardControl();
            _logBoxBorderPanel = new Panel();
            _logTextBox = new RichTextBoxCustom();
            _requestLogParentPanel = new Panel();
            _requestLogLinkLabel = new LinkLabel();
            _requestJsonLinkLabel = new LinkLabel();
            _chatTextPanel = new Panel();
            _chatTextBox = new TextBox();
            _timerLabel = new Label();
            _matchStatusLabel = new Label();
            _bingoBoardPanel = new Panel();
            _bingoControl = new BingoControl();
            adminControl1 = new AdminControl();
            _clientList = new ClientListControl();
            _adminInfoLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            _lobbyStatusPanel.SuspendLayout();
            _logBoxBorderPanel.SuspendLayout();
            _requestLogParentPanel.SuspendLayout();
            _chatTextPanel.SuspendLayout();
            _bingoBoardPanel.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel2;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(_lobbyStatusPanel);
            splitContainer1.Panel1.Controls.Add(_bingoBoardPanel);
            splitContainer1.Panel1.Controls.Add(adminControl1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(_clientList);
            splitContainer1.Panel2.Controls.Add(_adminInfoLabel);
            splitContainer1.Panel2MinSize = 80;
            splitContainer1.Size = new Size(1055, 567);
            splitContainer1.SplitterDistance = 851;
            splitContainer1.TabIndex = 0;
            // 
            // _lobbyStatusPanel
            // 
            _lobbyStatusPanel.Controls.Add(_scoreboardControl);
            _lobbyStatusPanel.Controls.Add(_logBoxBorderPanel);
            _lobbyStatusPanel.Controls.Add(_timerLabel);
            _lobbyStatusPanel.Controls.Add(_matchStatusLabel);
            _lobbyStatusPanel.Dock = DockStyle.Fill;
            _lobbyStatusPanel.Location = new Point(581, 0);
            _lobbyStatusPanel.Name = "_lobbyStatusPanel";
            _lobbyStatusPanel.Size = new Size(270, 421);
            _lobbyStatusPanel.TabIndex = 2;
            // 
            // _scoreboardControl
            // 
            _scoreboardControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _scoreboardControl.Client = null;
            _scoreboardControl.Location = new Point(17, 98);
            _scoreboardControl.Name = "_scoreboardControl";
            _scoreboardControl.Size = new Size(218, 0);
            _scoreboardControl.TabIndex = 10;
            _scoreboardControl.SizeChanged += _scoreboardControl_SizeChanged;
            // 
            // _logBoxBorderPanel
            // 
            _logBoxBorderPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            _logBoxBorderPanel.BackColor = Color.FromArgb(118, 110, 97);
            _logBoxBorderPanel.BorderStyle = BorderStyle.FixedSingle;
            _logBoxBorderPanel.Controls.Add(_logTextBox);
            _logBoxBorderPanel.Controls.Add(_requestLogParentPanel);
            _logBoxBorderPanel.Controls.Add(_chatTextPanel);
            _logBoxBorderPanel.Location = new Point(5, 104);
            _logBoxBorderPanel.Name = "_logBoxBorderPanel";
            _logBoxBorderPanel.Padding = new Padding(1);
            _logBoxBorderPanel.Size = new Size(259, 316);
            _logBoxBorderPanel.TabIndex = 9;
            // 
            // _logTextBox
            // 
            _logTextBox.BackColor = Color.FromArgb(20, 20, 20);
            _logTextBox.BorderColor = SystemColors.WindowFrame;
            _logTextBox.BorderStyle = BorderStyle.None;
            _logTextBox.Dock = DockStyle.Fill;
            _logTextBox.Font = new Font("Segoe UI", 9.75F);
            _logTextBox.ForeColor = Color.White;
            _logTextBox.Location = new Point(1, 28);
            _logTextBox.MustHideCaret = true;
            _logTextBox.Name = "_logTextBox";
            _logTextBox.ReadOnly = true;
            _logTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            _logTextBox.Size = new Size(255, 257);
            _logTextBox.TabIndex = 8;
            _logTextBox.TabStop = false;
            _logTextBox.Text = "";
            _logTextBox.LinkClicked += _logTextBox_LinkClicked;
            // 
            // _requestLogParentPanel
            // 
            _requestLogParentPanel.Controls.Add(_requestLogLinkLabel);
            _requestLogParentPanel.Controls.Add(_requestJsonLinkLabel);
            _requestLogParentPanel.Dock = DockStyle.Top;
            _requestLogParentPanel.Location = new Point(1, 1);
            _requestLogParentPanel.Name = "_requestLogParentPanel";
            _requestLogParentPanel.Size = new Size(255, 27);
            _requestLogParentPanel.TabIndex = 11;
            _requestLogParentPanel.Visible = false;
            // 
            // _requestLogLinkLabel
            // 
            _requestLogLinkLabel.ActiveLinkColor = Color.AliceBlue;
            _requestLogLinkLabel.Dock = DockStyle.Fill;
            _requestLogLinkLabel.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _requestLogLinkLabel.ForeColor = Color.SkyBlue;
            _requestLogLinkLabel.LinkBehavior = LinkBehavior.HoverUnderline;
            _requestLogLinkLabel.LinkColor = Color.SkyBlue;
            _requestLogLinkLabel.Location = new Point(0, 0);
            _requestLogLinkLabel.Name = "_requestLogLinkLabel";
            _requestLogLinkLabel.Size = new Size(175, 27);
            _requestLogLinkLabel.TabIndex = 11;
            _requestLogLinkLabel.TabStop = true;
            _requestLogLinkLabel.Text = "Download Match Log";
            _requestLogLinkLabel.TextAlign = ContentAlignment.MiddleRight;
            _requestLogLinkLabel.VisitedLinkColor = Color.SkyBlue;
            _requestLogLinkLabel.LinkClicked += _requestLogLinkLabel_LinkClicked;
            // 
            // _requestJsonLinkLabel
            // 
            _requestJsonLinkLabel.ActiveLinkColor = Color.AliceBlue;
            _requestJsonLinkLabel.Dock = DockStyle.Right;
            _requestJsonLinkLabel.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _requestJsonLinkLabel.ForeColor = Color.MediumSeaGreen;
            _requestJsonLinkLabel.LinkBehavior = LinkBehavior.HoverUnderline;
            _requestJsonLinkLabel.LinkColor = Color.MediumSeaGreen;
            _requestJsonLinkLabel.Location = new Point(175, 0);
            _requestJsonLinkLabel.Name = "_requestJsonLinkLabel";
            _requestJsonLinkLabel.Size = new Size(80, 27);
            _requestJsonLinkLabel.TabIndex = 12;
            _requestJsonLinkLabel.TabStop = true;
            _requestJsonLinkLabel.Text = "(as Json)";
            _requestJsonLinkLabel.TextAlign = ContentAlignment.MiddleLeft;
            _requestJsonLinkLabel.VisitedLinkColor = Color.PaleGreen;
            _requestJsonLinkLabel.LinkClicked += _requestJsonLinkLabel_LinkClicked;
            // 
            // _chatTextPanel
            // 
            _chatTextPanel.BackColor = Color.FromArgb(35, 35, 35);
            _chatTextPanel.Controls.Add(_chatTextBox);
            _chatTextPanel.Dock = DockStyle.Bottom;
            _chatTextPanel.Location = new Point(1, 285);
            _chatTextPanel.Name = "_chatTextPanel";
            _chatTextPanel.Padding = new Padding(4, 4, 0, 0);
            _chatTextPanel.Size = new Size(255, 28);
            _chatTextPanel.TabIndex = 12;
            // 
            // _chatTextBox
            // 
            _chatTextBox.BackColor = Color.FromArgb(35, 35, 35);
            _chatTextBox.BorderStyle = BorderStyle.None;
            _chatTextBox.Dock = DockStyle.Fill;
            _chatTextBox.ForeColor = Color.White;
            _chatTextBox.Location = new Point(4, 4);
            _chatTextBox.Margin = new Padding(0);
            _chatTextBox.MaxLength = 327670;
            _chatTextBox.Name = "_chatTextBox";
            _chatTextBox.PlaceholderText = "Send a message";
            _chatTextBox.Size = new Size(251, 16);
            _chatTextBox.TabIndex = 9;
            _chatTextBox.KeyPress += _chatTextBox_KeyPress;
            // 
            // _timerLabel
            // 
            _timerLabel.Font = new Font("Segoe UI", 36F, FontStyle.Bold);
            _timerLabel.ForeColor = Color.White;
            _timerLabel.Location = new Point(6, 30);
            _timerLabel.Name = "_timerLabel";
            _timerLabel.Size = new Size(243, 53);
            _timerLabel.TabIndex = 6;
            _timerLabel.Text = "00:00:00";
            _timerLabel.TextAlign = ContentAlignment.TopRight;
            // 
            // _matchStatusLabel
            // 
            _matchStatusLabel.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold);
            _matchStatusLabel.ForeColor = Color.White;
            _matchStatusLabel.Location = new Point(3, 4);
            _matchStatusLabel.Name = "_matchStatusLabel";
            _matchStatusLabel.Size = new Size(264, 23);
            _matchStatusLabel.TabIndex = 7;
            _matchStatusLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _bingoBoardPanel
            // 
            _bingoBoardPanel.Controls.Add(_bingoControl);
            _bingoBoardPanel.Dock = DockStyle.Left;
            _bingoBoardPanel.Location = new Point(0, 0);
            _bingoBoardPanel.Name = "_bingoBoardPanel";
            _bingoBoardPanel.Size = new Size(581, 421);
            _bingoBoardPanel.TabIndex = 1;
            // 
            // _bingoControl
            // 
            _bingoControl.AbideByMaxSize = true;
            _bingoControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _bingoControl.AspectRatio = 1.1F;
            _bingoControl.BingoBoard = null;
            _bingoControl.Client = null;
            _bingoControl.GridColor = Color.FromArgb(118, 110, 97);
            _bingoControl.LineWidth = 2;
            _bingoControl.Location = new Point(5, 5);
            _bingoControl.MaintainAspectRatio = true;
            _bingoControl.Name = "_bingoControl";
            _bingoControl.Size = new Size(454, 413);
            _bingoControl.TabIndex = 0;
            // 
            // adminControl1
            // 
            adminControl1.Client = null;
            adminControl1.Dock = DockStyle.Bottom;
            adminControl1.Location = new Point(0, 421);
            adminControl1.Name = "adminControl1";
            adminControl1.Size = new Size(851, 146);
            adminControl1.TabIndex = 4;
            // 
            // _clientList
            // 
            _clientList.BorderStyle = BorderStyle.FixedSingle;
            _clientList.Client = null;
            _clientList.Dock = DockStyle.Fill;
            _clientList.Font = new Font("Segoe UI", 11.25F);
            _clientList.Location = new Point(0, 0);
            _clientList.Name = "_clientList";
            _clientList.Size = new Size(200, 502);
            _clientList.TabIndex = 3;
            // 
            // _adminInfoLabel
            // 
            _adminInfoLabel.BackColor = SystemColors.Info;
            _adminInfoLabel.Dock = DockStyle.Bottom;
            _adminInfoLabel.Font = new Font("Segoe UI", 9.75F);
            _adminInfoLabel.Location = new Point(0, 502);
            _adminInfoLabel.Name = "_adminInfoLabel";
            _adminInfoLabel.Size = new Size(200, 65);
            _adminInfoLabel.TabIndex = 4;
            _adminInfoLabel.Text = "AdminSpectator Info: Check/count actions are made on behalf of selection's team";
            _adminInfoLabel.Visible = false;
            // 
            // LobbyControl
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(splitContainer1);
            Name = "LobbyControl";
            Size = new Size(1055, 567);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            _lobbyStatusPanel.ResumeLayout(false);
            _logBoxBorderPanel.ResumeLayout(false);
            _requestLogParentPanel.ResumeLayout(false);
            _chatTextPanel.ResumeLayout(false);
            _chatTextPanel.PerformLayout();
            _bingoBoardPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private ClientListControl _clientList;
        private Panel _lobbyStatusPanel;
        private Panel _bingoBoardPanel;
        private Label _timerLabel;
        private Label _matchStatusLabel;
        private AdminControl adminControl1;
        private BingoControl _bingoControl;
        private RichTextBoxCustom _logTextBox;
        private Panel _logBoxBorderPanel;
        private Label _adminInfoLabel;
        private ScoreboardControl _scoreboardControl;
        private TextBox _chatTextBox;
        private LinkLabel _requestLogLinkLabel;
        private Panel _chatTextPanel;
        private Panel _requestLogParentPanel;
        private LinkLabel _requestJsonLinkLabel;
    }
}
