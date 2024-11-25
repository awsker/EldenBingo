namespace EldenBingo.UI
{
    partial class ChangeTeamForm
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
            _okButton = new Button();
            _cancelButton = new Button();
            _teamComboBox = new ComboBox();
            _colorPanel = new Panel();
            SuspendLayout();
            // 
            // _okButton
            // 
            _okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _okButton.DialogResult = DialogResult.OK;
            _okButton.Location = new Point(35, 53);
            _okButton.Name = "_okButton";
            _okButton.Size = new Size(75, 23);
            _okButton.TabIndex = 12;
            _okButton.Text = "OK";
            _okButton.UseVisualStyleBackColor = true;
            _okButton.Click += _okButton_Click;
            // 
            // _cancelButton
            // 
            _cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _cancelButton.DialogResult = DialogResult.Cancel;
            _cancelButton.Location = new Point(116, 53);
            _cancelButton.Name = "_cancelButton";
            _cancelButton.Size = new Size(75, 23);
            _cancelButton.TabIndex = 13;
            _cancelButton.Text = "Cancel";
            _cancelButton.UseVisualStyleBackColor = true;
            _cancelButton.Click += _cancelButton_Click;
            // 
            // _teamComboBox
            // 
            _teamComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _teamComboBox.FormattingEnabled = true;
            _teamComboBox.Location = new Point(12, 15);
            _teamComboBox.Name = "_teamComboBox";
            _teamComboBox.Size = new Size(147, 23);
            _teamComboBox.TabIndex = 14;
            // 
            // _colorPanel
            // 
            _colorPanel.BorderStyle = BorderStyle.FixedSingle;
            _colorPanel.Location = new Point(165, 14);
            _colorPanel.Name = "_colorPanel";
            _colorPanel.Size = new Size(26, 25);
            _colorPanel.TabIndex = 15;
            // 
            // ChangeTeamForm
            // 
            AcceptButton = _okButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = _cancelButton;
            ClientSize = new Size(199, 88);
            Controls.Add(_colorPanel);
            Controls.Add(_teamComboBox);
            Controls.Add(_okButton);
            Controls.Add(_cancelButton);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "ChangeTeamForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Change Team";
            ResumeLayout(false);
        }

        #endregion

        private Button _okButton;
        private Button _cancelButton;
        private ComboBox _teamComboBox;
        private Panel _colorPanel;
    }
}