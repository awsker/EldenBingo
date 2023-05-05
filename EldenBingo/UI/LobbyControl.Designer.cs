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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel2 = new System.Windows.Forms.Panel();
            this._bingoControl = new EldenBingo.UI.BingoControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this._scoreboardControl = new EldenBingo.UI.ScoreboardControl();
            this._logBoxBorderPanel = new System.Windows.Forms.Panel();
            this._logTextBox = new EldenBingo.UI.RichTextBoxCustom();
            this._timerLabel = new System.Windows.Forms.Label();
            this._matchStatusLabel = new System.Windows.Forms.Label();
            this.adminControl1 = new EldenBingo.UI.AdminControl();
            this._clientList = new EldenBingo.UI.ClientListControl();
            this._adminInfoLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this._logBoxBorderPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel2);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            this.splitContainer1.Panel1.Controls.Add(this.adminControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._clientList);
            this.splitContainer1.Panel2.Controls.Add(this._adminInfoLabel);
            this.splitContainer1.Panel2MinSize = 80;
            this.splitContainer1.Size = new System.Drawing.Size(1055, 567);
            this.splitContainer1.SplitterDistance = 851;
            this.splitContainer1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this._bingoControl);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(581, 421);
            this.panel2.TabIndex = 1;
            // 
            // _bingoControl
            // 
            this._bingoControl.Client = null;
            this._bingoControl.Location = new System.Drawing.Point(5, 5);
            this._bingoControl.Name = "_bingoControl";
            this._bingoControl.Size = new System.Drawing.Size(568, 408);
            this._bingoControl.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._scoreboardControl);
            this.panel1.Controls.Add(this._logBoxBorderPanel);
            this.panel1.Controls.Add(this._timerLabel);
            this.panel1.Controls.Add(this._matchStatusLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(581, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(270, 421);
            this.panel1.TabIndex = 2;
            // 
            // _scoreboardControl
            // 
            this._scoreboardControl.Client = null;
            this._scoreboardControl.Location = new System.Drawing.Point(41, 98);
            this._scoreboardControl.Name = "_scoreboardControl";
            this._scoreboardControl.Size = new System.Drawing.Size(218, 0);
            this._scoreboardControl.TabIndex = 10;
            this._scoreboardControl.SizeChanged += new System.EventHandler(this._scoreboardControl_SizeChanged);
            // 
            // _logBoxBorderPanel
            // 
            this._logBoxBorderPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._logBoxBorderPanel.BackColor = System.Drawing.SystemColors.WindowFrame;
            this._logBoxBorderPanel.Controls.Add(this._logTextBox);
            this._logBoxBorderPanel.Location = new System.Drawing.Point(5, 104);
            this._logBoxBorderPanel.Name = "_logBoxBorderPanel";
            this._logBoxBorderPanel.Padding = new System.Windows.Forms.Padding(1);
            this._logBoxBorderPanel.Size = new System.Drawing.Size(259, 316);
            this._logBoxBorderPanel.TabIndex = 9;
            // 
            // _logTextBox
            // 
            this._logTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._logTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this._logTextBox.BorderColor = System.Drawing.SystemColors.WindowFrame;
            this._logTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._logTextBox.DetectUrls = false;
            this._logTextBox.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._logTextBox.ForeColor = System.Drawing.Color.White;
            this._logTextBox.Location = new System.Drawing.Point(1, 1);
            this._logTextBox.Name = "_logTextBox";
            this._logTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this._logTextBox.Size = new System.Drawing.Size(257, 314);
            this._logTextBox.TabIndex = 8;
            this._logTextBox.Text = "";
            // 
            // _timerLabel
            // 
            this._timerLabel.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._timerLabel.ForeColor = System.Drawing.Color.White;
            this._timerLabel.Location = new System.Drawing.Point(6, 30);
            this._timerLabel.Name = "_timerLabel";
            this._timerLabel.Size = new System.Drawing.Size(243, 53);
            this._timerLabel.TabIndex = 6;
            this._timerLabel.Text = "00:00:00";
            this._timerLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // _matchStatusLabel
            // 
            this._matchStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._matchStatusLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._matchStatusLabel.ForeColor = System.Drawing.Color.White;
            this._matchStatusLabel.Location = new System.Drawing.Point(3, 4);
            this._matchStatusLabel.Name = "_matchStatusLabel";
            this._matchStatusLabel.Size = new System.Drawing.Size(264, 23);
            this._matchStatusLabel.TabIndex = 7;
            this._matchStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // adminControl1
            // 
            this.adminControl1.Client = null;
            this.adminControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.adminControl1.Location = new System.Drawing.Point(0, 421);
            this.adminControl1.Name = "adminControl1";
            this.adminControl1.Size = new System.Drawing.Size(851, 146);
            this.adminControl1.TabIndex = 4;
            // 
            // _clientList
            // 
            this._clientList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._clientList.Client = null;
            this._clientList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._clientList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this._clientList.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._clientList.FormattingEnabled = true;
            this._clientList.IntegralHeight = false;
            this._clientList.ItemHeight = 20;
            this._clientList.Location = new System.Drawing.Point(0, 0);
            this._clientList.Name = "_clientList";
            this._clientList.Size = new System.Drawing.Size(200, 502);
            this._clientList.TabIndex = 3;
            // 
            // _adminInfoLabel
            // 
            this._adminInfoLabel.BackColor = System.Drawing.SystemColors.Info;
            this._adminInfoLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._adminInfoLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._adminInfoLabel.Location = new System.Drawing.Point(0, 502);
            this._adminInfoLabel.Name = "_adminInfoLabel";
            this._adminInfoLabel.Size = new System.Drawing.Size(200, 65);
            this._adminInfoLabel.TabIndex = 4;
            this._adminInfoLabel.Text = "AdminSpectator Info: All bingo board actions are made on behalf of the selected u" +
    "ser";
            this._adminInfoLabel.Visible = false;
            // 
            // LobbyControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.splitContainer1);
            this.Name = "LobbyControl";
            this.Size = new System.Drawing.Size(1055, 567);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this._logBoxBorderPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SplitContainer splitContainer1;
        private ClientListControl _clientList;
        private Panel panel1;
        private Panel panel2;
        private Label _timerLabel;
        private Label _matchStatusLabel;
        private AdminControl adminControl1;
        private BingoControl _bingoControl;
        private RichTextBoxCustom _logTextBox;
        private Panel _logBoxBorderPanel;
        private Label _adminInfoLabel;
        private ScoreboardControl _scoreboardControl;
    }
}
