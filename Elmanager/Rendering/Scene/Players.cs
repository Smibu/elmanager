using System;
using System.Collections.Generic;
using System.Linq;
using Elmanager.ElmaPrimitives;
using Elmanager.Rec;
using Elmanager.Rendering.OpenGL;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Elmanager.Rendering.Scene;

internal class Players : IDisposable
{
    private const string PlayerVertShader = @"
        #version 320 es
        precision highp float;
        layout(location = 0) in vec2 a_texcoord;
        layout(location = 1) in vec4 a_rotScale;
        layout(location = 2) in vec4 a_translateOp;

        out vec2 v_coord;
        out float v_opacity;

        layout(std140, binding = 0) uniform Camera {
            mat4 u_projection;
            vec2 u_camPos;
            float u_grassZoom;
            float u_zoom;
        };

        void main() {
            vec2 pos = a_texcoord - vec2(0.5);
            vec2 transformed = vec2(
                a_rotScale.x * pos.x + a_rotScale.z * pos.y + a_translateOp.x,
                a_rotScale.y * pos.x + a_rotScale.w * pos.y + a_translateOp.y
            );
            gl_Position = u_projection * vec4(transformed, a_translateOp.z, 1.0);
            v_coord = a_texcoord;
            v_opacity = a_translateOp.w;
        }
    ";

    private const string PlayerFragShader = @"
        #version 320 es
        precision highp float;

        in vec2 v_coord;
        in float v_opacity;

        uniform sampler2D u_texture;

        out vec4 color;

        void main() {
            vec4 col = texture(u_texture, v_coord);
            if (col.a == 0.0) {
                discard;
            }
            color = vec4(col.rgb, col.a * v_opacity);
        }
    ";

    internal static Pipeline CreatePipeline() => PipelineBuilder.Create(PlayerVertShader, PlayerFragShader)
        .WithStencil(Pipelines.StencilUnclipped)
        .WithDepthTest()
        .WithBlend()
        .Build();

    private const float HeadTurnOffset = 0.0915f;
    private const float ThighScale = 0.28f;
    private const float ThighXScale = 168.0f / 60.0f * 0.83f;
    private static readonly Vector2 ThighOffset = new(0.5f, -0.591f);
    private const float ThighJointOffset = 0.114f;

    private const float LegScale = 0.33f;
    private const float LegXScale = 159.0f / 88.0f * 1.0f;
    private const float LegYScale = 1.15f;
    private static readonly Vector2 LegOffsetBike = new(0.08f, -0.475f);
    private const float LegJointOffsetX = 0.025f;
    private const float LegJointOffsetY = -0.01f;
    private const float LegLength = LegXScale * LegScale * (1.0f - LegJointOffsetX - 0.025f);
    private const float ThighLength = ThighXScale * ThighScale * (1.0f - ThighJointOffset - 0.075f);

    private const float UpArmScale = 0.22f;
    private const float UpArmXScale = 180.0f / 72.0f * 0.95f;
    private static readonly Vector2 UpArmOffset = new(0.11f, -0.22f);
    private const float UpArmJointOffset = 0.1f;
    private const float UpArmLength = UpArmXScale * UpArmScale * (1.0f - UpArmJointOffset - 0.1f);

    private const float ForarmScale = 0.15f;
    private const float ForarmXScale = 238.0f / 68.0f * 0.95f;
    private static readonly Vector2 ForarmRestOffsetBike = new(-0.43f, 0.4f);
    private const float ForarmJointOffset = 0.1f;
    private const float ForarmLength = ForarmXScale * ForarmScale * (1.0f - ForarmJointOffset - 0.134f);

    private static readonly Vector2 Susp1Offset = new(-0.423f, 0.397f);
    private const float Susp1JointOffsetBike = 0.04f;
    private const float Susp1JointOffsetWheel = 0.0625f;

    private static readonly Vector2 Susp2Offset = new(0.1f, -0.37f);
    private const float Susp2JointOffsetWheel = 0.13f;
    private const float Susp2JointOffsetBikeX = 0.03f;
    private const float Susp2JointOffsetBikeY = -0.01f;

    private const float SuspYScale = 0.12f;

    private const float BodyScale = 0.43f;
    private const float BodyXScale = 149.0f / 101.0f * 1.10f;
    private const float BodyYScale = 0.95f;
    private static readonly Vector2 BodyOffset = new(0.2924f, -0.4515f);
    private const float BodyBaseRotation = 1.215f * MathF.PI;

    private const float BikeScale = 1.355f;
    private const float BikeXScale = 380.0f / 301.0f;
    private const float BikeBaseRotation = -MathF.PI * 0.198f;
    private static readonly Vector2 BikeRotationOffset = new(0.16f, -0.032f);

    private const int ForarmDist = 0;
    private const int UpArmDist = 1;
    private const int BodyDist = 2;
    private const int HeadDist = 3;
    private const int LegDist = 4;
    private const int ThighDist = 5;
    private const int BikeDist = 6;
    private const int SuspDist = 7;
    private const int WheelDist = 8;
    private const float HeadDiameter = (float)ElmaConstants.HeadDiameter;
    private const float ObjectDiameter = (float)(OpenGlLgr.ObjectRadius * 2);

    private BoundVertexArray Head { get; }
    private BoundVertexArray Body { get; }
    private BoundVertexArray LeftWheel { get; }
    private BoundVertexArray RightWheel { get; }
    private BoundVertexArray Bike { get; }
    private BoundVertexArray Thigh { get; }
    private BoundVertexArray Leg { get; }
    private BoundVertexArray Susp1 { get; }
    private BoundVertexArray Susp2 { get; }
    private BoundVertexArray UpArm { get; }
    private BoundVertexArray Forarm { get; }
    private Vertices Quad { get; }
    private int Count { get; set; }
    private BikeTextures Textures { get; }

    private Players(Vertices quad, BoundVertexArray head, BoundVertexArray body, BoundVertexArray leftWheel, BoundVertexArray rightWheel, BoundVertexArray bike, BoundVertexArray thigh,
        BoundVertexArray leg, BoundVertexArray susp1, BoundVertexArray susp2, BoundVertexArray upArm, BoundVertexArray forarm,
        BikeTextures textures)
    {
        Quad = quad;
        Head = head;
        Body = body;
        LeftWheel = leftWheel;
        RightWheel = rightWheel;
        Bike = bike;
        Thigh = thigh;
        Leg = leg;
        Susp1 = susp1;
        Susp2 = susp2;
        UpArm = upArm;
        Forarm = forarm;
        Textures = textures;
    }

    public static Players Create(OpenGlLgr lgr, Vertices quad)
    {
        return new Players(
            quad: quad,
            head: CreateInstanceBuffer(quad),
            body: CreateInstanceBuffer(quad),
            leftWheel: CreateInstanceBuffer(quad),
            rightWheel: CreateInstanceBuffer(quad),
            bike: CreateInstanceBuffer(quad),
            thigh: CreateInstanceBuffer(quad),
            leg: CreateInstanceBuffer(quad),
            susp1: CreateInstanceBuffer(quad),
            susp2: CreateInstanceBuffer(quad),
            upArm: CreateInstanceBuffer(quad),
            forarm: CreateInstanceBuffer(quad),
            textures: lgr.Bike
        );
    }

    private static readonly VertexInfo PerVertexInfo = new VertexInfo()
        .Attr(0, VertexFormat.Float32x2);

    private static readonly VertexInfo InstanceVertexInfo = new VertexInfo()
        .Attr(1, VertexFormat.Float32x4)
        .Attr(2, VertexFormat.Float32x4)
        .WithStepMode(VertexStepMode.Instance);

    private static BoundVertexArray CreateInstanceBuffer(Vertices quad)
    {
        return VertexArray.CreateInstanced(quad, PerVertexInfo, InstanceVertexInfo);
    }

    public void Draw(Pipeline pipeline)
    {
        if (Count == 0) return;

        pipeline.Use();

        var bindings = new[]
        {
            (Head, Textures.Head),
            (LeftWheel, Textures.Wheel),
            (RightWheel, Textures.Wheel),
            (Bike, Textures.Bike),
            (Body, Textures.Body),
            (Thigh, Textures.Thigh),
            (Leg, Textures.Leg),
            (Susp1, Textures.Susp1),
            (Susp2, Textures.Susp2),
            (UpArm, Textures.UpArm),
            (Forarm, Textures.Forarm)
        };

        foreach (var (buffer, tx) in bindings)
        {
            buffer.Bind();
            tx.Bind();
            Quad.DrawInstanced(Count);
        }

        Texture.Unbind();
    }

    private static float Dist(float baseDistance, int part, int player)
    {
        return baseDistance + part * 0.0001f + player * 0.01f;
    }

    private static float DirToF(Direction d) => d == Direction.Left ? -1f : 1f;

    public void UpdateBuffers(
        IReadOnlyList<VisualPlayer> states)
    {
        Count = states.Count;

        SetBuffer(Head, states, (i, s) =>
            MatHead(s.HeadNoTurn, HeadDiameter, s.TurnProgress, Dist(s.BaseDistance, HeadDist, i)));

        SetBuffer(Body, states, (i, s) =>
            Matrix4.CreateScale(new Vector3(BodyXScale, BodyYScale, 1.0f))
            * Matrix4.CreateRotationZ(BodyBaseRotation)
            * Matrix4.CreateTranslation(new Vector3(Vec3YNeg(BodyOffset)) / BodyScale)
            * MatHead(s.HeadNoTurn, BodyScale, s.TurnProgress, Dist(s.BaseDistance, BodyDist, i)));

        var armLegAngles = states.Select(s => (arm: GetArmAngles(s), leg: GetLegAngles(s))).ToArray();

        SetBuffer(UpArm, states, armLegAngles, (i, s, angles) =>
            Matrix4.CreateTranslation(new Vector3(-0.5f + UpArmJointOffset, 0.0f, 0.0f))
            * Matrix4.CreateScale(new Vector3(UpArmXScale, -1.0f, 1.0f))
            * Matrix4.CreateRotationZ(-angles.arm.angle2 + MathF.PI)
            * Matrix4.CreateTranslation(new Vector3(Vec3YNeg(UpArmOffset)) / UpArmScale)
            * MatHead(s.HeadNoTurn, UpArmScale, s.TurnProgress, Dist(s.BaseDistance, UpArmDist, i)));

        SetBuffer(Forarm, states, armLegAngles, (i, s, angles) =>
            Matrix4.CreateTranslation(new Vector3(-0.5f + ForarmJointOffset, 0.0f, 0.0f))
            * Matrix4.CreateScale(new Vector3(ForarmXScale, 1.0f, 1.0f))
            * Matrix4.CreateRotationZ(-angles.arm.angle1)
            * Matrix4.CreateTranslation(new Vector3(Vec3YNeg(angles.arm.pos)) / ForarmScale)
            * MatHead(s.HeadNoTurn, ForarmScale, s.TurnProgress, Dist(s.BaseDistance, ForarmDist, i)));

        SetBuffer(Thigh, states, armLegAngles, (i, s, angles) =>
            Matrix4.CreateTranslation(new Vector3(-0.5f + ThighJointOffset, 0.0f, 0.0f))
            * Matrix4.CreateScale(new Vector3(ThighXScale, 1.0f, 1.0f))
            * Matrix4.CreateRotationZ(-angles.leg.angle2 + MathF.PI)
            * Matrix4.CreateTranslation(new Vector3(Vec3YNeg(ThighOffset)) / ThighScale)
            * MatHead(s.HeadNoTurn, ThighScale, s.TurnProgress, Dist(s.BaseDistance, ThighDist, i)));

        SetBuffer(Leg, states, armLegAngles, (i, s, angles) =>
            Matrix4.CreateTranslation(new Vector3(-0.5f + LegJointOffsetX, LegJointOffsetY, 0.0f))
            * Matrix4.CreateScale(new Vector3(LegXScale, LegYScale, 1.0f))
            * Matrix4.CreateRotationZ(-angles.leg.angle1)
            * Matrix4.CreateTranslation(new Vector3(Vec3YNeg(angles.leg.pos)) / LegScale)
            * MatHead(s.HeadNoTurn, LegScale, s.TurnProgress, Dist(s.BaseDistance, LegDist, i)));

        SetBuffer(Bike, states, (i, s) =>
            Matrix4.CreateTranslation(new Vector3(BikeRotationOffset.X, BikeRotationOffset.Y, 0.0f))
            * Matrix4.CreateScale(new Vector3(BikeXScale, 1.0f, 1.0f))
            * Matrix4.CreateRotationZ(BikeBaseRotation)
            * MatBody(s.Body, BikeScale, s.TurnProgress, Dist(s.BaseDistance, BikeDist, i)));

        SetBuffer(Susp1, states, (i, s) =>
        {
            var wheel = s.Direction == Direction.Left ? s.LeftWheel : s.RightWheel;
            var wheelRelative = WheelRelativePos(wheel, s);
            var diff = wheelRelative - Susp1Offset;
            var angle = AngleBetween(Vector2.UnitX, diff);

            return Matrix4.CreateTranslation(new Vector3(-0.5f + Susp1JointOffsetBike, 0.0f, 0.0f))
                   * Matrix4.CreateScale(new Vector3(
                       diff.Length + (Susp1JointOffsetBike + Susp1JointOffsetWheel) * Math.Min(diff.Length, 1.0f),
                       SuspYScale,
                       1.0f))
                   * Matrix4.CreateRotationZ(-angle + MathF.PI)
                   * Matrix4.CreateTranslation(new Vector3(Vec3YNeg(Susp1Offset)))
                   * MatBody(s.Body, 1.0f, s.TurnProgress, Dist(s.BaseDistance, SuspDist, i));
        });

        SetBuffer(Susp2, states, (i, s) =>
        {
            var wheel = s.Direction == Direction.Left ? s.RightWheel : s.LeftWheel;
            var wheelRelative = WheelRelativePos(wheel, s);
            var diff = wheelRelative - Susp2Offset;
            var angle = AngleBetween(Vector2.UnitX, diff);

            return Matrix4.CreateTranslation(new Vector3(0.5f - Susp2JointOffsetBikeX, Susp2JointOffsetBikeY, 0.0f))
                   * Matrix4.CreateScale(new Vector3(
                       diff.Length + (Susp2JointOffsetBikeX + Susp2JointOffsetWheel) * Math.Min(diff.Length, 1.0f),
                       SuspYScale,
                       1.0f))
                   * Matrix4.CreateRotationZ(-angle)
                   * Matrix4.CreateTranslation(new Vector3(Vec3YNeg(Susp2Offset)))
                   * MatBody(s.Body, 1.0f, s.TurnProgress, Dist(s.BaseDistance, SuspDist, i));
        });

        SetBuffer(LeftWheel, states, (i, s) =>
            MatBody(s.LeftWheel, ObjectDiameter, 1.0f, Dist(s.BaseDistance, WheelDist, i)));

        SetBuffer(RightWheel, states, (i, s) =>
            MatBody(s.RightWheel, ObjectDiameter, 1.0f, Dist(s.BaseDistance, WheelDist, i)));
    }

    private static Vector2 WheelRelativePos(VisualBikePart wheel, VisualPlayer s)
    {
        var v = UndoRotation(wheel.Center - s.Body.Center, s.Body.Rotation);
        return new Vector2(v.X * (DirToF(s.Direction) * -1.0f), v.Y);
    }

    private static Vector2 GetHeadOffset(VisualPlayer s)
    {
        return UndoRotation(s.HeadNoTurn.Center - s.Body.Center, s.Body.Rotation);
    }

    private static (float angle1, float angle2, Vector2 pos) GetLegAngles(VisualPlayer s)
    {
        var headOffset = GetHeadOffset(s);
        var legOffset = -(new Vector2(
            headOffset.X * DirToF(s.Direction) * -1.0f - HeadTurnOffset,
            headOffset.Y)) + LegOffsetBike;

        var angles = CircleIntersectionAngles(legOffset, LegLength, ThighOffset, ThighLength, Intersection.First);
        if (angles is { } a)
        {
            return (a.Angle1, a.Angle2, legOffset + FromAngle(a.Angle1) * LegLength);
        }
        else
        {
            var angle = AngleBetween(Vector2.UnitX, ThighOffset - legOffset);
            return (angle, angle + MathF.PI, legOffset + FromAngle(angle) * LegLength);
        }
    }

    private static (float angle1, float angle2, Vector2 pos) GetArmAngles(VisualPlayer s)
    {
        var circleCenter = UpArmOffset;
        var headOffset = GetHeadOffset(s);

        var forarmRestOffset = -(new Vector2(
            headOffset.X * DirToF(s.Direction) * -1.0f - HeadTurnOffset,
            headOffset.Y)) + ForarmRestOffsetBike;

        var diff = forarmRestOffset - circleCenter;

        Vector2 forarmOffset;
        if (s.ArmProgress >= 0.0f)
        {
            float peakAngle = MathF.PI * 0.8f;
            float currAngle = peakAngle * s.ArmProgress;
            float baseAngle = AngleBetween(Vector2.UnitX, diff);
            float finalAngle = baseAngle - currAngle;
            forarmOffset = circleCenter + FromAngle(finalAngle) * (diff.Length * (1.0f - s.ArmProgress * 0.5f));
        }
        else
        {
            float peakAngle = MathF.PI * 0.5f;
            float currAngle = peakAngle * s.ArmProgress;
            float baseAngle = AngleBetween(Vector2.UnitX, diff);
            float finalAngle = baseAngle - currAngle;
            float maxLength = ForarmLength + UpArmLength;
            forarmOffset = circleCenter + FromAngle(finalAngle) *
                (diff.Length + (maxLength - diff.Length) * s.ArmProgress * -1.0f);
        }

        var angles = CircleIntersectionAngles(forarmOffset, ForarmLength, UpArmOffset, UpArmLength, Intersection.Second);
        if (angles is { } a)
        {
            return (a.Angle1, a.Angle2, forarmOffset + FromAngle(a.Angle1) * ForarmLength);
        }
        else
        {
            var angle = AngleBetween(Vector2.UnitX, UpArmOffset - forarmOffset);
            return (angle, angle + MathF.PI, forarmOffset + FromAngle(angle) * ForarmLength);
        }
    }

    private static Vector2 Vec3YNeg(Vector2 v) => new(v.X, -v.Y);

    private static void SetBuffer(
        BoundVertexArray buffer,
        IReadOnlyList<VisualPlayer> states,
        Func<int, VisualPlayer, Matrix4> matFn)
    {
        if (states.Count == 0) return;
        var flat = new float[states.Count * 8];
        for (int i = 0; i < states.Count; i++)
        {
            var mat = matFn(i, states[i]);
            flat[i * 8 + 0] = mat.M11;
            flat[i * 8 + 1] = mat.M12;
            flat[i * 8 + 2] = mat.M21;
            flat[i * 8 + 3] = mat.M22;
            flat[i * 8 + 4] = mat.M41;
            flat[i * 8 + 5] = mat.M42;
            flat[i * 8 + 6] = mat.M43;
            flat[i * 8 + 7] = states[i].Opacity;
        }

        buffer.SetData(flat, BufferUsageHint.StreamDraw);
    }

    private static void SetBuffer<TU>(
        BoundVertexArray buffer,
        IReadOnlyList<VisualPlayer> states,
        IReadOnlyList<TU> extra,
        Func<int, VisualPlayer, TU, Matrix4> matFn)
    {
        if (states.Count == 0) return;
        var flat = new float[states.Count * 8];
        for (int i = 0; i < states.Count; i++)
        {
            var mat = matFn(i, states[i], extra[i]);
            flat[i * 8 + 0] = mat.M11;
            flat[i * 8 + 1] = mat.M12;
            flat[i * 8 + 2] = mat.M21;
            flat[i * 8 + 3] = mat.M22;
            flat[i * 8 + 4] = mat.M41;
            flat[i * 8 + 5] = mat.M42;
            flat[i * 8 + 6] = mat.M43;
            flat[i * 8 + 7] = states[i].Opacity;
        }

        buffer.SetData(flat, BufferUsageHint.StreamDraw);
    }

    private static Matrix4 MatBody(VisualBikePart bikePart, float diameter, float xScale, float distance)
    {
        return Matrix4.CreateScale(new Vector3(diameter * xScale, diameter, 1.0f))
               * Matrix4.CreateRotationZ(bikePart.Rotation + MathF.PI)
               * Matrix4.CreateTranslation(bikePart.Center.X, bikePart.Center.Y, distance);
    }

    private static Matrix4 MatHead(VisualBikePart bikePart, float diameter, float xScale, float distance)
    {
        return Matrix4.CreateTranslation(new Vector3(-HeadTurnOffset / diameter, 0.0f, 0.0f))
               * Matrix4.CreateScale(new Vector3(diameter * xScale, diameter, 1.0f))
               * Matrix4.CreateRotationZ(bikePart.Rotation + MathF.PI)
               * Matrix4.CreateTranslation(bikePart.Center.X, bikePart.Center.Y, distance);
    }

    private static Vector2 UndoRotation(Vector2 v, float bikeRot)
    {
        float cos = MathF.Cos(-bikeRot);
        float sin = MathF.Sin(-bikeRot);
        return new Vector2(v.X * cos - v.Y * sin, v.X * sin + v.Y * cos);
    }

    private static float AngleBetween(Vector2 v1, Vector2 v2)
    {
        return MathF.Atan2(v1.X * v2.Y - v1.Y * v2.X, v1.X * v2.X + v1.Y * v2.Y);
    }

    private static Vector2 FromAngle(float angle)
    {
        return new Vector2(MathF.Cos(angle), MathF.Sin(angle));
    }

    private enum Intersection
    {
        First,
        Second
    }

    private static (float Angle1, float Angle2)? CircleIntersectionAngles(
        Vector2 center1, float radius1,
        Vector2 center2, float radius2,
        Intersection isect)
    {
        var diff = center2 - center1;
        float dist = diff.Length;

        if (dist > radius1 + radius2 || dist < Math.Abs(radius1 - radius2))
        {
            return null;
        }

        var basePt = 0.5f * (center1 + center2) + (radius1 * radius1 - radius2 * radius2) / (2.0f * dist * dist) * diff;
        float term = 2.0f * (radius1 * radius1 + radius2 * radius2) / (dist * dist)
                     - (radius1 * radius1 - radius2 * radius2) * (radius1 * radius1 - radius2 * radius2) /
                     (dist * dist * dist * dist)
                     - 1.0f;

        var offset = 0.5f * MathF.Sqrt(Math.Max(0.0f, term)) *
                     new Vector2(center2.Y - center1.Y, center1.X - center2.X);

        var intersection = isect == Intersection.First ? basePt - offset : basePt + offset;

        var angle1 = AngleBetween(Vector2.UnitX, intersection - center1);
        var angle2 = AngleBetween(Vector2.UnitX, intersection - center2);

        return (angle1, angle2);
    }

    public void Dispose()
    {
        Head.Dispose();
        Body.Dispose();
        LeftWheel.Dispose();
        RightWheel.Dispose();
        Bike.Dispose();
        Thigh.Dispose();
        Leg.Dispose();
        Susp1.Dispose();
        Susp2.Dispose();
        UpArm.Dispose();
        Forarm.Dispose();
    }
}

internal struct VisualBikePart
{
    public Vector2 Center;
    public float Rotation;
}

internal struct VisualPlayer
{
    public VisualBikePart Body;
    public VisualBikePart HeadNoTurn;
    public VisualBikePart LeftWheel;
    public VisualBikePart RightWheel;

    /// <summary>
    /// Value between -1 (= fully facing left) and 1 (= fully facing right).
    /// </summary>
    public float TurnProgress;

    /// <summary>
    /// Value between -1 and 1.
    /// 0 = no rotation (arm resting on bike)
    /// 1 = arm at highest position (near head)
    /// -1 = arm at lowest position (near "8" sign of bike)
    /// </summary>
    public float ArmProgress;

    public Direction Direction;
    public float BaseDistance;
    public float Opacity;
}
