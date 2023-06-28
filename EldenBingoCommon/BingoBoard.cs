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

        public BingoBoard(string[] squareTexts, string[] tooltips)
        {
            if (squareTexts.Length != 25)
                throw new ArgumentException("Needs exactly 25 strings");
            if (tooltips.Length != 25)
                throw new ArgumentException("Needs exactly 25 tooltips");

            Squares = new BingoBoardSquare[25];
            for (int i = 0; i < squareTexts.Length; ++i)
            {
                Squares[i] = new BingoBoardSquare(squareTexts[i], tooltips[i], null, false, Array.Empty<TeamCounter>());
            }
        }

        public BingoBoardSquare[] Squares { get; init; }
    }

    public record struct BingoBoardSquare(string Text, string Tooltip, int? Team, bool Marked, TeamCounter[] Counters)
    {
        public bool Checked = Team.HasValue;
        public override string ToString()
        {
            return Text;
        }
    }
}