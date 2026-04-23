using System.ComponentModel;
using System.Windows.Forms;
using Elmanager.Searching;

namespace Elmanager.UI;

internal partial class TriSelect : UserControl
{
    public TriSelect()
    {
        InitializeComponent();
    }

    [Description("Gets or sets the selected option."), DefaultValue(BoolOption.Dontcare)]
    public BoolOption SelectedOption
    {
        get
        {
            if (OptionButton1.Checked)
                return BoolOption.True;
            return OptionButton2.Checked ? BoolOption.False : BoolOption.Dontcare;
        }
        set
        {
            switch (value)
            {
                case BoolOption.True:
                    OptionButton1.Checked = true;
                    return;
                case BoolOption.False:
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
