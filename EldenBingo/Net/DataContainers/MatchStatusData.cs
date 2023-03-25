using EldenBingoCommon;

namespace EldenBingo.Net.DataContainers
{
    internal class MatchStatusData
    {
        public Match Match { get; init; }
        public MatchStatusData(Match match)
        {
            Match = match;
        }
    }
}
