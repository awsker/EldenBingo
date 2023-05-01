using System.Drawing;

namespace EldenBingoCommon
{
    public struct ColorCounter : IEquatable<ColorCounter>
    {
        public Color Color;
        public int Counter;
        public ColorCounter(Color c, int counter = 0)
        {
            Color = c;
            Counter = counter;
        }

        public ColorCounter(byte[] buffer, ref int offset)
        {
            Color = Color.FromArgb(PacketHelper.ReadInt(buffer, ref offset));
            Counter = PacketHelper.ReadInt(buffer, ref offset);
        }

        public byte[] GetBytes()
        {
            return PacketHelper.ConcatBytes(
                BitConverter.GetBytes(Color.ToArgb()),
                BitConverter.GetBytes(Counter));
        }

        public bool Equals(ColorCounter counter)
        {
            return Color == counter.Color && Counter == counter.Counter;
        }
    }
}
