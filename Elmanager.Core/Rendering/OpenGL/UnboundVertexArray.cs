using System;
using Silk.NET.OpenGL;

namespace Elmanager.Rendering.OpenGL;

internal class UnboundVertexArray : IDisposable
{
    private static GL GL => GlProvider.GL;
    private VertexArray Vao { get; }
    private VertexInfo InstanceInfo { get; }

    internal UnboundVertexArray(VertexArray vao, VertexInfo instanceInfo)
    {
        Vao = vao;
        InstanceInfo = instanceInfo;
    }

    public void BindWithInstanceBuffer(Buffer buffer)
    {
        Vao.Bind();
        buffer.Bind();

        int offset = 0;
        var instanceStride = InstanceInfo.CalculateStride();
        foreach (var attr in InstanceInfo.Attrs)
        {
            var glType = VertexArray.GetOpenGLType(attr.Format);
            var size = attr.Format.Size();
            var normalized = attr.Format.Normalized();

            unsafe
            {
                GL.VertexAttribPointer(
                    (uint)attr.Location,
                    size,
                    glType,
                    normalized,
                    (uint)instanceStride,
                    (void*)offset
                );
            }

            offset += attr.Format.Bytes();
        }
    }

    public void Dispose()
    {
        Vao.Dispose();
    }
}
