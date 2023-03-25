using EldenBingoCommon;

namespace EldenBingoServer
{
    internal class ClientInRoom : UserInRoom
    {
        public ClientModel Client { get; init; }
        public ClientInRoom(ClientModel client, string nick, Guid guid, int color, bool isAdmin, int team, bool isSpectator) : base(nick, guid, color, isAdmin, team, isSpectator)
        {
            Client = client;
        }
    }
}