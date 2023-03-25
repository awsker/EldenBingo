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
            this._matchStatusLabel = new System.Windows.Forms.Label();
            this._timerLabel = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.adminControl1 = new EldenBingo.UI.AdminControl();
            this._clientList = new EldenBingo.UI.ClientListControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.panel3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._clientList);
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
            this.panel2.Size = new System.Drawing.Size(621, 421);
            this.panel2.TabIndex = 2;
            // 
            // _bingoControl
            // 
            this._bingoControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._bingoControl.Client = null;
            this._bingoControl.Location = new System.Drawing.Point(10, 10);
            this._bingoControl.Margin = new System.Windows.Forms.Padding(0);
            this._bingoControl.MaximumSize = new System.Drawing.Size(600, 545);
            this._bingoControl.MinimumSize = new System.Drawing.Size(450, 409);
            this._bingoControl.Name = "_bingoControl";
            this._bingoControl.Size = new System.Drawing.Size(598, 409);
            this._bingoControl.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._matchStatusLabel);
            this.panel1.Controls.Add(this._timerLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(621, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(230, 421);
            this.panel1.TabIndex = 1;
            // 
            // _matchStatusLabel
            // 
            this._matchStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._matchStatusLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._matchStatusLabel.ForeColor = System.Drawing.Color.White;
            this._matchStatusLabel.Location = new System.Drawing.Point(3, 8);
            this._matchStatusLabel.Name = "_matchStatusLabel";
            this._matchStatusLabel.Size = new System.Drawing.Size(224, 23);
            this._matchStatusLabel.TabIndex = 7;
            this._matchStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _timerLabel
            // 
            this._timerLabel.Font = new System.Drawing.Font("Segoe UI", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._timerLabel.ForeColor = System.Drawing.Color.White;
            this._timerLabel.Location = new System.Drawing.Point(5, 39);
            this._timerLabel.Name = "_timerLabel";
            this._timerLabel.Size = new System.Drawing.Size(195, 50);
            this._timerLabel.TabIndex = 6;
            this._timerLabel.Text = "00:00:00";
            this._timerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.adminControl1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 421);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(851, 146);
            this.panel3.TabIndex = 8;
            // 
            // adminControl1
            // 
            this.adminControl1.Client = null;
            this.adminControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.adminControl1.Location = new System.Drawing.Point(0, 0);
            this.adminControl1.Name = "adminControl1";
            this.adminControl1.Size = new System.Drawing.Size(851, 146);
            this.adminControl1.TabIndex = 0;
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
            this._clientList.Size = new System.Drawing.Size(200, 567);
            this._clientList.TabIndex = 0;
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
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SplitContainer splitContainer1;
        private ClientListControl _clientList;
        private BingoControl _bingoControl;
        private Panel panel1;
        private Panel panel2;
        private Label _timerLabel;
        private Label _matchStatusLabel;
        private Panel panel3;
        private AdminControl adminControl1;
    }
}
