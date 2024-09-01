using EldenBingo.GameInterop;
using EldenBingo.Rendering.Game;
using EldenBingoCommon;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.ComponentModel;

namespace EldenBingo.Rendering
{
    public enum ToolMode
    {
        Draw,
        Erase
    }

    public class MapWindow : SimpleGameWindow
    {
        public static readonly Vector2f MapSize = new Vector2f(9645f, 9119f);
        public static readonly Vector2f DlcMapSize = new Vector2f(4879f, 5940f);
        private readonly Vector2f DlcMapOffsetTop = new Vector2f(3035f, 1864f);
        public static readonly RectangleF RoundTableRectangle = new RectangleF(2740f, 7510f, 200f, 200f);
        private const uint MapWindowDefaultWidth = 640, MapWindowDefaultHeight = 640;

        private static readonly SFML.Graphics.Font Font;
        private readonly ISet<Guid> _guids;
        private EldenRingMapDrawable _map;
        private EldenRingMapDrawable _dlcMap;
        private RoundTableDrawable _roundTable;
        private CameraController _cameraController;
        private LineLayer _lineLayer;
        private MapInstance _mapToShow = MapInstance.MainMap;

        //private RenderLayer _hudLayer;
        private EldenRingAvailableClassesDrawable? _availableClasses;

        private bool _showingClasses = false;

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

            Camera = new LerpCamera(new Vector2f(MapSize.X * 0.5f, MapSize.Y * 0.5f), new Vector2f(Size.X, Size.Y), 1f);
            Camera.MaxZoom = 13f;
            Camera.MinZoom = 0.4f;
            _cameraController = new CameraController(this, Camera);

            AddGameObject(_cameraController);

            _map = new EldenRingMapDrawable(MapInstance.MainMap, @"./Textures/Map", MapSize, new Vector2f(0, 0));
            AddGameObject(_map);

            _dlcMap = new EldenRingMapDrawable(MapInstance.DLC, @"./Textures/Map/DLC", DlcMapSize, DlcMapOffsetTop);
            AddGameObject(_dlcMap);

            _roundTable = new RoundTableDrawable(this);
            AddGameObject(_roundTable);

            _lineLayer = new LineLayer(this);
            AddGameObject(_lineLayer);

            Properties.Settings.Default.PropertyChanged += default_PropertyChanged;

            if (Properties.Settings.Default.ShowClassesOnMap)
            {
                initClassesDrawable();
            }
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

        public void ShowAvailableClasses(EldenRingClasses[] classes)
        {
            if (_availableClasses == null)
            {
                initClassesDrawable();
            }
            _availableClasses.SetAvailableClasses(classes);
            showAvailableClasses(classes.Length > 0);
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

        private void initClassesDrawable()
        {
            _availableClasses = new EldenRingAvailableClassesDrawable(this) { Visible = false };
            AddGameObject(_availableClasses);
            if (Running) //Init the textures if this window is already running
                _availableClasses.Init();
            showAvailableClasses(false);
        }

        private void updateMapInstanceToShow()
        {
            if (_cameraController.CameraMode == CameraMode.FitAll)
            {
                int validPlayersInWorld = Players.Count(p => p.ValidPosition && p.MapInstance == MapInstance.MainMap);
                int validPlayersInDlc = Players.Count(p => p.ValidPosition && p.MapInstance == MapInstance.DLC);
                if (validPlayersInWorld > validPlayersInDlc)
                {
                    _mapToShow = MapInstance.MainMap;
                }
                else if (validPlayersInDlc > validPlayersInWorld)
                {
                    _mapToShow = MapInstance.DLC;
                }
            }
            if (_cameraController.CameraMode == CameraMode.FollowTarget && _cameraController.CameraFollowTarget != null && _cameraController.CameraFollowTarget.ValidPosition)
            {
                _mapToShow = _cameraController.CameraFollowTarget.MapInstance;
            }
        }

        private void updateDrawableVisibility()
        {
            foreach(var p in Players)
            {
                p.Visible = p.MapInstance == _mapToShow;
            }
            _roundTable.Visible = _mapToShow == MapInstance.MainMap;
            _map.Visible = _mapToShow == MapInstance.MainMap;
            _dlcMap.Visible = _mapToShow == MapInstance.DLC;
        }

        private void default_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Properties.Settings.ShowClassesOnMap) && Properties.Settings.Default.ShowClassesOnMap && _availableClasses == null)
            {
                initClassesDrawable();
            }
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
            updateMapInstanceToShow();
            updateDrawableVisibility();
            SetView(Camera.GetView());
        }

        private void onAfterDraw(object? sender, EventArgs e)
        {
            //Draw available classes last
            if (_availableClasses != null && _showingClasses)
            {
                Draw(_availableClasses);
            }
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
            if (e.Code == Keyboard.Key.Space || e.Code == Keyboard.Key.Escape)
            {
                showAvailableClasses(false);
            }
            if (e.Code == Keyboard.Key.X)
            {
                _mapToShow = 1 - _mapToShow;
                //If following a target that's not on the map we switched to, cancel the following
                if(_cameraController.CameraMode == CameraMode.FollowTarget && 
                    _cameraController.CameraFollowTarget != null && 
                    _cameraController.CameraFollowTarget.MapInstance != _mapToShow)
                    _cameraController.CameraMode = CameraMode.FreeCam;
            }
        }

        private void showAvailableClasses(bool vis)
        {
            _showingClasses = vis;
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
                showAvailableClasses(false);
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