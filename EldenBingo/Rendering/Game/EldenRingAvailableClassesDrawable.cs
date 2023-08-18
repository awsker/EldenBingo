using EldenBingoCommon;
using SFML.Graphics;

namespace EldenBingo.Rendering.Game
{
    internal class EldenRingAvailableClassesDrawable : RenderLayer
    {
        private static bool _texturesLoaded;
        private static Texture[]? _classTextures;
        private static Texture? _backgroundTexture;

        private CenteredBackgroundDrawable _background;
        private ClassDrawable[] _classes;

        public EldenRingAvailableClassesDrawable(SimpleGameWindow window) : base(window)
        {
            _classes = Array.Empty<ClassDrawable>();
            CustomView = new SFML.Graphics.View(new FloatRect(0, 0, Window.Size.X, Window.Size.Y));
            window.Resized += window_Resized;
        }

        public void SetAvailableClasses(EldenRingClasses[] classes)
        {
            if (!_texturesLoaded || _classTextures == null)
                return;

            foreach (var cl in _classes)
            {
                RemoveGameObject(cl);
                cl.Dispose();
            }

            _classes = new ClassDrawable[classes.Length];
            for (int i = 0; i < classes.Length; ++i)
            {
                _classes[i] = new ClassDrawable(_classTextures[(int)classes[i]], i, classes.Length, Window.Size);
                AddGameObject(_classes[i]);
            }
        }

        public override void Init()
        {
            if (!_texturesLoaded)
            {
                try
                {
                    var allClasses = Enum.GetValues(typeof(EldenRingClasses));
                    if (allClasses == null)
                        return;
                    _classTextures = new Texture[allClasses.Length];
                    const string imgPath = "./Textures/Classes/";
                    for (int i = 0; i < allClasses.Length; ++i)
                    {
                        var className = allClasses.GetValue(i);
                        if (className == null)
                            return;
                        var cl = Path.Combine(imgPath, $"{className}.png");

                        var tex = new Texture(cl) { Smooth = true };
                        _classTextures[i] = tex;
                    }
                    _backgroundTexture = new Texture(Path.Combine(imgPath, "Classes_Background_2.jpg"));
                    _texturesLoaded = true;
                }
                catch (SFML.LoadingFailedException)
                {
                    return;
                }
            }
            _background = new CenteredBackgroundDrawable(_backgroundTexture, Window.Size);
            AddGameObject(_background);
        }

        public override void Dispose()
        {
            if (_texturesLoaded)
            {
                if (_classTextures != null)
                    foreach (var tex in _classTextures)
                        tex.Dispose();

                _background.Dispose();
                _backgroundTexture?.Dispose();
            }
            Window.Resized -= window_Resized;
        }

        private void window_Resized(object? sender, SFML.Window.SizeEventArgs e)
        {
            CustomView = new SFML.Graphics.View(new FloatRect(0, 0, e.Width, e.Height));
            var size = new SFML.System.Vector2u(e.Width, e.Height);
            _background.SetTargetSize(size);
            foreach (var cl in _classes)
                cl.SetTargetSize(size);
        }
    }
}