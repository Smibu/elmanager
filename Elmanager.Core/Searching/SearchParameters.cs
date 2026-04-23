using System;

namespace Elmanager.Searching;

public class SearchParameters
{
    public BoolOption AcrossLev = BoolOption.Dontcare;

    public Range<DateTime> Date = new(DateTime.MinValue,
        DateTime.MaxValue);

    public Range<int> Size = new(0, 10000000);
}
