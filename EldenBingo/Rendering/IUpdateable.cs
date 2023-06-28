namespace EldenBingo.Rendering
{
    public interface IUpdateable
    {
        public bool Enabled { get; }

        void Update(float dt);
    }
}