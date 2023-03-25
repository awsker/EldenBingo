namespace EldenBingo.Net.DataContainers
{
    internal class JoinRoomDeniedData
    {
        public string Message { get; init; }
        public JoinRoomDeniedData(string message)
        {
            Message = message;
        }
    }
}
