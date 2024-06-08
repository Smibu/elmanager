namespace Elmanager.Rendering;

internal readonly record struct TexCoord(double X1, double X2, double Y1, double Y2)
{
    public static TexCoord Default => new(0, 1, 0, 1);
}