using SFML.Graphics;
using SFML.System;

namespace EldenBingo.Rendering
{
    public static class FloatRectExtension
    {
        public static float Right(this FloatRect rect) { return rect.Left + rect.Width; }
        public static float Bottom(this FloatRect rect) { return rect.Top + rect.Height; }

        public static FloatRect MaxBounds(this FloatRect r1, FloatRect r2)
        {
            float minX, minY, maxX, maxY;
            minX = Math.Min(r1.Left, r2.Left);
            maxX = Math.Max(r1.Left + r1.Width, r2.Left + r2.Width);
            minY = Math.Min(r1.Top, r2.Top);
            maxY = Math.Max(r1.Top + r1.Height, r2.Top + r2.Height);
            return new FloatRect(minX, minY, maxX - minX, maxY - minY);
        }

        public static FloatRect MaxBounds(this FloatRect r1, Vector2f pos)
        {
            float minX, minY, maxX, maxY;
            minX = Math.Min(r1.Left, pos.X);
            maxX = Math.Max(r1.Right(), pos.X);
            minY = Math.Min(r1.Top, pos.Y);
            maxY = Math.Max(r1.Bottom(), pos.Y);
            return new FloatRect(minX, minY, maxX - minX, maxY - minY);
        }


        public static FloatRect Extrude(this FloatRect rect, float f)
        {
            return new FloatRect(rect.Left - f, rect.Top, rect.Width + 2f*f, rect.Height + 2f*f);
        }
    }
}
