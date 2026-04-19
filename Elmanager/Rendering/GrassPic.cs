
using System.Drawing;

namespace Elmanager.Rendering;

internal record GrassPic(DrawableImage Image, Bitmap Bmp, int Delta)
{
    public int Width => Bmp.Width;
    public int Height => Bmp.Height;
}
