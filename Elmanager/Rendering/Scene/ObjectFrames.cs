using System;
using Elmanager.Rendering.OpenGL;
using OpenTK.Graphics.OpenGL;
using Buffer = Elmanager.Rendering.OpenGL.Buffer;

namespace Elmanager.Rendering.Scene;

internal class ObjectFrames : IDisposable
{
    private const string VertexShader = @"
        #version 320 es
        precision highp float;
        layout(location = 0) in vec2 a_position;
        layout(location = 1) in vec4 a_instancePos;
        layout(location = 2) in float a_instanceAlpha;

        layout(std140, binding = 0) uniform Camera {
            mat4 u_projection;
            vec2 u_camPos;
            float u_grassZoom;
            float u_zoom;
        };
        layout(std140, binding = 1) uniform Colors {
            vec4 u_color;
        };

        out float v_alpha;

        void main() {
            float c = cos(a_instancePos.w);
            float s = sin(a_instancePos.w);
            vec2 rotatedPos = vec2(
                a_position.x * c - a_position.y * s,
                a_position.x * s + a_position.y * c
            );
            vec2 center = vec2(a_instancePos.x + 20.0 / 48.0, a_instancePos.y - 20.0 / 48.0);
            gl_Position = u_projection * vec4(center.x + rotatedPos.x, center.y + rotatedPos.y, a_instancePos.z, 1.0);
            v_alpha = a_instanceAlpha;
        }
    ";

    private const string FragmentShader = @"
        #version 320 es
        precision highp float;

        layout(std140, binding = 0) uniform Camera {
            mat4 u_projection;
            vec2 u_camPos;
            float u_grassZoom;
            float u_zoom;
        };
        layout(std140, binding = 1) uniform Colors {
            vec4 u_color;
        };

        out vec4 color;
        in float v_alpha;

        void main() {
            color = vec4(u_color.rgb, u_color.a * v_alpha);
        }
    ";

    internal static Pipeline CreatePipeline() => PipelineBuilder.Create(VertexShader, FragmentShader)
        .WithStencil(Pipelines.StencilUnclipped)
        .WithBlend()
        .Build();

    private static readonly VertexInfo PerVertexInfo = new VertexInfo()
        .Attr(0, VertexFormat.Float32x2);

    private const int InstanceStride = 5 * sizeof(float);

    private bool ShowObjectFrames { get; }
    private bool ShowGravityAppleArrows { get; }
    private ColorUniform KillerColor { get; }
    private ColorUniform FlowerColor { get; }
    private ColorUniform AppleColor { get; }
    private ColorUniform AppleGravityArrowColor { get; }
    private Vertices CircleVertices { get; }
    private Vertices ArrowVertices { get; }
    private VertexArray CircleVao { get; }
    private VertexArray ArrowVao { get; }

    private ObjectFrames(
        bool showObjectFrames,
        bool showGravityAppleArrows,
        ColorUniform killerColor,
        ColorUniform flowerColor,
        ColorUniform appleColor,
        ColorUniform appleGravityArrowColor,
        Vertices circleVertices,
        Vertices arrowVertices,
        VertexArray circleVao,
        VertexArray arrowVao)
    {
        ShowObjectFrames = showObjectFrames;
        ShowGravityAppleArrows = showGravityAppleArrows;
        KillerColor = killerColor;
        FlowerColor = flowerColor;
        AppleColor = appleColor;
        AppleGravityArrowColor = appleGravityArrowColor;
        CircleVertices = circleVertices;
        ArrowVertices = arrowVertices;
        CircleVao = circleVao;
        ArrowVao = arrowVao;
    }

    public static ObjectFrames Create(RenderingSettings settings)
    {
        var circleVertices = CreateCircleVertices(settings.CircleDrawingAccuracy);
        var arrowVertices = CreateArrowVertices();
        return new ObjectFrames(
            settings.ShowObjectFrames,
            settings.ShowGravityAppleArrows,
            new ColorUniform(settings.KillerColor),
            new ColorUniform(settings.FlowerColor),
            new ColorUniform(settings.AppleColor),
            new ColorUniform(settings.AppleGravityArrowColor),
            circleVertices,
            arrowVertices,
            VertexArray.CreateSeparateInstanced(circleVertices, PerVertexInfo, Objects.InstanceVertexInfo),
            VertexArray.CreateSeparateInstanced(arrowVertices, PerVertexInfo, Objects.InstanceVertexInfo)
        );
    }

    private static Vertices CreateArrowVertices()
    {
        var vertInfo = new VertexInfo().Attr(0, VertexFormat.Float32x2);
        float[] vertices =
        [
            -0.1f, -0.25f,
            -0.1f, 0.0f,
            -0.25f, 0.0f,
            0.0f, 0.25f,
            0.25f, 0.0f,
            0.1f, 0.0f,
            0.1f, -0.25f
        ];
        uint[] indices = [0, 1, 2, 3, 4, 5, 6];
        var vbo = VertexArray.Create(vertInfo, vertices);
        var ibo = Buffer.CreateIndex(indices);
        return new Vertices(vbo, ibo, PrimitiveType.LineLoop);
    }

    private static Vertices CreateCircleVertices(int accuracy)
    {
        var vertInfo = new VertexInfo().Attr(0, VertexFormat.Float32x2);
        var vertices = new float[accuracy * 2];
        var indices = new uint[accuracy];

        for (int i = 0; i < accuracy; i++)
        {
            var angle = 2 * Math.PI * i / accuracy;
            vertices[i * 2] = (float)(0.4 * Math.Cos(angle));
            vertices[i * 2 + 1] = (float)(0.4 * Math.Sin(angle));
            indices[i] = (uint)i;
        }

        var vbo = VertexArray.Create(vertInfo, vertices);
        var ibo = Buffer.CreateIndex(indices);
        return new Vertices(vbo, ibo, PrimitiveType.LineLoop);
    }

    public void Draw(Objects objects, UniformBuffer colorUniforms, Pipeline pipeline)
    {
        if (!ShowObjectFrames && !ShowGravityAppleArrows) return;

        pipeline.Use();

        if (ShowObjectFrames)
        {
            CircleVao.Bind();

            if (objects.Killers.Count > 0)
            {
                colorUniforms.SetData(KillerColor);
                CircleVao.BindInstanceBuffer(objects.Killers.InstanceBuffer.Buffer, InstanceStride);
                CircleVertices.DrawInstanced(objects.Killers.Count);
            }

            if (objects.Flowers.Count > 0)
            {
                colorUniforms.SetData(FlowerColor);
                CircleVao.BindInstanceBuffer(objects.Flowers.InstanceBuffer.Buffer, InstanceStride);
                CircleVertices.DrawInstanced(objects.Flowers.Count);
            }

            if (objects.Apples.Count > 0)
            {
                colorUniforms.SetData(AppleColor);
                foreach (var appleBatch in objects.Apples)
                {
                    if (appleBatch.Batch.Count == 0) continue;
                    CircleVao.BindInstanceBuffer(appleBatch.Batch.InstanceBuffer.Buffer, InstanceStride);
                    CircleVertices.DrawInstanced(appleBatch.Batch.Count);
                }
            }
        }

        if (ShowGravityAppleArrows && objects.GravityAppleArrows.Count > 0)
        {
            ArrowVao.Bind();
            colorUniforms.SetData(AppleGravityArrowColor);
            ArrowVao.BindInstanceBuffer(objects.GravityAppleArrows.InstanceBuffer.Buffer, InstanceStride);
            ArrowVertices.DrawInstanced(objects.GravityAppleArrows.Count);
        }
    }

    public void Dispose()
    {
        CircleVao.Dispose();
        ArrowVao.Dispose();
        CircleVertices.Dispose();
        ArrowVertices.Dispose();
    }
}
