using EldenBingoCommon;

namespace EldenBingo.GameInterop
{
    public interface ICoordinateProvider
    {
        Guid Guid { get; }
        public string Name { get; }
        public bool Changed { get; }
        public MapCoordinates? MapCoordinates { get; }
        public SFML.Graphics.Color Color { get; }
    }
}
