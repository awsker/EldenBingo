using SFML.Graphics;

namespace EldenBingo.Rendering
{
    public class SpriteDrawable : Sprite, IDrawable
    {
        public SpriteDrawable(Texture tex) : base(tex)
        {
        }

        public bool Visible { get; set; } = true;
    }
}