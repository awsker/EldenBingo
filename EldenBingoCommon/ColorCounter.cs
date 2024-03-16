namespace EldenBingoCommon
{
    public struct SquareCounter : IEquatable<SquareCounter>
    {
        public int Counter;
        public int Team;

        public SquareCounter(int team, int counter = 0)
        {
            Team = team;
            Counter = counter;
        }

        public bool Equals(SquareCounter counter)
        {
            return Team == counter.Team && Counter == counter.Counter;
        }
    }
}