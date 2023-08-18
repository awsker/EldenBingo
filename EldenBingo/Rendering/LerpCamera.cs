using SFML.System;
using View = SFML.Graphics.View;

namespace EldenBingo.Rendering
{
    public class LerpCamera : ICamera
    {
        private const float LERP_REMAINING_PER_SEC = 0.1f;
        private Vector2f _position, _size;
        private bool _snap;
        private Vector2f _targetPosition;
        private float _targetZoom;
        private View? _view;
        private float _zoom;

        public LerpCamera(Vector2f position, Vector2f size, float zoom)
        {
            _position = position;
            _targetPosition = position;
            _zoom = zoom;
            _targetZoom = zoom;
            Size = size;
        }

        /// <summary>
        /// True if this camera has changed
        /// </summary>
        public bool Changed { get; private set; }

        /// <summary>
        /// True if this camera is enabled and should update
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Center position of camera
        /// </summary>
        public virtual Vector2f Position
        {
            get => _position;
            set
            {
                if (_targetPosition != value)
                {
                    _targetPosition = value;
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
                var z = Math.Clamp(value, MinZoom, MaxZoom);
                if (_targetZoom != z)
                {
                    _targetZoom = z;
                    Changed = true;
                }
            }
        }

        public float MinZoom { get; set; } = float.MinValue;
        public float MaxZoom { get; set; } = float.MaxValue;

        public View GetView()
        {
            if (Changed || _view == null)
            {
                _view = new View(Position, Size);
                _view.Zoom(Zoom);
                Changed = false;
            }
            return _view;
        }

        public void StopLerp()
        {
            _targetZoom = _zoom;
            _targetPosition = _position;
        }

        public void Snap()
        {
            _snap = true;
        }

        public void Update(float dt)
        {
            if (_snap)
            {
                snapToTarget();
            }
            else
            {
                float d = (float)(1.0 - Math.Pow(LERP_REMAINING_PER_SEC, dt));
                _position += (_targetPosition - _position) * d;
                _zoom += (_targetZoom - _zoom) * d;
                Changed = true;
            }
        }

        private void snapToTarget()
        {
            _position = _targetPosition;
            _zoom = _targetZoom;
            Changed = true;
            _snap = false;
        }
    }
}