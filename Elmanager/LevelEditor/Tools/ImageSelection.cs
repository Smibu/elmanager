using Elmanager.Lgr;

namespace Elmanager.LevelEditor.Tools;

internal abstract record ImageSelection(ClippingType? Clipping, int? Distance)
{
    internal record PictureSelection(LgrImage Pic, ClippingType? Clipping, int? Distance) : ImageSelection(Clipping, Distance);
    internal record TextureSelection(LgrImage Txt, LgrImage Mask, ClippingType? Clipping, int? Distance) : ImageSelection(Clipping, Distance);
    internal record TextureSelectionMultipleMasks(LgrImage Txt, ClippingType? Clipping, int? Distance) : ImageSelection(Clipping, Distance);
    internal record TextureSelectionMultipleTextures(LgrImage Mask, ClippingType? Clipping, int? Distance) : ImageSelection(Clipping, Distance);
    internal record MixedSelection(ClippingType? Clipping, int? Distance) : ImageSelection(Clipping, Distance);

    internal static PictureSelection Picture(LgrImage picture, ClippingType? clipping, int? distance) => new(picture, clipping, distance);
    internal static TextureSelection Texture(LgrImage texture, LgrImage mask, ClippingType? clipping, int? distance) => new(texture, mask, clipping, distance);
    internal static TextureSelectionMultipleMasks TextureWithMultipleMasks(LgrImage texture, ClippingType? clipping, int? distance) => new(texture, clipping, distance);
    internal static TextureSelectionMultipleTextures MaskWithMultipleTextures(LgrImage mask, ClippingType? clipping, int? distance) => new(mask, clipping, distance);
    internal static MixedSelection Mixed(ClippingType? clipping, int? distance) => new(clipping, distance);
}