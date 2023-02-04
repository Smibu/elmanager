namespace Elmanager.UI;

internal class NumericTextBox : GenericTextBox<double>
{
    public NumericTextBox() : base(double.Parse)
    {
        DefaultValue = 0;
    }

    public int ValueAsInt => (int)Value;
}