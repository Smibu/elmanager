using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media;
using Elmanager.EditorTools;

namespace Elmanager.Forms
{
    public partial class SvgImportSettingsForm : Form
    {
        private const double Pow = 1.09648;
        public SvgImportSettingsForm()
        {
            InitializeComponent();
        }

        public static SvgImportOptions? ShowDefault(SvgImportOptions options, string svgFile)
        {
            var prompt = new SvgImportSettingsForm { Result = options, Text = $"SVG import options for {Path.GetFileNameWithoutExtension(svgFile)}"};
            if (prompt.ShowDialog() == DialogResult.OK)
                return prompt.Result;
            return null;
        }

        public SvgImportOptions Result
        {
            get => new SvgImportOptions
            {
                Smoothness = 10 * Math.Pow(Pow, -smoothnessBar.Value),
                FillRule = evenOddRadioButton.Checked ? FillRule.EvenOdd : FillRule.Nonzero,
                UseOutlinedGeometry = useOutlinedGeometryBox.Checked
            };
            set
            {
                smoothnessBar.Value = (int)Math.Round(-Math.Log(value.Smoothness / 10) / Math.Log(Pow));
                useOutlinedGeometryBox.Checked = value.UseOutlinedGeometry;
                if (value.FillRule == FillRule.EvenOdd)
                {
                    evenOddRadioButton.Checked = true;
                }
                else
                {
                    nonZeroRadioButton.Checked = true;
                }
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
