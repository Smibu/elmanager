using System;
using System.Collections.Generic;
using Elmanager.Rendering.OpenGL;
using Elmanager.Utilities;
using OpenTK.Graphics.OpenGL;

namespace Elmanager.Rendering.Scene;

internal class Grass : IDisposable
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

        uniform sampler2D u_qgrass;
        uniform sampler2D u_pic;

        void main() {
            vec2 texSize = vec2(textureSize(u_pic, 0));
            float ext = u_grassZoom * 40.0 - 20.0;
            float factor = 48.0 * u_grassZoom;

            float sx = texSize.x / factor;
            float sy = (texSize.y + ext) / factor;
            float extWorld = ext / factor;

            // need to stretch x size a tiny bit to avoid pixel gaps between adjacent grass pics
            vec2 pos = vec2(a_position.x + a_texcoord.x * sx * 1.0001, a_position.y + extWorld - a_texcoord.y * sy);

            gl_Position = u_projection * vec4(pos, a_position.z, 1.0);
            v_coord = vec2(a_texcoord.x, a_texcoord.y * (texSize.y + ext) / texSize.y - ext / texSize.y);
            v_pos = pos;
        }
    ";

    private const string FragmentShader = @"
        #version 320 es
        precision highp float;

        in vec2 v_coord;
        in vec2 v_pos;

        uniform sampler2D u_qgrass;
        uniform sampler2D u_pic;

        out vec4 color;

        layout(std140, binding = 0) uniform Camera {
            mat4 u_projection;
            vec2 u_camPos;
            float u_grassZoom;
            float u_zoom;
        };

        void main() {
            vec2 texSize = vec2(textureSize(u_qgrass, 0));
            float aspect = texSize.x / texSize.y;
            float f = 48.0 * u_grassZoom / texSize.y;
            float pixelSize = 1.0 / (48.0 * u_grassZoom);
            vec2 fix = vec2(v_pos.x - u_camPos.x * 0.5, -v_pos.y + u_camPos.y);
            fix -= mod(fix + pixelSize / 2.0, vec2(pixelSize));
            vec2 coords = vec2(fix.x * f / aspect, fix.y * f);

            if (v_coord.y < 0.0) {
               color = texture(u_qgrass, coords);
            } else {
               float picAlpha = texture(u_pic, v_coord).a;

               if (picAlpha == 0.0) {
                  color = texture(u_qgrass, coords);
               } else if (picAlpha == 1.0) {
                  color = texture(u_pic, v_coord);
               } else {
                  discard;
               }
            }
        }
    ";

    internal static Pipeline CreatePipeline() => PipelineBuilder.Create(VertexShader, FragmentShader)
        .WithTextureLocation(0, "u_qgrass")
        .WithTextureLocation(1, "u_pic")
        .WithStencil(Pipelines.StencilGround)
        .WithDepthTest()
        .WithBlend()
        .Build();

    private List<GrassBatch> Pics { get; }
    private Texture? Qgrass { get; set; }
    private bool ShowGrass { get; }
    private Vertices Quad { get; }

    private Grass(List<GrassBatch> pics, Texture? qgrass, bool showGrass, Vertices quad)
    {
        Pics = pics;
        Qgrass = qgrass;
        ShowGrass = showGrass;
        Quad = quad;
    }

    public static Grass Create(LevEditState state, OpenGlLgr lgr, RenderingSettings settings, Vertices quad)
    {
        var grass = new Grass([], null, settings.ShowGrass, quad);
        grass.Update(state, lgr);
        return grass;
    }

    public void Update(LevEditState state, OpenGlLgr lgr)
    {
        var grassData = lgr.GrassData;
        if (grassData == null || grassData.GrassPics.Count == 0)
        {
            foreach (var b in Pics) b.Update([]);
            Qgrass = null;
            return;
        }

        Qgrass = grassData.Qgrass;

        var groups = new Dictionary<Texture, List<float>>();

        int j = 1;
        foreach (var p in state.GetPolygons())
        {
            if (p.IsGrass)
            {
                foreach (var pic in p.SlopeInfo!.GetGrassPics(grassData.GrassPics))
                {
                    var tex = pic.PictureInfo.Texture;
                    var list = groups.GetOrCreate(tex);
                    list.Add((float)pic.Position.X);
                    list.Add((float)pic.Position.Y);
                    list.Add(pic.Distance + j * 0.0002f - 1);
                }

                j++;
            }
        }

        int i = 0;
        foreach (var kvp in groups)
        {
            var data = kvp.Value.ToArray();
            var tex = kvp.Key;
            if (i < Pics.Count)
            {
                Pics[i].Texture = tex;
                Pics[i].Update(data);
            }
            else
            {
                var batch = new GrassBatch(tex, CreateBatchBuffer(data, Quad), data.Length / 3);
                Pics.Add(batch);
            }
            i++;
        }

        for (; i < Pics.Count; i++)
        {
            Pics[i].Update([]);
        }
    }

    private static readonly VertexInfo PerVertexInfo = new VertexInfo()
        .Attr(0, VertexFormat.Float32x2);

    private static readonly VertexInfo InstanceVertexInfo = new VertexInfo()
        .Attr(1, VertexFormat.Float32x3)
        .WithStepMode(VertexStepMode.Instance);

    private static BoundVertexArray CreateBatchBuffer(float[] data, Vertices quad)
    {
        var bound = VertexArray.CreateInstanced(quad, PerVertexInfo, InstanceVertexInfo);
        bound.SetData(data, BufferUsageHint.DynamicDraw);
        return bound;
    }

    public void Draw(Pipeline pipeline)
    {
        if (!ShowGrass || Qgrass == null) return;

        pipeline.Use();

        foreach (var b in Pics)
        {
            b.InstanceBuffer.Bind();
            Qgrass.Bind();
            b.Texture.Bind(TextureUnit.Texture1);

            Quad.DrawInstanced(b.Count);

            Texture.Unbind(TextureUnit.Texture1);
            Texture.Unbind();
        }
    }

    public void Dispose()
    {
        foreach (var b in Pics) b.Dispose();
    }
}

internal class GrassBatch(Texture texture, BoundVertexArray instanceBuffer, int count) : IDisposable
{
    public Texture Texture { get; set; } = texture;
    public BoundVertexArray InstanceBuffer { get; } = instanceBuffer;
    public int Count { get; private set; } = count;

    public void Update(float[] data)
    {
        InstanceBuffer.SetData(data, BufferUsageHint.DynamicDraw);
        Count = data.Length / 3;
    }

    public void Dispose()
    {
        InstanceBuffer.Dispose();
    }
}
