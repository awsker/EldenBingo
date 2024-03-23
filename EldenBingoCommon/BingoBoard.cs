namespace EldenBingoCommon
{
    public class BingoBoard
    {
        public BingoBoard(int size, BingoBoardSquare[] squares, EldenRingClasses[] availableClasses)
        {
            Size = size;
            if (squares.Length != size * size)
                throw new ArgumentException($"Needs exactly {size * size} squares");
            Squares = squares;
            AvailableClasses = availableClasses;
        }

        public int Size { get; init; }
        public int SquareCount => Size * Size;
        public BingoBoardSquare[] Squares { get; init; }
        public EldenRingClasses[] AvailableClasses { get; init; }
        
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