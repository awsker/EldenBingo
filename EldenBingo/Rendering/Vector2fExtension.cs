using SFML.System;
using System.Runtime.CompilerServices;

namespace EldenBingo.Rendering
{
    internal static class Vector2fExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Length(this Vector2f v)
        {
            return (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LengthSquared(this Vector2f v)
        {
            return v.X * v.X + v.Y * v.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2f Normalize(this Vector2f v)
        {
            float l = v.Length();
            if (l <= float.Epsilon)
                return new Vector2f();
            return new Vector2f(v.X / l, v.Y / l);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2f Normal(this Vector2f v)
        {
            float l = v.Length();
            if (l <= float.Epsilon)
                return new Vector2f();
            return new Vector2f(-v.Y / l, v.X / l);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(this Vector2f v, Vector2f other)
        {
            return v.X * other.X + v.Y * other.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cross(this Vector2f v, Vector2f other)
        {
            return v.X * other.Y - v.Y * other.X;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot2(this Vector2f v, Vector2f other)
        {
            // sqrt(a) * sqrt(b) = sqrt(a * b) -- valid for real numbers
            float denominator = (float)Math.Sqrt(v.LengthSquared() * other.LengthSquared());
            if (denominator < 1E-15f)
                return 0f;

            return Math.Clamp(v.Dot(other) / denominator, -1f, 1f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(this Vector2f v, Vector2f to)
        {
            // sqrt(a) * sqrt(b) = sqrt(a * b) -- valid for real numbers
            float denominator = (float)Math.Sqrt(v.LengthSquared() * to.LengthSquared());
            if (denominator < 1E-15f)
                return 0f;

            float dot = Math.Clamp(v.Dot(to) / denominator, -1f, 1f);
            return (float)Math.Acos(dot);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2f Rotate(this Vector2f v, float rad)
        {
            var c = Math.Cos(rad);
            var s = Math.Sin(rad);

            return new Vector2f((float)(v.X * c - v.Y * s), (float)(v.X * s + v.Y * c));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceTo(this Vector2f v, Vector2f other)
        {
            var dx = v.X - other.X;
            var dy = v.Y - other.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
    }
}