using SFML.System;
using SFML.Window;

namespace EldenBingo.Rendering.Game
{
    public enum UIActions
    {
        MoveMap,
        Draw,
        ZoomIn,
        ZoomOut,
        FitAllPlayers,
        FollowPlayer,
        UndoDrawing,
        ClearDrawing
    }

    public class InputHandler : IUpdateable
    {
        private readonly MapWindow _window;

        private IDictionary<UIActions, uint> _actionsHeld;

        public InputHandler(MapWindow window)
        {
            _window = window;
            _actionsHeld = new Dictionary<UIActions, uint>();
            listenToWindowEvents();
        }

        public event EventHandler<UIActionEvent>? ActionJustPressed;

        public event EventHandler<UIActionEvent>? ActionJustReleased;

        public event EventHandler<FollowPlayerEvent>? FollowPlayerPressed;

        public bool Enabled => true;

        public void Update(float dt)
        {
            foreach (var key in _actionsHeld.Keys)
                _actionsHeld[key] += 1;
        }

        public uint GetFramesHeld(UIActions action)
        {
            if (_actionsHeld.TryGetValue(action, out var frames))
            {
                return Math.Max(1, frames);
            }
            return 0;
        }

        private void listenToWindowEvents()
        {
            _window.KeyPressed += window_onKeyPressed;
            _window.KeyReleased += window_onKeyReleased;
            _window.MouseButtonPressed += window_onMouseButtonPressed;
            _window.MouseButtonReleased += window_onMouseButtonReleased;
            _window.MouseWheelScrolled += window_onMouseScrolled;
        }

        private void unlistenToWindowEvents()
        {
            _window.KeyPressed -= window_onKeyPressed;
            _window.KeyReleased -= window_onKeyReleased;
        }

        private UIActions? getActionFromMouseButton(Mouse.Button key)
        {
            switch (key)
            {
                case Mouse.Button.Left:
                    return Properties.Settings.Default.FlipMouseButtons ? UIActions.Draw : UIActions.MoveMap;

                case Mouse.Button.Right:
                    return Properties.Settings.Default.FlipMouseButtons ? UIActions.MoveMap : UIActions.Draw;
            }
            return null;
        }

        private UIActions? getActionFromMouseWheel(float delta)
        {
            if (delta > 0) return UIActions.ZoomIn;
            if (delta < 0) return UIActions.ZoomOut;
            return null;
        }

        private UIActions? getActionFromKey(Keyboard.Key key, out int? pindex)
        {
            pindex = -1;
            switch (key)
            {
                case Keyboard.Key.Num0:
                case Keyboard.Key.Numpad0:
                    return UIActions.FitAllPlayers;

                case Keyboard.Key.Z:
                    return UIActions.UndoDrawing;

                case Keyboard.Key.C:
                    return UIActions.ClearDrawing;
            }
            if (key >= Keyboard.Key.Num1 && key <= Keyboard.Key.Num9)
            {
                pindex = key - Keyboard.Key.Num1;
                return UIActions.FollowPlayer;
            }

            if (key >= Keyboard.Key.Numpad1 && key <= Keyboard.Key.Numpad9)
            {
                pindex = key - Keyboard.Key.Numpad1;
                return UIActions.FollowPlayer;
            }
            return null;
        }

        private void window_onKeyPressed(object? sender, SFML.Window.KeyEventArgs e)
        {
            var action = getActionFromKey(e.Code, out var pindex);
            if (action.HasValue)
            {
                if (action == UIActions.FollowPlayer && pindex.HasValue)
                {
                    followPlayerDown(pindex.Value);
                }
                else
                {
                    actionDown(action.Value);
                }
            }
        }

        private void window_onKeyReleased(object? sender, SFML.Window.KeyEventArgs e)
        {
            var action = getActionFromKey(e.Code, out _);
            if (action.HasValue)
                actionUp(action.Value);
        }

        private void window_onMouseButtonPressed(object? sender, MouseButtonEventArgs e)
        {
            var action = getActionFromMouseButton(e.Button);
            if (action.HasValue)
                actionDown(action.Value, new Vector2i(e.X, e.Y));
        }

        private void window_onMouseButtonReleased(object? sender, MouseButtonEventArgs e)
        {
            var action = getActionFromMouseButton(e.Button);
            if (action.HasValue)
                actionUp(action.Value, new Vector2i(e.X, e.Y));
        }

        private void window_onMouseScrolled(object? sender, MouseWheelScrollEventArgs e)
        {
            var action = getActionFromMouseWheel(e.Delta);
            if (action.HasValue)
            {
                actionDown(action.Value, new Vector2i(e.X, e.Y));
                actionUp(action.Value, new Vector2i(e.X, e.Y));
            }
        }

        private void actionDown(UIActions action, Vector2i? mousePos = null)
        {
            if (!_actionsHeld.ContainsKey(action))
            {
                _actionsHeld[action] = 0;
                ActionJustPressed?.Invoke(this, new UIActionEvent(action, mousePosition: mousePos));
            }
        }

        private void actionUp(UIActions action, Vector2i? mousePos = null)
        {
            if (_actionsHeld.TryGetValue(action, out var frames))
            {
                _actionsHeld.Remove(action);
                ActionJustPressed?.Invoke(this, new UIActionEvent(action, Math.Max(1, frames), mousePosition: mousePos));
            }
        }

        private void followPlayerDown(int playerIndex)
        {
            FollowPlayerPressed?.Invoke(this, new FollowPlayerEvent(playerIndex));
        }
    }

    public class UIActionEvent : EventArgs
    {
        public UIActionEvent(UIActions action, uint framesHeld = 0, Vector2i? mousePosition = null)
        {
            Action = action;
            FramesHeld = framesHeld;
            MousePosition = mousePosition;
        }

        public UIActions Action { get; init; }
        public uint FramesHeld { get; init; }
        public Vector2i? MousePosition { get; init; }
    }

    public class FollowPlayerEvent : EventArgs
    {
        public FollowPlayerEvent(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }

        public int PlayerIndex { get; init; }
    }
}