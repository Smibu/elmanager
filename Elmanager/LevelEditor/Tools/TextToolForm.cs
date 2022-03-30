using System;
using System.Drawing;
using System.Windows.Forms;
using Elmanager.UI;

namespace Elmanager.LevelEditor.Tools;

internal partial class TextToolForm : FormMod
{
    public event Action<TextToolOptions>? EnteredTextChanged;
    private Font? _font;
    private string _fontStyleName = "";
    private const double LineHeightFactor = 500.0;

    private TextToolForm()
    {
        InitializeComponent();
        void HandleChange(object? sender, EventArgs e) => EnteredTextChanged?.Invoke(Result);
        textBox.TextChanged += HandleChange;
        smoothnessBar.ValueChanged += HandleChange;
        lineHeightBar.ValueChanged += HandleChange;
    }

    public static TextToolOptions? ShowDefault(TextToolOptions options, Action<TextToolOptions> handler)
    {
        var prompt = new TextToolForm {Result = options};
        prompt.EnteredTextChanged += handler;
        prompt.EnteredTextChanged(prompt.Result);
        if (prompt.ShowDialog() == DialogResult.OK)
            return prompt.Result;
        return null;
    }

    private TextToolOptions Result
    {
        get => new()
        {
            Font = _font!,
            Text = textBox.Text,
            Smoothness = Math.Pow(1.1, -smoothnessBar.Value),
            LineHeight = lineHeightBar.Value / LineHeightFactor,
            FontStyleName = _fontStyleName
        };
        init
        {
            _font = value.Font;
            textBox.Text = value.Text;
            smoothnessBar.Value = (int) Math.Round(Math.Log(1 / value.Smoothness) / Math.Log(1.1));
            lineHeightBar.Value = (int) Math.Round(value.LineHeight * LineHeightFactor);
            EnteredTextChanged?.Invoke(Result);
        }
    }

    private void okButton_Click(object? sender, EventArgs? e)
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
        var dialog = new FontDialogMod
        {
            Font = _font,
            FontMustExist = true,
            ShowEffects = true,
            MinSize = 1,
            MaxSize = 100
        };
        dialog.Apply += (_, _) =>
        {
            var r = Result;
            r.Font = dialog.Font!;
            r.FontStyleName = dialog.FontStyleName;
            EnteredTextChanged?.Invoke(r);
        };
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            _font = dialog.Font;
            _fontStyleName = dialog.FontStyleName;
        }

        EnteredTextChanged?.Invoke(Result);
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