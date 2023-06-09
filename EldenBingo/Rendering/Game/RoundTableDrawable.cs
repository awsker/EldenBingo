﻿using SFML.Graphics;
using SFML.System;

namespace EldenBingo.Rendering.Game
{
    public class RoundTableDrawable : IDrawable
    {
        private static Sprite _sprite;
        private readonly Vector2f RoundTablePosition = new Vector2f(1750f, MapWindow.FullMapHeight - 1750f);
        private readonly Vector2f RoundTableZoomOffset = new Vector2f(35f, 35f);
        private MapWindow _window;

        static RoundTableDrawable()
        {
            var rtTex = new Texture("./Textures/RoundTable.png");
            rtTex.Smooth = true;
            _sprite = new Sprite(rtTex);
            _sprite.Origin = new Vector2f(113f, 113f);
        }

        public RoundTableDrawable(MapWindow window)
        {
            _window = window;
            _window.BeforeDraw += onBeforeDraw_SetPositionAndScale;
        }

        public bool Visible { get; set; } = true;

        public Vector2f Position => _sprite.Position;

        public static void DisposeStatic()
        {
            _sprite.Dispose();
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            _sprite.Draw(target, states);
        }

        private void onBeforeDraw_SetPositionAndScale(object? sender, EventArgs e)
        {
            var cam = _window.Camera;
            var scale = 0.5f * Math.Min(10f, cam.Zoom);
            var position = RoundTablePosition + RoundTableZoomOffset * scale;

            _sprite.Scale = new Vector2f(scale, scale);
            _sprite.Position = position;
        }
    }
}