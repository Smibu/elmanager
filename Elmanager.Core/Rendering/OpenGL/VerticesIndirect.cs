using System;
using System.Linq;
using OpenTK.Graphics.OpenGL;

namespace Elmanager.Rendering.OpenGL;

internal class VerticesIndirect : IDisposable
{
    public BoundVertexArray VertexArray { get; }
    private Buffer IndirectBuffer { get; } = new(BufferTarget.DrawIndirectBuffer);
    private int CommandCount { get; set; }
    public int VertexCount { get; private set; }

    public VerticesIndirect(BoundVertexArray vertexArray, float[] vertices, int[] firsts, int[] counts,
        BufferUsageHint usageHint = BufferUsageHint.DynamicDraw)
    {
        VertexArray = vertexArray;
        Update(vertices, firsts, counts, usageHint);
    }

    public void Update(float[] vertices, int[] firsts, int[] counts,
        BufferUsageHint usageHint = BufferUsageHint.DynamicDraw)
    {
        VertexArray.SetData(vertices, usageHint);
        VertexCount = vertices.Length / 2;
        var cmds = BuildIndirectCommands(firsts, counts);
        IndirectBuffer.SetData(cmds);
        CommandCount = firsts.Length;
    }

    public void Bind()
    {
        VertexArray.Bind();
        IndirectBuffer.Bind();
    }

    public void Draw(PrimitiveType primitiveType)
    {
        GL.MultiDrawArraysIndirect(primitiveType, IntPtr.Zero, CommandCount, 0);
    }

    public void Dispose()
    {
        VertexArray.Dispose();
        IndirectBuffer.Dispose();
    }

    private static DrawArraysIndirectCommand[] BuildIndirectCommands(int[] firsts, int[] counts) =>
        firsts.AsEnumerable().Zip(counts, (first, count) => new DrawArraysIndirectCommand
        {
            Count = (uint)count,
            InstanceCount = 1,
            First = (uint)first,
            BaseInstance = 0
        }).ToArray();
}
