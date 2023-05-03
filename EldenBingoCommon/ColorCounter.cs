namespace EldenBingoCommon
{
    public struct TeamCounter : IEquatable<TeamCounter>
    {
        public int Counter;
        public int Team;

        public TeamCounter(int team, int counter = 0)
        {
            Team = team;
            Counter = counter;
        }

        public TeamCounter(byte[] buffer, ref int offset)
        {
            Team = PacketHelper.ReadInt(buffer, ref offset);
            Counter = PacketHelper.ReadInt(buffer, ref offset);
        }

        public bool Equals(TeamCounter counter)
        {
            return Team == counter.Team && Counter == counter.Counter;
        }

        public byte[] GetBytes()
        {
            return PacketHelper.ConcatBytes(
                BitConverter.GetBytes(Team),
                BitConverter.GetBytes(Counter));
        }
    }
}