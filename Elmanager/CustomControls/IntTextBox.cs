namespace Elmanager.CustomControls
{
    internal class IntTextBox : GenericTextBox<int>
    {
        public IntTextBox() : base(int.Parse)
        {
            DefaultValue = 0;
        }
    }
}