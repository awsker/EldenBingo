using EldenBingo.Settings;
using System.ComponentModel;

namespace EldenBingo.UI
{
    internal partial class KeywordSquareColorEditorForm : Form
    {
        public KeywordSquareColorEditorForm()
        {
            _colors = new BindingList<KeywordSquareColor>();
            InitializeComponent();
            updateButtonsAvailability();
        }

        private BindingList<KeywordSquareColor> _colors;

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 1 && e.RowIndex >= 0)
            {
                e.Value = string.Empty;
                e.FormattingApplied = true;
            }
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == 1 && e.RowIndex >= 0 && e.RowIndex < _colors.Count)
            {
                var kwc = _colors[e.RowIndex];
                var b = new SolidBrush(kwc.Color);
                e.Graphics.FillRectangle(b, e.CellBounds);
                e.Handled = true;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1 && e.RowIndex >= 0 && e.RowIndex < _colors.Count)
            {
                var dialog = new ColorDialog();
                dialog.Color = _colors[e.RowIndex].Color;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    _colors[e.RowIndex].Color = dialog.Color;
                }
            }

        }
        private void updateButtonsAvailability()
        {
            var selectedRows = dataGridView1.SelectedRows;
            var row = selectedRow();
            _addButton.Enabled = true;
            _removeButton.Enabled = selectedRows.Count > 0;
            _moveUpButton.Enabled = row != null && !row.IsNewRow && row.Index > 0;
            _moveDownButton.Enabled = row != null && !row.IsNewRow && row.Index < _colors.Count - 1;
        }

        private bool validate()
        {
            bool success = true;
            for (int i = _colors.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrWhiteSpace(_colors[i].Keyword))
                {
                    dataGridView1.Rows[i].ErrorText = "No keyword set";
                    success = false;
                }
                else
                {
                    dataGridView1.Rows[i].ErrorText = string.Empty;
                }
            }
            return success;
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            if (validate())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            updateButtonsAvailability();
        }

        private void _addButton_Click(object sender, EventArgs e)
        {
            validate();
            _colors.Add(new KeywordSquareColor("", Color.FromArgb(170, 140, 0)));
        }

        private void _removeButton_Click(object sender, EventArgs e)
        {
            for (int i = _colors.Count - 1; i >= 0; i--)
            {
                var row = dataGridView1.Rows[i];
                if (row.Selected)
                {
                    _colors.RemoveAt(i);
                }
            }
            dataGridView1.ClearSelection();
        }

        private void _moveUpButton_Click(object sender, EventArgs e)
        {
            var row = selectedRow();
            if (row == null)
                return;

            int i = row.Index;
            if (!row.IsNewRow && i > 0)
            {
                var c = _colors[i];
                _colors.RemoveAt(i);
                _colors.Insert(i - 1, c);
                dataGridView1.ClearSelection();
                dataGridView1.Rows[i - 1].Selected = true;
            }
        }

        private void _moveDownButton_Click(object sender, EventArgs e)
        {
            var row = selectedRow();
            if (row == null)
                return;

            int i = row.Index;
            if (i < _colors.Count - 1)
            {
                var c = _colors[i];
                _colors.RemoveAt(i);
                _colors.Insert(i + 1, c);
                dataGridView1.ClearSelection();
                dataGridView1.Rows[i + 1].Selected = true;
            }
        }

        private DataGridViewRow? selectedRow()
        {
            if (dataGridView1.SelectedRows.Count == 1 && dataGridView1.SelectedRows[0] is DataGridViewRow row)
            {
                return row;
            }
            if (dataGridView1.CurrentRow != null && !dataGridView1.CurrentRow.IsNewRow)
            {
                return dataGridView1.CurrentRow;
            }
            return null;
        }

        private void _helpButton_Click(object sender, EventArgs e)
        {
            toolTip1.Active = true;
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            validate();
        }

        public List<KeywordSquareColor> Colors
        {
            get
            {
                return new List<KeywordSquareColor>(_colors);
            }
            set
            {
                _colors = new BindingList<KeywordSquareColor>(value.ToList());
                dataGridView1.DataSource = _colors;
            }
        }
    }
}
