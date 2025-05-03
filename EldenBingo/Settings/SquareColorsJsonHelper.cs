using Newtonsoft.Json;

namespace EldenBingo.Settings
{
    internal class KeywordSquareColor
    {
        public KeywordSquareColor(string kw, Color color)
        {
            Keyword = kw;
            Color = color;
        }
        [JsonProperty]
        public string Keyword { get; set; }
        [JsonProperty]
        public Color Color { get; set; }
    }

    internal static class SquareColorsJsonHelper
    {
        private static List<KeywordSquareColor> _colors;

        static SquareColorsJsonHelper()
        {
            _colors = new List<KeywordSquareColor>();
            LoadFromSettings();
        }

        public static void PutIntoSettings()
        {
            try
            {
                Properties.Settings.Default.SquareColorsJson = JsonConvert.SerializeObject(_colors, Formatting.None);
            } 
            catch
            {
                Properties.Settings.Default.SquareColorsJson = "[]";
            }
        }

        private static void LoadFromSettings()
        {
            var json = Properties.Settings.Default.SquareColorsJson;
            try
            {
                _colors = JsonConvert.DeserializeObject<List<KeywordSquareColor>>(json) ?? new List<KeywordSquareColor>();
            }
            catch
            {
                _colors = new List<KeywordSquareColor>();
            }
        }

        public static int NumColors => _colors.Count;

        public static KeywordSquareColor[] Colors
        {
            get { return _colors.ToArray(); }
            set
            {
                _colors = new List<KeywordSquareColor>(value);
                PutIntoSettings();
            }
        }

        public static KeywordSquareColor? GetColor(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;
            return _colors.FirstOrDefault(c => key.Contains(c.Keyword, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
