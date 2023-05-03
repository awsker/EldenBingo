namespace EldenBingoCommon
{
    public struct TeamCounter : IEquatable<TeamCounter>
    {
        public int Team;
        public int Counter;
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

        public byte[] GetBytes()
        {
            return PacketHelper.ConcatBytes(
                BitConverter.GetBytes(Team),
                BitConverter.GetBytes(Counter));
        }

        public bool Equals(TeamCounter counter)
        {
            return Team == counter.Team && Counter == counter.Counter;
        }
    }
}
