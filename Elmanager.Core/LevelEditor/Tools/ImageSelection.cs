using Elmanager.Lgr;

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
}

public union ImageSelectionKind(
    ImageSelection.PictureSelection,
    ImageSelection.TextureSelection,
    ImageSelection.TextureSelectionMultipleMasks,
    ImageSelection.TextureSelectionMultipleTextures,
    ImageSelection.MixedSelection);
