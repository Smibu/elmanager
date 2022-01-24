using Elmanager.Geometry;

namespace Elmanager.Physics;

internal class TaggedBodyPart
{
    public TaggedBodyPart(Vector position, BodyPartKind type)
    {
        Position = position;
        Type = type;
    }

    public Vector Position { get; set; }
    public BodyPartKind Type { get; }
}