namespace EldenBingo.Rendering
{
    public interface IGameObject
    {
        public bool Enabled { get; }
        public bool Visible { get; }

        public void Update(float dt);

        public void Draw(float dt);
    }
}