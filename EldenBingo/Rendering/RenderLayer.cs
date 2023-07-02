using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace EldenBingo.Rendering
{
    public class RenderLayer : IUpdateable, IDrawable
    {
        protected SimpleGameWindow Window;
        protected ISet<object> GameObjects;
        protected IList<IUpdateable> Updateables;
        protected IList<IDrawable> Drawables;
        private RenderTexture _renderTex;
        private Sprite _renderSprite;
        private SFML.Graphics.View _renderView;
        private SFML.Graphics.View? _customView;

        public RenderLayer(SimpleGameWindow window)
        {
            Window = window;

            _renderTex = new RenderTexture(window.Size.X, window.Size.Y);
            _renderSprite = new Sprite(_renderTex.Texture);
            _renderView = new SFML.Graphics.View(new FloatRect(0, 0, window.Size.X, window.Size.Y));
            GameObjects = new HashSet<object>();
            Updateables = new List<IUpdateable>();
            Drawables = new List<IDrawable>();
            ListenToWindowEvents();
        }

        public bool Enabled { get; set; } = true;
        public bool Visible { get; set; } = true;

        public Shader? Shader { get; set; }

        public virtual SFML.Graphics.View? CustomView { get; set; }

        public IReadOnlyCollection<object> GetGameObjects()
        {
            var list = new List<object>(GameObjects);
            return list.AsReadOnly();
        }

        public void AddGameObject(object go)
        {
            if (GameObjects.Contains(go))
                return;
            bool added = false;
            if (go is IUpdateable up)
            {
                Updateables.Add(up);
                added = true;
            }
            if (go is IDrawable draw)
            {
                Drawables.Add(draw);
                added = true;
            }
            if (added)
                GameObjects.Add(go);
        }

        public void RemoveGameObject(object go)
        {
            if (!GameObjects.Contains(go))
                return;
            if (go is IUpdateable up)
            {
                Updateables.Remove(up);
            }
            if (go is IDrawable draw)
            {
                Drawables.Remove(draw);
                if (Window.DisposeDrawables)
                    draw.Dispose();
            }
            GameObjects.Add(go);
        }

        public virtual void Update(float dt)
        {
            foreach (var go in Updateables)
                go.Update(dt);
        }

        public virtual void Draw(RenderTarget target, RenderStates states)
        {
            var oldView = new SFML.Graphics.View(target.GetView());

            var viewBounds = Window.GetViewBounds();
            _renderTex.Clear(SFML.Graphics.Color.Transparent);
            _renderTex.SetView(CustomView ?? oldView);
            foreach (var draw in Drawables.Where(d => d.Visible))
            {
                var rect = draw.GetBoundingBox();
                if (rect != null && !viewBounds.Intersects(rect.Value))
                    continue;
                
                _renderTex.Draw(draw, states);
            }

            var trans = Transform.Identity;
            trans.Translate(0, _renderTex.Size.Y);
            trans.Scale(new Vector2f(1f, -1f));
            states.Transform *= trans;
            if (Shader != null)
                states.Shader = Shader;

            target.SetView(_renderView);
            target.Draw(_renderSprite, states);
            target.SetView(oldView);
        }

        public virtual void Init()
        {
            foreach (var draw in Drawables)
                draw.Init();
        }

        public virtual void Dispose()
        {
            foreach (var draw in Drawables)
                draw.Dispose();

            Shader?.Dispose();
            UnlistenToWindowEvents();
        }

        protected virtual void ListenToWindowEvents()
        {
            Window.Resized += window_Resized;
        }

        protected virtual void UnlistenToWindowEvents()
        {
            Window.Resized -= window_Resized;
        }

        private void window_Resized(object? sender, SizeEventArgs e)
        {
            _renderTex = new RenderTexture(Window.Size.X, Window.Size.Y);
            _renderSprite = new Sprite(_renderTex.Texture);
            _renderView = new SFML.Graphics.View(new FloatRect(0, 0, Window.Size.X, Window.Size.Y));
        }
    }
}