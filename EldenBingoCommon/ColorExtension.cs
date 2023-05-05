using System.Drawing;

namespace EldenBingoCommon
{
    public static class ColorExtension
    {
        public static Color Brighten(this Color color, float brightenFactor)
        {
            var maxChange = Convert.ToInt32(Math.Max(Math.Max((255 - color.R) * brightenFactor, (255 - color.G) * brightenFactor), (255 - color.B) * brightenFactor));
            return Color.FromArgb(color.A,
                Math.Min(255, color.R + maxChange),
                Math.Min(255, color.G + maxChange),
                Math.Min(255, color.B + maxChange));
        }
    }
}
