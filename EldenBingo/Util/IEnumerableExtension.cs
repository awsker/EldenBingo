namespace EldenBingo.Util
{
    internal static class IEnumerableExtension
    {
        public static T? ElementWithMaxValue<T>(this IEnumerable<T> source, Func<T, double> valueFunc)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (valueFunc == null) throw new ArgumentNullException(nameof(valueFunc));

            bool hasValue = false;
            T? maxElement = default;
            double maxValue = double.MinValue;

            foreach (var element in source)
            {
                var value = valueFunc(element);
                if (!hasValue || value > maxValue)
                {
                    maxValue = value;
                    maxElement = element;
                    hasValue = true;
                }
            }

            if (!hasValue)
                throw new InvalidOperationException("Sequence contains no elements");

            return maxElement;
        }
   
        public static T? ElementWithMinValue<T>(this IEnumerable<T> source, Func<T, double> valueFunc)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (valueFunc == null) throw new ArgumentNullException(nameof(valueFunc));

            bool hasValue = false;
            T? minElement = default;
            double minValue = double.MaxValue;

            foreach (var element in source)
            {
                var value = valueFunc(element);
                if (!hasValue || value < minValue)
                {
                    minValue = value;
                    minElement = element;
                    hasValue = true;
                }
            }

            if (!hasValue)
                throw new InvalidOperationException("Sequence contains no elements");

            return minElement;
        }

        public static T? ElementWithMaxValue<T>(this IEnumerable<T> source, Func<T, int> valueFunc)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (valueFunc == null) throw new ArgumentNullException(nameof(valueFunc));

            bool hasValue = false;
            T? maxElement = default;
            int maxValue = int.MinValue;

            foreach (var element in source)
            {
                var value = valueFunc(element);
                if (!hasValue || value > maxValue)
                {
                    maxValue = value;
                    maxElement = element;
                    hasValue = true;
                }
            }

            if (!hasValue)
                throw new InvalidOperationException("Sequence contains no elements");

            return maxElement;
        }

        public static T? ElementWithMinValue<T>(this IEnumerable<T> source, Func<T, int> valueFunc)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (valueFunc == null) throw new ArgumentNullException(nameof(valueFunc));

            bool hasValue = false;
            T? minElement = default;
            int minValue = int.MaxValue;

            foreach (var element in source)
            {
                var value = valueFunc(element);
                if (!hasValue || value < minValue)
                {
                    minValue = value;
                    minElement = element;
                    hasValue = true;
                }
            }

            if (!hasValue)
                throw new InvalidOperationException("Sequence contains no elements");

            return minElement;
        }

        public static T? ElementWithMaxValue<T>(this IEnumerable<T> source, Func<T, float> valueFunc)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (valueFunc == null) throw new ArgumentNullException(nameof(valueFunc));

            bool hasValue = false;
            T? maxElement = default;
            float maxValue = float.MinValue;

            foreach (var element in source)
            {
                var value = valueFunc(element);
                if (!hasValue || value > maxValue)
                {
                    maxValue = value;
                    maxElement = element;
                    hasValue = true;
                }
            }

            if (!hasValue)
                throw new InvalidOperationException("Sequence contains no elements");

            return maxElement;
        }

        public static T? ElementWithMinValue<T>(this IEnumerable<T> source, Func<T, float> valueFunc)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (valueFunc == null) throw new ArgumentNullException(nameof(valueFunc));

            bool hasValue = false;
            T? minElement = default;
            float minValue = float.MaxValue;

            foreach (var element in source)
            {
                var value = valueFunc(element);
                if (!hasValue || value < minValue)
                {
                    minValue = value;
                    minElement = element;
                    hasValue = true;
                }
            }

            if (!hasValue)
                throw new InvalidOperationException("Sequence contains no elements");

            return minElement;
        }
    }
}
