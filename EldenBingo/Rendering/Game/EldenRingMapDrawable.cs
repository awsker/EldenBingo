using EldenBingo.Util;
using EldenBingoCommon;
using SFML.Graphics;
using SFML.System;
using System.Linq;

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
                if (texData != null && texData.Sprite.GetGlobalBounds().Intersects(viewBounds))
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
                if (_textures != null)
                {
                    _mapTextures[_mapInstance] = _textures;
                }
            }
        }

        private TextureData[,]? loadMapTexturesFromDir(string path, Vector2f fullMapSize, Vector2f mapOffset)
        {

            var width = -1;
            var height = -1;
            string[] images;
            try
            {
                images = Directory.EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly)
                   .Where(s => (s.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                               s.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                               s.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)) &&
                               isFileAvailableLocally(s))
                   .ToArray();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return null;
            }
            if (images.Length == 0)
            {
                return null;
            }
            for (int i = 0; i < images.Length; ++i)
            {
                var image = images[i];
                if (int.TryParse(image.AsSpan(image.Length - 9, 2), out int col) && int.TryParse(image.AsSpan(image.Length - 6, 2), out int row))
                {
                    width = Math.Max(width, col + 1);
                    height = Math.Max(height, row + 1);
                }
            }
            if (width == -1 || height == -1)
            {
                return null;
            }
            var texData = new TextureData[width, height];
            
            for (int i = 0; i < images.Length; ++i)
            {
                var image = images[i];
                //Skip image if column or row couldn't be established
                if (!int.TryParse(image.AsSpan(image.Length - 9, 2), out int col) || !int.TryParse(image.AsSpan(image.Length - 6, 2), out int row))
                {
                    printError($"Couldn't load map coordinates from image '{image}'");
                    continue;
                }
                try
                {
                    JPEGPicture pic = new JPEGPicture();
                    pic.Data = pic.ImageToByteArray(image);
                    pic.GetJPEGSize();
                    var tex = new Texture(image) { Smooth = true };
                    tex.GenerateMipmap();
                    var textureData = new TextureData(col, row, pic.Width, pic.Height, image, tex);
                    texData[col, row] = textureData;
                }
                catch (Exception ex)
                {
                    //Skip this image
                    Logger.LogException(ex);
                }
            }
            var totalTexSize = getTotalTextureSize(texData);
            var firstImage = texData[0, 0];
            if (totalTexSize ==  null || firstImage == null)
            {
                printError("Couldn't establish map width or height");
                return null;
            }
            var factors = new Vector2f(fullMapSize.X / totalTexSize.Value.X, fullMapSize.Y / totalTexSize.Value.Y);
            uint currX = 0, currY;
            for (int x = 0; x < texData.GetLength(0); ++x)
            {
                currY = 0;
                for (int y = 0; y < texData.GetLength(1); ++y)
                {
                    var img = texData[x, y];
                    if (img != null)
                    {
                        img.InitSpritePositionAndScale(new Vector2f(currX * factors.X, currY * factors.Y) + mapOffset, factors);
                        
                    }
                    currY += firstImage.Height;

                }
                currX += firstImage.Width;
            }
            return texData;
        }

        private void printError(string error)
        {
            MainForm.Instance?.PrintToConsole(error, System.Drawing.Color.Red, true);
        }

        private Vector2i? getTotalTextureSize(TextureData[, ] texData)
        {
            var width = texData.GetLength(0);
            var height = texData.GetLength(1);
            var widths = new int[width];
            var heights = new int[height];
            for(int x = 0; x < width; ++x)
            {
                for(int y = 0; y < height; ++y)
                {
                    var tex = texData[x, y];
                    if(tex != null)
                    {
                        widths[x] = Math.Max(widths[x], (int)tex.Width);
                        heights[y] = Math.Max(heights[y], (int)tex.Height);
                    }
                }
            }
            if(widths.Any(w => w == 0) || heights.Any(h => h == 0)) 
            {
                return null;
            }
            return new Vector2i(widths.Sum(), heights.Sum());
        }

        private bool isFileAvailableLocally(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);

            // OneDrive files may have this attribute if they're not fully downloaded locally
            const int FILE_ATTRIBUTE_RECALL_ON_DATA_ACCESS = 0x00400000;
            if((fileInfo.Attributes & (FileAttributes)FILE_ATTRIBUTE_RECALL_ON_DATA_ACCESS) != 0) 
            {
                printError($"File '{filePath}' isn't available locally");
            }
            return (fileInfo.Attributes & (FileAttributes)FILE_ATTRIBUTE_RECALL_ON_DATA_ACCESS) == 0;
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