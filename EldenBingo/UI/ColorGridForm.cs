using EldenBingoCommon;
namespace EldenBingo.UI
{
    public partial class ColorGridForm : Form
    {
        public Color? SelectedColor { get; private set; }
        public event EventHandler ColorClicked;

        public ColorGridForm()
        {
            InitializeComponent();
            initGrid();
        }

        private void initGrid()
        {
            var grid = new GridControl()
            {
                GridWidth = 3,
                GridHeight = 3,
                PaddingX = 2,
                PaddingY = 2,
                BorderX = 2,
                BorderY = 2,
                Dock = DockStyle.Fill,
                BackColor = Color.Black,
            };

            for (int i = 0; i < NetConstants.DefaultPlayerColors.Length; ++i)
            {
                var col = NetConstants.DefaultPlayerColors[i];
                var panel = new ColorPanel(col.Color);
                grid.Controls.Add(panel);
                panel.Click += panel_Click;
            }
            Controls.Add(grid);
        }

        private void panel_Click(object? sender, EventArgs e)
        {
            if(sender is ColorPanel cp) {
                SelectedColor = cp.Color;
                DialogResult = DialogResult.OK;
                ColorClicked?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
