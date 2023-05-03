using EldenBingoCommon;

namespace EldenBingo.Net.DataContainers
{
    internal class ChatData
    {
        public ChatData(UserInRoom user, string message)
        {
            User = user;
            Message = message;
        }

        public string Message { get; init; }
        public UserInRoom User { get; init; }
    }
}