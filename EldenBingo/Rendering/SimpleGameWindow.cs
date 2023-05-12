using SFML.Graphics;
using SFML.System;

namespace EldenBingo.Rendering
{
    public class SimpleGameWindow : RenderWindow
    {
        protected ISet<object> GameObjects;
        protected IList<IUpdateable> Updateables;
        protected IList<IDrawable> Drawables;
        protected bool Running { get; set; }

        public bool DisposeDrawables { get; private set; }

        public EventHandler? InitializingDrawables;
        public EventHandler? DisposingDrawables;
        public EventHandler? BeforeUpdate;
        public EventHandler? AfterUpdate;
        public EventHandler? BeforeDraw;
        public EventHandler? AfterDraw;

        public SimpleGameWindow(string title, uint width, uint height, SFML.Window.Styles styles = SFML.Window.Styles.Default) : 
            base(new SFML.Window.VideoMode(width, height), title, styles)
        {
            SetVerticalSyncEnabled(true);
            SetFramerateLimit(60);

            GameObjects = new HashSet<object>();
            Updateables = new List<IUpdateable>();
            Drawables = new List<IDrawable>();

            ListenToEvents();
        }

        public void Stop()
        {
            Running = false;
        }

        public void AddGameObject(object go)
        {
            if (GameObjects.Contains(go))
                return;
            bool added = false;
            if(go is IUpdateable up)
            {
                Updateables.Add(up);
                added = true;
            }
            if(go is IDrawable draw)
            {
                Drawables.Add(draw);
                if (Running) //If already running, initialization needs to be done manually
                    draw.Init();
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
                if(DisposeDrawables)
                    draw.Dispose();
            }
            GameObjects.Add(go);
        }

        public void Start()
        {
            try
            {
                Running = true;

                InitializingDrawables?.Invoke(this, EventArgs.Empty);
                foreach (var draw in Drawables)
                {
                    draw.Init();
                }

                renderLoop();
            } 
            finally
            {
                Running = false;

                if (DisposeDrawables)
                {
                    DisposingDrawables?.Invoke(this, EventArgs.Empty);
                    foreach (var draw in Drawables)
                    {
                        draw.Dispose();
                    }
                }
                Close();
            }
        }

        private void renderLoop()
        {
            Clock clock = new Clock();
            while (Running && IsOpen)
            {
                DispatchEvents();
                Time elapsed = clock.Restart();
                BeforeUpdate?.Invoke(this, EventArgs.Empty);
                Clear(SFML.Graphics.Color.Black);
                foreach (var up in Updateables.Where(u => u.Enabled))
                {
                    up.Update(elapsed.AsSeconds());
                }
                AfterUpdate?.Invoke(this, EventArgs.Empty);
                BeforeDraw?.Invoke(this, EventArgs.Empty);
                var viewBounds = GetViewBounds();
                foreach (var draw in Drawables.Where(d => d.Visible))
                {
                    var rect = draw.GetBoundingBox();
                    if (rect != null && !viewBounds.Intersects(rect.Value))
                        continue;
                    
                    Draw(draw);
                }
                AfterDraw?.Invoke(this, EventArgs.Empty);
                Display();
            }
        }

        protected virtual void ListenToEvents()
        {
            Closed += onWindowClosed;
        }


        protected virtual void UnlistenToEvents()
        {
            Closed -= onWindowClosed;
        }

        public FloatRect GetViewBounds()
        {
            var view = GetView();
            FloatRect rt;
            rt.Left = view.Center.X - view.Size.X * 0.5f;
            rt.Top = view.Center.Y - view.Size.Y * 0.5f;
            rt.Width = view.Size.X;
            rt.Height = view.Size.Y;
            return rt;
        }

        public void DisposeDrawablesOnExit()
        {
            DisposeDrawables = true;
        }

        private void onWindowClosed(object? sender, EventArgs e)
        {
            UnlistenToEvents();
            Stop();
        }
    }
}
