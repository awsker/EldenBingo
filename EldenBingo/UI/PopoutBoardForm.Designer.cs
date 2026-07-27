namespace EldenBingo.UI
{
    partial class PopoutBoardForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopoutBoardForm));
            bingoControl1 = new BingoControl();
            _buttonPanel = new Panel();
            _moveButton = new PictureBox();
            _resizeButton = new PictureBox();
            _opacityButton = new PictureBox();
            _closeButton = new PictureBox();
            scoreboardControl1 = new ScoreboardControl();
            _timerLabel = new Label();
            toolTip1 = new ToolTip(components);
            panel3 = new Panel();
            _statsPanel = new TableLayoutPanel();
            _bingoPanelFill = new Panel();
            _bingoPanelParent = new Panel();
            _buttonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_moveButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_resizeButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_opacityButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_closeButton).BeginInit();
            panel3.SuspendLayout();
            _statsPanel.SuspendLayout();
            _bingoPanelFill.SuspendLayout();
            _bingoPanelParent.SuspendLayout();
            SuspendLayout();
            // 
            // bingoControl1
            // 
            bingoControl1.Client = null;
            bingoControl1.Location = new Point(0, 0);
            bingoControl1.Name = "bingoControl1";
            bingoControl1.Size = new Size(346, 315);
            bingoControl1.TabIndex = 0;
            // 
            // _buttonPanel
            // 
            _buttonPanel.BackColor = Color.Transparent;
            _buttonPanel.Controls.Add(_moveButton);
            _buttonPanel.Controls.Add(_resizeButton);
            _buttonPanel.Controls.Add(_opacityButton);
            _buttonPanel.Controls.Add(_closeButton);
            _buttonPanel.Dock = DockStyle.Top;
            _buttonPanel.Location = new Point(0, 0);
            _buttonPanel.Name = "_buttonPanel";
            _buttonPanel.Size = new Size(346, 32);
            _buttonPanel.TabIndex = 1;
            // 
            // _moveButton
            // 
            _moveButton.Dock = DockStyle.Right;
            _moveButton.Image = (Image)resources.GetObject("_moveButton.Image");
            _moveButton.Location = new Point(218, 0);
            _moveButton.Name = "_moveButton";
            _moveButton.Size = new Size(32, 32);
            _moveButton.TabIndex = 4;
            _moveButton.TabStop = false;
            toolTip1.SetToolTip(_moveButton, "Move");
            _moveButton.MouseDown += _moveButton_MouseDown;
            _moveButton.MouseUp += _button_MouseUp;
            // 
            // _resizeButton
            // 
            _resizeButton.Dock = DockStyle.Right;
            _resizeButton.Image = (Image)resources.GetObject("_resizeButton.Image");
            _resizeButton.Location = new Point(250, 0);
            _resizeButton.Name = "_resizeButton";
            _resizeButton.Size = new Size(32, 32);
            _resizeButton.TabIndex = 3;
            _resizeButton.TabStop = false;
            toolTip1.SetToolTip(_resizeButton, "Resize");
            _resizeButton.MouseDown += _resizeButton_MouseDown;
            _resizeButton.MouseUp += _button_MouseUp;
            // 
            // _opacityButton
            // 
            _opacityButton.Dock = DockStyle.Right;
            _opacityButton.Image = (Image)resources.GetObject("_opacityButton.Image");
            _opacityButton.Location = new Point(282, 0);
            _opacityButton.Name = "_opacityButton";
            _opacityButton.Size = new Size(32, 32);
            _opacityButton.TabIndex = 5;
            _opacityButton.TabStop = false;
            toolTip1.SetToolTip(_opacityButton, "Opacity");
            _opacityButton.MouseDown += _opacityButton_MouseDown;
            _opacityButton.MouseUp += _button_MouseUp;
            // 
            // _closeButton
            // 
            _closeButton.Dock = DockStyle.Right;
            _closeButton.Image = (Image)resources.GetObject("_closeButton.Image");
            _closeButton.Location = new Point(314, 0);
            _closeButton.Name = "_closeButton";
            _closeButton.Size = new Size(32, 32);
            _closeButton.TabIndex = 2;
            _closeButton.TabStop = false;
            toolTip1.SetToolTip(_closeButton, "Close");
            _closeButton.MouseDown += _closeButton_MouseDown;
            _closeButton.MouseUp += _button_MouseUp;
            // 
            // scoreboardControl1
            // 
            scoreboardControl1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            scoreboardControl1.Client = null;
            scoreboardControl1.Location = new Point(8, 7);
            scoreboardControl1.Name = "scoreboardControl1";
            scoreboardControl1.Size = new Size(161, 24);
            scoreboardControl1.TabIndex = 8;
            // 
            // _timerLabel
            // 
            _timerLabel.Dock = DockStyle.Fill;
            _timerLabel.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            _timerLabel.ForeColor = Color.White;
            _timerLabel.Location = new Point(176, 0);
            _timerLabel.Name = "_timerLabel";
            _timerLabel.Size = new Size(167, 79);
            _timerLabel.TabIndex = 7;
            _timerLabel.Text = "00:00:00";
            _timerLabel.TextAlign = ContentAlignment.TopRight;
            // 
            // panel3
            // 
            panel3.Controls.Add(scoreboardControl1);
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(0, 0);
            panel3.Margin = new Padding(0);
            panel3.Name = "panel3";
            panel3.Size = new Size(173, 79);
            panel3.TabIndex = 9;
            // 
            // _statsPanel
            // 
            _statsPanel.ColumnCount = 2;
            _statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _statsPanel.Controls.Add(panel3, 0, 0);
            _statsPanel.Controls.Add(_timerLabel, 1, 0);
            _statsPanel.Dock = DockStyle.Fill;
            _statsPanel.Location = new Point(0, 332);
            _statsPanel.Name = "_statsPanel";
            _statsPanel.RowCount = 1;
            _statsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            _statsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            _statsPanel.Size = new Size(346, 79);
            _statsPanel.TabIndex = 9;
            // 
            // _bingoPanelFill
            // 
            _bingoPanelFill.Controls.Add(_bingoPanelParent);
            _bingoPanelFill.Dock = DockStyle.Top;
            _bingoPanelFill.Location = new Point(0, 32);
            _bingoPanelFill.Margin = new Padding(0);
            _bingoPanelFill.Name = "_bingoPanelFill";
            _bingoPanelFill.Size = new Size(346, 300);
            _bingoPanelFill.TabIndex = 6;
            // 
            // _bingoPanelParent
            // 
            _bingoPanelParent.Controls.Add(bingoControl1);
            _bingoPanelParent.Location = new Point(0, 0);
            _bingoPanelParent.Margin = new Padding(0);
            _bingoPanelParent.Name = "_bingoPanelParent";
            _bingoPanelParent.Size = new Size(200, 100);
            _bingoPanelParent.TabIndex = 6;
            // 
            // PopoutBoardForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(346, 411);
            Controls.Add(_statsPanel);
            Controls.Add(_bingoPanelFill);
            Controls.Add(_buttonPanel);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(300, 300);
            Name = "PopoutBoardForm";
            StartPosition = FormStartPosition.Manual;
            Text = "Bingo Board";
            FormClosed += PopoutBoardForm_FormClosed;
            _buttonPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_moveButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)_resizeButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)_opacityButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)_closeButton).EndInit();
            panel3.ResumeLayout(false);
            _statsPanel.ResumeLayout(false);
            _bingoPanelFill.ResumeLayout(false);
            _bingoPanelParent.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private BingoControl bingoControl1;
        private Panel _buttonPanel;
        private PictureBox _closeButton;
        private PictureBox _moveButton;
        private PictureBox _resizeButton;
        private PictureBox _opacityButton;
        private Label _timerLabel;
        private ScoreboardControl scoreboardControl1;
        private ToolTip toolTip1;
        private Panel panel3;
        private TableLayoutPanel _statsPanel;
        private Panel _bingoPanelFill;
        private Panel _bingoPanelParent;
    }
}