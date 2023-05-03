namespace EldenBingo.Net.DataContainers
{
    internal class AdminStatusMessageData
    {
        public AdminStatusMessageData(int color, string message)
        {
            Color = color;
            Message = message;
        }

        public int Color { get; init; }
        public string Message { get; init; }
    }
}