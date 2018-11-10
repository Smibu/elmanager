namespace Elmanager.CustomControls
{
    internal class TimeTextBox : GenericTextBox<double>
    {
        public TimeTextBox()
            : base(Utils.StringToTime)
        {
            DefaultValue = 0;
        }
    }
}