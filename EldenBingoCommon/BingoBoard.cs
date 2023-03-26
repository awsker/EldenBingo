using System.Drawing;

namespace EldenBingoCommon
{
    public class BingoBoard
    {
        public BingoBoardSquare[] Squares { get; init; }
        
        public BingoBoard(string[] squareTexts, string[] tooltips)
        {
            if (squareTexts.Length != 25)
                throw new ArgumentException("Needs exactly 25 strings");
            if (tooltips.Length != 25)
                throw new ArgumentException("Needs exactly 25 tooltips");

            Squares = new BingoBoardSquare[25];
            for(int i = 0; i < squareTexts.Length; ++i)
            {
                Squares[i] = new BingoBoardSquare(squareTexts[i], tooltips[i]);
            }
        }

        public BingoBoard(byte[] buffer, ref int offset)
        {
            Squares = new BingoBoardSquare[25];

            for (int i = 0; i < 25; ++i)
            {
                Squares[i] = new BingoBoardSquare(buffer, ref offset);
            }
        }
        
        public virtual byte[] GetBytes(UserInRoom user)
        {
            return PacketHelper.ConcatBytes(Squares.Select(s => s.GetBytes()));
        }
        
        public Dictionary<Color, int> GetNumberOfCheckedSquaresPerColor(IEnumerable<Color> colors)
        {
            var dict = colors.ToDictionary(c => c, c => 0);
            foreach(var square in Squares)
            {
                if (dict.ContainsKey(square.Color))
                    dict[square.Color]++;
            }
            return dict;
        }
    }

    public class BingoBoardSquare : INetSerializable
    {
        public string Text { get; init; }
        public string Tooltip { get; init; }
        public Color Color { get; set; }
        public bool Marked { get; set; }

        public BingoBoardSquare(string text, string tooltip)
        {
            Text = text;
            Tooltip = tooltip;
        }

        public BingoBoardSquare(byte[] buffer, ref int offset)
        {
            Text = PacketHelper.ReadString(buffer, ref offset);
            Tooltip = PacketHelper.ReadString(buffer, ref offset);
            Color = Color.FromArgb(PacketHelper.ReadInt(buffer, ref offset));
            Marked = PacketHelper.ReadBoolean(buffer, ref offset);
        }

        public byte[] GetBytes()
        {
            return PacketHelper.ConcatBytes(
                PacketHelper.GetStringBytes(Text),
                PacketHelper.GetStringBytes(Tooltip),
                BitConverter.GetBytes(Color.ToArgb()),
                BitConverter.GetBytes(Marked));
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
