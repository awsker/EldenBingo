﻿using EldenBingo.GameInterop;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static EldenBingo.GameInterop.MapCoordinateProviderHandler;

namespace EldenBingo.Rendering
{
    public class MapWindow2 : SimpleGameWindow
    {
        public const float FullMapWidth = 9645f, FullMapHeight = 9119f;
        public static readonly RectangleF RoundTableRectangle = new RectangleF(2740f, 7510f, 200f, 200f);
        private const uint MapWindowDefaultWidth = 640, MapWindowDefaultHeight = 640;
        private const float MapViewportWidth = 750f, MapViewportHeight = 750f;

        private EldenRingMapDrawable _map;
        private RoundTableDrawable _roundTable;
        private readonly IDictionary<Guid, PlayerDrawable> _players;
        private readonly IList<Guid> _guidsInOrder;

        private static readonly SFML.Graphics.Font Font;

        public LerpCamera Camera { get; init; }
        private CameraController _cameraController;

        public static MapWindow2? Instance { get; private set; }

        public bool ShowPlayerNames { get; set; } = true;

        static MapWindow2()
        {
            Font = new SFML.Graphics.Font("LibraSans.ttf");
        }

        public MapWindow2() : this(MapWindowDefaultWidth, MapWindowDefaultHeight)
        { }

        public MapWindow2(uint width, uint height) : base("Map Window", width, height)
        {
            Instance = this;
            _players = new Dictionary<Guid, PlayerDrawable>();
            _guidsInOrder = new List<Guid>();

            Camera = new LerpCamera(new Vector2f(FullMapWidth * 0.5f, FullMapHeight * 0.5f), new Vector2f(Size.X, Size.Y), 1f);
            _cameraController = new CameraController(this, Camera);

            AddGameObject(_cameraController);
            loadMap();
            updateCameraSize();

            var player = new MockUserCoordinateProvider("Asker", System.Drawing.Color.Red, new EldenBingoCommon.MapCoordinates(FullMapWidth * 0.5f, FullMapHeight * 0.5f, false, 15f));

            AddCoordinateProvider(player);
        }

        public void AddCoordinateProvider(ICoordinateProvider p)
        {
            if (!_players.ContainsKey(p.Guid))
            {
                var player = new PlayerDrawable(this, p, _roundTable);
                AddGameObject(player);
                _players[p.Guid] = player;
                _guidsInOrder.Add(p.Guid);
            }
        }

        public void RemoveCoordinateProvider(Guid g)
        {
            if (_players.TryGetValue(g, out var player))
            {
                RemoveGameObject(player);
                _players.Remove(g);
                _guidsInOrder.Remove(g);
            }
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
            updateCameraSize();

            Properties.Settings.Default.MapWindowLastWidth = (int)Size.X;
            Properties.Settings.Default.MapWindowLastHeight = (int)Size.Y;
            Properties.Settings.Default.Save();
        }

        private void updateCameraSize()
        {
            var factor = Math.Max(MapViewportWidth / Size.X, MapViewportHeight / Size.Y);
            Camera.Size = new Vector2f(Size.X * factor, Size.Y * factor);
        }

    }
}
