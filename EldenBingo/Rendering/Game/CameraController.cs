using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace EldenBingo.Rendering.Game
{
    public enum CameraMode
    {
        FreeCam,
        FitAll,
        FollowTarget
    }

    public class CameraController : IUpdateable
    {
        private const float MapViewportWidth = 750f, MapViewportHeight = 750f;

        private CameraMode _cameraMode = CameraMode.FreeCam;
        private PlayerDrawable? _cameraFollow = null;
        private MapWindow _window;
        private ICamera _camera;
        private float _lastZoom;
        private float _userZoom;
        private Vector2f _lastMouseWorldPosition;

        public CameraController(MapWindow window, ICamera camera)
        {
            _window = window;
            _camera = camera;
            _lastZoom = 1f;
            _userZoom = 1f;

            listenToWindowEvents(window);
            updateCameraSize();
        }

        public CameraMode CameraMode
        {
            get { return _cameraMode; }
            set
            {
                var prevMode = _cameraMode;
                _cameraMode = value;
                if (_cameraMode == CameraMode.FreeCam)
                {
                    CameraFollowTarget = null;
                    if (_camera is LerpCamera lerp)
                    {
                        lerp.StopLerp();
                    }
                    _userZoom = _camera.Zoom;
                    setZoom(1f);
                }
                if (_cameraMode == CameraMode.FitAll)
                {
                    CameraFollowTarget = null;
                    _userZoom = 1.0f;
                }
                //If moving from one target to another, immediately snap
                if (_cameraMode == CameraMode.FollowTarget && prevMode == CameraMode.FollowTarget)
                {
                    updateCameraZoomAndPosition();
                    if (_camera is LerpCamera lerp)
                        lerp.Snap();
                }
            }
        }

        public PlayerDrawable? CameraFollowTarget
        {
            get { return _cameraFollow; }
            set
            {
                _cameraFollow = value;
                if (value != null)
                    CameraMode = CameraMode.FollowTarget;
            }
        }

        public bool Enabled { get; set; } = true;

        public void Update(float dt)
        {
            updateCameraZoomAndPosition();
            _camera.Update(dt);
        }

        private void updateCameraZoomAndPosition()
        {
            if (CameraMode == CameraMode.FollowTarget)
            {
                fitPlayer();
            }
            if (CameraMode == CameraMode.FitAll)
            {
                fitAll();
            }
        }

        private void fitPlayer()
        {
            if (CameraFollowTarget == null)
            {
                CameraMode = CameraMode.FitAll;
                return;
            }
            var pos = CameraFollowTarget.GetConvertedPosition();
            setZoom(1f);
            _camera.Position = pos;
        }

        private void fitAll()
        {
            bool anyValid = false;
            float x = 0f, y = 0f;
            FloatRect? boundingBox = null;
            lock (_window.Players)
            {
                foreach (var player in _window.Players)
                {
                    if (player.Visible && player.ValidPosition)
                    {
                        var pos = player.GetConvertedPosition();
                        anyValid = true;
                        x += pos.X;
                        y += pos.Y;
                        if (boundingBox.HasValue)
                            boundingBox = boundingBox.Value.MaxBounds(new Vector2f(pos.X, pos.Y));
                        else
                            boundingBox = new FloatRect(x, y, 0f, 0f);
                    }
                }
            }
            if (anyValid && boundingBox.HasValue)
            {
                x = boundingBox.Value.Left + boundingBox.Value.Width * 0.5f;
                y = boundingBox.Value.Top + boundingBox.Value.Height * 0.5f;
                boundingBox = boundingBox.Value.Extrude(Math.Max(boundingBox.Value.Width, boundingBox.Value.Height) * 0.1f);
                _camera.Position = new Vector2f(x, y);
                var zoom = Math.Max(1f, Math.Max(boundingBox.Value.Width / _camera.Size.X, boundingBox.Value.Height / _camera.Size.Y));
                setZoom(zoom);
            }
            else
            {
                return;
            }
        }

        private void listenToWindowEvents(MapWindow window)
        {
            window.Resized += onWindowResized;
            window.InputHandler.ActionJustPressed += inputHandler_ActionPressed;
            window.InputHandler.FollowPlayerPressed += inputHandler_FollowPlayerPressed;
            window.MouseMoved += onMouseMoved;
            window.Closed += onWindowClosed;
        }

        private void unlistenToWindowEvents(MapWindow window)
        {
            window.Resized -= onWindowResized;
            window.InputHandler.ActionJustPressed -= inputHandler_ActionPressed;
            window.InputHandler.FollowPlayerPressed -= inputHandler_FollowPlayerPressed;
            window.MouseMoved -= onMouseMoved;
            window.Closed -= onWindowClosed;
        }

        private void onWindowResized(object? sender, SizeEventArgs e)
        {
            updateCameraSize();
        }

        private void inputHandler_ActionPressed(object? sender, UIActionEvent e)
        {
            switch (e.Action)
            {
                case UIActions.FitAllPlayers:
                    followPlayer(null);
                    break;

                case UIActions.ZoomIn:
                case UIActions.ZoomOut:
                    if (_window.InputHandler.GetFramesHeld(UIActions.MoveMap) > 0)
                        break;
                    var zoomChange = _userZoom * 0.12f;
                    if (e.Action == UIActions.ZoomIn)
                    {
                        _userZoom = Math.Max(0.5f, _userZoom - zoomChange);
                        _camera.Zoom = getZoom();
                    }
                    else if (e.Action == UIActions.ZoomOut)
                    {
                        _userZoom = Math.Max(0.5f, _userZoom + zoomChange);
                        _camera.Zoom = getZoom();
                    }
                    break;

                case UIActions.MoveMap:
                    if (e.MousePosition.HasValue)
                    {
                        _lastMouseWorldPosition = screenToWorldCoordinates(e.MousePosition.Value);
                        CameraMode = CameraMode.FreeCam;
                    }
                    break;
            }
        }

        private void inputHandler_FollowPlayerPressed(object? sender, FollowPlayerEvent e)
        {
            if (e.PlayerIndex >= 0)
                followPlayer(e.PlayerIndex);
        }

        private void updateCameraSize()
        {
            var factor = Math.Max(MapViewportWidth / _window.Size.X, MapViewportHeight / _window.Size.Y);
            _camera.Size = new Vector2f(_window.Size.X * factor, _window.Size.Y * factor);
        }

        private void followPlayer(int? i)
        {
            if (i.HasValue)
            {
                lock (_window.Players)
                {
                    if (i >= 0 && i < _window.Players.Count)
                    {
                        CameraFollowTarget = _window.Players[i.Value];
                    }
                }
            }
            else
            {
                CameraFollowTarget = null;
                CameraMode = CameraMode.FitAll;
            }
        }

        private void onMouseMoved(object? sender, MouseMoveEventArgs e)
        {
            var pos = screenToWorldCoordinates(new Vector2i(e.X, e.Y));

            if (Enabled && _window.InputHandler.GetFramesHeld(UIActions.MoveMap) > 0 && CameraMode == CameraMode.FreeCam)
            {
                var diff = _lastMouseWorldPosition - pos;
                _camera.Position += diff;
                if (_camera is LerpCamera lerp)
                    lerp.Snap();
            }
        }

        private void setZoom(float val)
        {
            _lastZoom = val;
            _camera.Zoom = getZoom();
        }

        private float getZoom()
        {
            return _lastZoom * (CameraMode == CameraMode.FitAll ? Math.Max(1.0f, _userZoom) : _userZoom);
        }

        private void onWindowClosed(object? sender, EventArgs e)
        {
            if (sender is MapWindow window)
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