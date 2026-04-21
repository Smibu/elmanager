using Elmanager.Lgr;

namespace Elmanager.LevelEditor.Tools;

public abstract record ImageSelection(ClippingType? Clipping, int? Distance)
{
    public record PictureSelection(LgrImage Pic, ClippingType? Clipping, int? Distance) : ImageSelection(Clipping, Distance);

    public record TextureSelection(LgrImage Txt, LgrImage Mask, ClippingType? Clipping, int? Distance) : ImageSelection(Clipping, Distance);

    public record TextureSelectionMultipleMasks(LgrImage Txt, ClippingType? Clipping, int? Distance) : ImageSelection(Clipping, Distance);

    public record TextureSelectionMultipleTextures(LgrImage Mask, ClippingType? Clipping, int? Distance) : ImageSelection(Clipping, Distance);

    public record MixedSelection(ClippingType? Clipping, int? Distance) : ImageSelection(Clipping, Distance);

    public static PictureSelection Picture(LgrImage picture, ClippingType? clipping, int? distance) => new(picture, clipping, distance);
    public static TextureSelection Texture(LgrImage texture, LgrImage mask, ClippingType? clipping, int? distance) => new(texture, mask, clipping, distance);
    public static TextureSelectionMultipleMasks TextureWithMultipleMasks(LgrImage texture, ClippingType? clipping, int? distance) => new(texture, clipping, distance);
    public static TextureSelectionMultipleTextures MaskWithMultipleTextures(LgrImage mask, ClippingType? clipping, int? distance) => new(mask, clipping, distance);
    public static MixedSelection Mixed(ClippingType? clipping, int? distance) => new(clipping, distance);
}
