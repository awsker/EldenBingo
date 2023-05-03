using EldenBingoCommon;

namespace EldenBingo.GameInterop
{
    public interface ICoordinateProvider
    {
        public bool Changed { get; }
        public SFML.Graphics.Color Color { get; }
        Guid Guid { get; }
        public MapCoordinates? MapCoordinates { get; }
        public string Name { get; }
    }
}