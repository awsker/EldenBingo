using SFML.System;
using SFML.Window;

namespace EldenBingo.Rendering
{
    public enum CameraMode
    {
        FreeCam,
        FitAll,
        FollowTarget
    }

    public class CameraController : IUpdateable
    {
        private CameraMode _cameraMode = CameraMode.FreeCam;
        private MapWindow2 _window;

        private ICamera _camera;

        private float _lastZoom;

        private float _userZoom;

        private bool _mouseLeftHeld;
        private bool _mouseRightHeld;

        private Vector2f _lastMouseWorldPosition;

        public CameraController(MapWindow2 window, ICamera camera)
        {
            _window = window;
            _camera = camera;
            _lastZoom = 1f;
            _userZoom = 1f;

            listenToWindowEvents(window);
        }

        public CameraMode CameraMode
        {
            get { return _cameraMode; }
            set
            {
                if (value != _cameraMode)
                {
                    _cameraMode = value;
                    if (_cameraMode == CameraMode.FreeCam)
                    {
                        //Make the camera stop in case its a lerp camera
                        _camera.Position = _camera.Position;
                    }
                }
            }
        }

        public bool Enabled { get; set; } = true;

        public void Update(float dt)
        {
            _camera.Update(dt);
        }

        private void listenToWindowEvents(MapWindow2 window)
        {
            window.MouseWheelScrolled += onMouseWheelScrolled;
            window.MouseButtonPressed += onMousePressed;
            window.MouseButtonReleased += onMouseReleased;
            window.MouseMoved += onMouseMoved;
            window.Closed += onWindowClosed;
        }

        private void unlistenToWindowEvents(MapWindow2 window)
        {
            window.MouseWheelScrolled -= onMouseWheelScrolled;
            window.MouseButtonPressed -= onMousePressed;
            window.MouseButtonReleased -= onMouseReleased;
            window.MouseMoved -= onMouseMoved;
            window.Closed -= onWindowClosed;
        }

        private void onMouseWheelScrolled(object? sender, MouseWheelScrollEventArgs e)
        {
            var change = _userZoom * 0.12f;
            if (e.Delta > 0f)
                _userZoom = Math.Max(1f, _userZoom - change);
            if (e.Delta < 0f)
                _userZoom = Math.Min(12f, _userZoom + change);
            _camera.Zoom = getZoom();
        }

        private void onMousePressed(object? sender, MouseButtonEventArgs e)
        {
            _lastMouseWorldPosition = screenToWorldCoordinates(new Vector2i(e.X, e.Y));

            if (e.Button == Mouse.Button.Left)
            {
                _mouseLeftHeld = true;
                if (_camera is LerpCamera lerp)
                {
                    lerp.StopLerp();
                    _userZoom = getUserZoomFromCamera(lerp);
                }
            }
            if (e.Button == Mouse.Button.Right)
                _mouseRightHeld = true;
        }

        private void onMouseReleased(object? sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
            {
                _mouseLeftHeld = false;
                
            }
            if (e.Button == Mouse.Button.Right)
                _mouseRightHeld = false;
        }

        private void onMouseMoved(object? sender, MouseMoveEventArgs e)
        {
            var pos = screenToWorldCoordinates(new Vector2i(e.X, e.Y));
            if (_mouseLeftHeld && CameraMode == CameraMode.FreeCam)
            {
                var diff = _lastMouseWorldPosition - pos;
                _camera.Position += diff;
                if (_camera is LerpCamera lerp)
                    lerp.Snap();
            }
        }

        private float getZoom()
        {
            return _lastZoom * _userZoom;
        }

        private float getUserZoomFromCamera(LerpCamera lerp)
        {
            return _userZoom * (lerp.Zoom / getZoom());
        }

        private void onWindowClosed(object? sender, EventArgs e)
        {
            if (sender is MapWindow2 window)
            {
                unlistenToWindowEvents(window);
            }
        }

        private Vector2f screenToWorldCoordinates(Vector2i screenCoords)
        {
            return _window.MapPixelToCoords(screenCoords);
        }

        private Vector2i worldToScreenCoordinates(Vector2f worldCoords)
        {
            return _window.MapCoordsToPixel(worldCoords);
        }
    }
}