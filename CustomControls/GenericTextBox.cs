using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Elmanager.CustomControls
{
    internal class GenericTextBox<T> : TextBox
    {
        private T _defaultValue;
        private Func<string, T> _parseInput;

        public GenericTextBox(Func<string, T> parser)
        {
            TextChanged += ValidateInput;
            SetParser(parser);
        }

        [Description("Gets or sets the default value.")]
        public T DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        public T Value
        {
            get
            {
                if (IsInputValid() && _parseInput != null)
                    return _parseInput(Text);
                return _defaultValue;
            }
        }

        public bool IsInputValid()
        {
            try
            {
                _parseInput(Text);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void SetParser(Func<string, T> val)
        {
            if (val == null)
                return;
            _parseInput = val;
        }

        private void ValidateInput(object sender, EventArgs eventArgs)
        {
            BackColor = IsInputValid() ? SystemColors.Window : Color.Red;
        }
    }
}