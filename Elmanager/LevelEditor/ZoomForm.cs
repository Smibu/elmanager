using System.Windows.Forms;
using Elmanager.UI;

namespace Elmanager.LevelEditor;

internal partial class ZoomForm : FormMod
{
    public ZoomForm()
    {
        InitializeComponent();
    }

    public static double? GetValue(double initial)
    {
        var zoomForm = new ZoomForm();
        zoomForm.zoomBox.Text = initial.ToString("F3");
        var result = zoomForm.ShowDialog();
        if (result == DialogResult.OK)
        {
            return zoomForm.zoomBox.Value;
        }

        return null;
    }

    private void button2_Click(object sender, System.EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void button1_Click(object sender, System.EventArgs e)
    {
        DialogResult = DialogResult.OK;
        Close();
    }

    private void zoomBox_TextChanged(object sender, System.EventArgs e)
    {
        okButton.Enabled = zoomBox.IsInputValid() && zoomBox.Value > 0;
    }
}