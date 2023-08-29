namespace EldenBingo.UI
{
    partial class ClientListControl
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
            this._clientList = new EldenBingo.UI.RichListBox();
            this.SuspendLayout();
            // 
            // _clientList
            // 
            this._clientList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._clientList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._clientList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this._clientList.IntegralHeight = false;
            this._clientList.ItemHeight = 20;
            this._clientList.Location = new System.Drawing.Point(0, 0);
            this._clientList.Name = "_clientList";
            this._clientList.Size = new System.Drawing.Size(150, 150);
            this._clientList.TabIndex = 0;
            // 
            // ClientListControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.Controls.Add(this._clientList);
            this.Name = "ClientListControl2";
            this.ResumeLayout(false);

        }

        #endregion

        private RichListBox _clientList;
    }
}
