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

        public bool Equals(TeamCounter counter)
        {
            return Team == counter.Team && Counter == counter.Counter;
        }

    }
}