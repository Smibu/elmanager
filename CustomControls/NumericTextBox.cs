using System;

namespace Elmanager.CustomControls
{
    internal class NumericTextBox : GenericTextBox<double>
    {
        public NumericTextBox() : base(Double.Parse)
        {
            DefaultValue = 0;
        }
        public int ValueAsInt
        {
            get { return (int) Value; }
        }
    }
}