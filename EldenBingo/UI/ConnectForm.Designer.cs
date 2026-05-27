namespace EldenBingo.UI
{
    partial class ConnectForm
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
            label1 = new Label();
            label2 = new Label();
            _cancelButton = new Button();
            _connectButton = new Button();
            _addressTextBox = new TextBox();
            _portTextBox = new TextBox();
            errorProvider1 = new ErrorProvider(components);
            _autoConnectCheckBox = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 12);
            label1.Name = "label1";
            label1.Size = new Size(52, 15);
            label1.TabIndex = 0;
            label1.Text = "Address:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 41);
            label2.Name = "label2";
            label2.Size = new Size(32, 15);
            label2.TabIndex = 2;
            label2.Text = "Port:";
            // 
            // _cancelButton
            // 
            _cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _cancelButton.Location = new Point(177, 103);
            _cancelButton.Name = "_cancelButton";
            _cancelButton.Size = new Size(75, 23);
            _cancelButton.TabIndex = 6;
            _cancelButton.Text = "Cancel";
            _cancelButton.UseVisualStyleBackColor = true;
            _cancelButton.Click += _cancelButton_Click;
            // 
            // _connectButton
            // 
            _connectButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _connectButton.Location = new Point(96, 103);
            _connectButton.Name = "_connectButton";
            _connectButton.Size = new Size(75, 23);
            _connectButton.TabIndex = 5;
            _connectButton.Text = "Connect";
            _connectButton.UseVisualStyleBackColor = true;
            _connectButton.Click += _connectButton_Click;
            // 
            // _addressTextBox
            // 
            _addressTextBox.Location = new Point(77, 9);
            _addressTextBox.Name = "_addressTextBox";
            _addressTextBox.Size = new Size(175, 23);
            _addressTextBox.TabIndex = 1;
            // 
            // _portTextBox
            // 
            _portTextBox.Location = new Point(77, 38);
            _portTextBox.Name = "_portTextBox";
            _portTextBox.Size = new Size(63, 23);
            _portTextBox.TabIndex = 3;
            // 
            // errorProvider1
            // 
            errorProvider1.ContainerControl = this;
            // 
            // _autoConnectCheckBox
            // 
            _autoConnectCheckBox.AutoSize = true;
            _autoConnectCheckBox.CheckAlign = ContentAlignment.MiddleRight;
            _autoConnectCheckBox.Checked = true;
            _autoConnectCheckBox.CheckState = CheckState.Checked;
            _autoConnectCheckBox.Location = new Point(12, 70);
            _autoConnectCheckBox.Name = "_autoConnectCheckBox";
            _autoConnectCheckBox.Size = new Size(103, 19);
            _autoConnectCheckBox.TabIndex = 4;
            _autoConnectCheckBox.Text = "Auto-connect:";
            _autoConnectCheckBox.UseVisualStyleBackColor = true;
            // 
            // ConnectForm
            // 
            AcceptButton = _connectButton;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = _cancelButton;
            ClientSize = new Size(264, 138);
            Controls.Add(_autoConnectCheckBox);
            Controls.Add(_portTextBox);
            Controls.Add(_addressTextBox);
            Controls.Add(_connectButton);
            Controls.Add(_cancelButton);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "ConnectForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Connect To Bingo Server";
            Load += ConnectForm_Load;
            ((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Button _cancelButton;
        private Button _connectButton;
        private TextBox _addressTextBox;
        private TextBox _portTextBox;
        private ErrorProvider errorProvider1;
        private CheckBox _autoConnectCheckBox;
    }
}