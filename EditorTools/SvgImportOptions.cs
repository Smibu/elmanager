using System.Windows.Media;

namespace Elmanager.EditorTools
{
    public struct SvgImportOptions
    {
        public double Smoothness;
        public bool UseOutlinedGeometry;
        public FillRule FillRule;

        public static SvgImportOptions Default => new SvgImportOptions
        {
            FillRule = FillRule.EvenOdd,
            UseOutlinedGeometry = false,
            Smoothness = 1
        };
    }
}
