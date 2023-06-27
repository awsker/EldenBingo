using EldenBingo.GameInterop;
using EldenBingo.Rendering.Game;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace EldenBingo.Rendering
{
    public enum ToolMode
    {
        Draw,
        Erase
    }

    public class MapWindow : SimpleGameWindow
    {
        public const float FullMapWidth = 9645f, FullMapHeight = 9119f;
        public static readonly RectangleF RoundTableRectangle = new RectangleF(2740f, 7510f, 200f, 200f);
        private const uint MapWindowDefaultWidth = 640, MapWindowDefaultHeight = 640;

        private static readonly SFML.Graphics.Font Font;
        private readonly ISet<Guid> _guids;
        private EldenRingMapDrawable _map;
        private RoundTableDrawable _roundTable;
        private CameraController _cameraController;
        private LineLayer _lineLayer;
        private RenderLayer _hudLayer;

        static MapWindow()
        {
            Font = new SFML.Graphics.Font("LibraSans.ttf");
        }

        public MapWindow() : this(MapWindowDefaultWidth, MapWindowDefaultHeight)
        { }

        public MapWindow(uint width, uint height) : base("Map Window", width, height)
        {
            Instance = this;
            _guids = new HashSet<Guid>();

            Players = new List<PlayerDrawable>();
            InputHandler = new InputHandler(this);
            AddGameObject(InputHandler);

            Camera = new LerpCamera(new Vector2f(FullMapWidth * 0.5f, FullMapHeight * 0.5f), new Vector2f(Size.X, Size.Y), 1f);
            _cameraController = new CameraController(this, Camera);

            AddGameObject(_cameraController);

            _map = new EldenRingMapDrawable();
            AddGameObject(_map);

            _roundTable = new RoundTableDrawable(this);
            AddGameObject(_roundTable);

            _lineLayer = new LineLayer(this);
            AddGameObject(_lineLayer);

            _hudLayer = new RenderLayer(this);
            _hudLayer.Visible = false;
            AddGameObject(_hudLayer);
            /*
            var text = new TextDrawable("Test", Font);
            _hudLayer.CustomView = new SFML.Graphics.View(new FloatRect(0, 0, 400, 400));
            _hudLayer.AddGameObject(text);*/
        }

        public static MapWindow? Instance { get; private set; }
        public bool MouseLeftHeld { get; private set; }
        public bool MouseRightHeld { get; private set; }

        public InputHandler InputHandler { get; init; }
        public IList<PlayerDrawable> Players { get; init; }
        public LerpCamera Camera { get; init; }
        public bool ShowPlayerNames { get; set; } = true;
        public ToolMode ToolMode { get; set; }

        public void AddCoordinateProvider(ICoordinateProvider p)
        {
            if (!_guids.Contains(p.Guid))
            {
                var player = new PlayerDrawable(this, p, _roundTable);
                AddGameObject(player);
                _guids.Add(p.Guid);
                Players.Add(player);
            }
        }

        public void RemoveCoordinateProvider(Guid g)
        {
            if (_guids.Contains(g))
            {
                for (int i = 0; i < Players.Count; ++i)
                {
                    var p = Players[i];
                    if (p.Guid == g)
                    {
                        RemoveGameObject(p);
                        if (_cameraController.CameraFollowTarget == p)
                        {
                            _cameraController.CameraMode = CameraMode.FitAll;
                        }

                        Players.RemoveAt(i);
                        break;
                    }
                }
            }
            _guids.Remove(g);
        }

        protected override void ListenToEvents()
        {
            base.ListenToEvents();
            InitializingDrawables += onInitializingDrawables;
            BeforeDraw += onBeforeDraw;
            AfterDraw += onAfterDraw;
            Resized += onWindowResized;
            KeyPressed += onKeyPressed;
            DisposingDrawables += onDisposingDrawables;
            MouseButtonPressed += onMousePressed;
            MouseButtonReleased += onMouseReleased;
        }

        protected override void UnlistenToEvents()
        {
            base.ListenToEvents();
            InitializingDrawables -= onInitializingDrawables;
            BeforeDraw -= onBeforeDraw;
            AfterDraw -= onAfterDraw;
            Resized -= onWindowResized;
            KeyPressed -= onKeyPressed;
            DisposingDrawables -= onDisposingDrawables;
            MouseButtonPressed -= onMousePressed;
            MouseButtonReleased -= onMouseReleased;
        }

        private void onInitializingDrawables(object? sender, EventArgs e)
        {
            Clear(new SFML.Graphics.Color(96, 96, 96));
            var text = new Text("Textures loading...", Font, 15);
            var viewSize = GetView().Size;
            var textSize = text.GetGlobalBounds();
            text.Position = new Vector2f((viewSize.X - textSize.Width) * 0.5f, (viewSize.Y - textSize.Height) * 0.5f);
            Draw(text);
            Display();
        }

        private void onBeforeDraw(object? sender, EventArgs e)
        {
            SetView(Camera.GetView());
        }

        private void onAfterDraw(object? sender, EventArgs e)
        {
            //Draw HUD last
            Draw(_hudLayer);
        }

        private void onWindowResized(object? sender, SizeEventArgs e)
        {
            Properties.Settings.Default.MapWindowLastWidth = (int)Size.X;
            Properties.Settings.Default.MapWindowLastHeight = (int)Size.Y;
            Properties.Settings.Default.Save();
        }

        private void onKeyPressed(object? sender, SFML.Window.KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.N)
            {
                ShowPlayerNames = !ShowPlayerNames;
            }
            if (e.Code == Keyboard.Key.Z)
            {
                _lineLayer.UndoLastLine();
            }
            if (e.Code == Keyboard.Key.C)
            {
                _lineLayer.ClearLines();
            }
        }

        private void onDisposingDrawables(object? sender, EventArgs e)
        {
            EldenRingMapDrawable.DisposeStatic();
            RoundTableDrawable.DisposeStatic();
            PlayerDrawable.DisposeStatic();
        }

        private void onMousePressed(object? sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
            {
                MouseLeftHeld = true;
            }
            if (e.Button == Mouse.Button.Right)
            {
                MouseRightHeld = false;
            }
        }

        private void onMouseReleased(object? sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
                MouseLeftHeld = false;
            if (e.Button == Mouse.Button.Right)
                MouseRightHeld = false;
        }
    }
}