using SFML.Graphics;

namespace EldenBingo.Rendering.Drawables
{
    public class TextDrawable : Text, IDrawable
    {
        public TextDrawable()
        {
        }

        public TextDrawable(string str, SFML.Graphics.Font font) : base(str, font)
        {
        }

        public TextDrawable(string str, SFML.Graphics.Font font, uint characterSize) : base(str, font, characterSize)
        {
        }

        public bool Visible { get; set; } = true;
    }
}