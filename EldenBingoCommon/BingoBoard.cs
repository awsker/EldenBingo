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
                Squares[i] = new BingoBoardSquare(squareTexts[i], tooltips[i], new BingoSquareStatus());
            }
        }

        public BingoBoardSquare[] Squares { get; init; }
    }

    public record struct BingoBoardSquare(string Text, string Tooltip, BingoSquareStatus Status)
    {
        public bool Checked => Status.Team.HasValue;
        public int? Team => Status.Team;
        public bool Marked => Status.Marked;
        public TeamCounter[] Counters => Status.Counters;

        public override string ToString()
        {
            return Text;
        }
    }

    public record struct BingoSquareStatus(int? Team, bool Marked, TeamCounter[] Counters);
}