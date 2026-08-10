using System.Runtime.InteropServices;
using Elmanager.Rendering.OpenGL;
using OpenTK.GLControl;
using Silk.NET.OpenGL;

namespace Elmanager.Rendering;

internal partial class GlControlContext : IGraphicsContext
{
    [LibraryImport("opengl32.dll", StringMarshalling = StringMarshalling.Utf8)]
    private static partial nint wglGetProcAddress(string procName);

    private static readonly nint OpenGlLibHandle = NativeLibrary.Load("opengl32.dll");

    private readonly GLControl _control;

    public GlControlContext(GLControl control)
    {
        _control = control;
        _control.MakeCurrent();
        GlProvider.Initialize(GL.GetApi(GetProcAddress));
        GlProvider.GL.Enable(EnableCap.ProgramPointSize);
    }

    private static nint GetProcAddress(string name)
    {
        var addr = wglGetProcAddress(name);
        if (addr != 0)
        {
            return addr;
        }
        NativeLibrary.TryGetExport(OpenGlLibHandle, name, out var p);
        return p;
    }

    public void MakeCurrent()
    {
        if (!_control.Context!.IsCurrent)
            _control.MakeCurrent();
    }

    public void SwapBuffers()
    {
        _control.SwapBuffers();
    }
}
