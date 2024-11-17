namespace EldenBingo.UI
{
    partial class GameSettingsControl
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
            label1 = new Label();
            _classesListBox = new CheckedListBox();
            _classLimitCheckBox = new CheckBox();
            label2 = new Label();
            _numClassesUpDown = new NumericUpDown();
            _maxCategoryUpDown = new NumericUpDown();
            _randomSeedUpDown = new NumericUpDown();
            label3 = new Label();
            _preparationTimeUpDown = new NumericUpDown();
            label4 = new Label();
            button1 = new Button();
            label5 = new Label();
            _bonusPointsUpDown = new NumericUpDown();
            label6 = new Label();
            _boardSizeComboBox = new ComboBox();
            panel1 = new Panel();
            _lockoutCheckBox = new CheckBox();
            panel2 = new Panel();
            panel3 = new Panel();
            panel4 = new Panel();
            panel5 = new Panel();
            panel6 = new Panel();
            ((System.ComponentModel.ISupportInitialize)_numClassesUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_maxCategoryUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_randomSeedUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_preparationTimeUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_bonusPointsUpDown).BeginInit();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            panel4.SuspendLayout();
            panel5.SuspendLayout();
            panel6.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(4, 4);
            label1.Name = "label1";
            label1.Size = new Size(82, 15);
            label1.TabIndex = 4;
            label1.Text = "Random seed:";
            // 
            // _classesListBox
            // 
            _classesListBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _classesListBox.CheckOnClick = true;
            _classesListBox.FormattingEnabled = true;
            _classesListBox.Location = new Point(0, 29);
            _classesListBox.Name = "_classesListBox";
            _classesListBox.Size = new Size(248, 184);
            _classesListBox.TabIndex = 17;
            // 
            // _classLimitCheckBox
            // 
            _classLimitCheckBox.AutoSize = true;
            _classLimitCheckBox.Location = new Point(4, 4);
            _classLimitCheckBox.Name = "_classLimitCheckBox";
            _classLimitCheckBox.Size = new Size(138, 19);
            _classLimitCheckBox.TabIndex = 15;
            _classLimitCheckBox.Text = "Limit starting classes:";
            _classLimitCheckBox.UseVisualStyleBackColor = true;
            _classLimitCheckBox.CheckedChanged += _classLimitCheckBox_CheckedChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(4, 4);
            label2.Name = "label2";
            label2.Size = new Size(169, 15);
            label2.TabIndex = 19;
            label2.Text = "Max squares in same category:";
            // 
            // _numClassesUpDown
            // 
            _numClassesUpDown.Location = new Point(145, 2);
            _numClassesUpDown.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            _numClassesUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _numClassesUpDown.Name = "_numClassesUpDown";
            _numClassesUpDown.Size = new Size(66, 23);
            _numClassesUpDown.TabIndex = 16;
            _numClassesUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // _maxCategoryUpDown
            // 
            _maxCategoryUpDown.Location = new Point(192, 2);
            _maxCategoryUpDown.Maximum = new decimal(new int[] { 25, 0, 0, 0 });
            _maxCategoryUpDown.Name = "_maxCategoryUpDown";
            _maxCategoryUpDown.Size = new Size(56, 23);
            _maxCategoryUpDown.TabIndex = 20;
            // 
            // _randomSeedUpDown
            // 
            _randomSeedUpDown.Location = new Point(99, 2);
            _randomSeedUpDown.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            _randomSeedUpDown.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
            _randomSeedUpDown.Name = "_randomSeedUpDown";
            _randomSeedUpDown.Size = new Size(87, 23);
            _randomSeedUpDown.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(4, 4);
            label3.Name = "label3";
            label3.Size = new Size(98, 15);
            label3.TabIndex = 8;
            label3.Text = "Preparation time:";
            // 
            // _preparationTimeUpDown
            // 
            _preparationTimeUpDown.Location = new Point(114, 2);
            _preparationTimeUpDown.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            _preparationTimeUpDown.Name = "_preparationTimeUpDown";
            _preparationTimeUpDown.Size = new Size(72, 23);
            _preparationTimeUpDown.TabIndex = 9;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(192, 4);
            label4.Name = "label4";
            label4.Size = new Size(50, 15);
            label4.TabIndex = 10;
            label4.Text = "seconds";
            // 
            // button1
            // 
            button1.Location = new Point(192, 1);
            button1.Name = "button1";
            button1.Size = new Size(51, 23);
            button1.TabIndex = 6;
            button1.Text = "Reset";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(4, 4);
            label5.Name = "label5";
            label5.Size = new Size(131, 15);
            label5.TabIndex = 12;
            label5.Text = "Bonus points for bingo:";
            // 
            // _bonusPointsUpDown
            // 
            _bonusPointsUpDown.Location = new Point(145, 2);
            _bonusPointsUpDown.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            _bonusPointsUpDown.Name = "_bonusPointsUpDown";
            _bonusPointsUpDown.Size = new Size(66, 23);
            _bonusPointsUpDown.TabIndex = 13;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(4, 4);
            label6.Name = "label6";
            label6.Size = new Size(63, 15);
            label6.TabIndex = 1;
            label6.Text = "Board size:";
            // 
            // _boardSizeComboBox
            // 
            _boardSizeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _boardSizeComboBox.FormattingEnabled = true;
            _boardSizeComboBox.Items.AddRange(new object[] { "3x3", "4x4", "5x5", "6x6", "7x7", "8x8" });
            _boardSizeComboBox.Location = new Point(73, 1);
            _boardSizeComboBox.MaxDropDownItems = 6;
            _boardSizeComboBox.Name = "_boardSizeComboBox";
            _boardSizeComboBox.Size = new Size(100, 23);
            _boardSizeComboBox.TabIndex = 2;
            // 
            // panel1
            // 
            panel1.Controls.Add(_lockoutCheckBox);
            panel1.Controls.Add(label6);
            panel1.Controls.Add(_boardSizeComboBox);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(256, 30);
            panel1.TabIndex = 0;
            // 
            // _lockoutCheckBox
            // 
            _lockoutCheckBox.AutoSize = true;
            _lockoutCheckBox.Location = new Point(180, 3);
            _lockoutCheckBox.Name = "_lockoutCheckBox";
            _lockoutCheckBox.Size = new Size(69, 19);
            _lockoutCheckBox.TabIndex = 19;
            _lockoutCheckBox.Text = "Lockout";
            _lockoutCheckBox.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            panel2.Controls.Add(label1);
            panel2.Controls.Add(_randomSeedUpDown);
            panel2.Controls.Add(button1);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 30);
            panel2.Name = "panel2";
            panel2.Size = new Size(256, 30);
            panel2.TabIndex = 3;
            // 
            // panel3
            // 
            panel3.Controls.Add(label3);
            panel3.Controls.Add(_preparationTimeUpDown);
            panel3.Controls.Add(label4);
            panel3.Dock = DockStyle.Top;
            panel3.Location = new Point(0, 60);
            panel3.Name = "panel3";
            panel3.Size = new Size(256, 30);
            panel3.TabIndex = 7;
            // 
            // panel4
            // 
            panel4.Controls.Add(label5);
            panel4.Controls.Add(_bonusPointsUpDown);
            panel4.Dock = DockStyle.Top;
            panel4.Location = new Point(0, 90);
            panel4.Name = "panel4";
            panel4.Size = new Size(256, 30);
            panel4.TabIndex = 11;
            // 
            // panel5
            // 
            panel5.Controls.Add(_classLimitCheckBox);
            panel5.Controls.Add(_numClassesUpDown);
            panel5.Controls.Add(_classesListBox);
            panel5.Dock = DockStyle.Top;
            panel5.Location = new Point(0, 120);
            panel5.Name = "panel5";
            panel5.Size = new Size(256, 216);
            panel5.TabIndex = 14;
            // 
            // panel6
            // 
            panel6.Controls.Add(label2);
            panel6.Controls.Add(_maxCategoryUpDown);
            panel6.Dock = DockStyle.Top;
            panel6.Location = new Point(0, 336);
            panel6.Name = "panel6";
            panel6.Size = new Size(256, 30);
            panel6.TabIndex = 18;
            // 
            // GameSettingsControl
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(panel6);
            Controls.Add(panel5);
            Controls.Add(panel4);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Name = "GameSettingsControl";
            Size = new Size(256, 367);
            ((System.ComponentModel.ISupportInitialize)_numClassesUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)_maxCategoryUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)_randomSeedUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)_preparationTimeUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)_bonusPointsUpDown).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            panel6.ResumeLayout(false);
            panel6.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private CheckedListBox _classesListBox;
        private CheckBox _classLimitCheckBox;
        private Label label2;
        private NumericUpDown _numClassesUpDown;
        private NumericUpDown _maxCategoryUpDown;
        private NumericUpDown _randomSeedUpDown;
        private Label label3;
        private NumericUpDown _preparationTimeUpDown;
        private Label label4;
        private Button button1;
        private Label label5;
        private NumericUpDown _bonusPointsUpDown;
        private Label label6;
        private ComboBox _boardSizeComboBox;
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private Panel panel4;
        private Panel panel5;
        private Panel panel6;
        private CheckBox _lockoutCheckBox;
    }
}
