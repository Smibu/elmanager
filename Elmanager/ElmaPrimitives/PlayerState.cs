using Elmanager.Geometry;
using Elmanager.Rec;

namespace Elmanager.ElmaPrimitives;

internal readonly struct PlayerState
{
    public PlayerState(double globalBodyX, double globalBodyY, double leftWheelX, double leftWheelY, double rightWheelX, double rightWheelY, double leftWheelRotation, double rightWheelRotation, double headX, double headY, double bikeRotation, Direction direction, double armRotation)
    {
        GlobalBodyX = globalBodyX;
        GlobalBodyY = globalBodyY;
        LeftWheelX = leftWheelX;
        LeftWheelY = leftWheelY;
        RightWheelX = rightWheelX;
        RightWheelY = rightWheelY;
        LeftWheelRotation = leftWheelRotation;
        RightWheelRotation = rightWheelRotation;
        HeadX = headX;
        HeadY = headY;
        BikeRotation = bikeRotation;
        Direction = direction;
        ArmRotation = armRotation;
    }

    public readonly double GlobalBodyX;
    public readonly double GlobalBodyY;
    public readonly double LeftWheelX;
    public readonly double LeftWheelY;
    public readonly double RightWheelX;
    public readonly double RightWheelY;
    public readonly double LeftWheelRotation;
    public readonly double RightWheelRotation;
    public readonly double HeadX;
    public readonly double HeadY;
    public readonly double BikeRotation;
    public readonly Direction Direction;
    public readonly double ArmRotation;
    public bool IsRight => Direction == Direction.Right;

    public Vector LeftWheel => new(LeftWheelX, LeftWheelY);
    public Vector RightWheel => new(RightWheelX, RightWheelY);
    public Vector Head => new(HeadX, HeadY);
}