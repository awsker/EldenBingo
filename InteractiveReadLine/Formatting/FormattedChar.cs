using System;

namespace InteractiveReadLine.Formatting
{
    /// <summary>
    /// A struct representing a single character and its foreground and background colors. Null colors represents the
    /// system defaults.
    /// </summary>
    public struct FormattedChar : IEquatable<FormattedChar>
    {
        public FormattedChar(char c, ConsoleColor? foreground, ConsoleColor? background)
        {
            Char = c;
            Foreground = foreground;
            Background = background;
        }

        public char Char { get; }
        public ConsoleColor? Foreground { get; }
        public ConsoleColor? Background { get; }

        public bool Equals(FormattedChar other)
        {
            return Char == other.Char && Foreground == other.Foreground && Background == other.Background;
        }

        public override bool Equals(object obj)
        {
            return obj is FormattedChar other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Char.GetHashCode();
                hashCode = (hashCode * 397) ^ Foreground.GetHashCode();
                hashCode = (hashCode * 397) ^ Background.GetHashCode();
                return hashCode;
            }
        }
    }
}