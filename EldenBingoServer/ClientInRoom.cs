using EldenBingoCommon;

namespace EldenBingoServer
{
    public class ClientInRoom : UserInRoom
    {
        public ClientModel Client { get; init; }
        public ClientInRoom(ClientModel client, string nick, Guid guid, bool isAdmin, int team) : base(nick, guid, isAdmin, team)
        {
            Client = client;
        }
    }
}