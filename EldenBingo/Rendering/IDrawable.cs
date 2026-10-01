using SFML.Graphics;

namespace EldenBingo.Rendering
{
    public interface IDrawable : SFML.Graphics.IDrawable
    {
        bool Visible { get; }

        FloatRect? GetBoundingBox() { return null; }

        void Init() { }

        void Dispose() { }
    }
}