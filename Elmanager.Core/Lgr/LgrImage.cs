using SkiaSharp;

namespace Elmanager.Lgr;

public record LgrImage(ImageMeta Meta, SKBitmap Bmp)
{
    public string Name => Meta.Name;

    public ImageType Type => Meta.Type;

    public ClippingType ClippingType => Meta.ClippingType;

    public int Distance => Meta.Distance;
}
