namespace EldenBingo.GameInterop
{
    internal static class GameData
    {
        internal const string PATTERN_CSMENUMAN = "48 8B 05 ?? ?? ?? ?? 33 DB 48 89 74 24";
        internal const string PATTERN_CSFD4VIRTUALMEMORYFLAG = "48 8B 3D ? ? ? ? 48 85 FF 74 ? 48 8B 49";
        internal const string PATTERN_WORLDCHRMAN = "48 8B 05 ?? ?? ?? ?? 48 85 C0 74 0F 48 39 88";
        internal const string PROCESS_DESCRIPTION = "elden";
        internal const string PROCESS_TITLE = "Elden Ring";

        internal static readonly string[] PROCESS_EXE_VERSION_SUPPORTED = new string[]
        {
            "1.12.0.0",
        };
    }
}