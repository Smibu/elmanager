using System;

namespace Elmanager.UI;

internal class NumericTextBox : GenericTextBox<double>
{
    public NumericTextBox() : base(Double.Parse)
    {
        DefaultValue = 0;
    }

    public int ValueAsInt => (int) Value;
}