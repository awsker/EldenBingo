namespace EldenBingo.Net.DataContainers
{
    internal class JoinRoomDeniedData
    {
        public JoinRoomDeniedData(string message)
        {
            Message = message;
        }

        public string Message { get; init; }
    }
}