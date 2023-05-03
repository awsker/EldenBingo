using EldenBingoCommon;

namespace EldenBingo.Net.DataContainers
{
    internal class MatchStatusData
    {
        public MatchStatusData(Match match)
        {
            Match = match;
        }

        public Match Match { get; init; }
    }
}