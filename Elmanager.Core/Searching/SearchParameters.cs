using System;
using Elmanager.UI;

namespace Elmanager.Searching;

public class SearchParameters
{
    public BoolOption AcrossLev = BoolOption.Dontcare;

    public Range<DateTime> Date = new(DateTime.MinValue,
        DateTime.MaxValue);

    public Range<int> Size = new(0, 10000000);

    public static BoolOption GetBoolOptionFromTriSelect(TriSelect select) =>
        select.SelectedOption switch
        {
            0 => BoolOption.True,
            1 => BoolOption.False,
            _ => BoolOption.Dontcare
        };
}
