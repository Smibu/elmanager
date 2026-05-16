
using SkiaSharp;

namespace Elmanager.Rendering;

public record GrassPic(DrawableImage Image, SKBitmap Bmp, int Delta)
{
    public int Width => Bmp.Width;
    public int Height => Bmp.Height;
}
