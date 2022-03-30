using Elmanager.Lgr;

namespace Elmanager.Rendering;

internal record DrawableImage(int TextureId, double Width, double Height, ImageMeta Meta) : IImageMeta
{
    internal double WidthMinusMargin => Width - 2 * EmptyPixelXMargin;

    internal double HeightMinusMargin => Height - 2 * EmptyPixelYMargin;

    internal double EmptyPixelXMargin =>
        Name switch
        {
            "maskhor" => 0.029,
            "masklitt" => 0.015,
            "maskbig" => 0.092,
            _ => 0.092
        };

    internal double EmptyPixelYMargin =>
        Name switch
        {
            "maskhor" => 0.029,
            "masklitt" => 0.015,
            "maskbig" => 0.112,
            _ => 0.112
        };

    public string Name => Meta.Name;

    public ImageType Type => Meta.Type;

    public ClippingType ClippingType => Meta.ClippingType;

    public int Distance => Meta.Distance;
}