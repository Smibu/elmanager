using System;
using OpenTK.Graphics.OpenGL;

namespace Elmanager.Rendering.OpenGL;

internal class Pipeline : IDisposable
{
    private Shader Shader { get; }
    private StencilOptions? Stencil { get; }
    private bool DepthTest { get; }
    private bool Blend { get; }
    private BlendingFactor SourceBlend { get; }
    private BlendingFactor DestinationBlend { get; }

    internal Pipeline(Shader shader, StencilOptions? stencil, bool depthTest = false, bool blend = false, BlendingFactor sourceBlend = BlendingFactor.SrcAlpha, BlendingFactor destinationBlend = BlendingFactor.OneMinusSrcAlpha)
    {
        Shader = shader;
        Stencil = stencil;
        DepthTest = depthTest;
        Blend = blend;
        SourceBlend = sourceBlend;
        DestinationBlend = destinationBlend;
    }

    public void Use()
    {
        Shader.Use();

        if (Stencil.HasValue)
        {
            GL.Enable(EnableCap.StencilTest);
            var opts = Stencil.Value;
            GL.StencilFunc(opts.Compare, (int)opts.Reference, (int)opts.ReadMask);
            GL.StencilOp(opts.StencilFail, opts.DepthFail, opts.Pass);
            GL.StencilMask(opts.WriteMask);
        }
        else
        {
            GL.Disable(EnableCap.StencilTest);
        }

        if (DepthTest)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
        }
        else
        {
            GL.Disable(EnableCap.DepthTest);
        }

        if (Blend)
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(SourceBlend, DestinationBlend);
        }
        else
        {
            GL.Disable(EnableCap.Blend);
        }
    }

    public void Dispose()
    {
        Shader.Dispose();
    }
}
