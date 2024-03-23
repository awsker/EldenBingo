namespace EldenBingoCommon
{
    public record struct BingoGameSettings(int BoardSize, bool RandomClasses, ISet<EldenRingClasses> ValidClasses, int NumberOfClasses, int CategoryLimit, int RandomSeed, int PreparationTime, int PointsPerBingoLine);
}