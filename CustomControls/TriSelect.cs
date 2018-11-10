using System.ComponentModel;
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
            get => OptionButton1.Text;
            set => OptionButton1.Text = value;
        }

        [Description("Gets or sets the text for the second option."), DefaultValue("Option 2")]
        public string Option2Text
        {
            get => OptionButton2.Text;
            set => OptionButton2.Text = value;
        }

        [Description("Gets or sets the text for the third option."), DefaultValue("Option 3")]
        public string Option3Text
        {
            get => OptionButton3.Text;
            set => OptionButton3.Text = value;
        }
    }
}