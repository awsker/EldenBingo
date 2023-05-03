using System.Drawing;

namespace EldenBingoCommon
{
    public class StatusEventArgs : EventArgs
    {
        public StatusEventArgs(string status, Color color)
        {
            Status = status;
            Color = color;
        }

        public Color Color { get; private set; }
        public string Status { get; private set; }
    }
}