using EldenBingo.GameInterop;
using SFML.Graphics;
using SFML.System;

namespace EldenBingo.Rendering.Game
{
    public class PlayerDrawable : IDrawable, IUpdateable
    {
        private static readonly Sprite _playerArrowSprite;
        private static readonly Sprite _playerIconSprite;
        private static readonly Shader? _shader;
        private static readonly SFML.Graphics.Font Font;

        private float _targetX, _targetY, _targetAngle, _previousX, _previousY, _previousAngle, _interpTime, _angleDiff;
        private float _timeLeftToInterpolate = 0f;

        private MapWindow _window;
        private ICoordinateProvider _coordinateProvider;
        private RoundTableDrawable _roundTable;

        static PlayerDrawable()
        {
            Font = new SFML.Graphics.Font("LibraSans.ttf");

            var pTex = new Texture("./Textures/player.png");
            pTex.Smooth = true;
            _playerArrowSprite = new Sprite(pTex);
            _playerArrowSprite.Origin = new Vector2f(33f, 86f);
            var pIconTex = new Texture("./Textures/player-icon.png");
            pIconTex.Smooth = true;
            _playerIconSprite = new Sprite(pIconTex);
            _playerIconSprite.Origin = new Vector2f(34f, 34f);

            try
            {
                _shader = SpriteShader.Create();
            }
            catch (Exception)
            {
                _shader = null;
            }
        }

        public PlayerDrawable(MapWindow window, ICoordinateProvider provider, RoundTableDrawable roundTable)
        {
            _window = window;
            _coordinateProvider = provider;
            _roundTable = roundTable;
            if (provider.MapCoordinates.HasValue)
            {
                var c = provider.MapCoordinates.Value;
                setNewTarget(c.X, c.Y, c.Angle, c.IsUnderground, 0f);
            }
            else
            {
                ValidPosition = false;
            }
            Name = provider.Name;
            if (!string.IsNullOrWhiteSpace(Name))
                NameTag = new Text(Name, Font, 26) { OutlineColor = SFML.Graphics.Color.Black, OutlineThickness = 2f };
        }

        public bool Enabled { get; set; } = true;
        public bool Visible { get; set; } = true;
        public Guid Guid => _coordinateProvider.Guid;

        public float Angle { get; private set; }
        public string Name { get; private set; }
        public Text? NameTag { get; private set; } = null;
        public bool Underground { get; private set; }
        public bool ValidPosition { get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }

        public static void DisposeStatic()
        {
            _playerArrowSprite.Dispose();
            _playerIconSprite.Dispose();
            _shader?.Dispose();
        }

        public void Update(float dt)
        {
            if (_coordinateProvider.Changed)
            {
                var newCoords = _coordinateProvider.MapCoordinates;
                if (newCoords.HasValue && newCoords.Value.X > 0 && newCoords.Value.Y > 0)
                {
                    setNewTarget(newCoords.Value.X, newCoords.Value.Y, newCoords.Value.Angle, newCoords.Value.IsUnderground, 0.1f);
                }
                else
                {
                    ValidPosition = false;
                }
            }
            _timeLeftToInterpolate = Math.Max(0, _timeLeftToInterpolate - dt);
            if (_interpTime > 0 && _timeLeftToInterpolate > 0)
            {
                var frac = _timeLeftToInterpolate / _interpTime;
                X = frac * _previousX + (1f - frac) * _targetX;
                Y = frac * _previousY + (1f - frac) * _targetY;
                Angle = _previousAngle + _angleDiff * (1 - frac);
            }
            else
            {
                X = _targetX;
                Y = _targetY;
                Angle = _targetAngle;
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            if (!ValidPosition)
                return;

            var pos = GetConvertedPosition();

            var trans = Transform.Identity;
            trans.Translate(pos);
            trans.Rotate(Angle);
            //Initial scale, scaled up to match window size
            var scale = 0.38f;
            //Scale map markers to the new scale
            scale *= (float)Math.Pow(_window.Camera.Zoom, 0.85);
            trans.Scale(scale, scale);
            var st = new RenderStates(BlendMode.Alpha, trans, _playerArrowSprite.Texture, _shader);

            var color = new SFML.Graphics.Glsl.Vec4(_coordinateProvider.Color.R, _coordinateProvider.Color.G, _coordinateProvider.Color.B, _coordinateProvider.Color.A);
            _shader?.SetUniform("alpha", Underground ? 0.4f : 1.0f);
            _shader?.SetUniform("tint", color);

            _window.Draw(_playerArrowSprite, st);

            if (_coordinateProvider is MapCoordinateProviderHandler.LocalCoordinateProvider)
            {
                trans = Transform.Identity;
                trans.Translate(pos);
                trans.Scale(scale, scale);
                st = new RenderStates(BlendMode.Alpha, trans, _playerIconSprite.Texture, _shader);
                _window.Draw(_playerIconSprite, st);
            }

            if (_window.ShowPlayerNames && NameTag != null)
            {
                var st2 = RenderStates.Default;
                var trans2 = Transform.Identity;
                var scale2 = 0.5f * _window.Camera.Zoom;
                var bounds = NameTag.GetLocalBounds();
                var width = bounds.Width * scale2;
                var height = 15 * scale2;
                trans2.Translate(new Vector2f(pos.X - width * 0.5f, pos.Y + height));
                trans2.Scale(scale2, scale2);
                st2.Transform = trans2;
                _window.Draw(NameTag, st2);
            }
        }

        public Vector2f GetConvertedPosition()
        {
            var pos = new Vector2f(X, Y);
            if (MapWindow.RoundTableRectangle.Contains(new PointF(X, Y)))
            {
                return _roundTable.Position;
            }
            return pos;
        }

        public void Dispose()
        {
            NameTag?.Dispose();
        }

        private void setNewTarget(float x, float y, float angle, bool underground, float interpolationTime)
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