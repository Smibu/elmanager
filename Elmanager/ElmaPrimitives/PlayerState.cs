using Elmanager.Geometry;
using Elmanager.Rec;
using Elmanager.Utilities;

namespace Elmanager.ElmaPrimitives;

internal readonly struct PlayerState
{
    public PlayerState(Vector globalBody, Vector leftWheel, Vector rightWheel,
        double leftWheelRotation, double rightWheelRotation, Vector head,
        double bikeRotation, Direction direction, double armRotation, double turnProgress)
    {
        GlobalBody = globalBody;
        LeftWheel = leftWheel;
        RightWheel = rightWheel;
        LeftWheelRotation = leftWheelRotation;
        RightWheelRotation = rightWheelRotation;
        Head = head;
        BikeRotation = bikeRotation;
        Direction = direction;
        ArmRotation = armRotation;
        TurnProgress = turnProgress;
    }

    public readonly Vector GlobalBody;
    public readonly Vector LeftWheel;
    public readonly Vector RightWheel;
    public readonly double LeftWheelRotation;
    public readonly double RightWheelRotation;
    public readonly Vector Head;
    public readonly double BikeRotation;
    public readonly Direction Direction;
    public readonly double ArmRotation;
    public readonly double TurnProgress;

    public Vector HeadCenter
    {
        get
        {
            var rotDir = Vector.FromRadians(BikeRotation * MathUtils.DegToRad);
            var dirSign = Direction == Direction.Right ? -1.0 : 1.0;
            return Head + rotDir * (dirSign * 0.09);
        }
    }
}
