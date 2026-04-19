using System;
using System.Collections.Generic;
using Elmanager.Rendering.OpenGL;
using OpenTK.Graphics.OpenGL;

namespace Elmanager.Rendering.Scene;

internal class PolygonFrames : IDisposable
{
    private bool ShowGroundEdges { get; }
    private bool ShowGrassEdges { get; }
    private bool ShowInactiveGrassEdges { get; }
    private bool ShowVertices { get; }
    private ColorUniform GroundEdgeColor { get; }
    private ColorUniform GrassEdgeColor { get; }
    private ColorUniform VertexColor { get; }
    private VerticesIndirect GrassVertices { get; }
    private BoundVertexArray InactiveGrassVertices { get; }

    private PolygonFrames(bool showGroundEdges, bool showGrassEdges, bool showInactiveGrassEdges, bool showVertices,
        ColorUniform groundEdgeColor, ColorUniform grassEdgeColor, ColorUniform vertexColor,
        VerticesIndirect grassVertices, BoundVertexArray inactiveGrassVertices)
    {
        ShowGroundEdges = showGroundEdges;
        ShowGrassEdges = showGrassEdges;
        ShowInactiveGrassEdges = showInactiveGrassEdges;
        ShowVertices = showVertices;
        GroundEdgeColor = groundEdgeColor;
        GrassEdgeColor = grassEdgeColor;
        VertexColor = vertexColor;
        GrassVertices = grassVertices;
        InactiveGrassVertices = inactiveGrassVertices;
    }

    public static PolygonFrames Create(LevEditState state, RenderingSettings settings)
    {
        var grassVao = new VerticesIndirect(VertexArray.Create(new VertexInfo().Attr(0, VertexFormat.Float32x2), null), [], [], []);
        var inactiveGrassVao = VertexArray.Create(new VertexInfo().Attr(0, VertexFormat.Float32x2), null);
        var frames = new PolygonFrames(
            showGroundEdges: settings.ShowGroundEdges,
            showGrassEdges: settings.ShowGrassEdges,
            showInactiveGrassEdges: settings.ShowInactiveGrassEdges,
            showVertices: settings.ShowVertices,
            groundEdgeColor: new ColorUniform(settings.GroundEdgeColor),
            grassEdgeColor: new ColorUniform(settings.GrassEdgeColor),
            vertexColor: new ColorUniform(settings.VertexColor),
            grassVertices: grassVao,
            inactiveGrassVertices: inactiveGrassVao);

        frames.Update(state);
        return frames;
    }

    public void Update(LevEditState state)
    {
        var grassVertices = new List<float>();
        var inactiveGrassVertices = new List<float>();
        var grassCounts = new List<int>();
        var grassFirsts = new List<int>();
        int grassIndex = 0;

        foreach (var p in state.GetPolygons())
        {
            if (p.IsGrass)
            {
                grassFirsts.Add(grassIndex);
                grassCounts.Add(p.Vertices.Count);

                int grassStart = p.SlopeInfo!.GrassStart;
                for (var i = grassStart; i < p.Vertices.Count; i++)
                {
                    grassVertices.Add((float)p.Vertices[i].X);
                    grassVertices.Add((float)p.Vertices[i].Y);
                    grassIndex++;
                }

                for (var i = 0; i < grassStart; i++)
                {
                    grassVertices.Add((float)p.Vertices[i].X);
                    grassVertices.Add((float)p.Vertices[i].Y);
                    grassIndex++;
                }

                var prevIndex = grassStart == 0 ? p.Vertices.Count - 1 : grassStart - 1;
                inactiveGrassVertices.Add((float)p.Vertices[prevIndex].X);
                inactiveGrassVertices.Add((float)p.Vertices[prevIndex].Y);
                inactiveGrassVertices.Add((float)p.Vertices[grassStart].X);
                inactiveGrassVertices.Add((float)p.Vertices[grassStart].Y);
            }
        }

        GrassVertices.Update(grassVertices.ToArray(), grassFirsts.ToArray(), grassCounts.ToArray());
        InactiveGrassVertices.SetData(inactiveGrassVertices.ToArray());
    }

    public void Draw(UniformBuffer colorUniforms, VerticesIndirect groundVertices, Pipeline linesPipeline, Pipeline linesDashedPipeline)
    {
        if (!ShowGroundEdges && !ShowGrassEdges && !ShowInactiveGrassEdges && !ShowVertices) return;

        if (ShowGroundEdges)
        {
            colorUniforms.SetData(GroundEdgeColor);
            linesPipeline.Use();
            groundVertices.Bind();
            groundVertices.Draw(PrimitiveType.LineLoop);
        }

        if (ShowGrassEdges)
        {
            colorUniforms.SetData(GrassEdgeColor);
            linesPipeline.Use();
            GrassVertices.Bind();
            GrassVertices.Draw(PrimitiveType.LineStrip);

            if (ShowInactiveGrassEdges && InactiveGrassVertices.Count > 0)
            {
                linesDashedPipeline.Use();
                InactiveGrassVertices.Bind();
                GL.DrawArrays(PrimitiveType.Lines, 0, InactiveGrassVertices.Count);
            }
        }

        if (ShowVertices)
        {
            colorUniforms.SetData(VertexColor);
            linesPipeline.Use();

            groundVertices.VertexArray.Bind();
            GL.DrawArrays(PrimitiveType.Points, 0, groundVertices.VertexCount);

            GrassVertices.VertexArray.Bind();
            GL.DrawArrays(PrimitiveType.Points, 0, GrassVertices.VertexCount);
        }
    }

    public void Dispose()
    {
        GrassVertices.Dispose();
        InactiveGrassVertices.Dispose();
    }
}
