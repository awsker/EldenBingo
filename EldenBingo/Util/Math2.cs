using System.Runtime.CompilerServices;

namespace EldenBingo.Util
{
    public static class Math2
    {
        public static float Dist(float x1, float y1, float x2, float y2)
        {
            return MathF.Sqrt(DistSqr(x1, y1, x2, y2));
        }

        public static float DistSqr(float x1, float y1, float x2, float y2)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;
            return dx * dx + dy * dy;
        }

        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        public static float InvLerp(float startValue, float endValue, float value)
        {
            if (startValue != endValue)
                return (value - startValue) / (endValue - startValue);
            else
                return startValue;
        }

        public static float LerpAngle(float startAngle, float endAngle, float time)
        {
            float delta = Repeat(endAngle - startAngle, 360.0f);
            if (delta > 180.0f)
                delta -= 360.0f;
            return startAngle + delta * Saturate(time);
        }

        [method:MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Saturate(float value)
        {
            return value < 0.0f ? 0.0f : value > 1.0f ? 1.0f : value;
        }

        public static float Repeat(float value, float length)
        {
            if (length == 0.0f)
                return value;
            return value - MathF.Floor(value / length) * length;
        }

        public static float MoveTowards(float currentValue, float targetValue, float delta)
        {
            if (currentValue < targetValue)
            {
                return Math.Min(currentValue + delta, targetValue);
            }
            else
            {
                return Math.Max(currentValue - delta, targetValue);
            }
        }

        /// <summary>
        /// Returns the closest angle difference in degrees between -180 to 180
        /// </summary>
        /// <param name="currentAngle"></param>
        /// <param name="targetAngle"></param>
        /// <returns></returns>
        public static float DeltaAngle(float currentAngle, float targetAngle)
        {
            float delta = Repeat(targetAngle - currentAngle, 360.0f);
            if (delta > 180.0f)
                delta -= 360.0f;
            return delta;
        }

        public static float RotateTowards(float currentAngle, float targetAngle, float delta)
        {
            float deltaAngle = DeltaAngle(currentAngle, targetAngle);
            float newAngle = Repeat(MoveTowards(currentAngle, currentAngle + deltaAngle, delta), 360.0f);
            if (newAngle > 180.0f)
                newAngle -= 360.0f;
            return newAngle;
        }
    }
}
