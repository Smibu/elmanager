
using System.Drawing;

namespace Elmanager.Rendering;

public record GrassPic(DrawableImage Image, Bitmap Bmp, int Delta)
{
    public int Width => Bmp.Width;
    public int Height => Bmp.Height;
}
