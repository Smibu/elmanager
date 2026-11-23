using Elmanager.Geometry;
using Elmanager.Lgr;

namespace Elmanager.Lev;

internal record GraphicElementFileItem(Vector Position, int Distance, ClippingType Clipping, GraphicElementFileItemKind Kind)
{
    internal record PictureFileItem(string PictureName);

    internal record TextureFileItem(string TextureName, string MaskName);

    internal static GraphicElementFileItem Picture(string pictureName, Vector position, int distance, ClippingType clipping) =>
        new(position, distance, clipping, new PictureFileItem(pictureName));

    internal static GraphicElementFileItem Texture(string textureName, string maskName, Vector position, int distance,
        ClippingType clipping) =>
        new(position, distance, clipping, new TextureFileItem(textureName, maskName));
}

internal union GraphicElementFileItemKind(GraphicElementFileItem.PictureFileItem, GraphicElementFileItem.TextureFileItem);
