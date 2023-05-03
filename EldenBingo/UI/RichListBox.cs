using EldenBingoCommon;

namespace EldenBingo.UI
{
    internal class RichListBox : ListBox
    {
        public RichListBox()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            DrawItem += new DrawItemEventHandler(listBox_DrawItem);
        }

        private void listBox_DrawItem(object? sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index >= 0 && e.Index < Items.Count)
            {
                var brush = ((RichListBox)sender).Items[e.Index] is UserInRoom item ? new SolidBrush(item.Color) : new SolidBrush(ForeColor);
                e.Graphics.DrawString(((ListBox)sender).Items[e.Index].ToString(),
                      e.Font, brush, e.Bounds, StringFormat.GenericDefault);
            }
            e.DrawFocusRectangle();
        }
    }
}
