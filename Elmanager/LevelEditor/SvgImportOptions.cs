using System.Windows.Media;

namespace Elmanager.LevelEditor
{
    internal struct SvgImportOptions
    {
        public double Smoothness;
        public bool UseOutlinedGeometry;
        public bool NeverWidenClosedPaths;
        public FillRule FillRule;

        public static SvgImportOptions Default => new()
        {
            FillRule = FillRule.EvenOdd,
            UseOutlinedGeometry = false,
            NeverWidenClosedPaths = false,
            Smoothness = 1
        };
    }
}
