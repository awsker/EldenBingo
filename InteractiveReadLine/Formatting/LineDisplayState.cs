namespace InteractiveReadLine.Formatting
{
    /// <summary>
    /// Holds all of the information to present to the user through the view.  The LineDisplayState is typically
    /// the output of a formatter, and is intended to be written directly to an IReadLineProvider
    /// </summary>
    /// <remarks>
    /// The LineDisplayState is the output of a formatter, which is a function which receives either a LineState or
    /// a TokenizedLine object and returns a LineDisplayState.
    ///
    /// A LineDisplayState consists of three pieces of formatted text in addition to a cursor position. The
    /// actual presentation of the LineDisplayState is the responsibility of the IReadLineProvider, but it is
    /// typically intended that the prefix be written first (like a prompt), the line body in the middle, and
    /// the suffix at the end. The cursor should be located as an offset of the first character of the line
    /// body in order to visually align with how the ReadLineHandler will perform character inserts.
    ///
    /// Text and word wrapping is left up to the provider.
    /// </remarks>
    public class LineDisplayState
    {
        public LineDisplayState(FormattedText prefix, FormattedText lineBody, FormattedText suffix, int cursor)
        {
            Prefix = prefix;
            LineBody = lineBody;
            Suffix = suffix;
            Cursor = cursor;
        }

        /// <summary>
        /// Gets the prefix, which is displayed before the line body
        /// </summary>
        public FormattedText Prefix { get; }

        /// <summary>
        /// Gets the line's body text, which is typically the text entered by the user or some representation of
        /// what they've entered
        /// </summary>
        public FormattedText LineBody { get; }

        /// <summary>
        /// Gets the position of the cursor as an integer offset from the beginning of the LineBody
        /// </summary>
        public int Cursor { get; }

        /// <summary>
        /// Gets the suffix, which is displayed after the line body
        /// </summary>
        public FormattedText Suffix { get; }
    }
}