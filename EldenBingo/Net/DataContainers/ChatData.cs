using EldenBingoCommon;

namespace EldenBingo.Net.DataContainers
{
    internal class ChatData
    {
        public UserInRoom User { get; init; }
        public string Message { get; init; }
        public ChatData(UserInRoom user, string message)
        {
            User = user;
            Message = message;
        }
    }
}
