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
            this._boardStatusLabel = new System.Windows.Forms.Label();
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
            // _boardStatusLabel
            // 
            this._boardStatusLabel.BackColor = System.Drawing.Color.Transparent;
            this._boardStatusLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._boardStatusLabel.Location = new System.Drawing.Point(0, 0);
            this._boardStatusLabel.Name = "_boardStatusLabel";
            this._boardStatusLabel.Size = new System.Drawing.Size(400, 400);
            this._boardStatusLabel.TabIndex = 1;
            this._boardStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._boardStatusLabel.Click += new System.EventHandler(this._boardStatusLabel_Click);
            // 
            // BingoControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this._boardStatusLabel);
            this.Controls.Add(this._gridControl);
            this.Name = "BingoControl";
            this.Size = new System.Drawing.Size(400, 400);
            this.ResumeLayout(false);

        }

        #endregion

        private GridControl _gridControl;
        private Label _boardStatusLabel;
    }
}
