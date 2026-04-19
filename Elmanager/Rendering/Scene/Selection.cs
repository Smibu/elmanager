using System;
using System.Collections.Generic;
using System.Drawing;
using Elmanager.Geometry;
using Elmanager.Rendering.OpenGL;
using OpenTK.Graphics.OpenGL;

namespace Elmanager.Rendering.Scene;

internal class Selection : IDisposable
{
    private readonly BoundVertexArray _pointBuffer;
    private readonly BoundVertexArray _lineBuffer;
    private readonly List<float> _points = [];
    private readonly List<float> _lines = [];

    private Selection()
    {
        var vertexInfo = new VertexInfo().Attr(0, VertexFormat.Float32x2);
        _pointBuffer = VertexArray.Create(vertexInfo, null);
        _lineBuffer = VertexArray.Create(vertexInfo, null);
    }

    public static Selection Create()
    {
        return new Selection();
    }

    public void AddPoint(Vector position)
    {
        _points.Add((float)position.X);
        _points.Add((float)position.Y);
    }

    public void AddLineLoop(IEnumerable<Vector> points)
    {
        Vector? firstPoint = null;
        Vector? previousPoint = null;

        foreach (var point in points)
        {
            if (firstPoint == null)
            {
                firstPoint = point;
            }
            else
            {
                _lines.Add((float)previousPoint!.Value.X);
                _lines.Add((float)previousPoint.Value.Y);
                _lines.Add((float)point.X);
                _lines.Add((float)point.Y);
            }
            previousPoint = point;
        }

        if (firstPoint != null && previousPoint != null)
        {
            _lines.Add((float)previousPoint.Value.X);
            _lines.Add((float)previousPoint.Value.Y);
            _lines.Add((float)firstPoint.Value.X);
            _lines.Add((float)firstPoint.Value.Y);
        }
    }

    public void Draw(Color color, UniformBuffer colorUniforms, Pipeline pipeline)
    {
        if (_points.Count == 0 && _lines.Count == 0) return;

        colorUniforms.SetData(new ColorUniform(color));
        pipeline.Use();

        if (_points.Count > 0)
        {
            var pointArray = _points.ToArray();
            var pointCount = pointArray.Length / 2;
            _pointBuffer.SetData(pointArray, BufferUsageHint.StreamDraw);

            _pointBuffer.Bind();
            GL.DrawArrays(PrimitiveType.Points, 0, pointCount);

            _points.Clear();
        }

        if (_lines.Count > 0)
        {
            var lineArray = _lines.ToArray();
            var lineCount = lineArray.Length / 2;
            _lineBuffer.SetData(lineArray, BufferUsageHint.StreamDraw);

            _lineBuffer.Bind();
            GL.DrawArrays(PrimitiveType.Lines, 0, lineCount);

            _lines.Clear();
        }
    }

    public void Dispose()
    {
        _pointBuffer.Dispose();
        _lineBuffer.Dispose();
    }
}
