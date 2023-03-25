namespace EldenBingo.UI
{
    partial class BingoControl
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
            this._gridControl = new EldenBingo.UI.GridControl();
            this.SuspendLayout();
            // 
            // _gridControl
            // 
            this._gridControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(118)))), ((int)(((byte)(110)))), ((int)(((byte)(97)))));
            this._gridControl.BorderX = 2;
            this._gridControl.BorderY = 2;
            this._gridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gridControl.GridHeight = 5;
            this._gridControl.GridWidth = 5;
            this._gridControl.Location = new System.Drawing.Point(0, 0);
            this._gridControl.MaintainAspectRatio = false;
            this._gridControl.Name = "_gridControl";
            this._gridControl.PaddingX = 2;
            this._gridControl.PaddingY = 2;
            this._gridControl.Size = new System.Drawing.Size(400, 400);
            this._gridControl.TabIndex = 0;
            this._gridControl.Text = "gridControl1";
            // 
            // BingoControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this._gridControl);
            this.Name = "BingoControl";
            this.Size = new System.Drawing.Size(400, 400);
            this.ResumeLayout(false);

        }

        #endregion

        private GridControl _gridControl;
    }
}
