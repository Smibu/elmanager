using System.Collections.Generic;

namespace Elmanager.Rendering.OpenGL;

internal enum VertexStepMode
{
    Vertex,
    Instance
}

internal struct VertexAttr(int location, VertexFormat format)
{
    public int Location { get; } = location;
    public VertexFormat Format { get; } = format;
}

internal class VertexInfo
{
    internal List<VertexAttr> Attrs { get; } = [];
    internal VertexStepMode StepMode { get; private set; } = VertexStepMode.Vertex;

    public VertexInfo WithStepMode(VertexStepMode stepMode)
    {
        StepMode = stepMode;
        return this;
    }

    public VertexInfo Attr(int location, VertexFormat format)
    {
        Attrs.Add(new VertexAttr(location, format));
        return this;
    }

    public int CalculateStride()
    {
        int stride = 0;
        foreach (var attr in Attrs)
        {
            stride += attr.Format.Bytes();
        }

        return stride;
    }
}
