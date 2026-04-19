using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Elmanager.ElmaPrimitives;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.Lgr;
using Elmanager.Rec;
using Elmanager.Rendering.Camera;
using Elmanager.Rendering.OpenGL;
using Elmanager.Rendering.Scene;
using Elmanager.UI;
using Elmanager.Utilities;
using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using Buffer = Elmanager.Rendering.OpenGL.Buffer;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Elmanager.Rendering;

internal class ElmaRenderer : IDisposable
{
    private const double GroundDepth = 1000.0;
    private const int GroundStencil = 0;
    private const float MinDistance = 0.0f;
    private const float MaxDistance = 999.0f;

    private readonly Vertices _quad;
    private LevelGraphics? _graphics;

    private readonly IGraphicsContext _gfxContext;
    private readonly Vertices _lineLoop;
    private int _frameBuffer;
    private int _colorRenderBuffer;
    private int _depthStencilRenderBuffer;
    private int _maxRenderbufferSize;
    private readonly LgrCache _lgrCache = new();
    private int _viewportWidth = 1;
    private int _viewportHeight = 1;
    internal double AspectRatio => _viewportWidth / (double)_viewportHeight;
    internal int ViewportWidth => _viewportWidth;
    internal int ViewportHeight => _viewportHeight;

    private UniformBuffer CameraUniforms { get; }
    private UniformBuffer ColorUniforms { get; }
    private Pipelines Pipelines { get; }
    private readonly bool _ownsPipelines;

    internal ElmaRenderer(GLControl renderingTarget, RenderingSettings settings, Pipelines? pipelines = null)
    {
        _gfxContext = renderingTarget.Context!;
        InitializeOpengl(disableFrameBuffer: settings.DisableFrameBuffer);
        _gfxContext.MakeCurrent();

        Pipelines = pipelines ?? new Pipelines();
        _ownsPipelines = pipelines == null;

        _quad = CreateQuad();
        _lineLoop = CreateLineLoop();

        CameraUniforms = new UniformBuffer(0);
        ColorUniforms = new UniformBuffer(1);
        ColorUniforms.BindBufferBase();
        CameraUniforms.BindBufferBase();
    }

    internal ElmaRenderer(GLControl renderingTarget, RenderingSettings settings, ElmaRenderer otherElmaRenderer) : this(renderingTarget, settings, otherElmaRenderer.Pipelines)
    {
    }

    public OpenGlLgr? OpenGlLgr => _graphics?.LgrGraphics?.Lgr;
    private float _grassZoom = 1.0f;
    private bool _zoomTextures = true;

    private void InitializeBuffers(Level lev, OpenGlLgr? lgr, RenderingSettings settings)
    {
        _graphics?.Dispose();
        _grassZoom = (float)settings.GrassZoom;
        _zoomTextures = settings.ZoomTextures;
        GL.LineWidth(settings.LineWidth);
        var state = new LevEditState(lev, TransientElements.Empty);
        _graphics = new LevelGraphics(
            GroundSky: GroundSky.Create(state, lgr, settings, Pipelines.White1X1Texture),
            Objects: Objects.Create(state, settings, _quad),
            ObjectFrames: ObjectFrames.Create(settings),
            PolygonFrames: PolygonFrames.Create(state, settings),
            GraphicElementFrames: GraphicElementFrames.Create(state, settings, _lineLoop),
            Lines: Lines.Create(settings), Selection: Selection.Create(), LgrGraphics: lgr != null ? new LgrGraphics(
                Lgr: lgr,
                Pictures: Pictures.Create(state, lgr, settings, _quad),
                Textures: Textures.Create(state, lgr, settings, _quad),
                Grass: Grass.Create(state, lgr, settings, _quad),
                Players: Players.Create(lgr, _quad)
            ) : null);
    }

    public void UpdateBuffers(LevEditState state, LevVisualChange mod)
    {
        if (_graphics == null)
        {
            return;
        }
        MakeCurrent();

        if (mod.HasFlag(LevVisualChange.Ground))
        {
            _graphics.GroundSky.Update(state);
        }

        if (_graphics.LgrGraphics?.Lgr != null)
        {
            var lgr = _graphics.LgrGraphics.Lgr;
            if (mod.HasFlag(LevVisualChange.Grass))
            {
                _graphics.LgrGraphics.Grass.Update(state, lgr);
            }

            if (mod.HasFlag(LevVisualChange.Pictures))
            {
                _graphics.LgrGraphics.Pictures.Update(state, lgr);
            }

            if (mod.HasFlag(LevVisualChange.Textures))
            {
                _graphics.LgrGraphics.Textures.Update(state, lgr);
            }
        }

        if (mod.HasFlag(LevVisualChange.Grass))
        {
            _graphics.PolygonFrames.Update(state);
        }

        if (mod.HasFlag(LevVisualChange.Apples) || mod.HasFlag(LevVisualChange.Killers) || mod.HasFlag(LevVisualChange.Flowers))
        {
            _graphics.Objects.Update(state, mod);
        }

        if (mod.HasFlag(LevVisualChange.Pictures) || mod.HasFlag(LevVisualChange.Textures))
        {
            _graphics.GraphicElementFrames.Update(state);
        }
    }

    public void AddSelectionPoint(Vector p)
    {
        _graphics?.Selection.AddPoint(p);
    }

    public void AddSelectionLineLoop(IEnumerable<Vector> points)
    {
        _graphics?.Selection.AddLineLoop(points);
    }

    public void DrawSelection(Color color)
    {
        _graphics?.Selection.Draw(color, ColorUniforms, Pipelines.Lines);
    }

    private static readonly float[] QuadVertices =
    [
        0.0f, 0.0f,
        1.0f, 0.0f,
        1.0f, 1.0f,
        0.0f, 1.0f
    ];

    private static Vertices CreateQuad()
    {
        var vertInfo = new VertexInfo().Attr(0, VertexFormat.Float32x2);
        var quadVbo = VertexArray.Create(vertInfo, QuadVertices);
        var quadIbo = Buffer.CreateIndex([0, 1, 2, 0, 2, 3]);
        return new Vertices(quadVbo, quadIbo, PrimitiveType.Triangles);
    }

    private static Vertices CreateLineLoop()
    {
        var vertInfo = new VertexInfo().Attr(0, VertexFormat.Float32x2);
        var quadVbo = VertexArray.Create(vertInfo, QuadVertices);
        var quadIbo = Buffer.CreateIndex([0, 1, 2, 3]);
        return new Vertices(quadVbo, quadIbo, PrimitiveType.LineLoop);
    }

    public void UpdateFadedObjects(Level lev, HashSet<int> driverTakenApples)
    {
        _graphics?.Objects.SetVisibleObjects(lev, null, driverTakenApples);
    }

    public void SetHiddenObjects(Level lev, HashSet<int>? hiddenObjectIndices)
    {
        _graphics?.Objects.SetVisibleObjects(lev, hiddenObjectIndices, null);
    }

    private Bitmap GetSnapShot(ZoomController zoomCtrl, SceneSettings sceneSettings, RenderingSettings settings)
    {
        Bitmap snapShotBmp;
        if (_maxRenderbufferSize > 0)
        {
            var width = _maxRenderbufferSize;
            var height = _maxRenderbufferSize;
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _frameBuffer);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _frameBuffer);
            var oldViewportWidth = _viewportWidth;
            var oldViewportHeight = _viewportHeight;
            ResetViewport(width, height);
            zoomCtrl.ZoomFill(settings, AspectRatio);
            DrawScene(zoomCtrl.Cam, 0, sceneSettings);
            snapShotBmp = new Bitmap(width, height);
            var bmpData = snapShotBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);
            GL.ReadBuffer(ReadBufferMode.ColorAttachment0);
            GL.ReadPixels(0, 0, width, height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte,
                bmpData.Scan0);
            snapShotBmp.UnlockBits(bmpData);
            snapShotBmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
            ResetViewport(oldViewportWidth, oldViewportHeight);
        }
        else
        {
            snapShotBmp = GetSnapShotOfCurrent();
        }

        return snapShotBmp;
    }

    internal Bitmap GetSnapShotOfCurrent()
    {
        var width = _viewportWidth;
        var height = _viewportHeight;
        var snapShotBmp = new Bitmap(width, height);
        var bmpData = snapShotBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
            PixelFormat.Format24bppRgb);
        GL.ReadBuffer(ReadBufferMode.Front);
        GL.ReadPixels(0, 0, width, height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte,
            bmpData.Scan0);
        snapShotBmp.UnlockBits(bmpData);
        snapShotBmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
        return snapShotBmp;
    }

    internal void DrawCircle(Vector v, double radius, Color circleColor, int accuracy)
    {
        _graphics?.Lines.DrawCircle(v, radius, circleColor, accuracy, ColorUniforms, Pipelines.Lines);
    }

    internal void DrawDummyPlayer(double leftWheelx, double leftWheely, PlayerRenderOpts opts, RenderingSettings settings)
    {
        var player = new PlayerState(
            new Vector(leftWheelx + Level.GlobalBodyDifferenceFromLeftWheelX, leftWheely + Level.GlobalBodyDifferenceFromLeftWheelY),
            new Vector(leftWheelx, leftWheely),
            new Vector(leftWheelx + Level.RightWheelDifferenceFromLeftWheelX, leftWheely),
            0, 0,
            new Vector(leftWheelx + Level.HeadDifferenceFromLeftWheelX, leftWheely + Level.HeadDifferenceFromLeftWheelY),
            0, Direction.Left, 0, -1);

        DrawPlayer(player, opts, settings);
    }

    internal void DrawLine(Vector v1, Vector v2, Color color)
    {
        _graphics?.Lines.DrawLine(v1, v2, color, ColorUniforms, Pipelines.Lines);
    }

    internal void DrawLineStrip(Polygon polygon, Color color)
    {
        _graphics?.Lines.DrawLineStrip(polygon.Vertices, color, ColorUniforms, Pipelines.Lines);
    }

    internal void DrawLineStrip(IEnumerable<Vector> points, Color color)
    {
        _graphics?.Lines.DrawLineStrip(points, color, ColorUniforms, Pipelines.Lines);
    }

    internal void DrawPoint(Vector v, Color color)
    {
        _graphics?.Lines.DrawPoint(v, color, ColorUniforms, Pipelines.Lines);
    }

    internal void DrawPolygon(Polygon polygon, Color color)
    {
        _graphics?.Lines.DrawLineLoop(polygon.Vertices, color, ColorUniforms, Pipelines.Lines);
    }

    internal void DrawRectangle(Vector v1, Vector v2, Color rectColor)
    {
        _graphics?.Lines.DrawRectangle(v1, v2, rectColor, ColorUniforms, Pipelines.Lines);
    }

    internal void MakeCurrent()
    {
        _gfxContext.MakeCurrent();
    }

    internal void DrawScene(ElmaCamera camera, double time, SceneSettings sceneSettings)
    {
        if (_graphics == null) return;
        MakeCurrent();
        GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

        var (projection, bounds) = SetCamera(camera);

        float[] groundVerts =
        [
            (float)bounds.XMin, (float)bounds.YMax,
            (float)bounds.XMin, (float)bounds.YMin,
            (float)bounds.XMax, (float)bounds.YMin,
            (float)bounds.XMax, (float)bounds.YMax
        ];
        _graphics.GroundSky.GroundVertices.VertexArray.SetData(groundVerts, BufferUsageHint.StreamDraw);
        _graphics.GroundSky.DrawSky(ColorUniforms, Pipelines.GroundSky);
        SetCameraUniforms(projection, 0, 0, camera.ZoomLevel);
        _graphics.GroundSky.DrawGround(ColorUniforms, Pipelines.GroundSky);
        if (_graphics.LgrGraphics != null)
        {
            _graphics.Objects.Draw(_graphics.LgrGraphics.Lgr, time, false, Pipelines.Objects);
            _graphics.LgrGraphics.Grass.Draw(Pipelines.Grass);
            _graphics.LgrGraphics.Textures.Draw(Pipelines.TextureUnclipped, Pipelines.TextureGround, Pipelines.TextureSky);
            _graphics.LgrGraphics.Pictures.Draw(Pipelines.PictureUnclipped, Pipelines.PictureGround, Pipelines.PictureSky);
        }
        _graphics.GraphicElementFrames.DrawPictureFrames(ColorUniforms, Pipelines.GraphicElementFrames);
        _graphics.GraphicElementFrames.DrawTextureFrames(ColorUniforms, Pipelines.GraphicElementFrames);
        _graphics.GraphicElementFrames.DrawMissingPictureFrames(ColorUniforms, Pipelines.GraphicElementFramesDashed);
        _graphics.GraphicElementFrames.DrawMissingTextureFrames(ColorUniforms, Pipelines.GraphicElementFramesDashed);
        _graphics.PolygonFrames.Draw(ColorUniforms, _graphics.GroundSky.SkyVertices, Pipelines.Lines, Pipelines.LinesDashed);
        _graphics.ObjectFrames.Draw(_graphics.Objects, ColorUniforms, Pipelines.ObjectFrames);
        _graphics.Lines.DrawGrid(
            bounds.XMin, bounds.XMax,
            bounds.YMin, bounds.YMax,
            sceneSettings.GridOffset,
            ColorUniforms,
            Pipelines.LinesDashed);
    }

    public (Matrix4 Projection, Bounds Bounds) SetCamera(ElmaCamera camera)
    {
        var bounds = camera.GetBounds(AspectRatio);
        var projection = GetProjectionMatrix(bounds);
        SetCameraUniforms(projection, camera.CenterX, camera.CenterY, camera.ZoomLevel);
        return (projection, bounds);
    }

    private void SetCameraUniforms(Matrix4 projection, double centerX, double centerY, double zoomLevel)
    {
        var zoom = _zoomTextures ? 1.0f : (float)zoomLevel * 0.15f;
        CameraUniforms.SetData(new CameraUniforms(
            projection,
            new Vector2((float)centerX, (float)centerY),
            _grassZoom,
            zoom
        ));
    }

    private static Matrix4 GetProjectionMatrix(Bounds bounds) =>
        Matrix4.CreateOrthographicOffCenter(
            (float)bounds.XMin, (float)bounds.XMax,
            (float)bounds.YMin, (float)bounds.YMax,
            -(MinDistance - 1.0f), -(MaxDistance + 1.0f));

    public void DrawGraphicElementFrame(GraphicElement e, RenderingSettings settings, Color color)
    {
        bool dashed;
        switch (e)
        {
            case GraphicElement.Picture when settings.ShowPictureFrames || settings.ShowPictures:
            case GraphicElement.Texture when settings.ShowTextureFrames || settings.ShowTextures:
                dashed = false;
                break;
            case GraphicElement.MissingPicture when settings.ShowPictureFrames || settings.ShowPictures:
            case GraphicElement.MissingTexture when settings.ShowTextureFrames || settings.ShowTextures:
                dashed = true;
                break;
            default:
                return;
        }

        _graphics?.Lines.DrawRectangle(
            new Vector(e.Position.X, e.Position.Y),
            new Vector(e.Position.X + e.Width, e.Position.Y - e.Height),
            color, ColorUniforms, dashed ? Pipelines.LinesDashed : Pipelines.Lines);
    }

    public void DrawGrassPolygon(Polygon polygon, Color color, RenderingSettings settings)
    {
        var pts = new List<Vector>();
        int grassStart = polygon.SlopeInfo!.GrassStart;
        for (var index = grassStart; index < polygon.Vertices.Count; index++)
        {
            pts.Add(polygon.Vertices[index]);
        }
        for (var index = 0; index < grassStart; index++)
        {
            pts.Add(polygon.Vertices[index]);
        }
        _graphics?.Lines.DrawLineStrip(pts, color, ColorUniforms, Pipelines.Lines);

        if (settings.ShowInactiveGrassEdges)
        {
            var p1 = polygon.Vertices[grassStart == 0 ? ^1 : grassStart - 1];
            var p2 = polygon.Vertices[grassStart];
            _graphics?.Lines.DrawDashLine(p1.X, p1.Y, p2.X, p2.Y, color, ColorUniforms, Pipelines.LinesDashed);
        }
    }

    internal void Swap()
    {
        _gfxContext.SwapBuffers();
    }

    internal void DrawSquare(Vector vector, double camZoomLevel, Color color)
    {
        _graphics?.Lines.DrawRectangle(
            new Vector(vector.X - camZoomLevel, vector.Y - camZoomLevel),
            new Vector(vector.X + camZoomLevel, vector.Y + camZoomLevel),
            color, ColorUniforms, Pipelines.Lines);
    }

    internal void ResetViewport(int width, int height)
    {
        GL.Viewport(0, 0, width, height);
        _viewportWidth = width;
        _viewportHeight = height;
    }

    internal RendererSettingsChangeResult UpdateSettings(Level lev, RenderingSettings newSettings)
    {
        var currentLgr = OpenGlLgr?.CurrentLgr.Path;
        var newLgr = newSettings.ResolveLgr(lev);
        var lgrUpdated = false;
        var lgr = OpenGlLgr;
        if (!currentLgr.EqualsIgnoreCase(newLgr))
        {
            if (File.Exists(newLgr))
            {
                var old = lgr;
                try
                {
                    lgr = new OpenGlLgr(_lgrCache.GetOrLoadLgr(newLgr));
                    lev.UpdateImages(lgr.DrawableImages);
                    lev.UpdateGrass(newSettings.GrassZoom);
                    old?.Dispose();
                    lgrUpdated = true;
                }
                catch (Exception e)
                {
                    UiUtils.ShowError("Error occurred when loading LGR file! Reason:\r\n\r\n" + e.Message);
                }
            }
        }
        else if (lgr != null)
        {
            lev.UpdateImages(lgr.DrawableImages);
            lev.UpdateGrass(newSettings.GrassZoom);
        }
        GL.ClearColor(newSettings.SkyFillColor);
        GL.LineWidth(newSettings.LineWidth);
        GL.PointSize((float)(newSettings.VertexSize * 300));
        InitializeBuffers(lev, lgr, newSettings);
        return new RendererSettingsChangeResult(lgrUpdated);
    }

    public void Dispose()
    {
        _graphics?.Dispose();
        _quad.Dispose();
        _lineLoop.Dispose();
        CameraUniforms.Dispose();
        ColorUniforms.Dispose();

        if (_ownsPipelines)
        {
            Pipelines.Dispose();
        }

        if (_frameBuffer != 0)
        {
            GL.DeleteFramebuffer(_frameBuffer);
        }
        if (_colorRenderBuffer != 0)
        {
            GL.DeleteRenderbuffer(_colorRenderBuffer);
        }
        if (_depthStencilRenderBuffer != 0)
        {
            GL.DeleteRenderbuffer(_depthStencilRenderBuffer);
        }
    }

    internal void DrawPlayer(PlayerState player, PlayerRenderOpts opts, RenderingSettings settings)
    {
        if (_graphics == null) return;

        if (opts.UseGraphics && _graphics.LgrGraphics != null)
        {
            var visual = ToVisualPlayer(player, settings, opts.IsActive, opts.UseTransparency);

            _graphics.LgrGraphics.Players.UpdateBuffers([visual]);
            _graphics.LgrGraphics.Players.Draw(Pipelines.Players);
        }
        else
        {
            DrawPlayerFrames(player, opts.Color, settings.CircleDrawingAccuracy, opts.UseTransparency);
        }
    }

    internal void DrawPlayers(IReadOnlyList<(PlayerState State, PlayerRenderOpts Opts)> players, RenderingSettings settings)
    {
        if (_graphics == null) return;

        var visuals = new List<VisualPlayer>();
        foreach (var (state, opts) in players)
        {
            if (opts.UseGraphics && _graphics.LgrGraphics != null)
            {
                visuals.Add(ToVisualPlayer(state, settings, opts.IsActive, opts.UseTransparency));
            }
            else
            {
                DrawPlayerFrames(state, opts.Color, settings.CircleDrawingAccuracy, opts.UseTransparency);
            }
        }

        if (visuals.Count > 0 && _graphics.LgrGraphics != null)
        {
            _graphics.LgrGraphics.Players.UpdateBuffers(visuals);
            _graphics.LgrGraphics.Players.Draw(Pipelines.Players);
        }
    }

    private static VisualPlayer ToVisualPlayer(PlayerState state, RenderingSettings sceneSettings, bool isActive, bool useTransparency)
    {
        return new VisualPlayer
        {
            Body = new VisualBikePart
            {
                Center = new Vector2((float)state.GlobalBody.X, (float)state.GlobalBody.Y),
                Rotation = (float)(state.BikeRotation * MathUtils.DegToRad)
            },
            HeadNoTurn = new VisualBikePart
            {
                Center = new Vector2((float)state.HeadCenter.X, (float)state.HeadCenter.Y),
                Rotation = (float)(state.BikeRotation * MathUtils.DegToRad)
            },
            LeftWheel = new VisualBikePart
            {
                Center = new Vector2((float)state.LeftWheel.X, (float)state.LeftWheel.Y),
                Rotation = (float)state.LeftWheelRotation
            },
            RightWheel = new VisualBikePart
            {
                Center = new Vector2((float)state.RightWheel.X, (float)state.RightWheel.Y),
                Rotation = (float)state.RightWheelRotation
            },
            Direction = state.Direction,
            ArmProgress = (float)state.ArmRotation,
            TurnProgress = (float)state.TurnProgress,
            BaseDistance = (sceneSettings.PicturesInBackground ? 0.0f : 499.0f) +
                           (isActive ? 0.0f : 0.5f),
            Opacity = useTransparency ? 0.5f : 1.0f
        };
    }

    private void DrawPlayerFrames(PlayerState player, Color playerColor, int circleAccuracy, bool dashed)
    {
        if (_graphics == null) return;

        var pipeline = dashed ? Pipelines.LinesDashed : Pipelines.Lines;
        const double wheelRadius = OpenGlLgr.ObjectRadius;
        const double headRadius = ElmaConstants.HeadDiameter / 2;

        _graphics.Lines.DrawCircle(player.LeftWheel, wheelRadius, playerColor, circleAccuracy, ColorUniforms, pipeline);
        _graphics.Lines.DrawCircle(player.RightWheel, wheelRadius, playerColor, circleAccuracy, ColorUniforms, pipeline);

        for (var k = 0; k < 2; k++)
        {
            var w = k == 0 ? player.LeftWheel : player.RightWheel;
            var wrot = k == 0 ? player.LeftWheelRotation : player.RightWheelRotation;
            for (var j = 0; j < 4; j++)
            {
                _graphics.Lines.DrawLine(
                    w,
                    new Vector(
                        w.X + wheelRadius * Math.Cos(wrot + j * Math.PI / 2),
                        w.Y + wheelRadius * Math.Sin(wrot + j * Math.PI / 2)),
                    playerColor, ColorUniforms, pipeline);
            }
        }

        _graphics.Lines.DrawCircle(player.Head, headRadius, playerColor, circleAccuracy, ColorUniforms, pipeline);

        var headCos = Math.Cos(player.BikeRotation * MathUtils.DegToRad);
        var headSin = Math.Sin(player.BikeRotation * MathUtils.DegToRad);
        var f = player.Direction == Direction.Right ? 1.0 : -1.0;
        _graphics.Lines.DrawLine(
            player.Head,
            new Vector(player.Head.X + f * headCos * headRadius, player.Head.Y + f * headSin * headRadius),
            playerColor, ColorUniforms, pipeline);
    }

    private void InitializeOpengl(bool disableFrameBuffer)
    {
        GL.ClearDepth(GroundDepth);
        GL.ClearStencil(GroundStencil);
        GL.Hint(HintTarget.TextureCompressionHint, HintMode.Fastest);
        GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Fastest);
        GL.Hint(HintTarget.LineSmoothHint, HintMode.Fastest);

        _maxRenderbufferSize = Math.Min(GL.GetInteger(GetPName.MaxRenderbufferSize), 4096);

        if (GL.GetError() != ErrorCode.NoError || disableFrameBuffer)
        {
            _maxRenderbufferSize = 0;
        }

        if (_maxRenderbufferSize > 0)
        {
            _colorRenderBuffer = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _colorRenderBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Rgba8, _maxRenderbufferSize,
                _maxRenderbufferSize);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

            _depthStencilRenderBuffer = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _depthStencilRenderBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthStencil,
                _maxRenderbufferSize, _maxRenderbufferSize);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

            _frameBuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _frameBuffer);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0,
                RenderbufferTarget.Renderbuffer, _colorRenderBuffer);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,
                FramebufferAttachment.DepthStencilAttachment,
                RenderbufferTarget.Renderbuffer, _depthStencilRenderBuffer);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
    }

    public void DrawDashLine(double x1, double y1, double x2, double y2, Color color)
    {
        _graphics?.Lines.DrawDashLine(x1, y1, x2, y2, color, ColorUniforms, Pipelines.LinesDashed);
    }

    public void SaveSnapShot(string fileName, ZoomController zoomCtrl, SceneSettings sceneSettings, RenderingSettings settings)
    {
        using var bmp = GetSnapShot(zoomCtrl, sceneSettings, settings);
        bmp.Save(fileName, ImageFormat.Png);
    }

    internal static double GetFirstGridLine(double size, double offset, double min)
    {
        if (offset < 0)
        {
            offset = size + (offset % size);
        }
        var tmp = (Math.Floor(min / size) + 1) * size;
        var left = (tmp - (size + offset));
        return left;
    }
}