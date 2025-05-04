namespace EldenBingo.UI
{
    partial class KeywordColorsEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeywordColorsEditorForm));
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            panel1 = new Panel();
            _helpButton = new Button();
            _okButton = new Button();
            _cancelButton = new Button();
            dataGridView1 = new DataGridView();
            KeywordColumn = new DataGridViewTextBoxColumn();
            ColorColumn = new DataGridViewTextBoxColumn();
            panel2 = new Panel();
            _moveDownButton = new Button();
            _moveUpButton = new Button();
            _removeButton = new Button();
            _addButton = new Button();
            toolTip1 = new ToolTip(components);
            toolStrip1 = new ToolStrip();
            _newToolstripButton = new ToolStripButton();
            _openToolstripButton = new ToolStripButton();
            _saveToolstripButton = new ToolStripButton();
            _helpText = new Label();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            panel2.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(_helpButton);
            panel1.Controls.Add(_okButton);
            panel1.Controls.Add(_cancelButton);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 238);
            panel1.Name = "panel1";
            panel1.Size = new Size(343, 38);
            panel1.TabIndex = 0;
            // 
            // _helpButton
            // 
            _helpButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _helpButton.Image = (Image)resources.GetObject("_helpButton.Image");
            _helpButton.Location = new Point(8, 6);
            _helpButton.Name = "_helpButton";
            _helpButton.Size = new Size(81, 27);
            _helpButton.TabIndex = 4;
            _helpButton.Text = "Help";
            _helpButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            _helpButton.UseVisualStyleBackColor = true;
            _helpButton.Click += _helpButton_Click;
            _helpButton.MouseLeave += _helpButton_MouseLeave;
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
            dataGridView1.Location = new Point(0, 25);
            dataGridView1.MultiSelect = false;
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowTemplate.Height = 25;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ShowCellErrors = false;
            dataGridView1.ShowCellToolTips = false;
            dataGridView1.ShowEditingIcon = false;
            dataGridView1.Size = new Size(300, 213);
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
            panel2.Controls.Add(_moveDownButton);
            panel2.Controls.Add(_moveUpButton);
            panel2.Controls.Add(_removeButton);
            panel2.Controls.Add(_addButton);
            panel2.Dock = DockStyle.Right;
            panel2.Location = new Point(300, 25);
            panel2.Name = "panel2";
            panel2.Size = new Size(43, 213);
            panel2.TabIndex = 16;
            // 
            // _moveDownButton
            // 
            _moveDownButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _moveDownButton.Image = (Image)resources.GetObject("_moveDownButton.Image");
            _moveDownButton.Location = new Point(5, 121);
            _moveDownButton.Name = "_moveDownButton";
            _moveDownButton.Size = new Size(34, 34);
            _moveDownButton.TabIndex = 3;
            toolTip1.SetToolTip(_moveDownButton, "Move Down");
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
            toolTip1.SetToolTip(_moveUpButton, "Move Up");
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
            toolTip1.SetToolTip(_removeButton, "Remove Rule");
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
            toolTip1.SetToolTip(_addButton, "Add Rule");
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
            // toolStrip1
            // 
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new ToolStripItem[] { _newToolstripButton, _openToolstripButton, _saveToolstripButton });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(343, 25);
            toolStrip1.TabIndex = 17;
            toolStrip1.Text = "toolStrip1";
            // 
            // _newToolstripButton
            // 
            _newToolstripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _newToolstripButton.Image = (Image)resources.GetObject("_newToolstripButton.Image");
            _newToolstripButton.ImageTransparentColor = Color.Magenta;
            _newToolstripButton.Name = "_newToolstripButton";
            _newToolstripButton.Size = new Size(23, 22);
            _newToolstripButton.ToolTipText = "New Ruleset...";
            _newToolstripButton.Click += _newToolstripButton_Click;
            // 
            // _openToolstripButton
            // 
            _openToolstripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _openToolstripButton.Image = (Image)resources.GetObject("_openToolstripButton.Image");
            _openToolstripButton.ImageTransparentColor = Color.Magenta;
            _openToolstripButton.Name = "_openToolstripButton";
            _openToolstripButton.Size = new Size(23, 22);
            _openToolstripButton.ToolTipText = "Open Ruleset...";
            _openToolstripButton.Click += _openToolstripButton_Click;
            // 
            // _saveToolstripButton
            // 
            _saveToolstripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _saveToolstripButton.Image = (Image)resources.GetObject("_saveToolstripButton.Image");
            _saveToolstripButton.ImageTransparentColor = Color.Magenta;
            _saveToolstripButton.Name = "_saveToolstripButton";
            _saveToolstripButton.Size = new Size(23, 22);
            _saveToolstripButton.ToolTipText = "Save Ruleset To File...";
            _saveToolstripButton.Click += _saveToolstripButton_Click;
            // 
            // _helpText
            // 
            _helpText.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _helpText.BackColor = SystemColors.Info;
            _helpText.BorderStyle = BorderStyle.FixedSingle;
            _helpText.FlatStyle = FlatStyle.Popup;
            _helpText.Location = new Point(75, 175);
            _helpText.Name = "_helpText";
            _helpText.Size = new Size(225, 87);
            _helpText.TabIndex = 4;
            _helpText.Text = "The first rule (top to bottom) with a keyword found in a square's text (case-insensitive) will be used to color that square's text";
            _helpText.Visible = false;
            // 
            // KeywordColorsEditorForm
            // 
            AcceptButton = _okButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = _cancelButton;
            ClientSize = new Size(343, 276);
            Controls.Add(_helpText);
            Controls.Add(dataGridView1);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(toolStrip1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "KeywordColorsEditorForm";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Keyword Colors Editor";
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            panel2.ResumeLayout(false);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
        private ToolStrip toolStrip1;
        private ToolStripButton _newToolstripButton;
        private ToolStripButton _openToolstripButton;
        private ToolStripButton _saveToolstripButton;
        private Button _helpButton;
        private Label _helpText;
    }
}