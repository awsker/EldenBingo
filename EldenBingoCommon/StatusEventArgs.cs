using System.Drawing;

namespace EldenBingoCommon
{
    public class StatusEventArgs : EventArgs
    {
        public string Status { get; private set; }
        public Color Color { get; private set; }
        public StatusEventArgs(string status, Color color)
        {
            Status = status;
            Color = color;
        }
    }
}
