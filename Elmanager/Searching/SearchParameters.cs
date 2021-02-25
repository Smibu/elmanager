using System;
using Elmanager.UI;

namespace Elmanager.Searching
{
    internal class SearchParameters
    {
        internal BoolOption AcrossLev = BoolOption.Dontcare;

        internal Range<DateTime> Date = new(DateTime.MinValue,
            DateTime.MaxValue);

        internal Range<int> Size = new(0, 10000000);

        public static BoolOption GetBoolOptionFromTriSelect(TriSelect select) =>
            select.SelectedOption switch
            {
                0 => BoolOption.True,
                1 => BoolOption.False,
                _ => BoolOption.Dontcare
            };
    }
}