using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media;
using Elmanager.EditorTools;

namespace Elmanager.Forms
{
    public partial class SvgImportOptionsForm : Form
    {
        private const double Pow = 1.09648;
        public SvgImportOptionsForm()
        {
            InitializeComponent();
        }

        public static SvgImportOptions? ShowDefault(SvgImportOptions options, string svgFile)
        {
            var prompt = new SvgImportOptionsForm { Result = options, Text = $"SVG import options for {Path.GetFileNameWithoutExtension(svgFile)}"};
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
                UseOutlinedGeometry = useOutlinedGeometryBox.Checked,
                NeverWidenClosedPaths = neverWidenClosedPathsBox.Checked
            };
            set
            {
                smoothnessBar.Value = (int)Math.Round(-Math.Log(value.Smoothness / 10) / Math.Log(Pow));
                useOutlinedGeometryBox.Checked = value.UseOutlinedGeometry;
                neverWidenClosedPathsBox.Checked = value.NeverWidenClosedPaths;
                if (value.FillRule == FillRule.EvenOdd)
                {
                    evenOddRadioButton.Checked = true;
                }
                else
                {
                    nonZeroRadioButton.Checked = true;
                }

                UseOutlinedGeometryBox_CheckedChanged();
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

        private void UseOutlinedGeometryBox_CheckedChanged(object sender = null, EventArgs e = null)
        {
            fillRuleGroupBox.Visible = useOutlinedGeometryBox.Checked;
        }
    }
}
