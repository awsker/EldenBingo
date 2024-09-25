namespace EldenBingo.GameInterop
{
    internal static class GameData
    {
        internal const string PATTERN_CSMENUMAN = "48 8B 05 ?? ?? ?? ?? 33 DB 48 89 74 24";
        internal const string PATTERN_CSFD4VIRTUALMEMORYFLAG = "48 8B 3D ?? ?? ?? ?? 48 85 FF 74 ?? 48 8B 49";
        internal const string PATTERN_SETEVENTFLAGFUNC =
            "?? ?? ?? ?? ?? 48 89 74 24 18 57 48 83 EC 30 48 8B DA 41 0F B6 F8 8B 12 48 8B F1 85 D2 0F 84 ?? ?? ?? ?? 45 84 C0";
        internal const string PATTERN_ISEVENTFLAGFUNC =
            "48 83 EC 28 8B 12 85 D2";
        internal const string PATTERN_WORLDCHRMAN = "48 8B 05 ?? ?? ?? ?? 48 85 C0 74 0F 48 39 88";
        internal const string PROCESS_DESCRIPTION = "elden";
        internal const string PROCESS_TITLE = "Elden Ring";

        internal static readonly string[] PROCESS_EXE_VERSION_SUPPORTED = new string[]
        {
            "1.12.0.0",
        };
        
        // Bingo flag range 1059350000 - 1059359999
        internal const uint GAME_STARTED_EVENT_ID = 1059350000;
    }
}