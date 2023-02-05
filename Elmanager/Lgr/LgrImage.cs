using System.Drawing;

namespace Elmanager.Lgr;

internal record LgrImage(ImageMeta Meta, Bitmap Bmp)
{
    public string Name => Meta.Name;

    public ImageType Type => Meta.Type;

    public ClippingType ClippingType => Meta.ClippingType;

    public int Distance => Meta.Distance;
}