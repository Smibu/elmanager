using System.Runtime.InteropServices;

namespace Elmanager.Rendering.OpenGL;

[StructLayout(LayoutKind.Sequential)]
internal struct DrawArraysIndirectCommand
{
    public uint Count;
    public uint InstanceCount;
    public uint First;
    public uint BaseInstance;
}
