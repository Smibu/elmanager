using System;

namespace Elmanager.Searching;

public class Range<T> where T : IComparable
{
    private T _max;
    private T _min;

    public Range(T min, T max)
    {
        _min = min;
        _max = max;
    }

    internal bool Accepts(T x)
    {
        return _min.CompareTo(x) <= 0 && _max.CompareTo(x) >= 0;
    }

    public static Range<int> FromNumericBoxes(int min, int max)
    {
        return new(min, max);
    }
}
