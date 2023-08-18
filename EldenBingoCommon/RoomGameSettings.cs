namespace EldenBingoCommon
{
    public record struct RoomGameSettings(int RandomSeed, ISet<EldenRingClasses> ValidClasses, int? NumberOfClasses, int? CategoryLimit);
}