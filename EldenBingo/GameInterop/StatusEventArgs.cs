namespace EldenBingo
{
    public class StatusEventArgs : EventArgs
    {
        public StatusEventArgs(string status, Color color)
        {
            Status = status;
            Color = color;
        }

        public string Status { get; private set; }
        public Color Color { get; private set; }
    }
}