using SFML.System;
using View = SFML.Graphics.View;

namespace EldenBingo.Rendering
{
    public class Camera : ICamera
    {
        private Vector2f _position, _size;
        private View? _view;
        private float _zoom = 1.0f;

        public Camera(Vector2f position, Vector2f size)
        {
            Position = position;
            Size = size;
        }

        /// <summary>
        /// True if this camera has changed
        /// </summary>
        public bool Changed { get; private set; }

        /// <summary>
        /// Center position of camera
        /// </summary>
        public virtual Vector2f Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    Changed = true;
                }
            }
        }

        public Vector2f Size
        {
            get => _size;
            set
            {
                if (_size != value)
                {
                    _size = value;
                    Changed = true;
                }
            }
        }

        public float Zoom
        {
            get => _zoom;
            set
            {
                if (_zoom != value)
                {
                    _zoom = value;
                    Changed = true;
                }
            }
        }

        public bool Enabled => throw new NotImplementedException();

        public View GetView()
        {
            if (Changed || _view == null)
            {
                _view = new View(Position, Size);
                _view.Zoom(_zoom);
                Changed = false;
            }
            return _view;
        }

        public void Update(float dt)
        { }
    }
}