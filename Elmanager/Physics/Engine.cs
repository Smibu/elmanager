using System;
using System.Collections.Generic;
using System.Linq;
using Elmanager.ElmaPrimitives;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.Rec;

namespace Elmanager.Physics;

internal class Engine
{
    private const double GravityStrength = 10.0;
    private const double SpringConstant = 10000.0;
    private const double Damping = 1000.0;
    private const double PrimaryRotationStrength = 12.0;
    private const double SecondaryRotationStrength = 3.0;
    private const double SuspensionTensionFactor = 1.0;
    private const double HeadFactor = 0.48 / 0.26 * 0.48 / 0.26;
    private const double HeadThreshold = 0.48 * 0.48;
    private const double HeadRadius = 0.238;
    private const double Pi = 3.141592;
    private const double HalfPi = Pi * 0.5;

    private IElmaEdgeTree _ground = null!;
    private ObjectTree _objectTree = null!;
    private int _numApples;
    private Vector _startLocation;
    private readonly Queue<PendingEvent> _pendingEvents = new();
    private readonly double _headFactor2 = Math.Sqrt(HeadThreshold);
    private readonly Vector _defaultWheelForce = new();
    private readonly Vector _headVec1 = new(-0.35, 0.13);
    private readonly Vector _headVec2 = new Vector(0.49, 0.23).Ortho().Unit();
    private readonly ElmaTime _rotationDelay = new(0.4);

    public Engine(List<Polygon> polys, List<LevObject> objects, IElmaEdgeTree tree)
    {
        InitPolysAndObjects(polys, objects, tree);
    }

    public Engine(IElmaEdgeTree edgeTree)
    {
        _ground = edgeTree;
    }

    public void InitPolysAndObjects(List<Polygon> polys, List<LevObject> objects, IElmaEdgeTree tree)
    {
        var edges = polys.Where(p => !p.IsGrass)
            .SelectMany(p => p.Vertices.Zip(p.VerticesRing.Skip(1), (v1, v2) => new Edge(v1, v2)));
        tree.Init(edges.ToList(), 0.4);
        _ground = tree;
        _objectTree = new ObjectTree(objects);
        _numApples = objects.Count(o => o.Type == ObjectType.Apple);
        var start = objects.Find(o => o.Type == ObjectType.Start);
        if (start is null)
        {
            throw new Exception("Start object not found");
        }
        _startLocation = start.Position;
    }

    public Driver InitDriver()
    {
        return new(_startLocation);
    }

    private static double Mod2Pi2(double angle)
    {
        if (-Pi > angle)
        {
            return Pi + Pi + angle;
        }

        if (angle > Pi)
        {
            return angle - (Pi + Pi);
        }

        return angle;
    }

    public void NextFrame(
        Driver driver,
        InputKeys keys,
        RideRecorder recorder,
        ElmaTime timeStep
    )
    {
        switch (driver.Condition)
        {
            case DriverCondition.Dead:
            case DriverCondition.Finished:
                break;
            case DriverCondition.Alive:
                CalcPhysics(driver, keys, recorder, timeStep);
                break;
        }

        if (driver.Condition == DriverCondition.Alive)
        {
            driver.CurrentTime += timeStep;
            driver.ComputedFrames += 1;
        }
    }

    private void HandleTurning(
        Driver driver,
        InputKeys keys,
        RideRecorder rec,
        ElmaTime time
    )
    {
        if (!driver.IsTurnKeyDown && keys.Turn)
        {
            driver.Direction = driver.Direction == Direction.Left ? Direction.Right : Direction.Left;
            driver.UpdateHeadLocation();
            rec.SaveEvent(new Event(time.Value, new EventTypeTurn()));
        }

        driver.IsTurnKeyDown = keys.Turn;
    }

    private void CalcPhysics(
        Driver driver,
        InputKeys keys,
        RideRecorder recorder,
        ElmaTime timeStep
    )
    {
        var time = driver.CurrentTime;
        var rotatingLeft = false;
        var rotatingRight = false;
        HandleTurning(driver, keys, recorder, time);
        var lastRotationTime = driver.LastRotation?.Time ?? new ElmaTime(-100.0);
        if (_rotationDelay + lastRotationTime < time)
        {
            if (keys.RightVolt || keys.AloVolt)
            {
                driver.LastRotation = new Rotation { Kind = RotationKind.Right, Time = time };
                rotatingRight = true;
                _pendingEvents
                    .Enqueue(new PendingEventOther(new EventTypeVoltRight()));
            }

            if (keys.LeftVolt || keys.AloVolt)
            {
                driver.LastRotation = new Rotation { Kind = RotationKind.Left, Time = time };
                rotatingLeft = true;
                _pendingEvents
                    .Enqueue(new PendingEventOther(new EventTypeVoltLeft()));
            }
        }

        driver.MaxTension = CalcPhysicsSub(
            driver,
            time,
            timeStep,
            keys.Gas,
            keys.Brake,
            rotatingRight,
            rotatingLeft
        );
        if (CheckObjectCollision(driver) == DriverCondition.Dead)
        {
            driver.MaxTension = 0.0;
            driver.BackWheelSpeed = -1.0;
            driver.IsThrottling = false;
            recorder.SaveFrame(time, driver);
            driver.Condition = DriverCondition.Dead;
            return;
        }

        var wheelRotationSpeed = driver.Direction == Direction.Right
            ? driver.LeftWheel.RotationSpeed
            : driver.RightWheel.RotationSpeed;

        driver.BackWheelSpeed = Math.Abs(wheelRotationSpeed) * 0.025;
        if (driver.BackWheelSpeed > 30.0)
        {
            driver.BackWheelSpeed = 403.0;
        }

        driver.BackWheelSpeed = 2.0 - Math.Exp(-driver.BackWheelSpeed);
        driver.IsThrottling = keys.Gas;
        recorder.SaveFrame(time, driver);
        while (_pendingEvents.Count > 0 && driver.Condition == DriverCondition.Alive)
        {
            var e = _pendingEvents.Dequeue();
            var evt = e switch
            {
                PendingEventObject io => new EventTypeObjectTouch(io.Obj.Index),
                PendingEventOther ev => ev.EventType,
                _ => throw new ArgumentOutOfRangeException()
            };

            if (e is PendingEventObject xx)
            {
                driver.Condition = TakeLevelObject(xx.Obj.Obj, driver);
            }

            recorder.SaveEvent(new Event(time.Value, evt));
        }
    }

    private double CalcPhysicsSub(
        Driver driver,
        ElmaTime time,
        ElmaTime timeStep,
        bool isThrottling,
        bool isBraking,
        bool rotatingRight,
        bool rotatingLeft
    )
    {
        var rotationDir = Vector.FromRadians(driver.Body.Rotation);
        var rotationDirOrtho = rotationDir.Ortho();
        if (!driver.IsBraking && isBraking)
        {
            driver.LeftWheelRotationDiffBody =
                driver.LeftWheel.Rotation - driver.Body.Rotation;
            driver.RightWheelRotationDiffBody =
                driver.RightWheel.Rotation - driver.Body.Rotation;
        }

        driver.IsBraking = isBraking;
        var (leftWheelRotationForce, rightWheelRotationForce) = (0.0, 0.0);
        if (isThrottling)
            (leftWheelRotationForce, rightWheelRotationForce) =
                driver.Direction is Direction.Right && driver.LeftWheel.RotationSpeed > -110.0
                    ? (-600.0, 0.0)
                    : driver.Direction is Direction.Left && driver.RightWheel.RotationSpeed < 110.0
                        ? (0.0, 600.0)
                        : (0.0, 0.0);

        if (isBraking)
        {
            leftWheelRotationForce = (driver.LeftWheel.Rotation
                                      - (driver.LeftWheelRotationDiffBody + driver.Body.Rotation))
                                     * -1000.0
                                     - (driver.LeftWheel.RotationSpeed - driver.Body.RotationSpeed) * 100.0;
            rightWheelRotationForce = (driver.RightWheel.Rotation
                                       - (driver.RightWheelRotationDiffBody + driver.Body.Rotation))
                                      * -1000.0
                                      - (driver.RightWheel.RotationSpeed - driver.Body.RotationSpeed) * 100.0;
        }
        else
        {
            driver.LeftWheel.Rotation = Mod2Pi2(driver.LeftWheel.Rotation);
            driver.RightWheel.Rotation = Mod2Pi2(driver.RightWheel.Rotation);
        }

        var (lwForce, lwBodyForce, lwBodyRotationForce, lwTension) = ComputeSuspensionForces(
            driver,
            driver.LeftWheel,
            rotationDir,
            rotationDirOrtho,
            driver.LeftWheelOffset,
            leftWheelRotationForce
        );
        var (rwForce, rwBodyForce, rwBodyRotationForce, rwTension) = ComputeSuspensionForces(
            driver,
            driver.RightWheel,
            rotationDir,
            rotationDirOrtho,
            driver.RightWheelOffset,
            rightWheelRotationForce
        );
        var prevRotSpeed = rotatingRight || rotatingLeft ? driver.Body.RotationSpeed : 0.0;
        if (driver.RotRightVar is var (rotTimeR, rotSpeedR))
        {
            if (rotatingRight || rotatingLeft || time > _rotationDelay * 0.25 + rotTimeR)
            {
                var tmp = driver.Body.RotationSpeed + PrimaryRotationStrength;
                driver.Body.RotationSpeed = Math.Min(tmp, rotSpeedR);
                if (driver.Body.RotationSpeed > 0.0)
                {
                    var tmp2 = driver.Body.RotationSpeed - SecondaryRotationStrength;
                    driver.Body.RotationSpeed = Math.Max(tmp2, 0.0);
                }

                driver.RotRightVar = null;
            }
        }

        if (driver.RotLeftVar is var (rotTimeL, rotSpeedL))
        {
            if (rotatingRight || rotatingLeft || time > _rotationDelay * 0.25 + rotTimeL)
            {
                var tmp = driver.Body.RotationSpeed - PrimaryRotationStrength;
                driver.Body.RotationSpeed = Math.Max(tmp, rotSpeedL);
                if (driver.Body.RotationSpeed < 0.0)
                {
                    var tmp2 = driver.Body.RotationSpeed + SecondaryRotationStrength;
                    driver.Body.RotationSpeed = Math.Min(tmp2, 0.0);
                }

                driver.RotLeftVar = null;
            }
        }

        if (rotatingRight)
        {
            driver.RotRightVar = (time, driver.Body.RotationSpeed);
            driver.Body.RotationSpeed -= PrimaryRotationStrength;
        }

        if (rotatingLeft)
        {
            driver.RotLeftVar = (time, driver.Body.RotationSpeed);
            driver.Body.RotationSpeed += PrimaryRotationStrength;
        }

        if (rotatingRight || rotatingLeft)
        {
            driver.HeadVelocity += (driver.HeadCenterLocation - driver.Body.Location).Ortho()
                                   * (driver.Body.RotationSpeed - prevRotSpeed);
        }

        var gravVect = driver.GravityDirection switch
        {
            GravityDirection.Down => new Vector(0.0, -1.0),
            GravityDirection.Up => new Vector(0.0, 1.0),
            GravityDirection.Left => new Vector(-1.0, 0.0),
            GravityDirection.Right => new Vector(1.0, 0.0),
            GravityDirection.None => new Vector(),
            _ => throw new ArgumentOutOfRangeException()
        };

        ComputeHeadCenter(
            driver,
            gravVect,
            rotationDir,
            rotationDirOrtho,
            timeStep
        );
        var bodyTotalForce =
            (lwBodyForce + rwBodyForce) + gravVect * driver.Body.Mass * GravityStrength;
        ApplyForcesToBodypart(
            driver.Body,
            bodyTotalForce,
            rwBodyRotationForce + lwBodyRotationForce,
            timeStep,
            CollisionCheck.None
        );
        var lwTotalForce = lwForce + gravVect * driver.LeftWheel.Mass * GravityStrength;
        driver.LeftWheel.TouchingGround = ApplyForcesToBodypart(
            driver.LeftWheel,
            lwTotalForce,
            leftWheelRotationForce,
            timeStep,
            CollisionCheck.Edges
        );
        var rwTotalForce = rwForce + gravVect * driver.RightWheel.Mass * GravityStrength;
        driver.RightWheel.TouchingGround = ApplyForcesToBodypart(
            driver.RightWheel,
            rwTotalForce,
            rightWheelRotationForce,
            timeStep,
            CollisionCheck.Edges
        );
        driver.UpdateHeadLocation();
        return Math.Max(lwTension, rwTension);
    }

    private void ComputeHeadCenter(
        Driver driver,
        Vector gravVect,
        Vector rotationDir,
        Vector rotationDirOrtho,
        ElmaTime timeStep
    )
    {
        driver.HeadCenterLocation =
            ComputeInitialHead(driver, rotationDir, rotationDirOrtho);
        var headDiff = (driver.Body.Location + rotationDirOrtho * driver.HeadOffset)
                       - driver.HeadCenterLocation;
        var headDiffNorm = headDiff.Length;
        if (headDiffNorm < 0.000_000_1)
        {
            headDiffNorm = 0.000_000_1;
        }

        var tmp1 = headDiff * (1.0 / headDiffNorm) * SpringConstant * headDiffNorm * 5.0;
        var tmp2 = (driver.HeadVelocity
                    - ((driver.HeadCenterLocation - driver.Body.Location).Ortho()
                       * driver.Body.RotationSpeed
                       + driver.Body.Velocity))
                   * Damping
                   * 3.0;
        driver.HeadVelocity += ((tmp1 - tmp2) + gravVect * driver.Body.Mass * GravityStrength)
                               * (1.0 / driver.Body.Mass)
                               * timeStep.Value;
        driver.HeadCenterLocation += driver.HeadVelocity * timeStep.Value;
    }

    private Vector ComputeInitialHead(
        Driver driver,
        Vector rotationDir,
        Vector rotationDirOrtho
    )
    {
        var x = driver.Direction switch
        {
            Direction.Right => (driver.Body.Location - driver.HeadCenterLocation).Dotp(rotationDir),
            Direction.Left => (driver.HeadCenterLocation - driver.Body.Location).Dotp(rotationDir),
            _ => throw new ArgumentOutOfRangeException()
        };

        var y = (driver.HeadCenterLocation - driver.Body.Location).Dotp(rotationDirOrtho);
        var v = new Vector(x, y);
        x = (v - _headVec1).Dotp(_headVec2);
        if (x < 0.0)
        {
            v -= _headVec2 * x;
        }

        if (v.Y > 0.48)
        {
            v.Y = 0.48;
        }

        if (v.X < -0.5)
        {
            v.X = -0.5;
        }

        if (v.X > 0.26)
        {
            v.X = 0.26;
        }

        if (v.X > 0.0 && v.Y > 0.0)
        {
            var tmp = HeadFactor * v.X * v.X + v.Y * v.Y;
            if (tmp > HeadThreshold)
            {
                v *= _headFactor2 / Math.Sqrt(tmp);
            }
        }

        return driver.Direction switch
        {
            Direction.Right => driver.Body.Location + rotationDirOrtho * v.Y - rotationDir * v.X,
            Direction.Left => driver.Body.Location + rotationDirOrtho * v.Y + rotationDir * v.X,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private DriverCondition CheckObjectCollision(Driver driver)
    {
        if (!Invulnerable && _ground.GetTouchingEdges(driver.HeadLocation, HeadRadius) is not (null, null))
        {
            return DriverCondition.Dead;
        }

        Array.ForEach(new[]
        {
            (
                BodyPartKind.LeftWheel,
                driver.LeftWheel.Location,
                driver.LeftWheel.Radius
            ),
            (
                BodyPartKind.RightWheel,
                driver.RightWheel.Location,
                driver.RightWheel.Radius
            ),
            (BodyPartKind.Head, driver.HeadLocation, HeadRadius),
        }, tuple =>
        {
            var (_, loc, r) = tuple;
            var objs = _objectTree.GetCollidingObjects(loc, r);
            foreach (var io in objs)
            {
                if (driver.TakenApples.Contains(io.Index))
                {
                    continue;
                }

                _pendingEvents
                    .Enqueue(new PendingEventObject(io));
                if (io.Obj.Type != ObjectType.Apple)
                {
                    continue;
                }

                driver.TakenApples.Add(io.Index);
                driver.TakenAppleEvents.Add(new Event(driver.CurrentTime.Value, new EventTypeApple()));
            }
        });
        return DriverCondition.Alive;
    }

    public bool Invulnerable { get; set; }

    private (Vector, Vector, double, double) ComputeSuspensionForces(
        Driver driver,
        BodyPart bp,
        Vector rotationDir,
        Vector rotationDirOrtho,
        Vector bpOffset,
        double bpRotationForce
    )
    {
        var restBpOffset = rotationDir * bpOffset.X + rotationDirOrtho * bpOffset.Y;
        var diffFromRest = (driver.Body.Location + restBpOffset) - bp.Location;
        var (tmp1, tmp2, tmp3) = (new Vector(), new Vector(), 0.0);
        if (Math.Max(Math.Abs(diffFromRest.X), Math.Abs(diffFromRest.Y)) > 0.0001)
        {
            var restOffsetNorm = restBpOffset.Length;
            var restOffsetUnit = restBpOffset * (1.0 / restOffsetNorm);
            var restOffsetUnitOrtho = restOffsetUnit.Ortho();
            var tmp11 = diffFromRest.Dotp(restOffsetUnitOrtho) * SpringConstant;
            var tmp22 = restOffsetUnit * (diffFromRest.Dotp(restOffsetUnit) * SpringConstant)
                        + restOffsetUnitOrtho * tmp11;
            (tmp1, tmp2, tmp3) = (
                tmp22,
                _defaultWheelForce - tmp22,
                -(restOffsetNorm * tmp11)
            );
        }

        var offset = bp.Location - driver.Body.Location;
        var offsetNormInv = 1.0 / offset.Length;
        var offsetUnit = offset * offsetNormInv;
        var offsetOrtho = offset.Ortho();
        var offsetUnitOrtho = offsetUnit.Ortho();
        var vDiff =
            (offsetOrtho * driver.Body.RotationSpeed + driver.Body.Velocity) - bp.Velocity;
        var tmp4 = offsetUnit * (vDiff.Dotp(offsetUnit) * Damping);
        var tmp5 = offsetUnitOrtho * (vDiff.Dotp(offsetUnitOrtho) * Damping);
        var tmp6 = offsetUnitOrtho * (offsetNormInv * bpRotationForce);
        return (
            ((tmp1 + tmp4) + tmp5) - tmp6,
            ((tmp2 - tmp4) - tmp5) + tmp6,
            tmp3 - tmp5.Dotp(offsetOrtho),
            GetMaxTension(driver, diffFromRest, vDiff)
        );
    }

    private double GetMaxTension(Driver driver, Vector diff, Vector vDiff)
    {
        var r = driver.Body.Rotation - HalfPi;
        var rv = Vector.FromRadians(r);
        var s1 = rv.Dotp(diff);
        var s2 = rv.Dotp(vDiff);
        if (s1 > 0.0 && s2 > 0.0)
        {
            return s2 * SuspensionTensionFactor * s1;
        }

        return 0.0;
    }

    private DriverCondition TakeLevelObject(LevObject obj, Driver bp)
    {
        switch (obj.Type)
        {
            case ObjectType.Flower when bp.TakenApples.Count >= _numApples:
                return DriverCondition.Finished;
            case ObjectType.Apple:
                _pendingEvents.Enqueue(new PendingEventOther(new EventTypeApple()));
                bp.GravityDirection = obj.AppleType switch
                {
                    AppleType.Normal => bp.GravityDirection,
                    AppleType.GravityUp => GravityDirection.Up,
                    AppleType.GravityDown => GravityDirection.Down,
                    AppleType.GravityLeft => GravityDirection.Left,
                    AppleType.GravityRight => GravityDirection.Right,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return DriverCondition.Alive;
            case ObjectType.Killer when !Invulnerable:
                return DriverCondition.Dead;
            default:
                return DriverCondition.Alive;
        }
    }

    private bool ApplyForcesToBodypart(
        BodyPart bp,
        Vector force,
        double angularForce,
        ElmaTime timeStep,
        CollisionCheck collision
    )
    {
        var touched = false;

        void Part1()
        {
            bp.RotationSpeed += angularForce / bp.AngularMass * timeStep.Value;
            bp.Rotation += bp.RotationSpeed * timeStep.Value;
            bp.Velocity += force * (1.0 / bp.Mass) * timeStep.Value;
            bp.Location += bp.Velocity * timeStep.Value;
        }

        void Part2(Vector arg4)
        {
            var (v, s) = bp.TangentFrom(arg4);
            bp.RotationSpeed = (bp.Velocity.Dotp(v)) * (1.0 / bp.Radius);
            bp.RotationSpeed += ((force.Dotp(v)) * bp.Radius + angularForce) /
                (s * bp.Mass * s + bp.AngularMass) * timeStep.Value;
            bp.Rotation += bp.RotationSpeed * timeStep.Value;
            bp.Velocity = v * (bp.RotationSpeed * bp.Radius);
            bp.Location += bp.Velocity * timeStep.Value;
        }

        void Ending((Vector?, Vector?) touches, Queue<PendingEvent> evs, Vector constPt00)
        {
            switch (touches)
            {
                case ({ } p, null) when bp.IsSignificantTouch(p, force) is var (t, f):
                    bp.ApplyGroundTouch(evs, t, f);
                    Part2(p);
                    break;
                case ({ }, null):
                    Part1();
                    break;
                case ({ } p1, { }) when bp.IsSignificantTouch(p1, force) is var (t, f):
                    bp.ApplyGroundTouch(evs, t, f);
                    bp.Velocity = constPt00;
                    bp.RotationSpeed = 0.0;
                    break;
                case ({ }, { } p2):
                    Part2(p2);
                    break;
                default:
                    throw new Exception("unreachable");
            }
        }

        var edges = collision switch
        {
            CollisionCheck.Edges => _ground.GetTouchingEdges(bp.Location, bp.Radius),
            CollisionCheck.None => (null, null),
            _ => throw new ArgumentOutOfRangeException()
        };

        switch (edges)
        {
            case ({ } p1, { } p2):
                MaybePushWheelAway(bp, p1);
                MaybePushWheelAway(bp, p2);
                break;
            case ({ } p1, null):
                MaybePushWheelAway(bp, p1);
                break;
        }

        switch (edges)
        {
            case (null, null):
                Part1();
                break;
            case ({ }, null):
                Ending(edges, _pendingEvents, _defaultWheelForce);
                break;
            case ({ } p1, { } p2):
                var slice = edges;
                if (bp.Velocity.Length > 1.0)
                {
                    if (!bp.FirstTouchIsSignificant(p1, p2))
                    {
                        slice = (p2, null);
                    }

                    if (!bp.FirstTouchIsSignificant(p2, p1))
                    {
                        slice = (p1, null);
                    }
                }

                if (slice is ({ }, { }))
                {
                    if (bp.Velocity.Length >= 1.0)
                    {
                        if (bp.IsSignificantTouch(p2, force) is var (t, f))
                        {
                            bp.ApplyGroundTouch(_pendingEvents, t, f);
                        }
                        else
                        {
                            slice = (p1, null);
                        }
                    }
                    else if (!CheckTouch(p1, p2, bp, force, angularForce))
                    {
                        slice = (p2, null);
                    }
                    else if (CheckTouch(p2, p1, bp, force, angularForce))
                    {
                        if (bp.IsSignificantTouch(p2, force) is var (t, f))
                        {
                            bp.ApplyGroundTouch(_pendingEvents, t, f);
                        }
                        else
                        {
                            slice = (p1, null);
                        }
                    }
                    else
                    {
                        slice = (p1, null);
                    }
                }

                Ending(slice, _pendingEvents, _defaultWheelForce);
                break;
            default:
                throw new Exception("impsy");
        }
        return touched;
    }

    private void MaybePushWheelAway(BodyPart bp, Vector p)
    {
        var (v, dist) = bp.DifferenceFrom(p);
        var min = bp.Radius - 0.005;
        if (dist < min)
        {
            bp.Location += v * (min - dist);
        }
    }

    private bool CheckTouch(
        Vector touch1,
        Vector touch2,
        BodyPart bp,
        Vector force,
        double angularForce
    )
    {
        var (v, s) = bp.TangentFrom(touch2);
        return ((touch1 - touch2).Dotp(v)) * (angularForce + (v * s).Dotp(force)) >= 0.0;
    }
}