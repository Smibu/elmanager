using System;
using System.Drawing;
using System.Windows.Forms;
using Elmanager.EditorTools;

namespace Elmanager.Forms
{
    public partial class Prompt : Form
    {
        public event Action<TextToolOptions> EnteredTextChanged = delegate { };
        private Font _font;
        private const double LineHeightFactor = 500.0;

        public Prompt()
        {
            InitializeComponent();
            textBox.TextChanged += (sender, e) => EnteredTextChanged(Result);
            smoothnessBar.ValueChanged += (sender, e) => EnteredTextChanged(Result);
            lineHeightBar.ValueChanged += (sender, e) => EnteredTextChanged(Result);
        }

        public static TextToolOptions? ShowDefault(TextToolOptions options, Action<TextToolOptions> handler)
        {
            var prompt = new Prompt();
            prompt.Result = options;
            prompt.EnteredTextChanged += handler;
            prompt.EnteredTextChanged(prompt.Result);
            if (prompt.ShowDialog() == DialogResult.OK)
                return prompt.Result;
            return null;
        }

        public TextToolOptions Result
        {
            get
            {
                return new TextToolOptions
                {
                    Font = _font,
                    Text = textBox.Text,
                    Smoothness = Math.Pow(1.1, -smoothnessBar.Value),
                    LineHeight = lineHeightBar.Value/LineHeightFactor
                };
            }
            set
            {
                _font = value.Font;
                textBox.Text = value.Text;
                smoothnessBar.Value = (int) Math.Round(Math.Log(1/value.Smoothness)/Math.Log(1.1));
                lineHeightBar.Value = (int) Math.Round(value.LineHeight*LineHeightFactor);
                EnteredTextChanged(Result);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void fontButton_Click(object sender, EventArgs e)
        {
            var dialog = new FontDialog
            {
                Font = _font,
                FontMustExist = true,
                ShowEffects = true
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _font = dialog.Font;
                EnteredTextChanged(Result);
            }
        }

        private void Prompt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                e.Handled = e.SuppressKeyPress = true;
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        okButton_Click(null, null);
                        break;
                    case Keys.A:
                        textBox.SelectAll();
                        break;
                    default:
                        e.Handled = e.SuppressKeyPress = false;
                        break;
                }
            }
        }
    }
}