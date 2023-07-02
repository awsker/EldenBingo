using SFML.Graphics;
using SFML.System;

namespace EldenBingo.Rendering.Game
{
    internal class ClassDrawable : IDrawable, IUpdateable
    {
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

        private const float MaxHeightProportionateToWindow = 0.75f;

        //Fraction of movement remaining per second (position interpolation)
        const float InterpTime = 1.0f;

        public ClassDrawable(Texture tex, int index, int numClasses, Vector2u renderTargetSize)
        {
            if (numClasses <= 0)
                throw new ArgumentException("Number of classes must be > 0", nameof(numClasses));
            _sprite = new Sprite(tex);
            _index = index;
            _numClasses = numClasses;
            
            _renderTargetSize = renderTargetSize;
            _proportion = 1f;
            SetTargetSize(renderTargetSize);
        }

        public bool Visible { get; set; } = true;

        public bool Enabled { get; set; } = true;

        public void Draw(RenderTarget target, RenderStates states)
        {
            _sprite.Color = new SFML.Graphics.Color(255, 255, 255, (byte)Math.Min(255,_opacity*255));
            target.Draw(_sprite, states);
        }

        public void Dispose() 
        {
            _sprite.Dispose();
        }

        public void Update(float dt)
        {
            _interp += dt;
            var interpProp = Math.Clamp(_interp / InterpTime, 0f, 1f);

            float prop = (float)Math.Sin(Math.PI * 0.5 * interpProp);
            var pos = _startPosition * (1f - prop) + _targetPosition * prop;
            _sprite.Position = pos;
            _opacity = interpProp;
        }

        public void SetTargetSize(Vector2u size)
        {
            _renderTargetSize = size;

            var xProp = (float)_renderTargetSize.X / (_sprite.Texture.Size.X * _numClasses);
            var yProp = _renderTargetSize.Y* MaxHeightProportionateToWindow / _sprite.Texture.Size.Y;

            _proportion = Math.Min(xProp, yProp); //Use the smallest calculated proportion;
            _sprite.Scale = new Vector2f(_proportion, _proportion);

            _targetSize = new Vector2f(_sprite.Texture.Size.X * _proportion, _sprite.Texture.Size.Y * _proportion);

            var subWindow = new RectangleF(_index * (float)_renderTargetSize.X / _numClasses, 0, _renderTargetSize.X / _numClasses, _renderTargetSize.Y);
            _targetPosition = new Vector2f(subWindow.X + (subWindow.Width - _targetSize.X) * 0.5f, subWindow.Y + (subWindow.Height - _targetSize.Y) * 0.5f);
            _startPosition = _targetPosition - new Vector2f(subWindow.Width * 0.1f, 0f);
            _interp = 0f;
        }
    }
}
