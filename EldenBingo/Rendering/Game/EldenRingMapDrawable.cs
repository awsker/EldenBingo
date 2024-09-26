using EldenBingoCommon;
using SFML.Graphics;
using SFML.System;

namespace EldenBingo.Rendering.Game
{
    public class EldenRingMapDrawable : IDrawable
    {
        private string _path;
        private static Dictionary<MapInstance, TextureData[,]> _mapTextures;

        private TextureData[,]? _textures;
        private MapInstance _mapInstance;
        private Vector2f _mapSize;
        private Vector2f _mapOffset;
        public bool Visible { get; set; } = true;

        public static void DisposeStatic()
        {
            if (_mapTextures != null)
            {
                foreach (var textures in _mapTextures.Values)
                {
                    foreach (var texData in textures)
                    {
                        texData.Dispose();
                    }
                }
            }
        }

        static EldenRingMapDrawable()
        {
            _mapTextures = new Dictionary<MapInstance, TextureData[,]>();
        }

        public EldenRingMapDrawable(MapInstance map, string path, Vector2f mapSize, Vector2f offset)
        {
            _mapInstance = map;
            _path = path;
            _mapSize = mapSize;
            _mapOffset = offset;
        }

        public void Init()
        {
            initMapTextures();
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            if (_textures == null ||  MapWindow.Instance == null)
                return;

            var viewBounds = MapWindow.Instance.GetViewBounds();
            foreach (var texData in _textures)
            {
                if (texData.Sprite.GetGlobalBounds().Intersects(viewBounds))
                    target.Draw(texData.Sprite);
            }
        }

        public FloatRect? GetBoundingBox()
        {
            return new FloatRect(_mapOffset, _mapSize);
        }

        private void initMapTextures()
        {
            if (_textures != null)
                return;

            //Textures for this map instance already cached, so we use that
            if (_mapTextures.TryGetValue(_mapInstance, out var t))
            {
                _textures = t;
            }
            else
            {
                _textures = loadMapTexturesFromDir(_path, _mapSize, _mapOffset);
                _mapTextures[_mapInstance] = _textures;
            }
        }

        private TextureData[,]? loadMapTexturesFromDir(string path, Vector2f fullMapSize, Vector2f mapOffset)
        {
            var width = -1;
            var height = -1;
            string[] images;
            try
            {
                images = Directory.GetFiles(path);
            }
            catch
            {
                return null;
            }
            if (images.Length == 0)
            {
                return null;
            }
            for (int i = 0; i < images.Length; ++i)
            {
                var image = images[i];
                var x = int.Parse(image.Substring(image.Length - 9, 2));
                var y = int.Parse(image.Substring(image.Length - 6, 2));
                width = Math.Max(width, x + 1);
                height = Math.Max(height, y + 1);
            }
            var texData = new TextureData[width, height];
            for (int i = 0; i < images.Length; ++i)
            {
                var image = images[i];
                var x = int.Parse(image.Substring(image.Length - 9, 2));
                var y = int.Parse(image.Substring(image.Length - 6, 2));
                JPEGPicture pic = new JPEGPicture();
                pic.Data = pic.ImageToByteArray(image);
                pic.GetJPEGSize();
                var tex = new Texture(image) { Smooth = true };
                tex.GenerateMipmap();
                texData[x, y] = new TextureData(x, y, pic.Width, pic.Height, image, tex);
            }
            var mapSize = new Vector2u();
            for (int x = 0; x < texData.GetLength(0); ++x)
            {
                mapSize.X += texData[x, 0].Width;
            }
            for (int y = 0; y < texData.GetLength(1); ++y)
            {
                mapSize.Y += texData[0, y].Height;
            }

            var factors = new Vector2f(mapSize.X / fullMapSize.X, mapSize.Y / fullMapSize.Y);
            uint currX = 0, currY;
            for (int x = 0; x < texData.GetLength(0); ++x)
            {
                currY = 0;
                for (int y = 0; y < texData.GetLength(1); ++y)
                {
                    texData[x, y].InitSpritePositionAndScale(new Vector2f(currX * factors.X, currY * factors.Y) + mapOffset, factors);
                    currY += texData[x, y].Height;
                }
                currX += texData[x, 0].Width;
            }
            return texData;
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