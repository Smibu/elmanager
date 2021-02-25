using System.Drawing;

namespace Elmanager.LevelEditor.Tools
{
    internal struct TextToolOptions
    {
        public Font Font;
        public string FontStyleName;
        public double LineHeight;
        public double Smoothness;
        public string Text;

        public static TextToolOptions Default => new()
        {
            Font = new Font(new FontFamily("Arial"), 9.0f),
            FontStyleName = "",
            Smoothness = 0.03,
            Text = "Type text here!",
            LineHeight = 1
        };
    }
}