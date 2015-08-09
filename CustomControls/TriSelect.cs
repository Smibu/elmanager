using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Elmanager.CustomControls
{
    public partial class TriSelect : UserControl
    {
        public TriSelect()
        {
            InitializeComponent();
        }

        [Description("Gets or sets the selected option."), DefaultValue(0)]
        public int SelectedOption
        {
            get
            {
                if (OptionButton1.Checked)
                    return 0;
                return OptionButton2.Checked ? 1 : 2;
            }
            set
            {
                switch (value)
                {
                    case 0:
                        OptionButton1.Checked = true;
                        return;
                    case 1:
                        OptionButton2.Checked = true;
                        return;
                    default:
                        OptionButton3.Checked = true;
                        return;
                }
            }
        }

        [Description("Gets or sets the text for the first option."), DefaultValue("Option 1")]
        public string Option1Text
        {
            get { return OptionButton1.Text; }
            set
            {
                OptionButton1.Text = value;
                Resized();
            }
        }

        [Description("Gets or sets the text for the second option."), DefaultValue("Option 2")]
        public string Option2Text
        {
            get { return OptionButton2.Text; }
            set
            {
                OptionButton2.Text = value;
                Resized();
            }
        }

        [Description("Gets or sets the text for the third option."), DefaultValue("Option 3")]
        public string Option3Text
        {
            get { return OptionButton3.Text; }
            set
            {
                OptionButton3.Text = value;
                Resized();
            }
        }

        private void Resized(object sender = null, EventArgs e = null)
        {
            Height = Math.Max(36, Height);
            OptionBox.Size = Size;
            int optionBoxY = OptionBox.Height / 2 + 1 - OptionButton1.Height / 2;
            OptionButton1.Location = new Point(OptionButton1.Location.X, optionBoxY);
            OptionButton2.Location = new Point(
                Math.Max(OptionButton1.Location.X + OptionButton1.Width, OptionBox.Width / 2 - OptionButton2.Width / 2),
                optionBoxY);
            OptionButton3.Location =
                new Point(
                    Math.Max(OptionBox.Width - OptionButton3.Width, OptionButton2.Location.X + OptionButton2.Width),
                    optionBoxY);
            Width = Math.Max(Width, OptionButton3.Location.X + OptionButton3.Width);
        }
    }
}