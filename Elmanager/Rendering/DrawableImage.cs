using Elmanager.Lgr;

namespace Elmanager.Rendering;

internal record DrawableImage(int TextureId, double Width, double Height, ImageMeta Meta)
{
    private const double PixelFactor = 1 / 48.0;

    internal double WidthMinusMargin => Width - 2 * EmptyPixelXMargin;

    internal double HeightMinusMargin => Height - 2 * EmptyPixelYMargin;

    internal double EmptyPixelXMargin =>
        Name switch
        {
            "maskhor" => PixelFactor,
            "masklitt" => PixelFactor,
            "maskbig" => 4 * PixelFactor,
            _ => PixelFactor
        };

    internal double EmptyPixelYMargin =>
        Name switch
        {
            "maskhor" => PixelFactor,
            "masklitt" => PixelFactor,
            "maskbig" => 5 * PixelFactor,
            _ => PixelFactor
        };

    public string Name => Meta.Name;

    public ImageType Type => Meta.Type;

    public ClippingType ClippingType => Meta.ClippingType;

    public int Distance => Meta.Distance;

    internal double AspectRatio => Width / Height;
}