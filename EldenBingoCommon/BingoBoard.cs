namespace EldenBingoCommon
{
    public class BingoBoard
    {
        public BingoBoard(BingoBoardSquare[] squares, EldenRingClasses[] availableClasses, int pointsPerBingo)
        {
            if (squares.Length != 25)
                throw new ArgumentException("Needs exactly 25 squares");
            Squares = squares;
            AvailableClasses = availableClasses;
            PointsPerBingoLine = pointsPerBingo;
        }

        public BingoBoardSquare[] Squares { get; init; }
        public EldenRingClasses[] AvailableClasses { get; init; }
        public int PointsPerBingoLine { get; init; }
    }

    public record struct BingoBoardSquare(string Text, string Tooltip, int MaxCount, int? Team, bool Marked, SquareCounter[] Counters)
    {
        public bool Checked => Team.HasValue;
        public override string ToString()
        {
            return Text;
        }
    }
}