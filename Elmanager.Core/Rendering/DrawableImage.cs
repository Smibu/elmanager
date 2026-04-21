using Elmanager.Lgr;
using Elmanager.Rendering.OpenGL;

namespace Elmanager.Rendering;

public record DrawableImage(Texture Texture, double Width, double Height, ImageMeta Meta)
{
    private const double PixelFactor = 1 / 48.0;

    public double WidthMinusMargin => Width - 2 * EmptyPixelXMargin;

    public double HeightMinusMargin => Height - 2 * EmptyPixelYMargin;

    public double EmptyPixelXMargin =>
        Name switch
        {
            "maskhor" => PixelFactor,
            "masklitt" => PixelFactor,
            "maskbig" => 4 * PixelFactor,
            _ => PixelFactor
        };

    public double EmptyPixelYMargin =>
        Name switch
        {
            "maskhor" => PixelFactor,
            "masklitt" => PixelFactor,
            "maskbig" => 5 * PixelFactor,
            _ => PixelFactor
        };

    public string Name => Meta.Name;

    public ImageType Type => Meta.Type;
}
