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
            panel1 = new Panel();
            _moveButton = new PictureBox();
            _resizeButton = new PictureBox();
            _opacityButton = new PictureBox();
            _closeButton = new PictureBox();
            scoreboardControl1 = new ScoreboardControl();
            _timerLabel = new Label();
            toolTip1 = new ToolTip(components);
            panel3 = new Panel();
            tableLayoutPanel1 = new TableLayoutPanel();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_moveButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_resizeButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_opacityButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_closeButton).BeginInit();
            panel3.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // bingoControl1
            // 
            bingoControl1.Client = null;
            bingoControl1.Dock = DockStyle.Top;
            bingoControl1.Location = new Point(0, 32);
            bingoControl1.Name = "bingoControl1";
            bingoControl1.Size = new Size(346, 315);
            bingoControl1.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.BackColor = Color.Transparent;
            panel1.Controls.Add(_moveButton);
            panel1.Controls.Add(_resizeButton);
            panel1.Controls.Add(_opacityButton);
            panel1.Controls.Add(_closeButton);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(346, 32);
            panel1.TabIndex = 1;
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
            scoreboardControl1.Location = new Point(9, 7);
            scoreboardControl1.Name = "scoreboardControl1";
            scoreboardControl1.Size = new Size(155, 24);
            scoreboardControl1.TabIndex = 8;
            // 
            // _timerLabel
            // 
            _timerLabel.Dock = DockStyle.Fill;
            _timerLabel.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            _timerLabel.ForeColor = Color.White;
            _timerLabel.Location = new Point(176, 0);
            _timerLabel.Name = "_timerLabel";
            _timerLabel.Size = new Size(167, 64);
            _timerLabel.TabIndex = 7;
            _timerLabel.Text = "00:00:00";
            _timerLabel.TextAlign = ContentAlignment.TopRight;
            // 
            // panel3
            // 
            panel3.Controls.Add(scoreboardControl1);
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(3, 3);
            panel3.Name = "panel3";
            panel3.Size = new Size(167, 58);
            panel3.TabIndex = 9;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(panel3, 0, 0);
            tableLayoutPanel1.Controls.Add(_timerLabel, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 347);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(346, 64);
            tableLayoutPanel1.TabIndex = 9;
            // 
            // PopoutBoardForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(346, 411);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(bingoControl1);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(300, 300);
            Name = "PopoutBoardForm";
            StartPosition = FormStartPosition.Manual;
            Text = "Bingo Board";
            FormClosed += PopoutBoardForm_FormClosed;
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_moveButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)_resizeButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)_opacityButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)_closeButton).EndInit();
            panel3.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private BingoControl bingoControl1;
        private Panel panel1;
        private PictureBox _closeButton;
        private PictureBox _moveButton;
        private PictureBox _resizeButton;
        private PictureBox _opacityButton;
        private Label _timerLabel;
        private ScoreboardControl scoreboardControl1;
        private ToolTip toolTip1;
        private Panel panel3;
        private TableLayoutPanel tableLayoutPanel1;
    }
}