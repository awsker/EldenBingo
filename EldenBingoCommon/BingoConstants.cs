using System.Drawing;

namespace EldenBingoCommon
{
    public enum EldenRingClasses
    {
        Vagabond,
        Warrior,
        Hero,
        Bandit,
        Astrologer,
        Prophet,
        Samurai,
        Prisoner,
        Confessor,
        Wretch
    }

    public record struct Team(int Index, string Name);

    public record struct TeamScore(int Team, string Name, int Score);

    public readonly record struct ColorName(Color Color, string Name);

    public record struct BingoLine(int Team, string Name, int Type, int BingoIndex) : IEquatable<BingoLine>
    {
        public bool Equals(BingoLine other)
        {
            return Team == other.Team && Type == other.Type && BingoIndex == other.BingoIndex;
        }

        public override int GetHashCode()
        {
            return Team.GetHashCode() ^ (61 + Type.GetHashCode()) ^ (1337 + BingoIndex.GetHashCode());
        }
    }

    public static class BingoConstants
    {
        public const int DefaultPort = 4501;

        public static readonly ColorName[] TeamColors = new[]
        {
            new ColorName(Color.FromArgb(190, 18, 16), "Red"),
            new ColorName(Color.FromArgb(9, 92, 168), "Blue"),
            new ColorName(Color.FromArgb(5, 149, 15), "Green"),
            new ColorName(Color.FromArgb(205, 128, 4), "Orange"),
            new ColorName(Color.FromArgb(135, 35, 208), "Purple"),
            new ColorName(Color.FromArgb(78, 204, 204), "Cyan"),
            new ColorName(Color.FromArgb(237, 115, 216), "Pink"),
            new ColorName(Color.FromArgb(131, 80, 22), "Brown"),
            new ColorName(Color.FromArgb(215, 195, 0), "Yellow")
        };

        public static Color[] ClassColors = new Color[]
        {
            Color.FromArgb(42, 141, 166),
            Color.FromArgb(44, 97, 178),
            Color.FromArgb(158, 43, 37),
            Color.FromArgb(19, 79, 30),
            Color.FromArgb(171, 103, 32),
            Color.FromArgb(203, 177, 134),
            Color.FromArgb(177, 73, 44),
            Color.FromArgb(130, 130, 130),
            Color.FromArgb(140, 53, 185),
            Color.FromArgb(204, 204, 204),
        };

        public static Color AdminSpectatorColor = Color.White;

        public static Color SpectatorColor = Color.LightGray;

        public static Color GetTeamColor(int team)
        {
            if (team == -1)
                return SpectatorColor;
            if (team >= 0 && team < TeamColors.Length)
                return TeamColors[team].Color;
            return Color.Empty;
        }

        public static Color GetTeamColorBright(int team)
        {
            if (team == -1)
                return SpectatorColor.Brighten(0.2f);
            if (team >= 0 && team < TeamColors.Length)
                return TeamColors[team].Color.Brighten(0.2f);
            return Color.Empty;
        }

        public static string GetTeamName(int team)
        {
            if (team == -1)
                return "Spectator";
            if (team >= 0 && team < TeamColors.Length)
                return TeamColors[team].Name + " Team";
            return String.Empty;
        }
    }
}