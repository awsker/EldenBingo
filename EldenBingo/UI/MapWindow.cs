using EldenBingo.GameInterop;
using EldenBingo.Rendering;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace EldenBingo.UI
{
    internal class MapWindow
    {
        private RenderWindow? _window;
        private Thread? _renderThread;
        private bool _running;
        private readonly object _renderLock = new object();

        private Vector2u? _initSize;
        private Vector2i? _initPosition;

        private float _userZoom = 1.0f;

        private readonly IDictionary<Guid, CoordinateEntity> _coordinateEntities;
        private readonly IList<Guid> _guids;

        private static bool _texturesLoaded;
        private static TextureData[,]? _textureData;
        private static Sprite _player;
        private static Sprite _playerIcon;
        private static Shader? _shader;
        private static bool _disposeTexturesAfterThreadCompletes = false;
        private static int _imageMapWidth, _imageMapHeight;

        const int MapWindowDefaultWidth = 640, MapWindowDefaultHeight = 640;
        const int MapSizeNativeX = 550, MapSizeNativeY = 550;
        const float FullMapWidth = 9645f, FullMapHeight = 9119f;

        private static readonly SFML.Graphics.Font Font;

        public bool ShowPlayerNames { get; set; } = true;

        private ICamera? _camera;
        private Guid? _cameraTarget;

        //Init static variables
        static MapWindow()
        {
            Font = new SFML.Graphics.Font("LibraSans.ttf");
        }

        /// <summary>
        /// Guid of the Coordinate entity (player) that the camera should follow. Or null to try to fit everyone on screen
        /// </summary>
        public Guid? CameraFollowTarget
        {
            get { return _cameraTarget; }
            set
            {
                if (_cameraTarget != value)
                {
                    _cameraTarget = value;
                    if (_camera is LerpCamera lerp)
                    {
                        lerp.Snap();
                    }
                }
            }
        }
        private float _zoomDueToWindowSize;

        public MapWindow()
        {
            _coordinateEntities = new Dictionary<Guid, CoordinateEntity>();
            _guids = new List<Guid>();
            _camera = null;
        }

        public Vector2u Size
        {
            get
            {
                return _window?.Size ?? _initSize.GetValueOrDefault();
            }
            set
            {
                if (_window == null)
                    _initSize = value;
                else
                    _window.Size = value;
            }
        }

        public Vector2i Position
        {
            get
            {
                return _window?.Position ?? _initPosition.GetValueOrDefault();
            }
            set
            {
                if (_window == null)
                    _initPosition = value;
                else
                    _window.Position = value;
            }
        }

        public void Show()
        {
            _renderThread = new Thread(renderLoop);
            _renderThread.Start();
        }

        public void AddCoordinateProvider(ICoordinateProvider p)
        {
            _coordinateEntities[p.Guid] = new CoordinateEntity(p) { NameTag = initPlayerNameTag(p.Name) };
            _guids.Add(p.Guid);
        }

        public void RemoveCoordinateProvider(Guid g)
        {
            _coordinateEntities.Remove(g);
            _guids.Remove(g);
        }
        
        public void Stop()
        {
            _running = false;
        }

        private void renderLoop()
        {
            _running = true;
            _texturesLoaded = false;

            if (_window == null)
                initWindow();
            if(!_texturesLoaded)
                initAllTextures();

            //InitCamera must be done after textures are initialized because it needs
            //the size of the map images to calculate zoom factors
            initCamera(_window);

            if (_window == null)
                throw new Exception("Window not created");
            if (_textureData == null)
                throw new Exception("Textures not loaded");
            try
            {
                Clock clock = new Clock();
                while (_running && _window.IsOpen)
                {
                    _window.DispatchEvents();
                    Time elapsed = clock.Restart();

                    update(elapsed.AsSeconds());
                    
                    lock (_renderLock)
                    {
                        //Clear window with black
                        _window.Clear(SFML.Graphics.Color.Black);
                        SFML.Graphics.View view;
                        if (_camera == null)
                            view = _window.GetView();
                        else
                        {
                            view = _camera.GetView();
                            _window.SetView(view);
                        }
                        //Get the view bounds of the current view
                        var viewBounds = getViewBounds(view);

                        if (_texturesLoaded)
                        {
                            //Draw part of the map that is inside the view bounds
                            drawMap(viewBounds);

                            //Draw all players
                            drawPlayers();
                        }
                        _window.Display();
                    }
                }
            } 
            finally
            {
                if (_disposeTexturesAfterThreadCompletes)
                    disposeStaticTextureData();
            }
        }

        private void update(float dt)
        {
            updateCamera(dt);

            foreach (var ent in _coordinateEntities.Values)
            {
                lock (ent.CoordinateProvider)
                {
                    if (ent.CoordinateProvider.Changed)
                    {
                        var newCoords = ent.CoordinateProvider.MapCoordinates;
                        if (newCoords.HasValue && newCoords.Value.X > 0 && newCoords.Value.Y > 0)
                        {
                            ent.SetNewTarget(newCoords.Value.X, newCoords.Value.Y, newCoords.Value.Angle, newCoords.Value.IsUnderground, 0.1f);
                        } 
                        else
                        {
                            ent.SetNewTargetInvalid();
                        }
                    }
                    ent.Update(dt);
                }
            }
        }
        
        private void updateCamera(float dt)
        {
            if (_camera == null)
                return;

            _camera.Update(dt);

            //Midpoint coordinates
            float x = 0f, y = 0f;
            FloatRect? boundingBox = null;
            if (CameraFollowTarget.HasValue)
            {
                if (_coordinateEntities.TryGetValue(CameraFollowTarget.Value, out var ent) && ent.ValidPosition)
                {
                    x = ent.X;
                    y = ent.Y;
                    boundingBox = new FloatRect(x, y, 0f, 0f);
                } 
                else
                {
                    return;
                }
            }
            else 
            {
                bool anyValid = false;
                foreach (var ent in _coordinateEntities.Values)
                {
                    if (ent.ValidPosition)
                    {
                        anyValid = true;
                        x += ent.X;
                        y += ent.Y;
                        if (boundingBox.HasValue)
                            boundingBox = boundingBox.Value.MaxBounds(new Vector2f(ent.X, ent.Y));
                        else
                            boundingBox = new FloatRect(x, y, 0f, 0f);
                    }
                }
                if (anyValid && boundingBox.HasValue)
                {
                    x = boundingBox.Value.Left + boundingBox.Value.Width * 0.5f;
                    y = boundingBox.Value.Top + boundingBox.Value.Height * 0.5f;
                }
                else
                {
                    return;
                }
            }
            float zoom = _zoomDueToWindowSize;
            if (boundingBox.HasValue)
            {
                var tempZoom = Math.Max(_zoomDueToWindowSize, Math.Max((float)boundingBox.Value.Width / _window.Size.X, (float)boundingBox.Value.Height / _window.Size.Y));
                boundingBox = boundingBox.Value.Extrude(100f * tempZoom); //Extend the bounding box 100 map units in all directions, so the players aren't drawn at the window edge
                zoom = Math.Max(_zoomDueToWindowSize, Math.Max((float)boundingBox.Value.Width / _window.Size.X, (float)boundingBox.Value.Height / _window.Size.Y));
            }
            var imageScaleFactors = getMapScaleFactors(); //Scale camera position with map image size, in case user has a lower resolution map
            _camera.Position = new Vector2f(x * imageScaleFactors.X, y * imageScaleFactors.Y);
            _camera.Zoom = zoom * _userZoom; 
        }

        private void drawMap(FloatRect viewBounds)
        {
            foreach (var texData in _textureData)
            {
                if (texData.Sprite.GetGlobalBounds().Intersects(viewBounds))
                    _window.Draw(texData.Sprite);
            }
        }

        private void drawPlayers()
        {
            var imageFactor = getMapScaleFactors();

            foreach (var ent in _coordinateEntities.Values)
            {
                if (!ent.ValidPosition)
                    continue;

                if (ShowPlayerNames && ent.NameTag != null)
                {
                    var st2 = RenderStates.Default;
                    var trans2 = Transform.Identity;
                    var scale2 = 0.5f * _camera.Zoom / _zoomDueToWindowSize;
                    var bounds = ent.NameTag.GetLocalBounds();
                    var width = bounds.Width * scale2;
                    var height = 15 * scale2;
                    trans2.Translate(new Vector2f(ent.X * imageFactor.X - width * 0.5f, ent.Y * imageFactor.Y + height));
                    trans2.Scale(scale2, scale2);
                    st2.Transform = trans2;
                    _window.Draw(ent.NameTag, st2);
                }
                var trans = Transform.Identity;
                trans.Translate(new Vector2f(ent.X * imageFactor.X, ent.Y * imageFactor.Y));
                trans.Rotate(ent.Angle);
                //Initial scale, scaled up to match window size
                var scale = 0.38f / _zoomDueToWindowSize;
                if (_camera != null)
                {
                    //Scale map markers to the new scale
                    scale *= (float)Math.Pow(_camera.Zoom, 0.75);
                }
                trans.Scale(scale, scale);
                var st = new RenderStates(BlendMode.Alpha, trans, _player.Texture, _shader);

                var color = new SFML.Graphics.Glsl.Vec4(ent.CoordinateProvider.Color.R, ent.CoordinateProvider.Color.G, ent.CoordinateProvider.Color.B, ent.CoordinateProvider.Color.A);
                _shader?.SetUniform("alpha", ent.Underground ? 0.4f : 1.0f);
                _shader?.SetUniform("tint", color);

                _window.Draw(_player, st);

                if(ent.CoordinateProvider is GameInterop.MapCoordinateProviderHandler.LocalCoordinateProvider)
                {
                    trans = Transform.Identity;
                    trans.Translate(new Vector2f(ent.X * imageFactor.X, ent.Y * imageFactor.Y));
                    trans.Scale(scale, scale);
                    st = new RenderStates(BlendMode.Alpha, trans, _playerIcon.Texture, _shader);
                    _window.Draw(_playerIcon, st);
                }
            }
        }

        private Vector2f getMapScaleFactors()
        {
            return new Vector2f(_imageMapWidth / FullMapWidth, _imageMapHeight / FullMapHeight);
        }

        private FloatRect getViewBounds(SFML.Graphics.View view)
        {
            FloatRect rt;
            rt.Left = view.Center.X - view.Size.X * 0.5f;
            rt.Top = view.Center.Y - view.Size.Y * 0.5f;
            rt.Width = view.Size.X;
            rt.Height = view.Size.Y;
            return rt;
        }

        private void initWindow()
        {
            if (_window != null)
            {
                unlistenWindowEvents(_window);
            }
            _window = new RenderWindow(new VideoMode(_initSize?.X ?? MapWindowDefaultWidth, _initSize?.Y ?? MapWindowDefaultHeight), "Map View", Styles.Default);
            if (_initPosition.HasValue)
                _window.Position = _initPosition.Value;
            _window.SetVerticalSyncEnabled(true);
            _window.SetFramerateLimit(60);
            listenWindowEvents(_window);
        }

        private void initCamera(Window window)
        {
            var imageScale = getMapScaleFactors();

            var previousZoom = _zoomDueToWindowSize;
            _zoomDueToWindowSize = Math.Max((float)MapSizeNativeX / window.Size.X, (float)MapSizeNativeY / window.Size.Y);

            //If no previous camera set, start camera at middle of map
            if (_camera == null)
            {
                _camera = new LerpCamera(new Vector2f(FullMapWidth * 0.5f * imageScale.X, FullMapHeight * 0.5f * imageScale.Y), new Vector2f(window.Size.X, window.Size.Y), 1.0f);
            }
            //Copy position and zoom from previous camera
            else
            {
                
                _camera = new LerpCamera(_camera.Position, new Vector2f(window.Size.X, window.Size.Y), _camera.Zoom * _zoomDueToWindowSize / previousZoom);
            }
        }

        private void listenWindowEvents(RenderWindow window)
        {
            window.Closed += onWindowClosed;
            window.Resized += onWindowResized;
            window.KeyPressed += onWindowKeyPressed;
            window.MouseWheelScrolled += onMouseWheelScrolled;
            window.MouseButtonPressed += onMouseButtonPressed;
        }

        private void unlistenWindowEvents(RenderWindow window)
        {
            window.Closed -= onWindowClosed;
            window.Resized -= onWindowResized;
            window.KeyPressed -= onWindowKeyPressed;
            window.MouseWheelScrolled -= onMouseWheelScrolled;
            window.MouseButtonPressed -= onMouseButtonPressed;
        }

        private void onWindowClosed(object? sender, EventArgs e)
        {
            if (sender is RenderWindow w)
            {
                unlistenWindowEvents(w);
                w.Close();
                _running = false;
            }
        }

        private void onWindowResized(object? sender, SizeEventArgs e)
        {
            initCamera(_window);
            Properties.Settings.Default.MapWindowWidth = (int)_window.Size.X;
            Properties.Settings.Default.MapWindowHeight = (int)_window.Size.Y;
            Properties.Settings.Default.Save();
        }

        private void onWindowKeyPressed(object? sender, SFML.Window.KeyEventArgs e)
        {
            if (_camera == null)
                return;


            if (e.Code == Keyboard.Key.N)
            {
                ShowPlayerNames = !ShowPlayerNames;
            }

            void followPlayer(int? i)
            {
                if(i.HasValue)
                {
                    if(i >= 0 && i < _guids.Count)
                    {
                        CameraFollowTarget = _guids[i.Value];
                    }
                }
                else
                {
                    CameraFollowTarget = null;
                }
            }

            if(e.Code == Keyboard.Key.Num0 || e.Code == Keyboard.Key.Numpad0)
            {
                followPlayer(null);
            }
            else if(e.Code >= Keyboard.Key.Num1 && e.Code <= Keyboard.Key.Num9) 
            {
                followPlayer(e.Code - Keyboard.Key.Num1);
            }

            if (e.Code >= Keyboard.Key.Numpad1 && e.Code <= Keyboard.Key.Numpad9)
            {
                followPlayer(e.Code - Keyboard.Key.Numpad1);
            }
        }

        private void onMouseWheelScrolled(object? sender, MouseWheelScrollEventArgs e)
        {
            var change = _userZoom * 0.12f;
            if (e.Delta > 0f)
                _userZoom = Math.Max(1f, _userZoom - change);
            if (e.Delta < 0f)
                _userZoom = Math.Min(10f, _userZoom + change);
        }

        private void onMouseButtonPressed(object? sender, MouseButtonEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void initAllTextures()
        {
            if (_window != null)
            {
                _window.Clear(new SFML.Graphics.Color(96, 96, 96));
                var text = new Text("Textures loading...", Font, 15);
                var viewSize = _window.GetView().Size;
                var textSize = text.GetGlobalBounds();
                text.Position = new Vector2f((viewSize.X - textSize.Width) * 0.5f, (viewSize.Y - textSize.Height) * 0.5f);
                _window.Draw(text);
                _window.Display();
            }
            initPlayerTexture();
            initMapTextures();
            _texturesLoaded = true;
        }

        private void initPlayerTexture()
        {
            var pTex = new Texture("./Textures/player.png");
            _player = new Sprite(pTex);
            _player.Origin = new Vector2f(33f, 86f);
            var pIconTex = new Texture("./Textures/player-icon.png");
            _playerIcon = new Sprite(pIconTex);
            _playerIcon.Origin = new Vector2f(34f, 34f);
            try
            {
                _shader = SpriteShader.Create();
            }
            catch (Exception)
            {
                _shader = null;
            }
            
        }

        private Text initPlayerNameTag(string name)
        {
            return new Text(name, Font, 30) { OutlineColor = SFML.Graphics.Color.Black, OutlineThickness = 2f };
        }

        private void initMapTextures()
        {
            _textureData = new TextureData[10, 9];
            const string mapPath = "./Textures/Map/";
            var images = Directory.GetFiles(mapPath);
            for (int i = 0; i < images.Length; ++i)
            {
                var image = images[i];
                var x = int.Parse(image.Substring(mapPath.Length + 0, 2));
                var y = int.Parse(image.Substring(mapPath.Length + 3, 2));
                JPEGPicture pic = new JPEGPicture();
                pic.Data = pic.ImageToByteArray(image);
                pic.GetJPEGSize();
                var tex = new Texture(image) { Smooth = true };
                tex.GenerateMipmap();
                _textureData[x, y] = new TextureData(x, y, pic.Width, pic.Height, image, tex);
            }
            int currX = 0;
            int currY = 0;

            for (int x = 0; x < _textureData.GetLength(0); ++x)
            {
                currY = 0;
                for (int y = 0; y < _textureData.GetLength(1); ++y)
                {
                    _textureData[x, y].InitSpriteWithPosition(currX, currY);
                    currY += _textureData[x, y].Height;
                }
                currX += _textureData[x, 0].Width;
            }
            _imageMapWidth = currX;
            _imageMapHeight = currY;
        }

        public  void DisposeStaticTextureData()
        {
            _disposeTexturesAfterThreadCompletes = true;
        }

        private void disposeStaticTextureData()
        {
            if (_textureData == null)
                return;

            _player?.Dispose();
            _player?.Texture?.Dispose();

            foreach (var tex in _textureData)
            {
                tex?.Dispose();
            }
        }

        private class TextureData : IDisposable
        {
            public int GridX, GridY;
            public int Width, Height;
            public Vector2f Position;
            public string Filename;
            public Texture Texture;
            public Sprite Sprite;

            public TextureData(int x, int y, int w, int h, string fn, Texture texture)
            {
                GridX = x;
                GridY = y;
                Width = w;
                Height = h;
                Filename = fn;
                Texture = texture;
                Sprite = new Sprite(texture);
            }

            public void InitSpriteWithPosition(float worldXPos, float worldYPos) {
                Position = new Vector2f(worldXPos, worldYPos);
                Sprite.Position = Position;
            }

            public void Dispose()
            {
                Sprite?.Dispose();
                Texture?.Dispose();
            }
        }
        
        private class CoordinateEntity
        {
            public string Name { get; }
            public ICoordinateProvider CoordinateProvider { get; init; }
            public float X { get; private set; }
            public float Y { get; private set; }
            public float Angle { get; private set; }
            public bool Underground { get; private set; }
            public bool ValidPosition { get; private set; }

            private float _targetX, _targetY, _targetAngle, _previousX, _previousY, _previousAngle, _interpTime, _angleDiff;
            private float _timeLeftToInterpolate = 0f;

            public Text? NameTag { get; set; } = null;

            public CoordinateEntity(ICoordinateProvider provider)
            {
                CoordinateProvider = provider;
                if (provider.MapCoordinates.HasValue)
                {
                    var c = provider.MapCoordinates.Value;
                    SetNewTarget(c.X, c.Y, c.Angle, c.IsUnderground, 0f);
                }
                else
                {
                    ValidPosition = false;
                }
                Name = provider.Name;
            }

            public void Update(float dt)
            {
                _timeLeftToInterpolate = Math.Max(0, _timeLeftToInterpolate - dt);
                if (_interpTime > 0 && _timeLeftToInterpolate > 0)
                {
                    var frac = _timeLeftToInterpolate / _interpTime;
                    X = frac * _previousX + (1f - frac) * _targetX;
                    Y = frac * _previousY + (1f - frac) * _targetY;
                    Angle = _previousAngle + _angleDiff * (1-frac);
                } 
                else
                {
                    X = _targetX;
                    Y = _targetY;
                    Angle = _targetAngle;
                }
            }

            public void SetNewTargetInvalid()
            {
                ValidPosition = false;
            }

            public void SetNewTarget(float x, float y, float angle, bool underground, float interpolationTime)
            {
                ValidPosition = true;
                _previousX = X;
                _previousY = Y;
                _previousAngle = Angle;
                _targetX = x;
                _targetY = y;
                _targetAngle = convertAngle(angle);
                if (dist(_previousX, _previousY, _targetX, _targetY) > 10)
                {
                    //Distance > 10, Teleport instead of interpolate
                    _interpTime = 0;
                    _timeLeftToInterpolate = 0;
                }
                else
                {
                    _angleDiff = (_targetAngle - Angle + 540) % 360 - 180;
                    _interpTime = interpolationTime;
                    _timeLeftToInterpolate = interpolationTime;
                }
                Underground = underground;
            }

            private float convertAngle(float degreeAngle)
            {
                return degreeAngle;
            }

            private double dist(float x1, float y1, float x2, float y2)
            {
                return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
            }
        }
    }
}
