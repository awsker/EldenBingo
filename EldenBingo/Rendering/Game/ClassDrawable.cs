using SFML.Graphics;
using SFML.System;

namespace EldenBingo.Rendering.Game
{
    internal class ClassDrawable : IDrawable, IUpdateable
    {
        private const float MaxHeightProportionateToWindow = 0.8f;

        //Fraction of movement remaining per second (position interpolation)
        private const float InterpTime = 1.0f;

        private readonly Sprite _sprite;
        private readonly int _index;
        private readonly int _numClasses;

        private Vector2u _renderTargetSize;
        private float _proportion;
        private Vector2f _targetSize;
        private Vector2f _startPosition;
        private Vector2f _targetPosition;
        private float _interp = 0f;
        private float _opacity = 0f;

        public ClassDrawable(Texture tex, int index, int numClasses, Vector2u renderTargetSize)
        {
            if (numClasses <= 0)
                throw new ArgumentException("Number of classes must be > 0", nameof(numClasses));
            _sprite = new Sprite(tex);
            _index = index;
            _numClasses = numClasses;

            _renderTargetSize = renderTargetSize;
            _proportion = 1f;
            _interp = 0f;
            SetTargetSize(renderTargetSize);
        }

        public bool Visible { get; set; } = true;

        public bool Enabled { get; set; } = true;

        public void Draw(RenderTarget target, RenderStates states)
        {
            _sprite.Color = new SFML.Graphics.Color(255, 255, 255, (byte)Math.Min(255, _opacity * 255));
            target.Draw(_sprite, states);
        }

        public void Dispose()
        {
            _sprite.Dispose();
        }

        public void Update(float dt)
        {
            _interp = Math.Min(InterpTime, _interp + dt);
            var interpProportion = _interp / InterpTime;

            float prop = (float)Math.Sin(Math.PI * 0.5 * interpProportion);
            var pos = _startPosition * (1f - prop) + _targetPosition * prop;
            _sprite.Position = pos;
            _opacity = interpProportion;
        }

        public void SetTargetSize(Vector2u size)
        {
            _renderTargetSize = size;

            var xProp = (float)_renderTargetSize.X / (_sprite.Texture.Size.X * _numClasses);
            var yProp = _renderTargetSize.Y * MaxHeightProportionateToWindow / _sprite.Texture.Size.Y;

            _proportion = Math.Min(xProp, yProp); //Use the smallest calculated proportion;
            _sprite.Scale = new Vector2f(_proportion, _proportion);

            _targetSize = new Vector2f(_sprite.Texture.Size.X * _proportion, _sprite.Texture.Size.Y * _proportion);

            var subWindow = new RectangleF(_index * (float)_renderTargetSize.X / _numClasses, 0, _renderTargetSize.X / _numClasses, _renderTargetSize.Y);
            var yPos = Math.Max(_renderTargetSize.Y * 0.2f, subWindow.Y + (subWindow.Height - _targetSize.Y) * 0.5f); //At least a 20% gap between top of window and classes

            _targetPosition = new Vector2f(subWindow.X + (subWindow.Width - _targetSize.X) * 0.5f, yPos);
            _startPosition = _targetPosition - new Vector2f(subWindow.Width * 0.1f, 0f);
        }
    }
}