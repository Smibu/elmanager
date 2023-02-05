using Elmanager.Geometry;
using Elmanager.Lgr;

namespace Elmanager.Lev;

internal abstract record GraphicElementFileItem(Vector Position, int Distance, ClippingType Clipping)
{
    internal bool IsPicture => this is PictureFileItem;

    internal record PictureFileItem
        (string PictureName, Vector Position, int Distance, ClippingType Clipping) : GraphicElementFileItem(Position,
            Distance, Clipping);

    internal record TextureFileItem
        (string TextureName, string MaskName, Vector Position, int Distance, ClippingType Clipping) :
            GraphicElementFileItem(Position,
                Distance, Clipping);

    internal static PictureFileItem Picture(string pictureName, Vector position, int distance, ClippingType clipping) =>
        new(pictureName, position, distance, clipping);

    internal static TextureFileItem Texture(string textureName, string maskName, Vector position, int distance,
        ClippingType clipping) =>
        new(textureName, maskName, position, distance, clipping);
}