namespace EldenBingoCommon
{
    public record struct BingoGameSettings(bool RandomClasses, ISet<EldenRingClasses> ValidClasses, int NumberOfClasses, int CategoryLimit, int RandomSeed);
}