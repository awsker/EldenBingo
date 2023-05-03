using SFML.System;

namespace EldenBingo.Rendering
{
    public interface ICamera
    {
        public bool Changed { get; }
        public Vector2f Position { get; set; }
        public Vector2f Size { get; }
        public float Zoom { get; set; }

        public SFML.Graphics.View GetView();

        public void Update(float dt);
    }
}