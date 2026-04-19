using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Elmanager.Rendering.OpenGL;

namespace Elmanager.Rendering.Scene;

internal class GraphicElementFrames : IDisposable
{
    private const string VertexShader = @"
        #version 320 es
        precision highp float;
        layout(location = 0) in vec2 a_texcoord;
        layout(location = 1) in vec2 a_position;
        layout(location = 2) in vec2 a_size;

        layout(std140, binding = 0) uniform Camera {
            mat4 u_projection;
            vec2 u_camPos;
            float u_grassZoom;
            float u_zoom;
        };
        layout(std140, binding = 1) uniform Colors {
            vec4 u_color;
        };

        void main() {
            gl_Position = u_projection * vec4(a_position.x + a_texcoord.x * a_size.x, a_position.y - a_texcoord.y * a_size.y, 1.0, 1.0);
        }
    ";

    private const string FragmentShader = @"
        #version 320 es
        precision highp float;

        layout(std140, binding = 0) uniform Camera {
            mat4 u_projection;
            vec2 u_camPos;
            float u_grassZoom;
            float u_zoom;
        };
        layout(std140, binding = 1) uniform Colors {
            vec4 u_color;
        };

        out vec4 color;

        void main() {
            color = u_color;
        }
    ";

    private const string DashedVertexShader = @"
        #version 320 es
        precision highp float;
        layout(location = 0) in vec2 a_texcoord;
        layout(location = 1) in vec2 a_position;
        layout(location = 2) in vec2 a_size;

        layout(std140, binding = 0) uniform Camera {
            mat4 u_projection;
            vec2 u_camPos;
            float u_grassZoom;
            float u_zoom;
        };

        out vec2 v_pos;
        flat out vec2 v_startPos;

        void main() {
            gl_Position = u_projection * vec4(a_position.x + a_texcoord.x * a_size.x, a_position.y - a_texcoord.y * a_size.y, 1.0, 1.0);

            float aspect = u_projection[1][1] / u_projection[0][0];
            vec2 uniformPos = vec2(gl_Position.x * aspect, gl_Position.y);

            v_pos = uniformPos * 100.0;
            v_startPos = v_pos;
        }
    ";

    private const string DashedFragmentShader = @"
        #version 320 es
        precision highp float;

        layout(std140, binding = 1) uniform Colors {
            vec4 u_color;
        };

        in vec2 v_pos;
        flat in vec2 v_startPos;

        out vec4 color;

        void main() {
            float dist = distance(v_pos, v_startPos);
            if (fract(dist * 1.05) < 0.5) {
                discard;
            }
            color = u_color;
        }
    ";

    internal static Pipeline CreatePipeline() => PipelineBuilder.Create(VertexShader, FragmentShader)
        .WithStencil(Pipelines.StencilUnclipped)
        .WithBlend()
        .Build();

    internal static Pipeline CreateDashedPipeline() => PipelineBuilder.Create(DashedVertexShader, DashedFragmentShader)
        .WithStencil(Pipelines.StencilUnclipped)
        .WithBlend()
        .Build();

    [StructLayout(LayoutKind.Sequential)]
    private readonly record struct FrameInstance(float X, float Y, float Width, float Height);

    private Vertices LineLoop { get; }
    private BoundVertexArray PicBuffer { get; }
    private BoundVertexArray TexBuffer { get; }
    private BoundVertexArray MissingPicBuffer { get; }
    private BoundVertexArray MissingTexBuffer { get; }
    private bool ShowPictureFrames { get; }
    private bool ShowTextureFrames { get; }
    private bool ShowPictures { get; }
    private bool ShowTextures { get; }
    private ColorUniform PictureColor { get; }
    private ColorUniform TextureColor { get; }

    private GraphicElementFrames(Vertices lineLoop, BoundVertexArray picBuffer, BoundVertexArray texBuffer,
        BoundVertexArray missingPicBuffer, BoundVertexArray missingTexBuffer,
        bool showPictureFrames, bool showTextureFrames, bool showPictures, bool showTextures,
        ColorUniform pictureColor, ColorUniform textureColor)
    {
        LineLoop = lineLoop;
        PicBuffer = picBuffer;
        TexBuffer = texBuffer;
        MissingPicBuffer = missingPicBuffer;
        MissingTexBuffer = missingTexBuffer;
        ShowPictureFrames = showPictureFrames;
        ShowTextureFrames = showTextureFrames;
        ShowPictures = showPictures;
        ShowTextures = showTextures;
        PictureColor = pictureColor;
        TextureColor = textureColor;
    }

    public static GraphicElementFrames Create(LevEditState state, RenderingSettings settings, Vertices lineLoop)
    {
        var picBuffer = CreateInstanceBuffer(lineLoop);
        var texBuffer = CreateInstanceBuffer(lineLoop);
        var missingPicBuffer = CreateInstanceBuffer(lineLoop);
        var missingTexBuffer = CreateInstanceBuffer(lineLoop);

        var frames = new GraphicElementFrames(
            lineLoop,
            picBuffer,
            texBuffer,
            missingPicBuffer,
            missingTexBuffer,
            settings.ShowPictureFrames,
            settings.ShowTextureFrames,
            settings.ShowPictures,
            settings.ShowTextures,
            new ColorUniform(settings.PictureFrameColor),
            new ColorUniform(settings.TextureFrameColor));

        frames.Update(state);
        return frames;
    }

    private static readonly VertexInfo PerVertexInfo = new VertexInfo()
        .Attr(0, VertexFormat.Float32x2);

    private static readonly VertexInfo InstanceVertexInfo = new VertexInfo()
        .Attr(1, VertexFormat.Float32x2)
        .Attr(2, VertexFormat.Float32x2)
        .WithStepMode(VertexStepMode.Instance);

    private static BoundVertexArray CreateInstanceBuffer(Vertices lineLoop)
    {
        return VertexArray.CreateInstanced(lineLoop, PerVertexInfo, InstanceVertexInfo);
    }

    public void Update(LevEditState state)
    {
        var picInstances = new List<FrameInstance>();
        var texInstances = new List<FrameInstance>();
        var missingPicInstances = new List<FrameInstance>();
        var missingTexInstances = new List<FrameInstance>();

        foreach (var ge in state.GetGraphicElements())
        {
            switch (ge)
            {
                case GraphicElement.Picture p:
                    {
                        var info = p.PictureInfo;
                        picInstances.Add(new((float)p.Position.X, (float)p.Position.Y, (float)info.Width, (float)info.Height));
                        break;
                    }
                case GraphicElement.Texture t:
                    {
                        var info = t.MaskInfo;
                        texInstances.Add(new((float)t.Position.X, (float)t.Position.Y, (float)info.Width, (float)info.Height));
                        break;
                    }
                case GraphicElement.MissingPicture mp:
                    missingPicInstances.Add(new((float)mp.Position.X, (float)mp.Position.Y, (float)mp.Width, (float)mp.Height));
                    break;
                case GraphicElement.MissingTexture mt:
                    missingTexInstances.Add(new((float)mt.Position.X, (float)mt.Position.Y, (float)mt.Width, (float)mt.Height));
                    break;
            }
        }

        PicBuffer.SetData(picInstances.ToArray());
        TexBuffer.SetData(texInstances.ToArray());
        MissingPicBuffer.SetData(missingPicInstances.ToArray());
        MissingTexBuffer.SetData(missingTexInstances.ToArray());
    }

    public void DrawPictureFrames(UniformBuffer colorUniforms, Pipeline pipeline)
    {
        if (!ShowPictureFrames || PicBuffer.Count == 0) return;

        colorUniforms.SetData(PictureColor);
        pipeline.Use();
        PicBuffer.Bind();
        LineLoop.DrawInstanced(PicBuffer.Count);
    }

    public void DrawTextureFrames(UniformBuffer colorUniforms, Pipeline pipeline)
    {
        if (!ShowTextureFrames || TexBuffer.Count == 0) return;

        colorUniforms.SetData(TextureColor);
        pipeline.Use();
        TexBuffer.Bind();
        LineLoop.DrawInstanced(TexBuffer.Count);
    }

    public void DrawMissingPictureFrames(UniformBuffer colorUniforms, Pipeline dashedPipeline)
    {
        if (!(ShowPictures || ShowPictureFrames) || MissingPicBuffer.Count == 0) return;

        colorUniforms.SetData(PictureColor);
        dashedPipeline.Use();
        MissingPicBuffer.Bind();
        LineLoop.DrawInstanced(MissingPicBuffer.Count);
    }

    public void DrawMissingTextureFrames(UniformBuffer colorUniforms, Pipeline dashedPipeline)
    {
        if (!(ShowTextures || ShowTextureFrames) || MissingTexBuffer.Count == 0) return;

        colorUniforms.SetData(TextureColor);
        dashedPipeline.Use();
        MissingTexBuffer.Bind();
        LineLoop.DrawInstanced(MissingTexBuffer.Count);
    }

    public void Dispose()
    {
        PicBuffer.Dispose();
        TexBuffer.Dispose();
        MissingPicBuffer.Dispose();
        MissingTexBuffer.Dispose();
    }
}
