using SFML.Graphics;

namespace EldenBingo.Rendering
{
    public interface IDrawable : Drawable
    {
        bool Visible { get; }

        FloatRect? GetBoundingBox() { return null; }

        void Init() { }

        void Dispose() { }
    }
}