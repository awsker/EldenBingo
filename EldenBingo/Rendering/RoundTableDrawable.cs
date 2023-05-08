using SFML.Graphics;
using SFML.System;

namespace EldenBingo.Rendering
{
    public class RoundTableDrawable : IDrawable
    {
        private static Sprite _sprite;
        private MapWindow2 _window;

        private readonly Vector2f RoundTablePosition = new Vector2f(1100f, MapWindow2.FullMapHeight - 750f);
        private readonly Vector2f RoundTableZoomOffset = new Vector2f(113f + 50f, -113f - 50f);

        static RoundTableDrawable()
        {
            var rtTex = new Texture("./Textures/RoundTable.png");
            rtTex.Smooth = true;
            _sprite = new Sprite(rtTex);
            _sprite.Origin = new Vector2f(113f, 113f);
        }

        public RoundTableDrawable(MapWindow2 window)
        {
            _window = window;
            _window.BeforeDraw += onBeforeDraw_SetPositionAndScale;
        }

        public bool Visible { get; set; } = true;

        public Vector2f Position => _sprite.Position;

        public void Draw(RenderTarget target, RenderStates states)
        {
            _sprite.Draw(target, states);
        }

        public static void DisposeStatic()
        {
            _sprite.Dispose();
        }

        private void onBeforeDraw_SetPositionAndScale(object? sender, EventArgs e)
        {
            var cam = _window.Camera;
            var scale = (float)Math.Pow(cam.Zoom, 0.6666);
            var position = RoundTablePosition + RoundTableZoomOffset * scale;

            _sprite.Scale = new Vector2f(scale, scale);
            _sprite.Position = position;
        }

    }
}
