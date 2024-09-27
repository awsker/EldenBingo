namespace EldenBingoCommon
{
    public static class IdentityToken
    {
        private static Random _random = new Random();

        public static string GenerateIdentityToken(int length)
        {
            string token = string.Empty;
            for (int i = 0; i < length; ++i)
            {
                var t = _random.Next(0, 3);
                token += t switch
                {
                    0 => (char)_random.Next(48, 58),
                    1 => (char)_random.Next(65, 91),
                    _ => (char)_random.Next(97, 123),
                };
            }
            return token;
        }
    }
}
