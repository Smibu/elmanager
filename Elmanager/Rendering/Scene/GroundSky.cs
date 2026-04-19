using System;
using System.Collections.Generic;
using System.Drawing;
using Elmanager.Rendering.OpenGL;
using OpenTK.Graphics.OpenGL;
using Buffer = Elmanager.Rendering.OpenGL.Buffer;

namespace Elmanager.Rendering.Scene;

internal class GroundSky : IDisposable
{
    private const string VertexShader = @"
        #version 320 es
        precision highp float;
        layout(location = 0) in vec2 a_pos;

        out vec2 v_pos;

        layout(std140, binding = 0) uniform Camera {
            mat4 u_projection;
            vec2 u_camPos;
            float u_grassZoom;
            float u_zoom;
        };

        layout(std140, binding = 1) uniform Colors {
            vec4 u_color;
        };

        void main() {
            v_pos = a_pos;
            gl_Position = u_projection * vec4(a_pos, 1000.0, 1.0);
        }
    ";

    private const string FragmentShader = @"
        #version 320 es
        precision highp float;
        in vec2 v_pos;
        out vec4 color;

        uniform sampler2D u_texture;

        layout(std140, binding = 0) uniform Camera {
            mat4 u_projection;
            vec2 u_camPos;
            float u_grassZoom;
            float u_zoom;
        };

        layout(std140, binding = 1) uniform Colors {
            vec4 u_color;
        };

        void main() {
            vec2 texSize = vec2(textureSize(u_texture, 0));
            float aspect = texSize.x / texSize.y;
            float f = 48.0 / texSize.y;
            vec2 fix = vec2(v_pos.x - u_camPos.x * 0.5, -v_pos.y + u_camPos.y);
            vec2 coords = vec2(fix.x * f / aspect, fix.y * f) / u_zoom;
            color = texture(u_texture, coords) * u_color;
        }
    ";

    internal static Pipeline CreatePipeline() => PipelineBuilder.Create(VertexShader, FragmentShader)
        .WithStencil(new StencilOptions
        {
            Compare = StencilFunction.Equal,
            StencilFail = StencilOp.Incr,
            Pass = StencilOp.Incr,
            Reference = 0,
            DepthFail = StencilOp.Replace,
            ReadMask = 0xff,
            WriteMask = 1
        })
        .Build();

    public VerticesIndirect SkyVertices { get; }
    private Texture SkyTexture { get; }
    private ColorUniform SkyColor { get; }

    /// <summary>
    /// Contains 6 vertices (2 triangles) for ground rendering, updated every frame to always cover whole screen.
    /// </summary>
    public Vertices GroundVertices { get; }

    private Texture GroundTexture { get; }
    private ColorUniform GroundColor { get; }

    private GroundSky(VerticesIndirect skyVertices, Texture skyTexture, ColorUniform skyColor,
        Vertices groundVertices, Texture groundTexture, ColorUniform groundColor)
    {
        SkyVertices = skyVertices;
        SkyTexture = skyTexture;
        SkyColor = skyColor;
        GroundVertices = groundVertices;
        GroundTexture = groundTexture;
        GroundColor = groundColor;
    }

    private static (float[] vertices, int[] firsts, int[] counts) BuildSkyVertexData(LevEditState state)
    {
        var vertices = new List<float>();
        var firsts = new List<int>();
        var counts = new List<int>();
        int index = 0;

        foreach (var p in state.GetPolygons())
        {
            if (p.IsGrass) continue;

            firsts.Add(index);
            counts.Add(p.Vertices.Count);

            foreach (var v in p.Vertices)
            {
                vertices.Add((float)v.X);
                vertices.Add((float)v.Y);
                index++;
            }
        }

        return (vertices.ToArray(), firsts.ToArray(), counts.ToArray());
    }

    public static GroundSky Create(LevEditState state, OpenGlLgr? lgr, RenderingSettings settings,
        Texture white1X1Texture)
    {
        var (skyVerts, skyFirsts, skyCounts) = BuildSkyVertexData(state);
        var vertInfo = new VertexInfo().Attr(0, VertexFormat.Float32x2);
        var groundSky = lgr?.GetGroundAndSky(state.Lev, settings.DefaultGroundAndSky);

        var skyTexture = settings.SkyTextureEnabled && groundSky is { } gs1
            ? gs1.Sky.Texture
            : white1X1Texture;
        Color? skySolidColor = settings.SkyTextureEnabled && groundSky is not null ? null : settings.SkyFillColor;
        var skyColor = skySolidColor is { } sc ? new ColorUniform(sc) : new ColorUniform(Color.White);
        var skyVertBuffer = VertexArray.Create(vertInfo, null);
        var skyVerticesIndirect = new VerticesIndirect(skyVertBuffer, skyVerts, skyFirsts, skyCounts);

        var quadIndices = new uint[] { 0, 1, 2, 0, 2, 3 };
        var quadVertBuffer = VertexArray.Create(vertInfo, null);
        var quadIndBuffer = Buffer.CreateIndex(quadIndices);
        var groundVertices = new Vertices(quadVertBuffer, quadIndBuffer, PrimitiveType.Triangles);

        var groundTexture = settings.GroundTextureEnabled && groundSky is { } gs2
            ? gs2.Ground.Texture
            : white1X1Texture;
        Color? groundSolidColor = settings.GroundTextureEnabled && groundSky is not null ? null :
            settings.ShowGround ? settings.GroundFillColor : settings.SkyFillColor;
        var groundColor = groundSolidColor is { } gc ? new ColorUniform(gc) : new ColorUniform(Color.White);

        return new GroundSky(skyVerticesIndirect, skyTexture, skyColor,
            groundVertices, groundTexture, groundColor);
    }

    public void Update(LevEditState state)
    {
        var (skyVerts, skyFirsts, skyCounts) = BuildSkyVertexData(state);
        SkyVertices.Update(skyVerts, skyFirsts, skyCounts);
    }

    public void DrawSky(UniformBuffer colorUniforms, Pipeline pipeline)
    {
        colorUniforms.SetData(SkyColor);
        pipeline.Use();
        SkyVertices.Bind();
        SkyTexture.Bind();

        // https://web.archive.org/web/20240118160026/http://www.glprogramming.com/red/chapter14.html#name13
        // Using inversion as stencil action, we can draw the polygons as triangle fans.
        // This way we avoid the need to triangulate them (making editing simpler and faster).
        SkyVertices.Draw(PrimitiveType.TriangleFan);
    }

    public void DrawGround(UniformBuffer colorUniforms, Pipeline pipeline)
    {
        colorUniforms.SetData(GroundColor);
        pipeline.Use();
        GroundVertices.Bind();
        GroundTexture.Bind();
        GroundVertices.DrawInstanced(1);
    }

    public void Dispose()
    {
        SkyVertices.Dispose();
        GroundVertices.Dispose();
    }
}
