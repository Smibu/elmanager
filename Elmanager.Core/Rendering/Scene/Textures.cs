using System;
using System.Collections.Generic;
using Elmanager.Lgr;
using Elmanager.Rendering.OpenGL;
using Elmanager.Utilities;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Elmanager.Rendering.Scene;

internal class Textures : IDisposable
{
    private const string VertexShader = @"
        #version 320 es
        precision highp float;
        layout(location = 0) in vec2 a_texcoord;
        layout(location = 1) in vec3 a_position;

        out vec2 v_coord;
        out vec2 v_pos;

        layout(std140, binding = 0) uniform Camera {
            mat4 u_projection;
            vec2 u_camPos;
            float u_grassZoom;
            float u_zoom;
        };

        uniform sampler2D u_texture;
        uniform sampler2D u_mask;

        void main() {
            vec2 s = vec2(textureSize(u_mask, 0)) / 48.0;
            vec2 pos = vec2(a_position.x + a_texcoord.x * s.x, a_position.y - a_texcoord.y * s.y);
            gl_Position = u_projection * vec4(pos, a_position.z, 1.0);
            v_coord = a_texcoord;
            v_pos = pos;
        }
    ";

    private const string FragmentShader = @"
        #version 320 es
        precision highp float;

        in vec2 v_coord;
        in vec2 v_pos;

        uniform sampler2D u_texture;
        uniform sampler2D u_mask;

        out vec4 color;

        layout(std140, binding = 0) uniform Camera {
            mat4 u_projection;
            vec2 u_camPos;
            float u_grassZoom;
            float u_zoom;
        };

        void main() {
            float maskAlpha = texture(u_mask, v_coord).a;
            if (maskAlpha == 0.0) {
               discard;
            }

            vec2 texSize = vec2(textureSize(u_texture, 0));
            float aspect = texSize.x / texSize.y;
            float f = 48.0 / texSize.y;
            vec2 fix = vec2(v_pos.x - u_camPos.x * 0.5, -v_pos.y + u_camPos.y);
            vec2 coords = vec2(fix.x * f / aspect, fix.y * f) / u_zoom;
            color = texture(u_texture, coords);
        }
    ";

    internal static Pipeline CreatePipeline(StencilOptions stencil)
    {
        return PipelineBuilder.Create(VertexShader, FragmentShader)
            .WithStencil(stencil)
            .WithDepthTest()
            .WithBlend()
            .WithTextureLocation(0, "u_texture")
            .WithTextureLocation(1, "u_mask")
            .Build();
    }

    private TextureClipBatch Unclipped { get; }
    private TextureClipBatch Ground { get; }
    private TextureClipBatch Sky { get; }
    private bool ShowTextures { get; }
    private Vertices Quad { get; }

    private Textures(TextureClipBatch unclipped, TextureClipBatch ground, TextureClipBatch sky, bool showTextures,
        Vertices quad)
    {
        Unclipped = unclipped;
        Ground = ground;
        Sky = sky;
        ShowTextures = showTextures;
        Quad = quad;
    }

    public static Textures Create(LevEditState state, OpenGlLgr lgr, RenderingSettings settings, Vertices quad)
    {
        var txts = new Textures(
            new TextureClipBatch(),
            new TextureClipBatch(),
            new TextureClipBatch(),
            settings.ShowTextures,
            quad
        );
        txts.Update(state, lgr);
        return txts;
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
        var instances = new Dictionary<(string Text, string Mask, ClippingType Clip), List<Vector3>>();

        foreach (var ge in state.GetGraphicElements())
        {
            if (ge is GraphicElement.Texture t)
            {
                var key = (t.TextureInfo.Name, t.MaskInfo.Name, t.Clipping);
                instances.GetOrCreate(key).Add(new Vector3((float)t.Position.X, (float)t.Position.Y, t.Distance));
            }
        }

        var unclippedTxts = new List<(Texture tex, Texture mask, List<Vector3> pos)>();
        var groundTxts = new List<(Texture tex, Texture mask, List<Vector3> pos)>();
        var skyTxts = new List<(Texture tex, Texture mask, List<Vector3> pos)>();

        foreach (var kvp in instances)
        {
            var (texName, maskName, clip) = kvp.Key;
            var positions = kvp.Value;

            if (!lgr.DrawableImages.TryGetValue(texName, out var texImg)) continue;
            if (!lgr.DrawableImages.TryGetValue(maskName, out var maskImg)) continue;

            var tex = texImg.Texture;
            var mask = maskImg.Texture;

            switch (clip)
            {
                case ClippingType.Unclipped: unclippedTxts.Add((tex, mask, positions)); break;
                case ClippingType.Ground: groundTxts.Add((tex, mask, positions)); break;
                case ClippingType.Sky: skyTxts.Add((tex, mask, positions)); break;
            }
        }

        UpdateClipBatch(Unclipped, unclippedTxts);
        UpdateClipBatch(Ground, groundTxts);
        UpdateClipBatch(Sky, skyTxts);
    }

    private void UpdateClipBatch(TextureClipBatch clipBatch,
        List<(Texture tex, Texture mask, List<Vector3> pos)> newTxts)
    {
        int i = 0;
        foreach (var (tex, mask, positions) in newTxts)
        {
            if (i < clipBatch.Pics.Count)
            {
                clipBatch.Pics[i].Texture = tex;
                clipBatch.Pics[i].Mask = mask;
                clipBatch.Pics[i].Update(positions);
            }
            else
            {
                var b = new TextureBatch(tex, mask, CreateBatchBuffer(Quad), positions.Count);
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
        if (!ShowTextures) return;

        DrawClipBatch(Unclipped, pipelineUnclipped);
        DrawClipBatch(Ground, pipelineGround);
        DrawClipBatch(Sky, pipelineSky);
    }

    private void DrawClipBatch(TextureClipBatch clipBatch, Pipeline pipeline)
    {
        pipeline.Use();

        foreach (var b in clipBatch.Pics)
        {
            b.InstanceBuffer.Bind();
            b.Texture.Bind();
            b.Mask.Bind(TextureUnit.Texture1);
            Quad.DrawInstanced(b.Count);
            Texture.Unbind(TextureUnit.Texture1);
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

internal class TextureBatch(Texture texture, Texture mask, BoundVertexArray instanceBuffer, int count)
    : IDisposable
{
    public Texture Texture { get; set; } = texture;
    public Texture Mask { get; set; } = mask;
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

internal class TextureClipBatch : IDisposable
{
    public List<TextureBatch> Pics { get; } = new();

    public void Dispose()
    {
        foreach (var b in Pics) b.Dispose();
    }
}
