using System;
using System.Collections.Generic;
using Elmanager.Lgr;
using Elmanager.Rendering.OpenGL;
using Elmanager.Utilities;
using OpenTK.Mathematics;

namespace Elmanager.Rendering.Scene;

internal class Pictures : IDisposable
{
    private const string VertexShader = @"
        #version 320 es
        precision highp float;
        layout(location = 0) in vec2 a_texcoord;
        layout(location = 1) in vec3 a_position;

        out vec2 v_coord;

        layout(std140, binding = 0) uniform Camera {
            mat4 u_projection;
            vec2 u_camPos;
            float u_grassZoom;
            float u_zoom;
        };

        uniform sampler2D u_texture;

        void main() {
            vec2 s = vec2(textureSize(u_texture, 0)) / 48.0;
            gl_Position = u_projection * vec4(a_position.x + a_texcoord.x * s.x, a_position.y - a_texcoord.y * s.y, a_position.z, 1.0);
            v_coord = a_texcoord;
        }
    ";

    private const string FragmentShader = @"
        #version 320 es
        precision highp float;

        in vec2 v_coord;

        uniform sampler2D u_texture;

        out vec4 color;

        void main() {
            vec4 col = texture(u_texture, v_coord);
            if (col.a == 0.0) {
                discard;
            }
            color = col;
        }
    ";

    internal static Pipeline CreatePipeline(StencilOptions stencil)
    {
        return PipelineBuilder.Create(VertexShader, FragmentShader)
            .WithTextureLocation(0, "u_texture")
            .WithStencil(stencil)
            .WithDepthTest()
            .WithBlend()
            .Build();
    }

    private PictureClipBatch Unclipped { get; }
    private PictureClipBatch Ground { get; }
    private PictureClipBatch Sky { get; }
    private bool ShowPictures { get; }
    private Vertices Quad { get; }

    private Pictures(PictureClipBatch unclipped, PictureClipBatch ground, PictureClipBatch sky, bool showPictures,
        Vertices quad)
    {
        Unclipped = unclipped;
        Ground = ground;
        Sky = sky;
        ShowPictures = showPictures;
        Quad = quad;
    }

    public static Pictures Create(LevEditState state, OpenGlLgr lgr, RenderingSettings settings, Vertices quad)
    {
        var pics = new Pictures(
            new PictureClipBatch(),
            new PictureClipBatch(),
            new PictureClipBatch(),
            settings.ShowPictures,
            quad
        );
        pics.Update(state, lgr);
        return pics;
    }

    private static readonly VertexInfo PerVertexInfo = new VertexInfo()
        .Attr(0, VertexFormat.Float32x2);

    private static readonly VertexInfo InstanceVertexInfo = new VertexInfo()
        .Attr(1, VertexFormat.Float32x3)
        .WithStepMode(VertexStepMode.Instance);

    private static BoundVertexArray CreateBatchBuffer(Vertices quad) =>
        VertexArray.CreateInstanced(quad, PerVertexInfo, InstanceVertexInfo);

    public void Update(LevEditState state, OpenGlLgr lgr)
    {
        var instances = new Dictionary<(string Text, ClippingType Clip), List<Vector3>>();

        foreach (var ge in state.GetGraphicElements())
        {
            if (ge is GraphicElement.Picture p)
            {
                var key = (p.PictureInfo.Name, p.Clipping);
                instances.GetOrCreate(key).Add(new Vector3((float)p.Position.X, (float)p.Position.Y, p.Distance));
            }
        }

        var unclippedPics = new List<(Texture tex, List<Vector3> pos)>();
        var groundPics = new List<(Texture tex, List<Vector3> pos)>();
        var skyPics = new List<(Texture tex, List<Vector3> pos)>();

        foreach (var kvp in instances)
        {
            var (picName, clip) = kvp.Key;
            var positions = kvp.Value;

            if (!lgr.DrawableImages.TryGetValue(picName, out var di))
                continue;

            var tex = di.Texture;
            switch (clip)
            {
                case ClippingType.Unclipped: unclippedPics.Add((tex, positions)); break;
                case ClippingType.Ground: groundPics.Add((tex, positions)); break;
                case ClippingType.Sky: skyPics.Add((tex, positions)); break;
            }
        }

        UpdateClipBatch(Unclipped, unclippedPics);
        UpdateClipBatch(Ground, groundPics);
        UpdateClipBatch(Sky, skyPics);
    }

    private void UpdateClipBatch(PictureClipBatch clipBatch, List<(Texture tex, List<Vector3> pos)> newPics)
    {
        int i = 0;
        foreach (var (tex, positions) in newPics)
        {
            if (i < clipBatch.Pics.Count)
            {
                clipBatch.Pics[i].Texture = tex;
                clipBatch.Pics[i].Update(positions);
            }
            else
            {
                var b = new PictureBatch(tex, CreateBatchBuffer(Quad), positions.Count);
                b.Update(positions);
                clipBatch.Pics.Add(b);
            }

            i++;
        }

        for (; i < clipBatch.Pics.Count; i++)
        {
            clipBatch.Pics[i].Update([]);
        }
    }

    public void Draw(Pipeline pipelineUnclipped, Pipeline pipelineGround, Pipeline pipelineSky)
    {
        if (!ShowPictures) return;

        DrawClipBatch(Ground, pipelineGround);
        DrawClipBatch(Sky, pipelineSky);
        DrawClipBatch(Unclipped, pipelineUnclipped);
    }

    private void DrawClipBatch(PictureClipBatch clipBatch, Pipeline pipeline)
    {
        pipeline.Use();

        foreach (var b in clipBatch.Pics)
        {
            b.InstanceBuffer.Bind();
            b.Texture.Bind();
            Quad.DrawInstanced(b.Count);
            Texture.Unbind();
        }
    }

    public void Dispose()
    {
        Unclipped.Dispose();
        Ground.Dispose();
        Sky.Dispose();
    }
}

internal class PictureBatch(Texture texture, BoundVertexArray instanceBuffer, int count) : IDisposable
{
    public Texture Texture { get; set; } = texture;
    public BoundVertexArray InstanceBuffer { get; } = instanceBuffer;
    public int Count { get; private set; } = count;

    public void Update(List<Vector3> positions)
    {
        var data = new float[positions.Count * 3];
        for (int i = 0; i < positions.Count; i++)
        {
            data[i * 3] = positions[i].X;
            data[i * 3 + 1] = positions[i].Y;
            data[i * 3 + 2] = positions[i].Z;
        }

        InstanceBuffer.SetData(data);
        Count = positions.Count;
    }

    public void Dispose()
    {
        InstanceBuffer.Dispose();
    }
}

internal class PictureClipBatch : IDisposable
{
    public List<PictureBatch> Pics { get; } = new();

    public void Dispose()
    {
        foreach (var b in Pics) b.Dispose();
    }
}
