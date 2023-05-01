
using System.Drawing;

namespace Elmanager.Rendering;

internal record GrassPic(DrawableImage Image, Bitmap Bmp, int Delta, int HeightExtension)
{
    public int Width => Bmp.Width;
    public int HeightWithoutExtension => Bmp.Height - HeightExtension;
}
