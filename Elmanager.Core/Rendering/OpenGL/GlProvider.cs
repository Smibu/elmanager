using System;
using Silk.NET.OpenGL;

namespace Elmanager.Rendering.OpenGL;

public static class GlProvider
{
    private static GL? _gl;

    public static GL GL => _gl ?? throw new InvalidOperationException("GL context has not been initialized.");

    public static void Initialize(GL gl)
    {
        _gl = gl;
    }
}
