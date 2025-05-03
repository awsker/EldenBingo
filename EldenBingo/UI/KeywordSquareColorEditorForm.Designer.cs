namespace EldenBingo.UI
{
    partial class KeywordSquareColorEditorForm
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeywordSquareColorEditorForm));
            panel1 = new Panel();
            _okButton = new Button();
            _cancelButton = new Button();
            dataGridView1 = new DataGridView();
            KeywordColumn = new DataGridViewTextBoxColumn();
            ColorColumn = new DataGridViewTextBoxColumn();
            panel2 = new Panel();
            _helpImage = new PictureBox();
            _moveDownButton = new Button();
            _moveUpButton = new Button();
            _removeButton = new Button();
            _addButton = new Button();
            toolTip1 = new ToolTip(components);
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_helpImage).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(_okButton);
            panel1.Controls.Add(_cancelButton);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 238);
            panel1.Name = "panel1";
            panel1.Size = new Size(343, 38);
            panel1.TabIndex = 0;
            // 
            // _okButton
            // 
            _okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _okButton.Location = new Point(180, 8);
            _okButton.Name = "_okButton";
            _okButton.Size = new Size(75, 23);
            _okButton.TabIndex = 14;
            _okButton.Text = "OK";
            _okButton.UseVisualStyleBackColor = true;
            _okButton.Click += _okButton_Click;
            // 
            // _cancelButton
            // 
            _cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _cancelButton.DialogResult = DialogResult.Cancel;
            _cancelButton.Location = new Point(261, 8);
            _cancelButton.Name = "_cancelButton";
            _cancelButton.Size = new Size(75, 23);
            _cancelButton.TabIndex = 15;
            _cancelButton.Text = "Cancel";
            _cancelButton.UseVisualStyleBackColor = true;
            _cancelButton.Click += _cancelButton_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(248, 248, 248);
            dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { KeywordColumn, ColorColumn });
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.EditMode = DataGridViewEditMode.EditOnKeystroke;
            dataGridView1.Location = new Point(0, 0);
            dataGridView1.MultiSelect = false;
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowTemplate.Height = 25;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ShowCellErrors = false;
            dataGridView1.ShowCellToolTips = false;
            dataGridView1.ShowEditingIcon = false;
            dataGridView1.Size = new Size(300, 238);
            dataGridView1.TabIndex = 1;
            dataGridView1.CellClick += dataGridView1_CellClick;
            dataGridView1.CellFormatting += dataGridView1_CellFormatting;
            dataGridView1.CellPainting += dataGridView1_CellPainting;
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            // 
            // KeywordColumn
            // 
            KeywordColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            KeywordColumn.DataPropertyName = "Keyword";
            KeywordColumn.HeaderText = "Keyword";
            KeywordColumn.Name = "KeywordColumn";
            KeywordColumn.Resizable = DataGridViewTriState.False;
            // 
            // ColorColumn
            // 
            ColorColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ColorColumn.DataPropertyName = "Color";
            ColorColumn.HeaderText = "Color";
            ColorColumn.MinimumWidth = 60;
            ColorColumn.Name = "ColorColumn";
            ColorColumn.ReadOnly = true;
            ColorColumn.Resizable = DataGridViewTriState.False;
            ColorColumn.Width = 60;
            // 
            // panel2
            // 
            panel2.Controls.Add(_helpImage);
            panel2.Controls.Add(_moveDownButton);
            panel2.Controls.Add(_moveUpButton);
            panel2.Controls.Add(_removeButton);
            panel2.Controls.Add(_addButton);
            panel2.Dock = DockStyle.Right;
            panel2.Location = new Point(300, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(43, 238);
            panel2.TabIndex = 16;
            // 
            // _helpImage
            // 
            _helpImage.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _helpImage.BackColor = SystemColors.Control;
            _helpImage.Image = (Image)resources.GetObject("_helpImage.Image");
            _helpImage.Location = new Point(5, 200);
            _helpImage.Name = "_helpImage";
            _helpImage.Size = new Size(34, 34);
            _helpImage.SizeMode = PictureBoxSizeMode.CenterImage;
            _helpImage.TabIndex = 17;
            _helpImage.TabStop = false;
            toolTip1.SetToolTip(_helpImage, "The first rule (top to bottom) with a keyword that matches a square's text will be used to color the square with 10% intensity");
            // 
            // _moveDownButton
            // 
            _moveDownButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _moveDownButton.Image = (Image)resources.GetObject("_moveDownButton.Image");
            _moveDownButton.Location = new Point(5, 121);
            _moveDownButton.Name = "_moveDownButton";
            _moveDownButton.Size = new Size(34, 34);
            _moveDownButton.TabIndex = 3;
            _moveDownButton.UseVisualStyleBackColor = true;
            _moveDownButton.Click += _moveDownButton_Click;
            // 
            // _moveUpButton
            // 
            _moveUpButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _moveUpButton.Image = Properties.Resources.Up;
            _moveUpButton.Location = new Point(5, 85);
            _moveUpButton.Name = "_moveUpButton";
            _moveUpButton.Size = new Size(34, 34);
            _moveUpButton.TabIndex = 2;
            _moveUpButton.UseVisualStyleBackColor = true;
            _moveUpButton.Click += _moveUpButton_Click;
            // 
            // _removeButton
            // 
            _removeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _removeButton.Image = Properties.Resources.Delete;
            _removeButton.Location = new Point(5, 39);
            _removeButton.Name = "_removeButton";
            _removeButton.Size = new Size(34, 34);
            _removeButton.TabIndex = 1;
            _removeButton.UseVisualStyleBackColor = true;
            _removeButton.Click += _removeButton_Click;
            // 
            // _addButton
            // 
            _addButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _addButton.Image = (Image)resources.GetObject("_addButton.Image");
            _addButton.Location = new Point(5, 3);
            _addButton.Name = "_addButton";
            _addButton.Size = new Size(34, 34);
            _addButton.TabIndex = 0;
            _addButton.UseVisualStyleBackColor = true;
            _addButton.Click += _addButton_Click;
            // 
            // toolTip1
            // 
            toolTip1.AutomaticDelay = 0;
            toolTip1.AutoPopDelay = 600000;
            toolTip1.InitialDelay = 0;
            toolTip1.IsBalloon = true;
            toolTip1.ReshowDelay = 0;
            // 
            // KeywordSquareColorEditorForm
            // 
            AcceptButton = _okButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = _cancelButton;
            ClientSize = new Size(343, 276);
            Controls.Add(dataGridView1);
            Controls.Add(panel2);
            Controls.Add(panel1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "KeywordSquareColorEditorForm";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Keyword Square Colors Rules Editor";
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_helpImage).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Button _okButton;
        private Button _cancelButton;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn KeywordColumn;
        private DataGridViewTextBoxColumn ColorColumn;
        private Panel panel2;
        private Button _addButton;
        private Button _moveDownButton;
        private Button _moveUpButton;
        private Button _removeButton;
        private ToolTip toolTip1;
        private PictureBox _helpImage;
    }
}