using EldenBingo.GameInterop;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace EldenBingo.Rendering
{
    public class MapWindow : SimpleGameWindow
    {
        public const float FullMapWidth = 9645f, FullMapHeight = 9119f;
        public static readonly RectangleF RoundTableRectangle = new RectangleF(2740f, 7510f, 200f, 200f);
        private const uint MapWindowDefaultWidth = 640, MapWindowDefaultHeight = 640;
        

        private EldenRingMapDrawable _map;
        private RoundTableDrawable _roundTable;
        private readonly ISet<Guid> _guids;
        public IList<PlayerDrawable> Players { get; init; }

        private static readonly SFML.Graphics.Font Font;

        public LerpCamera Camera { get; init; }
        private CameraController _cameraController;

        public static MapWindow? Instance { get; private set; }

        public bool ShowPlayerNames { get; set; } = true;

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

            Camera = new LerpCamera(new Vector2f(FullMapWidth * 0.5f, FullMapHeight * 0.5f), new Vector2f(Size.X, Size.Y), 1f);
            _cameraController = new CameraController(this, Camera);

            AddGameObject(_cameraController);
            loadMap();
        }

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

        public void DisposeStaticTextureData()
        {
            DisposeDrawables = true;
        }

        protected override void ListenToEvents()
        {
            base.ListenToEvents();
            InitializingDrawables += onInitializingDrawables;
            BeforeDraw += onBeforeDraw;
            Resized += onWindowResized;
            KeyPressed += onKeyPressed;
        }

        protected override void UnlistenToEvents()
        {
            base.ListenToEvents();
            InitializingDrawables -= onInitializingDrawables;
        }

        private void loadMap()
        {
            _map = new EldenRingMapDrawable();
            AddGameObject(_map);

            _roundTable = new RoundTableDrawable(this);
            AddGameObject(_roundTable);
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
            if (Camera.Changed)
                SetView(Camera.GetView());
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
        }
    }
}
