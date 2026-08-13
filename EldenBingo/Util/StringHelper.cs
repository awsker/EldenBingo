using System.Text;

namespace EldenBingo.Util
{
    static internal class StringHelper
    {
        public static string AddSpacesBeforeCapitals(string input)
        {
            var result = new StringBuilder(input.Length);

            for (int i = 0; i < input.Length; i++)
            {
                // Space before captital letters if the previous or next character are lower case
                if (i > 0 && char.IsUpper(input[i]) &&
                    (char.IsLower(input[i - 1]) ||
                    (i < input.Length - 1 && char.IsLower(input[i + 1]))))
                    result.Append(' ');

                result.Append(input[i]);
            }
            return result.ToString();
        }
    }
}
