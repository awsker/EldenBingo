using SFML.Graphics;
using SFML.System;

namespace EldenBingo.Rendering.Game
{
    public class CenteredBackgroundDrawable : IDrawable
    {
        private readonly Sprite _sprite;

        private Vector2u _renderTargetSize;
        public CenteredBackgroundDrawable(Texture tex, Vector2u renderTargetSize)
        {
            _sprite = new Sprite(tex);
            _renderTargetSize = renderTargetSize;
            SetTargetSize(renderTargetSize);
        }


        public bool Visible { get; set; } = true;

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(_sprite, states);
        }

        public void SetTargetSize(Vector2u size)
        {
            _renderTargetSize = size;
            var xProp = (float)_renderTargetSize.X / _sprite.Texture.Size.X;
            var yProp = (float)_renderTargetSize.Y / _sprite.Texture.Size.Y;
            var prop = Math.Max(xProp, yProp);
            _sprite.Scale = new Vector2f(prop, prop);
            _sprite.Position = new Vector2f((_renderTargetSize.X - _sprite.Texture.Size.X * prop) * 0.5f, (_renderTargetSize.Y - _sprite.Texture.Size.Y * prop) * 0.5f);
        }

        public void Dispose()
        {
            _sprite.Dispose();
        }
    }
}
