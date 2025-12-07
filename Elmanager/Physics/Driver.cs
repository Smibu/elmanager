using System;
using System.Collections.Generic;
using Elmanager.ElmaPrimitives;
using Elmanager.Geometry;
using Elmanager.Rec;

namespace Elmanager.Physics;

internal class Driver
{
    public BodyPart Body;
    public BodyPart LeftWheel;
    public BodyPart RightWheel;
    public DriverCondition Condition;
    public Direction Direction;
    public GravityDirection GravityDirection;
    public Vector HeadCenterLocation;
    public Vector HeadLocation;
    public Vector HeadVelocity;
    public bool IsBraking;
    public bool IsThrottling;
    public double LeftWheelRotationDiffBody;
    public double RightWheelRotationDiffBody;
    public (ElmaTime, double)? RotRightVar;
    public (ElmaTime, double)? RotLeftVar;
    public double BackWheelSpeed;
    public double MaxTension;
    public Rotation? LastRotation;
    public bool IsTurnKeyDown;
    public ElmaTime CurrentTime;
    public Vector LeftWheelOffset;
    public Vector RightWheelOffset;
    public double HeadOffset;
    public List<Event> TakenAppleEvents;
    public int ComputedFrames;
    public readonly HashSet<int> TakenApples = new();

    public Driver(Vector leftWheelLocation)
    {
        var bd = BodyPart.Body();
        var lw = BodyPart.LeftWheel();
        var rw = BodyPart.RightWheel();
        var hcl = new Vector(2.75, 4.04);

        ComputedFrames = 0;
        CurrentTime = 0;
        Direction = Direction.Left;
        LastRotation = null;
        IsTurnKeyDown = false;
        Condition = DriverCondition.Alive;
        GravityDirection = GravityDirection.Down;
        IsThrottling = false;
        IsBraking = false;
        HeadVelocity = new Vector();
        LeftWheelRotationDiffBody = 0;
        RightWheelRotationDiffBody = 0;
        RotLeftVar = null;
        RotRightVar = null;
        BackWheelSpeed = -1.0;
        MaxTension = -1.0;
        LeftWheelOffset = lw.Location - bd.Location;
        RightWheelOffset = rw.Location - bd.Location;
        HeadOffset = hcl.Y - bd.Location.Y;
        Body = bd;
        LeftWheel = lw;
        RightWheel = rw;
        HeadCenterLocation = hcl;
        HeadLocation = new Vector();
        TakenAppleEvents = new List<Event>();

        var tmp = leftWheelLocation - LeftWheel.Location;
        Body.Location += tmp;
        LeftWheel.Location += tmp;
        RightWheel.Location += tmp;
        HeadCenterLocation += tmp;
        UpdateHeadLocation();
    }

    public Driver(Driver other)
    {
        Body = other.Body.Clone();
        LeftWheel = other.LeftWheel.Clone();
        RightWheel = other.RightWheel.Clone();
        Condition = other.Condition;
        Direction = other.Direction;
        GravityDirection = other.GravityDirection;
        HeadCenterLocation = other.HeadCenterLocation;
        HeadLocation = other.HeadLocation;
        HeadVelocity = other.HeadVelocity;
        IsBraking = other.IsBraking;
        IsThrottling = other.IsThrottling;
        LeftWheelRotationDiffBody = other.LeftWheelRotationDiffBody;
        RightWheelRotationDiffBody = other.RightWheelRotationDiffBody;
        RotRightVar = other.RotRightVar;
        RotLeftVar = other.RotLeftVar;
        BackWheelSpeed = other.BackWheelSpeed;
        MaxTension = other.MaxTension;
        LastRotation = other.LastRotation;
        IsTurnKeyDown = other.IsTurnKeyDown;
        CurrentTime = other.CurrentTime;
        LeftWheelOffset = other.LeftWheelOffset;
        RightWheelOffset = other.RightWheelOffset;
        HeadOffset = other.HeadOffset;
        TakenApples = new(other.TakenApples);
        TakenAppleEvents = new(other.TakenAppleEvents);
        ComputedFrames = other.ComputedFrames;
    }

    public bool Bugged => double.IsNaN(Body.Location.X) || double.IsNaN(Body.Location.Y) || OutOfBounds;
    public bool OutOfBounds { get; set; }

    public void UpdateHeadLocation()
    {
        var rotDir = Vector.FromRadians(Body.Rotation);
        var turnedHead = Direction switch
        {
            Direction.Right => HeadCenterLocation + rotDir * 0.09,
            Direction.Left => HeadCenterLocation - rotDir * 0.09,
            _ => throw new ArgumentOutOfRangeException()
        };

        HeadLocation = turnedHead + rotDir.Ortho() * 0.63;
    }

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
        //yield return new NamedBodyPart(head_location, SolidBodyPart.Head);
        //yield return new NamedBodyPart(left_wheel.location, SolidBodyPart.LeftWheel);
        //yield return new NamedBodyPart(right_wheel.location, SolidBodyPart.RightWheel);
    }

    public TaggedBodyPart GetBody()
    {
        return new(Body.Location, BodyPartKind.Body);
    }

    public void SetPosition(TaggedBodyPart part, bool paused)
    {
        switch (part.Type)
        {
            case BodyPartKind.Head:
                HeadLocation = part.Position;
                break;
            case BodyPartKind.LeftWheel:
                LeftWheel.Location = part.Position;
                break;
            case BodyPartKind.RightWheel:
                RightWheel.Location = part.Position;
                break;
            case BodyPartKind.Body:
                var oldLoc = Body.Location;
                var lwDiff = LeftWheel.Location - oldLoc;
                var rwDiff = RightWheel.Location - oldLoc;
                var headDiff = HeadLocation - oldLoc;
                var headCenterDiff = HeadCenterLocation - oldLoc;
                Body.Location = part.Position;
                LeftWheel.Location = part.Position + lwDiff;
                RightWheel.Location = part.Position + rwDiff;
                if (paused)
                {
                    HeadLocation = part.Position + headDiff;
                    HeadCenterLocation = part.Position + headCenterDiff;
                }
                else
                {
                    Body.Velocity = new Vector();
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    internal Driver Clone()
    {
        return new(this);
    }
}