namespace EldenBingoCommon
{
    public class BingoBoard
    {
        public BingoBoard(BingoBoardSquare[] squares)
        {
            if (squares.Length != 25)
                throw new ArgumentException("Needs exactly 25 squares");
            Squares = squares;
        }

        public BingoBoardSquare[] Squares { get; init; }
    }

    public record struct BingoBoardSquare(string Text, string Tooltip, int Count, int? Team, bool Marked, TeamCounter[] Counters)
    {
        public bool Checked => Team.HasValue;
        public override string ToString()
        {
            return Text;
        }
    }
}