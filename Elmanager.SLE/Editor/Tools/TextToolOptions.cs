namespace Elmanager.SLE.Editor.Tools;

internal struct TextToolOptions
{
    public string FontFamily;
    public double FontSize;
    public bool Bold;
    public bool Italic;
    public bool Underline;
    public bool Strikeout;
    public double LineHeight;
    public double Smoothness;
    public string Text;

    public const string DefaultFontFamily = "Noto Mono";

    public static TextToolOptions Default => new()
    {
        FontFamily = DefaultFontFamily,
        FontSize = 9.0,
        Bold = false,
        Italic = false,
        Underline = false,
        Strikeout = false,
        LineHeight = 1,
        Smoothness = 0.03,
        Text = "Type text here!"
    };
}
