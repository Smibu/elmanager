using System;
using Elmanager.Geometry;

namespace Elmanager.Lev;

public class LevObject : IPositionable
{
    public int AnimationNumber;
    public AppleType AppleType;
    public Vector Position { get; set; }
    public readonly ObjectType Type;

    public LevObject(Vector position, ObjectType type, AppleType appleType, int animNum = 1)
    {
        Position = position;
        Type = type;
        AppleType = appleType;
        AnimationNumber = Math.Min(Math.Max(animNum, 1), 9);
    }

    private LevObject(LevObject o)
    {
        AnimationNumber = o.AnimationNumber;
        AppleType = o.AppleType;
        Position = o.Position.Clone();
        Type = o.Type;
    }

    internal static LevObject ExitObject(Vector exitPosition)
    {
        return new(exitPosition, ObjectType.Flower, AppleType.Normal);
    }

    internal static LevObject StartObject(Vector startPosition)
    {
        return new(startPosition, ObjectType.Start, AppleType.Normal);
    }

    public LevObject Clone()
    {
        return new(this);
    }

    public double X => Position.X;
    public double Y => Position.Y;

    public VectorMark Mark
    {
        get => Position.Mark;
        set => Position = Position with { Mark = value };
    }

    public bool Equals(LevObject other) =>
        Position.X.Equals(other.Position.X) && Position.Y.Equals(other.Position.Y) && Type == other.Type &&
        AppleType == other.AppleType && AnimationNumber == other.AnimationNumber;

    public static int ObjSortOrder(ObjectType type) => type switch
    {
        ObjectType.Killer => 1,
        ObjectType.Apple => 2,
        ObjectType.Flower => 3,
        ObjectType.Start => 4,
        _ => throw new ArgumentOutOfRangeException(nameof(type))
    };
}
