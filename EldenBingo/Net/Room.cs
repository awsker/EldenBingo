using EldenBingoCommon;

namespace EldenBingo.Net
{
    internal class Room : Room<UserInRoom>
    {
        public Room(string name) : base(name)
        {}

        public Room(byte[] buffer, ref int offset) : base(string.Empty)
        {
            Name = PacketHelper.ReadString(buffer, ref offset);
            var numClients = PacketHelper.ReadInt(buffer, ref offset);
            for (int i = 0; i < numClients; ++i)
            {
                var cl = PacketHelper.ReadUserInRoom(buffer, ref offset);
                clients[cl.Guid] = cl;
            }
            Match = new Match(buffer, ref offset);
            Match.UpdateMatchStatus(Match.MatchStatus, Match.ServerTimer, Match.Board);
        }
    }
}
