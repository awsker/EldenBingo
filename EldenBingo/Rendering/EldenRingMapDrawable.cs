using SFML.Graphics;
using SFML.System;

namespace EldenBingo.Rendering
{
    public class EldenRingMapDrawable : IDrawable
    {
        public bool Visible { get; set; } = true;
        
        public uint ImageWidth { get; private set; }
        public uint ImageHeight { get; private set; }

        private static TextureData[,]? _textureData;
        private static bool _texturesLoaded;

        public void Init()
        {
            initMapTextures();
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            if (!_texturesLoaded || _textureData == null || MapWindow2.Instance == null)
                return;
            var viewBounds = MapWindow2.Instance.GetViewBounds();
            foreach (var texData in _textureData)
            {
                if (texData.Sprite.GetGlobalBounds().Intersects(viewBounds))
                    target.Draw(texData.Sprite);
            }
        }

        public FloatRect? GetBoundingBox()
        {
            return new FloatRect(new Vector2f(0f, 0f), new Vector2f(ImageWidth, ImageHeight));
        }

        public static void DisposeStatic()
        {
            if (_textureData != null)
            {
                foreach (var texData in _textureData)
                {
                    texData.Dispose();
                }
            }
        }

        private void initMapTextures()
        {
            if (_texturesLoaded)
                return;
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
            ImageWidth = 0;
            ImageHeight = 0;
            for (int x = 0; x < _textureData.GetLength(0); ++x)
            {
                ImageWidth += _textureData[x, 0].Width;
            }
            for (int y = 0; y < _textureData.GetLength(1); ++y)
            {
                ImageHeight += _textureData[0, y].Height;
            }

            var factors = new Vector2f(ImageWidth / MapWindow2.FullMapWidth, ImageHeight / MapWindow2.FullMapHeight);
            uint currX = 0, currY;
            for (int x = 0; x < _textureData.GetLength(0); ++x)
            {
                currY = 0;
                for (int y = 0; y < _textureData.GetLength(1); ++y)
                {
                    _textureData[x, y].InitSpritePositionAndScale(new Vector2f(currX * factors.X, currY * factors.Y), factors);
                    currY += _textureData[x, y].Height;
                }
                currX += _textureData[x, 0].Width;
            }
            _texturesLoaded = true;
        }

        private class TextureData : IDisposable
        {
            public string Filename;
            public int GridX, GridY;
            public Sprite Sprite;
            public Texture Texture;
            public uint Width, Height;

            public TextureData(int x, int y, uint w, uint h, string fn, Texture texture)
            {
                GridX = x;
                GridY = y;
                Width = w;
                Height = h;
                Filename = fn;
                Texture = texture;
                Sprite = new Sprite(texture);
            }

            public void Dispose()
            {
                Sprite?.Dispose();
                Texture?.Dispose();
            }

            public void InitSpritePositionAndScale(Vector2f position, Vector2f scale)
            {
                Sprite.Position = position;
                Sprite.Scale = scale;
            }
        }
    }
}
