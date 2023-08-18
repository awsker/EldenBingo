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
            this.label1 = new System.Windows.Forms.Label();
            this._classesListBox = new System.Windows.Forms.CheckedListBox();
            this._classLimitCheckBox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this._numClassesUpDown = new System.Windows.Forms.NumericUpDown();
            this._maxCategoryUpDown = new System.Windows.Forms.NumericUpDown();
            this._randomSeedUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this._preparationTimeUpDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._numClassesUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._maxCategoryUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._randomSeedUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._preparationTimeUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 257);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "Random Seed:";
            // 
            // _classesListBox
            // 
            this._classesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._classesListBox.CheckOnClick = true;
            this._classesListBox.FormattingEnabled = true;
            this._classesListBox.Location = new System.Drawing.Point(4, 32);
            this._classesListBox.Name = "_classesListBox";
            this._classesListBox.Size = new System.Drawing.Size(231, 184);
            this._classesListBox.TabIndex = 3;
            // 
            // _classLimitCheckBox
            // 
            this._classLimitCheckBox.AutoSize = true;
            this._classLimitCheckBox.Location = new System.Drawing.Point(6, 5);
            this._classLimitCheckBox.Name = "_classLimitCheckBox";
            this._classLimitCheckBox.Size = new System.Drawing.Size(136, 19);
            this._classLimitCheckBox.TabIndex = 1;
            this._classLimitCheckBox.Text = "Randomized Classes:";
            this._classLimitCheckBox.UseVisualStyleBackColor = true;
            this._classLimitCheckBox.CheckedChanged += new System.EventHandler(this._classLimitCheckBox_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 227);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(169, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Max squares in same category:";
            // 
            // _numClassesUpDown
            // 
            this._numClassesUpDown.Location = new System.Drawing.Point(172, 4);
            this._numClassesUpDown.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this._numClassesUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._numClassesUpDown.Name = "_numClassesUpDown";
            this._numClassesUpDown.Size = new System.Drawing.Size(62, 23);
            this._numClassesUpDown.TabIndex = 2;
            this._numClassesUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // _maxCategoryUpDown
            // 
            this._maxCategoryUpDown.Location = new System.Drawing.Point(186, 225);
            this._maxCategoryUpDown.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this._maxCategoryUpDown.Name = "_maxCategoryUpDown";
            this._maxCategoryUpDown.Size = new System.Drawing.Size(48, 23);
            this._maxCategoryUpDown.TabIndex = 5;
            // 
            // _randomSeedUpDown
            // 
            this._randomSeedUpDown.Location = new System.Drawing.Point(99, 255);
            this._randomSeedUpDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this._randomSeedUpDown.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this._randomSeedUpDown.Name = "_randomSeedUpDown";
            this._randomSeedUpDown.Size = new System.Drawing.Size(81, 23);
            this._randomSeedUpDown.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 287);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "Preparation Time:";
            // 
            // _preparationTimeUpDown
            // 
            this._preparationTimeUpDown.Location = new System.Drawing.Point(114, 284);
            this._preparationTimeUpDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this._preparationTimeUpDown.Name = "_preparationTimeUpDown";
            this._preparationTimeUpDown.Size = new System.Drawing.Size(66, 23);
            this._preparationTimeUpDown.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(185, 287);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 15);
            this.label4.TabIndex = 10;
            this.label4.Text = "seconds";
            // 
            // GameSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label4);
            this.Controls.Add(this._preparationTimeUpDown);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._randomSeedUpDown);
            this.Controls.Add(this._maxCategoryUpDown);
            this.Controls.Add(this._numClassesUpDown);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._classLimitCheckBox);
            this.Controls.Add(this._classesListBox);
            this.Controls.Add(this.label1);
            this.Name = "GameSettingsControl";
            this.Size = new System.Drawing.Size(239, 310);
            ((System.ComponentModel.ISupportInitialize)(this._numClassesUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._maxCategoryUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._randomSeedUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._preparationTimeUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}
