using Elmanager.Utilities;

namespace Elmanager.UI;

internal class TimeTextBox : GenericTextBox<double>
{
    public TimeTextBox()
        : base(StringUtils.StringToTime)
    {
        DefaultValue = 0;
    }
}