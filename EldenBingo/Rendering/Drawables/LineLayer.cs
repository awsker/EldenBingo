using SFML.Graphics;
using SFML.Graphics.Glsl;
using SFML.System;
using SFML.Window;

namespace EldenBingo.Rendering.Drawables
{
    public class LineLayer : RenderLayer
    {
        private bool _mouseLeftHeld;
        private bool _mouseRightHeld;
        private Vector2f _lastMouseWorldPosition;
        private MapWindow _mapWindow;

        private Line? _currentLine;
        private List<Line> _lines;

        public System.Drawing.Color DrawColor { get; set; } = System.Drawing.Color.White;

        public LineLayer(MapWindow window) : base(window)
        {
            _mapWindow = window;
            _lines = new List<Line>();
            Shader = OutlineShader.Create();
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            var zoom = Math.Pow(_mapWindow.Camera.Zoom, 0.75);
            foreach(var line in GameObjects.OfType<Line>())
            {
                line.Width = 5f * (float)zoom;
            }
            base.Draw(target, states);
        }

        protected override void ListenToWindowEvents()
        {
            base.ListenToWindowEvents();
            Window.BeforeDraw += window_BeforeDraw;
            Window.MouseButtonPressed += onMousePressed;
            Window.MouseButtonReleased += onMouseReleased;
            Window.MouseMoved += onMouseMoved;
        }

        protected override void UnlistenToWindowEvents()
        {
            base.UnlistenToWindowEvents();
            Window.BeforeDraw -= window_BeforeDraw;
            Window.MouseButtonPressed -= onMousePressed;
            Window.MouseButtonReleased -= onMouseReleased;
            Window.MouseMoved -= onMouseMoved;
        }

        private void window_BeforeDraw(object? sender, EventArgs e)
        {
            Shader?.SetUniform("outlinewidth", 3);
            Shader?.SetUniform("outlinecolor", new Vec4(0f, 0f, 0f, 1f));
        }

        private void onMousePressed(object? sender, MouseButtonEventArgs e)
        {
            _lastMouseWorldPosition = screenToWorldCoordinates(new Vector2i(e.X, e.Y));

            if (e.Button == Mouse.Button.Left)
            {
                _mouseLeftHeld = true;
                if (Enabled && _mapWindow.ToolMode == ToolMode.Draw)
                {
                    if(_currentLine == null)
                    {
                        _currentLine = new Line(DrawColor, _lastMouseWorldPosition);
                        _lines.Add(_currentLine);
                        AddGameObject(_currentLine);
                    }
                }
            }
            if (e.Button == Mouse.Button.Right)
            {
                _mouseRightHeld = true;
            }
        }

        private void onMouseReleased(object? sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
            {
                _mouseLeftHeld = false;
                _currentLine = null;
            }
            if (e.Button == Mouse.Button.Right)
                _mouseRightHeld = false;
        }

        private void onMouseMoved(object? sender, MouseMoveEventArgs e)
        {
            var pos = screenToWorldCoordinates(new Vector2i(e.X, e.Y));

            if (Enabled && _mapWindow.ToolMode == ToolMode.Draw && _mouseLeftHeld && _currentLine != null)
            {
                _currentLine.AddPoint(pos);
            }
        }

        private Vector2f screenToWorldCoordinates(Vector2i screenCoords)
        {
            return Window.MapPixelToCoords(screenCoords);
        }

        public void UndoLastLine()
        {
            if (_lines.Count == 0)
                return;

            var line = _lines[_lines.Count - 1];
            if (_currentLine != null)
                _currentLine = null;

            _lines.RemoveAt(_lines.Count - 1);
            RemoveGameObject(line);
            line.Dispose();
        }

        public void ClearLines()
        {
            foreach (var line in _lines)
            {
                RemoveGameObject(line);
                line.Dispose();
            }
        }

    }
}
