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
            components = new System.ComponentModel.Container();
            _clientList = new RichListBox();
            contextMenuStrip1 = new ContextMenuStrip(components);
            _banPlayerToolStripMenuItem = new ToolStripMenuItem();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // _clientList
            // 
            _clientList.BorderStyle = BorderStyle.None;
            _clientList.ContextMenuStrip = contextMenuStrip1;
            _clientList.Dock = DockStyle.Fill;
            _clientList.DrawMode = DrawMode.OwnerDrawFixed;
            _clientList.IntegralHeight = false;
            _clientList.ItemHeight = 20;
            _clientList.Location = new Point(0, 0);
            _clientList.Name = "_clientList";
            _clientList.Size = new Size(150, 150);
            _clientList.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { _banPlayerToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(182, 26);
            contextMenuStrip1.Opening += contextMenuStrip1_Opening;
            // 
            // _banPlayerToolStripMenuItem
            // 
            _banPlayerToolStripMenuItem.Name = "_banPlayerToolStripMenuItem";
            _banPlayerToolStripMenuItem.Size = new Size(181, 22);
            _banPlayerToolStripMenuItem.Text = "Ban user from lobby";
            _banPlayerToolStripMenuItem.Click += _banPlayerToolStripMenuItem_Click;
            // 
            // ClientListControl
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(_clientList);
            Name = "ClientListControl";
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private RichListBox _clientList;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem _banPlayerToolStripMenuItem;
    }
}
