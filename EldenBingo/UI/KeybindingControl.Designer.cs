namespace EldenBingo.UI
{
    partial class KeybindingControl
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
            _rebindTextBox = new TextBox();
            _label = new Label();
            _clearBindingButton = new Button();
            SuspendLayout();
            // 
            // _rebindTextBox
            // 
            _rebindTextBox.Dock = DockStyle.Right;
            _rebindTextBox.Location = new Point(211, 0);
            _rebindTextBox.Name = "_rebindTextBox";
            _rebindTextBox.ReadOnly = true;
            _rebindTextBox.Size = new Size(105, 23);
            _rebindTextBox.TabIndex = 44;
            _rebindTextBox.Enter += _rebindTextBox_Enter;
            _rebindTextBox.KeyDown += _rebindTextBox_KeyDown;
            _rebindTextBox.Leave += _rebindTextBox_Leave;
            // 
            // _label
            // 
            _label.Dock = DockStyle.Fill;
            _label.Location = new Point(0, 0);
            _label.Name = "_label";
            _label.Size = new Size(211, 22);
            _label.TabIndex = 45;
            _label.Text = "ButtonName";
            _label.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _clearBindingButton
            // 
            _clearBindingButton.Dock = DockStyle.Right;
            _clearBindingButton.Location = new Point(316, 0);
            _clearBindingButton.Name = "_clearBindingButton";
            _clearBindingButton.Size = new Size(22, 22);
            _clearBindingButton.TabIndex = 46;
            _clearBindingButton.Text = "X";
            _clearBindingButton.UseVisualStyleBackColor = true;
            _clearBindingButton.Click += _clearBindingButton_Click;
            // 
            // KeybindingControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(_label);
            Controls.Add(_rebindTextBox);
            Controls.Add(_clearBindingButton);
            Name = "KeybindingControl";
            Size = new Size(338, 22);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox _rebindTextBox;
        private Label _label;
        private Button _clearBindingButton;
    }
}
