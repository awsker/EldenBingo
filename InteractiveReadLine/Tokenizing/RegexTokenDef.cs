using System.Text.RegularExpressions;

namespace InteractiveReadLine.Tokenizing
{
    /// <summary>
    /// A definition of a token type for a regex based parser.  Contains a matching pattern which must begin with
    /// the start character ^ and an optional type code to assign to tokens matched by the regex.
    /// </summary>
    public class RegexTokenDef
    {
        public RegexTokenDef(string pattern, int typeCode=0)
        {
            this.Pattern = pattern;
            this.TypeCode = typeCode;

            if (!this.Pattern.StartsWith("^"))
                this.Pattern = "^" + this.Pattern;

            this.Regex = new Regex(this.Pattern);
        }

        /// <summary>
        /// Gets the string version of the regex pattern defined to match tokens of this type
        /// </summary>
        public string Pattern { get; }

        /// <summary>
        /// Gets the optional type code to assign to tokens matched by the Pattern property
        /// </summary>
        public int TypeCode { get; }

        /// <summary>
        /// Gets the compiled regex for the Pattern property
        /// </summary>
        public Regex Regex { get; }
    }
}