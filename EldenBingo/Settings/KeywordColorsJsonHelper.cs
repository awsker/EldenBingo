using Newtonsoft.Json;

namespace EldenBingo.Settings
{
    internal class KeywordColor
    {
        public KeywordColor(string kw, Color color)
        {
            Keyword = kw;
            Color = color;
        }
        [JsonProperty]
        public string Keyword { get; set; }
        [JsonProperty]
        public Color Color { get; set; }
    }

    internal static class KeywordColorsJsonHelper
    {
        private static List<KeywordColor> _colors;

        static KeywordColorsJsonHelper()
        {
            _colors = new List<KeywordColor>();
            LoadFromSettings();
        }

        public static void PutIntoSettings()
        {
            try
            {
                Properties.Settings.Default.KeywordColorsJson = JsonConvert.SerializeObject(_colors, Formatting.None);
            } 
            catch
            {
                Properties.Settings.Default.KeywordColorsJson = "[]";
            }
        }

        private static void LoadFromSettings()
        {
            var json = Properties.Settings.Default.KeywordColorsJson;
            try
            {
                _colors = JsonConvert.DeserializeObject<List<KeywordColor>>(json) ?? new List<KeywordColor>();
            }
            catch
            {
                _colors = new List<KeywordColor>();
            }
        }

        public static int NumColors => _colors.Count;

        public static KeywordColor[] Colors
        {
            get { return _colors.ToArray(); }
            set
            {
                _colors = new List<KeywordColor>(value);
                PutIntoSettings();
            }
        }

        public static KeywordColor? GetColor(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;
            return _colors.FirstOrDefault(c => key.Contains(c.Keyword, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
