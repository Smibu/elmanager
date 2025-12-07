using System.Collections.Generic;
using Elmanager.Geometry;

namespace Elmanager.Physics;

internal class BodyPart
{
    public bool TouchingGround;
    public double Rotation;
    public double RotationSpeed;
    public double Radius;
    public double Mass;
    public double AngularMass;
    public Vector Location;
    public Vector Velocity;

    public (Vector, double) DifferenceFrom(Vector p)
    {
        var norm = (Location - p).Length;
        var unit = (Location - p) * (1.0 / norm);
        return (unit, norm);
    }

    public (Vector, double) TangentFrom(Vector p)
    {
        var (unit, norm) = DifferenceFrom(p);
        return (unit.Ortho(), norm);
    }

    private static BodyPart Create(double radius, double mass, double angularMass, Vector location)
    {
        return new()
        {
            TouchingGround = false,
            Rotation = 0,
            RotationSpeed = 0,
            Radius = radius,
            Mass = mass,
            AngularMass = angularMass,
            Location = location,
            Velocity = new Vector(),
        };
    }

    public static BodyPart Body()
    {
        return Create(0.3, 200.0, 60.5, new Vector(2.75, 3.6));
    }

    public static BodyPart LeftWheel()
    {
        return Wheel(1.9);
    }

    public static BodyPart RightWheel()
    {
        return Wheel(3.6);
    }

    private static BodyPart Wheel(double xOffset)
    {
        return Create(0.4, 10.0, 0.32, new Vector(xOffset, 3.0));
    }

    public (Vector, double)? IsSignificantTouch(Vector touch, Vector force)
    {
        var (v, _) = DifferenceFrom(touch);
        var x = v.Dotp(Velocity);
        if (-0.01 >= x || v.Dotp(force) <= 0.0)
        {
            return (v, x);
        }
        else
        {
            return null;
        }
    }

    public void ApplyGroundTouch(Queue<PendingEvent> evs, Vector v, double a)
    {
        var tmp1 = v * a;
        Velocity -= tmp1;
        var tmp2 = tmp1.Length;
        if (tmp2 <= 1.5)
        {
        }
        else
        {
            var tmp3 = tmp2 * 0.125;
            if (tmp3 >= 0.99)
            {
                tmp3 = 0.99;
            }

            evs.Enqueue(new PendingEventOther(new EventTypeGround(tmp3)));
        }
    }

    public bool FirstTouchIsSignificant(Vector touch1, Vector touch2)
    {
        var (v, s) = TangentFrom(touch2);
        return (touch1 - touch2).Dotp(v) * (Velocity.Dotp(v * s) + RotationSpeed) >= 0.0;
    }

    public BodyPart Clone()
    {
        var other = this;
        return new()
        {
            AngularMass = other.AngularMass,
            Location = other.Location,
            Mass = other.Mass,
            Radius = other.Radius,
            Rotation = other.Rotation,
            RotationSpeed = other.RotationSpeed,
            TouchingGround = other.TouchingGround,
            Velocity = other.Velocity,
        };
    }
}