using System;
using Elmanager.UI;

namespace Elmanager.Searching
{
    internal class Range<T> where T : IComparable
    {
        private T _max;
        private T _min;

        internal Range(T min, T max)
        {
            _min = min;
            _max = max;
        }

        internal bool Accepts(T x)
        {
            return _min.CompareTo(x) <= 0 && _max.CompareTo(x) >= 0;
        }

        internal static Range<int> FromNumericBoxes(NumericTextBox min, NumericTextBox max)
        {
            return new(min.ValueAsInt, max.ValueAsInt);
        }
    }
}