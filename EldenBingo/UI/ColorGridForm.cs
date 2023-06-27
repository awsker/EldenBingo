using EldenBingoCommon;

namespace EldenBingo.UI
{
    public partial class ColorGridForm : Form
    {
        public ColorGridForm()
        {
            InitializeComponent();
            initGrid();
        }

        public event EventHandler? ColorClicked;

        public int SelectedIndex { get; set; }

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

            for (int i = 0; i < BingoConstants.TeamColors.Length; ++i)
            {
                var col = BingoConstants.TeamColors[i];
                var panel = new ColorPanel(col.Color);
                grid.Controls.Add(panel);
                var temp = i;
                panel.Click += (o, e) => panel_Click(temp);
            }
            Controls.Add(grid);
        }

        private void panel_Click(int index)
        {
            SelectedIndex = index;
            DialogResult = DialogResult.OK;
            ColorClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}