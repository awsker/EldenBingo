using EldenBingoCommon;

namespace EldenBingoServer
{
    public class BingoClientInRoom : UserInRoom
    {
        public BingoClientInRoom(BingoClientModel client, string nick, Guid guid, bool isAdmin, int team) : base(nick, guid, isAdmin, team)
        {
            Client = client;
        }

        public BingoClientModel Client { get; init; }
    }
}