namespace EldenBingo.Net.DataContainers
{
    internal class AdminStatusMessageData
    {
        public int Color { get; init; }
        public string Message { get; init; }
        public AdminStatusMessageData(int color, string message)
        {
            Color = color;
            Message = message;
        }
    }
}
