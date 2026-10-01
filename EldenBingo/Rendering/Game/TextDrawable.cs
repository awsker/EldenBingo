using SFML.Graphics;

namespace EldenBingo.Rendering.Game
{
    public class TextDrawable : Text, IDrawable
    {
        public TextDrawable(string str, SFML.Graphics.Font font) : base(font, str)
        {
        }

        public TextDrawable(string str, SFML.Graphics.Font font, uint characterSize) : base(font, str, characterSize)
        {
        }

        public bool Visible { get; set; } = true;
    }
}