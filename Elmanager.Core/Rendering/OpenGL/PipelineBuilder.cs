using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace Elmanager.Rendering.OpenGL;

internal class PipelineBuilder
{
    private readonly string _vertexShader;
    private readonly string _fragmentShader;
    private StencilOptions? _stencil;
    private bool _depthTest;
    private bool _blend;
    private BlendingFactor _sourceBlend = BlendingFactor.SrcAlpha;
    private BlendingFactor _destinationBlend = BlendingFactor.OneMinusSrcAlpha;
    private readonly Dictionary<string, int> _textureLocations = new();

    private PipelineBuilder(string vertexShader, string fragmentShader)
    {
        _vertexShader = vertexShader;
        _fragmentShader = fragmentShader;
    }

    public static PipelineBuilder Create(string vertexShader, string fragmentShader) =>
        new(vertexShader, fragmentShader);

    public PipelineBuilder WithStencil(StencilOptions stencil)
    {
        _stencil = stencil;
        return this;
    }

    public PipelineBuilder WithDepthTest(bool enable = true)
    {
        _depthTest = enable;
        return this;
    }

    public PipelineBuilder WithBlend(BlendingFactor sourceBlend = BlendingFactor.SrcAlpha, BlendingFactor destinationBlend = BlendingFactor.OneMinusSrcAlpha)
    {
        _blend = true;
        _sourceBlend = sourceBlend;
        _destinationBlend = destinationBlend;
        return this;
    }

    public PipelineBuilder WithTextureLocation(int location, string name)
    {
        _textureLocations[name] = location;
        return this;
    }

    public Pipeline Build()
    {
        var shader = new Shader(_vertexShader, _fragmentShader);

        if (_textureLocations.Count > 0)
        {
            shader.Use();
            foreach (var kvp in _textureLocations)
            {
                int loc = GL.GetUniformLocation(shader.Handle, kvp.Key);
                if (loc != -1)
                {
                    GL.Uniform1(loc, kvp.Value);
                }
                else
                {
                    throw new InvalidOperationException($"Uniform '{kvp.Key}' not found in shader");
                }
            }

            GL.UseProgram(0);
        }

        return new Pipeline(shader, _stencil, _depthTest, _blend, _sourceBlend, _destinationBlend);
    }
}
