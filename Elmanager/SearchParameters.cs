using System;
using Elmanager.CustomControls;

namespace Elmanager
{
    internal class SearchParameters
    {
        internal BoolOption AcrossLev = BoolOption.Dontcare;

        internal Range<DateTime> Date = new Range<DateTime>(DateTime.MinValue,
            DateTime.MaxValue);

        internal Range<int> Size = new Range<int>(0, 10000000);

        public static BoolOption GetBoolOptionFromTriSelect(TriSelect select)
        {
            switch (select.SelectedOption)
            {
                case 0:
                    return BoolOption.True;
                case 1:
                    return BoolOption.False;
                default:
                    return BoolOption.Dontcare;
            }
        }
    }
}