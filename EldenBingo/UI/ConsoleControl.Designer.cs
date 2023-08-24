namespace EldenBingo.UI
{
    partial class ConsoleControl
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
            this._consoleTextBox = new RichTextBoxCustom();
            this.SuspendLayout();
            // 
            // _consoleTextBox
            // 
            this._consoleTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._consoleTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._consoleTextBox.Location = new System.Drawing.Point(0, 0);
            this._consoleTextBox.Margin = new System.Windows.Forms.Padding(0);
            this._consoleTextBox.Name = "_consoleTextBox";
            this._consoleTextBox.ReadOnly = true;
            this._consoleTextBox.Size = new System.Drawing.Size(400, 300);
            this._consoleTextBox.TabIndex = 1;
            this._consoleTextBox.Text = "";
            this._consoleTextBox.MustHideCaret = true;
            // 
            // ConsoleControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.Controls.Add(this._consoleTextBox);
            this.Name = "ConsoleControl";
            this.Size = new System.Drawing.Size(400, 300);
            this.ResumeLayout(false);

        }

        #endregion

        private RichTextBoxCustom _consoleTextBox;
    }
}
