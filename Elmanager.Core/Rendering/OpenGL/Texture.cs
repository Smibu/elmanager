using System;
using Silk.NET.OpenGL;

namespace Elmanager.Rendering.OpenGL;

public class Texture : IDisposable
{
    private static GL GL => GlProvider.GL;
    private uint Handle { get; } = GlProvider.GL.GenTexture();

    public void Bind(TextureUnit unit = TextureUnit.Texture0)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, Handle);
    }

    public static void Unbind(TextureUnit unit = TextureUnit.Texture0)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    public void Dispose()
    {
        GL.DeleteTexture(Handle);
    }
}
