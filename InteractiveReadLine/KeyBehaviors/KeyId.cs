using System;
using System.Collections.Generic;

namespace InteractiveReadLine.KeyBehaviors
{
    /// <summary>
    /// An identity for a key, is equatable so that it can be used in a dictionary
    /// </summary>
    public struct KeyId : IEquatable<KeyId>
    {
        public KeyId(char c)
        {
            this.Char = c;
            this.Key = null;
            this.HasAlt = false;
            this.HasCtrl = false;
            this.HasShift = false;
        }

        public KeyId(ConsoleKey key, bool ctrl, bool alt, bool shift)
        {
            this.Char = null;
            this.Key = key;
            this.HasCtrl = ctrl;
            this.HasAlt = alt;
            this.HasShift = shift;
        }

        public char? Char { get; set; }
        public ConsoleKey? Key { get; set; }

        public bool HasCtrl { get; set; }
        public bool HasAlt { get; set; }
        public bool HasShift { get; set; }

        public override string ToString()
        {
            var repr = new List<string>();
            if (HasCtrl)
                repr.Add("Ctrl");
            if (HasAlt)
                repr.Add("Alt");
            if (HasShift)
                repr.Add("Shift");
            if (Key != null)
                repr.Add(Key.ToString());
            else 
                repr.Add("'" + Char + "'");

            return string.Join("+", repr);
        }

        public bool Equals(KeyId other)
        {
            return Char == other.Char && Key == other.Key && HasCtrl == other.HasCtrl && HasAlt == other.HasAlt && HasShift == other.HasShift;
        }

        public override bool Equals(object obj)
        {
            return obj is KeyId other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Char.GetHashCode();
                hashCode = (hashCode * 397) ^ Key.GetHashCode();
                hashCode = (hashCode * 397) ^ HasCtrl.GetHashCode();
                hashCode = (hashCode * 397) ^ HasAlt.GetHashCode();
                hashCode = (hashCode * 397) ^ HasShift.GetHashCode();
                return hashCode;
            }
        }
    }
}