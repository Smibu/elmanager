using System;
using System.Collections.Generic;
using Elmanager.ElmaPrimitives;
using Elmanager.Geometry;
using Elmanager.Rec;

namespace Elmanager.Physics;

#pragma warning disable

internal class Driver
{
    public BodyPart Body;
    public BodyPart LeftWheel;
    public BodyPart RightWheel;
    public DriverCondition Condition;
    public Direction Direction;
    public GravityDirection GravityDirection;
    public Vector HeadLocation;
    public Rotation? LastRotation;
    public ElmaTime CurrentTime;
    public readonly HashSet<int> TakenApples;

    public Driver(Vector leftWheelLocation)
    {

    }

    public bool Bugged => double.IsNaN(Body.Location.X) || double.IsNaN(Body.Location.Y) || OutOfBounds;
    public bool OutOfBounds { get; set; }

    private double ArmRot()
    {
        if (LastRotation == null)
        {
            return 0;
        }

        var rot = LastRotation.Value;
        var diff = CurrentTime - rot.Time;
        if (diff.ToSeconds() > Player.ArmRotationDelay)
        {
            return 0;
        }

        return Player.GetArmRotationFromLastVolt(diff.ToSeconds(), rot.Kind == RotationKind.Right);
    }

    public PlayerState GetState()
    {
        return new(
            Body.Location.X, Body.Location.Y,
            LeftWheel.Location.X, LeftWheel.Location.Y,
            RightWheel.Location.X, RightWheel.Location.Y,
            LeftWheel.Rotation, RightWheel.Rotation,
            HeadLocation.X, HeadLocation.Y,
            Body.Rotation / (2 * Math.PI) * 360, Direction, ArmRot()
        );
    }

    public IEnumerable<TaggedBodyPart> BodyParts()
    {
        yield return GetBody();
    }

    public TaggedBodyPart GetBody()
    {
        return new(Body.Location, BodyPartKind.Body);
    }

    public void SetPosition(TaggedBodyPart part, bool paused)
    {

    }

    public Driver Clone()
    {
        throw new NotImplementedException();
    }
}