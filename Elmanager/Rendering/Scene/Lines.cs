using System;
using System.Collections.Generic;
using System.Drawing;
using Elmanager.Geometry;
using Elmanager.Rendering.OpenGL;
using OpenTK.Graphics.OpenGL;

namespace Elmanager.Rendering.Scene;

internal class Lines : IDisposable
{
    private const string VertexShader = @"
        #version 320 es
        precision highp float;
        layout(location = 0) in vec2 a_position;

        layout(std140, binding = 0) uniform Camera {
            mat4 u_projection;
            vec2 u_camPos;
            float u_grassZoom;
            float u_zoom;
        };

        void main() {
            gl_Position = u_projection * vec4(a_position.x, a_position.y, 0.0, 1.0);
        }
    ";

    private const string FragmentShader = @"
        #version 320 es
        precision highp float;

        layout(std140, binding = 1) uniform Colors {
            vec4 u_color;
        };

        out vec4 color;

        void main() {
            color = u_color;
        }
    ";

    private const string DashedVertexShader = @"
        #version 320 es
        precision highp float;
        layout(location = 0) in vec2 a_position;

        layout(std140, binding = 0) uniform Camera {
            mat4 u_projection;
            vec2 u_camPos;
            float u_grassZoom;
            float u_zoom;
        };

        out vec2 v_pos;
        flat out vec2 v_startPos;

        void main() {
            gl_Position = u_projection * vec4(a_position.x, a_position.y, 0.0, 1.0);

            float aspect = u_projection[1][1] / u_projection[0][0];
            vec2 uniformPos = vec2(gl_Position.x * aspect, gl_Position.y);

            v_pos = uniformPos * 100.0;
            v_startPos = v_pos;
        }
    ";

    private const string DashedFragmentShader = @"
        #version 320 es
        precision highp float;

        layout(std140, binding = 1) uniform Colors {
            vec4 u_color;
        };

        in vec2 v_pos;
        flat in vec2 v_startPos;

        out vec4 color;

        void main() {
            float dist = distance(v_pos, v_startPos);
            if (fract(dist * 1.05) < 0.5) {
                discard;
            }
            color = u_color;
        }
    ";

    internal static Pipeline CreatePipeline() => PipelineBuilder.Create(VertexShader, FragmentShader)
        .WithStencil(Pipelines.StencilUnclipped)
        .WithBlend()
        .Build();

    internal static Pipeline CreateDashedPipeline() => PipelineBuilder.Create(DashedVertexShader, DashedFragmentShader)
        .WithStencil(Pipelines.StencilUnclipped)
        .WithBlend()
        .Build();

    private bool ShowGrid { get; }
    private ColorUniform GridColor { get; }
    private double GridSize { get; }
    private readonly BoundVertexArray _vertexBuffer;

    private Lines(bool showGrid, ColorUniform gridColor, double gridSize)
    {
        ShowGrid = showGrid;
        GridColor = gridColor;
        GridSize = gridSize;

        _vertexBuffer = VertexArray.Create(new VertexInfo().Attr(0, VertexFormat.Float32x2), null);
    }

    public static Lines Create(RenderingSettings settings)
    {
        return new Lines(
            showGrid: settings.ShowGrid,
            gridColor: new ColorUniform(settings.GridColor),
            gridSize: settings.GridSize
        );
    }

    public void DrawGrid(
        double xMin, double xMax,
        double yMin, double yMax,
        Vector gridOffset,
        UniformBuffer colorUniforms,
        Pipeline pipeline)
    {
        if (!ShowGrid) return;

        var lineData = new List<float>();

        var x = ElmaRenderer.GetFirstGridLine(GridSize, gridOffset.X, xMin);
        while (x <= xMax)
        {
            lineData.Add((float)x);
            lineData.Add((float)yMin);
            lineData.Add((float)x);
            lineData.Add((float)yMax);
            x += GridSize;
        }

        var y = ElmaRenderer.GetFirstGridLine(GridSize, gridOffset.Y, yMin);
        while (y <= yMax)
        {
            lineData.Add((float)xMin);
            lineData.Add((float)y);
            lineData.Add((float)xMax);
            lineData.Add((float)y);
            y += GridSize;
        }

        if (lineData.Count == 0) return;

        DrawInternal(lineData.ToArray(), GridColor, colorUniforms, PrimitiveType.Lines, pipeline);
    }

    public void DrawDashLine(double x1, double y1, double x2, double y2, Color color, UniformBuffer colorUniforms, Pipeline pipeline)
    {
        DrawInternal([
            (float)x1, (float)y1,
            (float)x2, (float)y2
        ], new ColorUniform(color), colorUniforms, PrimitiveType.Lines, pipeline);
    }

    public void DrawPoint(Vector v, Color color, UniformBuffer colorUniforms, Pipeline pipeline)
    {
        DrawInternal([(float)v.X, (float)v.Y], new ColorUniform(color), colorUniforms, PrimitiveType.Points, pipeline);
    }

    public void DrawLine(Vector v1, Vector v2, Color color, UniformBuffer colorUniforms, Pipeline pipeline)
    {
        DrawInternal([(float)v1.X, (float)v1.Y, (float)v2.X, (float)v2.Y], new ColorUniform(color), colorUniforms, PrimitiveType.Lines, pipeline);
    }

    public void DrawLineStrip(IEnumerable<Vector> points, Color color, UniformBuffer colorUniforms, Pipeline pipeline)
    {
        var data = new List<float>();
        foreach (var p in points)
        {
            data.Add((float)p.X);
            data.Add((float)p.Y);
        }
        if (data.Count > 0)
            DrawInternal(data.ToArray(), new ColorUniform(color), colorUniforms, PrimitiveType.LineStrip, pipeline);
    }

    public void DrawLineLoop(IEnumerable<Vector> points, Color color, UniformBuffer colorUniforms, Pipeline pipeline)
    {
        var data = new List<float>();
        foreach (var p in points)
        {
            data.Add((float)p.X);
            data.Add((float)p.Y);
        }
        if (data.Count > 0)
            DrawInternal(data.ToArray(), new ColorUniform(color), colorUniforms, PrimitiveType.LineLoop, pipeline);
    }

    public void DrawRectangle(Vector v1, Vector v2, Color color, UniformBuffer colorUniforms, Pipeline pipeline)
    {
        DrawInternal([
            (float)v1.X, (float)v1.Y,
            (float)v2.X, (float)v1.Y,
            (float)v2.X, (float)v2.Y,
            (float)v1.X, (float)v2.Y
        ], new ColorUniform(color), colorUniforms, PrimitiveType.LineLoop, pipeline);
    }

    public void DrawCircle(Vector center, double radius, Color color, int accuracy, UniformBuffer colorUniforms, Pipeline pipeline)
    {
        var data = new List<float>();
        for (var i = 0; i < accuracy; i++)
        {
            var angle = i * 2.0 * Math.PI / accuracy;
            data.Add((float)(center.X + Math.Cos(angle) * radius));
            data.Add((float)(center.Y + Math.Sin(angle) * radius));
        }
        if (data.Count > 0)
            DrawInternal(data.ToArray(), new ColorUniform(color), colorUniforms, PrimitiveType.LineLoop, pipeline);
    }

    private void DrawInternal(float[] lineData, ColorUniform color, UniformBuffer colorUniforms, PrimitiveType primitiveType, Pipeline pipeline)
    {
        var vertexCount = lineData.Length / 2;
        _vertexBuffer.SetData(lineData, BufferUsageHint.StreamDraw);

        colorUniforms.SetData(color);
        pipeline.Use();
        _vertexBuffer.Bind();
        GL.DrawArrays(primitiveType, 0, vertexCount);
    }

    public void Dispose()
    {
        _vertexBuffer.Dispose();
    }
}
