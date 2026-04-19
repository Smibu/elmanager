using System;
using System.Collections.Generic;
using System.Linq;
using Elmanager.Lev;
using Elmanager.Rendering.OpenGL;

namespace Elmanager.Rendering.Scene;

internal class Objects : IDisposable
{
    private const string VertexShader = @"
        #version 320 es
        precision highp float;
        layout(location = 0) in vec2 a_texcoord;
        layout(location = 1) in vec4 a_instancePos;
        layout(location = 2) in float a_instanceAlpha;

        out vec2 v_coord;
        out float v_alpha;

        layout(std140, binding = 0) uniform Camera {
            mat4 u_projection;
            vec2 u_camPos;
            float u_grassZoom;
            float u_zoom;
        };

        uniform sampler2D u_texture;

        void main() {
            vec2 s = vec2(textureSize(u_texture, 0)) / 48.0;
            gl_Position = u_projection * vec4(a_instancePos.x + a_texcoord.x * s.x, a_instancePos.y - a_texcoord.y * s.y, a_instancePos.z, 1.0);
            v_coord = a_texcoord;
            v_alpha = a_instanceAlpha;
        }
    ";

    private const string FragmentShader = @"
        #version 320 es
        precision highp float;

        in vec2 v_coord;
        in float v_alpha;

        uniform sampler2D u_texture;

        out vec4 color;

        void main() {
            vec4 col = texture(u_texture, v_coord);
            if (col.a == 0.0) {
                discard;
            }
            color = vec4(col.rgb, col.a * v_alpha);
        }
    ";

    internal static Pipeline CreatePipeline() => PipelineBuilder.Create(VertexShader, FragmentShader)
        .WithTextureLocation(0, "u_texture")
        .WithStencil(Pipelines.StencilUnclipped)
        .WithDepthTest()
        .WithBlend()
        .Build();

    public List<AppleBatch> Apples { get; private set; }
    public ObjectBatch Killers { get; }
    public ObjectBatch Flowers { get; }
    public ObjectBatch GravityAppleArrows { get; private set; }
    private Vertices Quad { get; }
    private bool ShowObjects { get; }
    private bool PicturesInBackground { get; }

    private Objects(Vertices quad, List<AppleBatch> apples, ObjectBatch killers, ObjectBatch flowers,
        ObjectBatch gravityAppleArrows,
        bool showObjects, bool picturesInBackground)
    {
        Quad = quad;
        Apples = apples;
        Killers = killers;
        Flowers = flowers;
        GravityAppleArrows = gravityAppleArrows;
        ShowObjects = showObjects;
        PicturesInBackground = picturesInBackground;
    }

    public void SetVisibleObjects(Level lev, HashSet<int>? hiddenObjects, HashSet<int>? fadedObjects)
    {
        var visible = GetVisibleObjects(new LevEditState(lev, TransientElements.Empty), picturesInBackground: false, hiddenObjects: hiddenObjects, fadedObjects: fadedObjects);
        DisposeAppleBatches();
        Apples = CreateAppleBatches(visible.Apples, Quad);
        GravityAppleArrows = CreateBatchBuffer(visible.GravityAppleArrows, Quad);
    }

    public void Update(LevEditState state, LevVisualChange mod)
    {
        var visible = GetVisibleObjects(state, picturesInBackground: PicturesInBackground);

        if (mod.HasFlag(LevVisualChange.Killers))
        {
            Killers.Update(visible.Killers);
        }

        if (mod.HasFlag(LevVisualChange.Flowers))
        {
            Flowers.Update(visible.Flowers);
        }

        if (mod.HasFlag(LevVisualChange.Apples))
        {
            UpdateAppleBatches(visible.Apples);
            GravityAppleArrows.Update(visible.GravityAppleArrows);
        }
    }

    private void UpdateAppleBatches(Dictionary<int, List<float[]>> apples)
    {
        int i = 0;
        foreach (var (animNum, positions) in apples)
        {
            if (i < Apples.Count)
            {
                Apples[i].AnimNum = animNum;
                Apples[i].Batch.Update(positions);
            }
            else
            {
                Apples.Add(new AppleBatch(animNum, CreateBatchBuffer(positions, Quad)));
            }
            i++;
        }

        for (; i < Apples.Count; i++)
        {
            Apples[i].AnimNum = 0;
            Apples[i].Batch.Update([]);
        }
    }

    public static Objects Create(LevEditState state, RenderingSettings settings, Vertices quad)
    {
        var visible = GetVisibleObjects(state, picturesInBackground: settings.PicturesInBackground);

        return new Objects(
            quad: quad,
            apples: CreateAppleBatches(visible.Apples, quad),
            killers: CreateBatchBuffer(visible.Killers, quad),
            flowers: CreateBatchBuffer(visible.Flowers, quad),
            gravityAppleArrows: CreateBatchBuffer(visible.GravityAppleArrows, quad),
            showObjects: settings.ShowObjects,
            picturesInBackground: settings.PicturesInBackground
        );
    }

    private static List<AppleBatch> CreateAppleBatches(Dictionary<int, List<float[]>> apples, Vertices quad)
    {
        var result = new List<AppleBatch>();
        foreach (var (animNum, positions) in apples)
        {
            result.Add(new AppleBatch(
                animNum: animNum,
                batch: CreateBatchBuffer(positions, quad)
            ));
        }

        return result;
    }

    private static VisibleObjects GetVisibleObjects(
        LevEditState state,
        bool picturesInBackground,
        HashSet<int>? hiddenObjects = null,
        HashSet<int>? fadedObjects = null)
    {
        var apples = new Dictionary<int, List<float[]>>();
        var killers = new List<float[]>();
        var flowers = new List<float[]>();
        var gravityApples = new List<float[]>();
        float distance = picturesInBackground ? 0f : 500.0f;
        var sortedObjs = state.GetObjects().OrderBy(a => LevObject.ObjSortOrder(a.Type));
        float halfPicSize = 40.0f / 48.0f / 2.0f;

        foreach (var (idx, obj) in sortedObjs.Select((obj, index) => (index, obj)))
        {
            if (hiddenObjects != null && hiddenObjects.Contains(idx))
                continue;

            float alpha = fadedObjects != null && fadedObjects.Contains(idx) ? 0.3f : 1.0f;
            float[] pos =
            [
                (float)obj.Position.X - halfPicSize,
                (float)obj.Position.Y + halfPicSize,
                distance - (idx + 1) * 0.0002f,
                0.0f,
                alpha
            ];

            switch (obj.Type)
            {
                case ObjectType.Apple:
                    if (!apples.TryGetValue(obj.AnimationNumber, out var list))
                    {
                        list = [];
                        apples[obj.AnimationNumber] = list;
                    }

                    list.Add(pos);

                    if (obj.AppleType != AppleType.Normal)
                    {
                        float rotation = obj.AppleType switch
                        {
                            AppleType.GravityUp => 0.0f,
                            AppleType.GravityDown => MathF.PI,
                            AppleType.GravityLeft => MathF.PI / 2.0f,
                            AppleType.GravityRight => -MathF.PI / 2.0f,
                            _ => 0.0f
                        };
                        gravityApples.Add([pos[0], pos[1], pos[2], rotation, alpha]);
                    }

                    break;
                case ObjectType.Flower:
                    flowers.Add(pos);
                    break;
                case ObjectType.Killer:
                    killers.Add(pos);
                    break;
                case ObjectType.Start:
                    break;
            }
        }

        return new VisibleObjects(apples, killers, flowers, gravityApples);
    }

    public void Draw(
        OpenGlLgr lgr,
        double time,
        bool animatedObjects,
        Pipeline pipeline)
    {
        if (!ShowObjects) return;

        pipeline.Use();

        DrawObjectBatch(Quad, Flowers, [lgr.Flower], time, animatedObjects);
        foreach (var ab in Apples)
        {
            var frames = GetAppleFrames(lgr, ab.AnimNum);
            DrawObjectBatch(Quad, ab.Batch, frames, time, animatedObjects);
        }
        DrawObjectBatch(Quad, Killers, [lgr.Killer], time, animatedObjects);
    }

    private static void DrawObjectBatch(Vertices quad, ObjectBatch batch, Texture[] frames, double time, bool animatedObjects)
    {
        if (batch.Count == 0 || frames.Length == 0) return;

        int frameIndex = animatedObjects
            ? GetAnimFrameIndex(time, 40.0, frames.Length)
            : 0;

        frames[frameIndex].Bind();

        batch.InstanceBuffer.Bind();
        quad.DrawInstanced(batch.Count);
        Texture.Unbind();
    }

    private static int GetAnimFrameIndex(double time, double speed, int frames)
    {
        return (int)(time * speed) % frames;
    }

    private static Texture[] GetAppleFrames(OpenGlLgr lgr, int animNum)
    {
        if (lgr.Apples.TryGetValue(animNum, out var tex))
            return [tex];
        if (lgr.Apples.TryGetValue(1, out var fallback))
            return [fallback];
        return [];
    }

    private static readonly VertexInfo PerVertexInfo = new VertexInfo()
        .Attr(0, VertexFormat.Float32x2);

    internal static readonly VertexInfo InstanceVertexInfo = new VertexInfo()
        .Attr(1, VertexFormat.Float32x4)
        .Attr(2, VertexFormat.Float32)
        .WithStepMode(VertexStepMode.Instance);

    private static ObjectBatch CreateBatchBuffer(List<float[]> positions, Vertices quad)
    {
        var batch = new ObjectBatch(CreateInstanceBuffer(quad, InstanceVertexInfo), 0);
        batch.Update(positions);
        return batch;
    }

    private static BoundVertexArray CreateInstanceBuffer(Vertices geometry, VertexInfo instanceVertexInfo)
    {
        return VertexArray.CreateInstanced(geometry, PerVertexInfo, instanceVertexInfo);
    }

    private void DisposeBatches()
    {
        DisposeAppleBatches();
        Killers.Dispose();
        Flowers.Dispose();
    }

    private void DisposeAppleBatches()
    {
        foreach (var ab in Apples) ab.Dispose();
        GravityAppleArrows.Dispose();
    }

    public void Dispose()
    {
        DisposeBatches();
    }
}

internal class VisibleObjects(
    Dictionary<int, List<float[]>> apples,
    List<float[]> killers,
    List<float[]> flowers,
    List<float[]> gravityAppleArrows)
{
    public Dictionary<int, List<float[]>> Apples { get; } = apples;
    public List<float[]> Killers { get; } = killers;
    public List<float[]> Flowers { get; } = flowers;
    public List<float[]> GravityAppleArrows { get; } = gravityAppleArrows;
}

internal class ObjectBatch(BoundVertexArray instanceBuffer, int count) : IDisposable
{
    public BoundVertexArray InstanceBuffer { get; } = instanceBuffer;
    public int Count { get; private set; } = count;

    public void Update(List<float[]> positions)
    {
        var flat = new float[positions.Count * 5];
        for (int i = 0; i < positions.Count; i++)
        {
            flat[i * 5 + 0] = positions[i][0];
            flat[i * 5 + 1] = positions[i][1];
            flat[i * 5 + 2] = positions[i][2];
            flat[i * 5 + 3] = positions[i].Length > 3 ? positions[i][3] : 0.0f;
            flat[i * 5 + 4] = positions[i].Length > 4 ? positions[i][4] : 1.0f;
        }

        InstanceBuffer.SetData(flat);
        Count = positions.Count;
    }

    public void Dispose()
    {
        InstanceBuffer.Dispose();
    }
}

internal class AppleBatch(int animNum, ObjectBatch batch) : IDisposable
{
    public int AnimNum { get; set; } = animNum;
    public ObjectBatch Batch { get; } = batch;

    public void Dispose() => Batch.Dispose();
}
