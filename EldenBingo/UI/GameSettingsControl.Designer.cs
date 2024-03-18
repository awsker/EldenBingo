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
            ((System.ComponentModel.ISupportInitialize)_numClassesUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_maxCategoryUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_randomSeedUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_preparationTimeUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_bonusPointsUpDown).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(4, 257);
            label1.Name = "label1";
            label1.Size = new Size(82, 15);
            label1.TabIndex = 6;
            label1.Text = "Random seed:";
            // 
            // _classesListBox
            // 
            _classesListBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _classesListBox.CheckOnClick = true;
            _classesListBox.FormattingEnabled = true;
            _classesListBox.Location = new Point(4, 32);
            _classesListBox.Name = "_classesListBox";
            _classesListBox.Size = new Size(231, 184);
            _classesListBox.TabIndex = 3;
            // 
            // _classLimitCheckBox
            // 
            _classLimitCheckBox.AutoSize = true;
            _classLimitCheckBox.Location = new Point(6, 5);
            _classLimitCheckBox.Name = "_classLimitCheckBox";
            _classLimitCheckBox.Size = new Size(138, 19);
            _classLimitCheckBox.TabIndex = 1;
            _classLimitCheckBox.Text = "Limit starting classes:";
            _classLimitCheckBox.UseVisualStyleBackColor = true;
            _classLimitCheckBox.CheckedChanged += _classLimitCheckBox_CheckedChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(4, 227);
            label2.Name = "label2";
            label2.Size = new Size(169, 15);
            label2.TabIndex = 4;
            label2.Text = "Max squares in same category:";
            // 
            // _numClassesUpDown
            // 
            _numClassesUpDown.Location = new Point(172, 4);
            _numClassesUpDown.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            _numClassesUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _numClassesUpDown.Name = "_numClassesUpDown";
            _numClassesUpDown.Size = new Size(62, 23);
            _numClassesUpDown.TabIndex = 2;
            _numClassesUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // _maxCategoryUpDown
            // 
            _maxCategoryUpDown.Location = new Point(186, 225);
            _maxCategoryUpDown.Maximum = new decimal(new int[] { 25, 0, 0, 0 });
            _maxCategoryUpDown.Name = "_maxCategoryUpDown";
            _maxCategoryUpDown.Size = new Size(48, 23);
            _maxCategoryUpDown.TabIndex = 5;
            // 
            // _randomSeedUpDown
            // 
            _randomSeedUpDown.Location = new Point(99, 255);
            _randomSeedUpDown.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            _randomSeedUpDown.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
            _randomSeedUpDown.Name = "_randomSeedUpDown";
            _randomSeedUpDown.Size = new Size(81, 23);
            _randomSeedUpDown.TabIndex = 7;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(4, 287);
            label3.Name = "label3";
            label3.Size = new Size(98, 15);
            label3.TabIndex = 9;
            label3.Text = "Preparation time:";
            // 
            // _preparationTimeUpDown
            // 
            _preparationTimeUpDown.Location = new Point(114, 284);
            _preparationTimeUpDown.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            _preparationTimeUpDown.Name = "_preparationTimeUpDown";
            _preparationTimeUpDown.Size = new Size(66, 23);
            _preparationTimeUpDown.TabIndex = 10;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(185, 287);
            label4.Name = "label4";
            label4.Size = new Size(50, 15);
            label4.TabIndex = 11;
            label4.Text = "seconds";
            // 
            // button1
            // 
            button1.Location = new Point(185, 255);
            button1.Name = "button1";
            button1.Size = new Size(51, 23);
            button1.TabIndex = 8;
            button1.Text = "Reset";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(4, 317);
            label5.Name = "label5";
            label5.Size = new Size(131, 15);
            label5.TabIndex = 12;
            label5.Text = "Bonus points for bingo:";
            // 
            // _bonusPointsUpDown
            // 
            _bonusPointsUpDown.Location = new Point(143, 315);
            _bonusPointsUpDown.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            _bonusPointsUpDown.Name = "_bonusPointsUpDown";
            _bonusPointsUpDown.Size = new Size(66, 23);
            _bonusPointsUpDown.TabIndex = 13;
            // 
            // GameSettingsControl
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(_bonusPointsUpDown);
            Controls.Add(label5);
            Controls.Add(button1);
            Controls.Add(label4);
            Controls.Add(_preparationTimeUpDown);
            Controls.Add(label3);
            Controls.Add(_randomSeedUpDown);
            Controls.Add(_maxCategoryUpDown);
            Controls.Add(_numClassesUpDown);
            Controls.Add(label2);
            Controls.Add(_classLimitCheckBox);
            Controls.Add(_classesListBox);
            Controls.Add(label1);
            Name = "GameSettingsControl";
            Size = new Size(239, 343);
            ((System.ComponentModel.ISupportInitialize)_numClassesUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)_maxCategoryUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)_randomSeedUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)_preparationTimeUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)_bonusPointsUpDown).EndInit();
            ResumeLayout(false);
            PerformLayout();
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
    }
}
