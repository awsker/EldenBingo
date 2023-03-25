using SFML.System;

namespace EldenBingo.Rendering
{
    public interface ICamera
    {
        public bool Changed { get; }
        public Vector2f Position { get; set; }
        public float Zoom { get; set; }
        public SFML.Graphics.View GetView();
        public Vector2f Size { get; }

        public void Update(float dt);
    }
}
