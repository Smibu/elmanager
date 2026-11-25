using Elmanager.Geometry;
using Elmanager.Lgr;
using Elmanager.Rendering;

namespace Elmanager.LevelEditor.Tools;

public record ImageSelection(ClippingType? Clipping, int? Distance, ImageSelectionKind Kind)
{
    public record PictureSelection(LgrImage Pic);

    public record TextureSelection(LgrImage Txt, LgrImage Mask);

    public record TextureSelectionMultipleMasks(LgrImage Txt);

    public record TextureSelectionMultipleTextures(LgrImage Mask);

    public record MixedSelection;

    public static ImageSelection Picture(LgrImage picture, ClippingType? clipping, int? distance) => new(clipping, distance, new PictureSelection(picture));
    public static ImageSelection Texture(LgrImage texture, LgrImage mask, ClippingType? clipping, int? distance) => new(clipping, distance, new TextureSelection(texture, mask));
    public static ImageSelection TextureWithMultipleMasks(LgrImage texture, ClippingType? clipping, int? distance) => new(clipping, distance, new TextureSelectionMultipleMasks(texture));
    public static ImageSelection MaskWithMultipleTextures(LgrImage mask, ClippingType? clipping, int? distance) => new(clipping, distance, new TextureSelectionMultipleTextures(mask));
    public static ImageSelection Mixed(ClippingType? clipping, int? distance) => new(clipping, distance, new MixedSelection());

    public SingleImageSelection? AsSingle()
    {
        if (Clipping is not { } clipping || Distance is not { } distance)
        {
            return null;
        }

        return Kind switch
        {
            PictureSelection picture => new SingleImageSelection(clipping, distance, picture),
            TextureSelection texture => new SingleImageSelection(clipping, distance, texture),
            TextureSelectionMultipleMasks or TextureSelectionMultipleTextures or MixedSelection => null
        };
    }
}

public union ImageSelectionKind(
    ImageSelection.PictureSelection,
    ImageSelection.TextureSelection,
    ImageSelection.TextureSelectionMultipleMasks,
    ImageSelection.TextureSelectionMultipleTextures,
    ImageSelection.MixedSelection);

public record SingleImageSelection(ClippingType Clipping, int Distance, SingleImageSelectionKind Kind)
{
    public GraphicElement ToGraphicElement(OpenGlLgr lgr, Vector position) => Kind switch
    {
        ImageSelection.PictureSelection picture => GraphicElement.Pic(
            lgr.DrawableImageFromLgrImage(picture.Pic), position, Distance, Clipping),
        ImageSelection.TextureSelection texture => GraphicElement.Text(Clipping, Distance, position,
            lgr.DrawableImageFromLgrImage(texture.Txt),
            lgr.DrawableImageFromLgrImage(texture.Mask))
    };
}

public union SingleImageSelectionKind(ImageSelection.PictureSelection, ImageSelection.TextureSelection);
