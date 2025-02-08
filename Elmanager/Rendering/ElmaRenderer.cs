using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Elmanager.Application;
using Elmanager.ElmaPrimitives;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.Lgr;
using Elmanager.Rec;
using Elmanager.Rendering.Camera;
using Elmanager.UI;
using Elmanager.Utilities;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.GLControl;
//using OpenTK.Graphics.OpenGL;
using Color = System.Drawing.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Elmanager.Rendering;

internal class ElmaRenderer : IDisposable
{
    private const double GroundDepth = 0;
    public const int GroundStencil = 2;
    private const double SkyDepth = 0;
    private const int SkyStencil = 1;
    public const int StencilMask = 255;
    public const double TextureCoordConst = TextureVertexConst * 2;
    public const double TextureVertexConst = 1000;
    public const double TextureZoomConst = 10000.0;
    public const double ZFar = 1;
    public const double ZNear = -1;
    private const int LinePattern = 0b0101010101010101;

    private bool _disposed;

    private readonly IGraphicsContext _gfxContext;
    private int _frameBuffer;
    private int _colorRenderBuffer;
    private int _depthStencilRenderBuffer;
    private int _maxRenderbufferSize;
    public LgrCache _lgrCache = new();

    internal ElmaRenderer(GLControl renderingTarget, RenderingSettings settings)
    {
        _gfxContext = renderingTarget.Context;
        InitializeOpengl(disableFrameBuffer: settings.DisableFrameBuffer);
    }

    public OpenGlLgr? OpenGlLgr { get; set; }

    public void Dispose()
    {
        Dispose(true);
    }

    private Bitmap GetSnapShot(Level lev, ZoomController zoomCtrl, SceneSettings sceneSettings, RenderingSettings settings)
    {
        Bitmap snapShotBmp;
        if (_maxRenderbufferSize > 0)
        {
            var width = _maxRenderbufferSize;
            var height = _maxRenderbufferSize;
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _frameBuffer);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _frameBuffer);
            var oldViewPort = (int[])zoomCtrl.Cam.Viewport.Clone();
            ResetViewport(width, height);
            zoomCtrl.ZoomFill(settings);
            DrawScene(lev, zoomCtrl.Cam, sceneSettings, settings);
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
            ResetViewport(oldViewPort[2], oldViewPort[3]);
        }
        else
        {
            snapShotBmp = GetSnapShotOfCurrent(zoomCtrl);
        }

        return snapShotBmp;
    }

    public Bitmap GetSnapShotForCustomShape(Level lev, ZoomController zoomCtrl, SceneSettings sceneSettings, RenderingSettings settings)
    {
        var width = zoomCtrl.Cam.ViewPortWidth;
        var height = zoomCtrl.Cam.ViewPortHeight;
        GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _frameBuffer);
        GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _frameBuffer);
        DrawScene(lev, zoomCtrl.Cam, sceneSettings, settings);
        var snapShotBmp = new Bitmap(width, height);
        var bmpData = snapShotBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
            PixelFormat.Format24bppRgb);
        GL.ReadBuffer(ReadBufferMode.Front);
        GL.ReadPixels(0, 0, width, height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte,
            bmpData.Scan0);
        snapShotBmp.UnlockBits(bmpData);
        snapShotBmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
        GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
        GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
        return snapShotBmp;
    }

    internal static Bitmap GetSnapShotOfCurrent(ZoomController zoomCtrl)
    {
        var width = zoomCtrl.Cam.ViewPortWidth;
        var height = zoomCtrl.Cam.ViewPortHeight;
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

    private void DrawCircle(double x, double y, double radius, Color circleColor, int accuracy)
    {
        GL.Color4(circleColor);
        GL.Begin(PrimitiveType.LineLoop);
        for (var i = 0; i <= accuracy; i++)
            GL.Vertex2(x + Math.Cos(i * 360 / (double)accuracy * Math.PI / 180) * radius,
                y + Math.Sin(i * 360 / (double)accuracy * Math.PI / 180) * radius);
        GL.End();
    }

    internal void DrawCircle(Vector v, double radius, Color circleColor, int accuracy)
    {
        DrawCircle(v.X, v.Y, radius, circleColor, accuracy);
    }

    internal void DrawDummyPlayer(double leftWheelx, double leftWheely, SceneSettings sceneSettings, PlayerRenderOpts opts, RenderingSettings settings)
    {
        DrawPlayer(new PlayerState(leftWheelx + Level.GlobalBodyDifferenceFromLeftWheelX,
            leftWheely + Level.GlobalBodyDifferenceFromLeftWheelY, leftWheelx, leftWheely,
            leftWheelx + Level.RightWheelDifferenceFromLeftWheelX, leftWheely, 0, 0,
            leftWheelx + Level.HeadDifferenceFromLeftWheelX, leftWheely + Level.HeadDifferenceFromLeftWheelY,
            0, Direction.Left, 0), opts, sceneSettings, settings);
    }

    internal void DrawLine(Vector v1, Vector v2, Color color, double depth = 0)
    {
        GL.Color4(color);
        GL.Begin(PrimitiveType.Lines);
        GL.Vertex3(v1.X, v1.Y, depth);
        GL.Vertex3(v2.X, v2.Y, depth);
        GL.End();
    }

    private void DrawLine(double x1, double y1, double x2, double y2, Color color, double depth = 0)
    {
        GL.Color4(color);
        GL.Begin(PrimitiveType.Lines);
        GL.Vertex3(x1, y1, depth);
        GL.Vertex3(x2, y2, depth);
        GL.End();
    }

    private void DrawLineFast(double x1, double y1, double x2, double y2, double depth = 0)
    {
        GL.Vertex3(x1, y1, depth);
        GL.Vertex3(x2, y2, depth);
    }

    internal void DrawLineStrip(Polygon polygon, Color color, double depth = 0)
    {
        GL.Color4(color);
        GL.Begin(PrimitiveType.LineStrip);
        foreach (var x in polygon.Vertices)
            GL.Vertex3(x.X, x.Y, depth);
        GL.End();
    }

    internal void DrawPoint(Vector v, Color color, double depth = 0)
    {
        GL.Color4(color);
        GL.Begin(PrimitiveType.Points);
        GL.Vertex3(v.X, v.Y, depth);
        GL.End();
    }

    private void DrawPoint(double x, double y, Color color, double depth = 0)
    {
        GL.Color4(color);
        GL.Begin(PrimitiveType.Points);
        GL.Vertex3(x, y, depth);
        GL.End();
    }

    internal void DrawPolygon(Polygon polygon, Color color, double depth = 0)
    {
        GL.Color4(color);
        GL.Begin(PrimitiveType.LineLoop);
        foreach (var x in polygon.Vertices)
            GL.Vertex3(x.X, x.Y, depth);
        GL.End();
    }

    internal void DrawRectangle(double x1, double y1, double x2, double y2, Color rectColor)
    {
        GL.Color4(rectColor);
        GL.Begin(PrimitiveType.LineLoop);
        GL.Vertex2(x1, y1);
        GL.Vertex2(x2, y1);
        GL.Vertex2(x2, y2);
        GL.Vertex2(x1, y2);
        GL.End();
    }

    private void DrawRectangle(double x1, double y1, double x2, double y2)
    {
        GL.Begin(PrimitiveType.LineLoop);
        GL.Vertex2(x1, y1);
        GL.Vertex2(x2, y1);
        GL.Vertex2(x2, y2);
        GL.Vertex2(x1, y2);
        GL.End();
    }

    internal void DrawRectangle(Vector v1, Vector v2, Color rectColor)
    {
        GL.Color4(rectColor);
        GL.Begin(PrimitiveType.LineLoop);
        GL.Vertex2(v1.X, v1.Y);
        GL.Vertex2(v2.X, v1.Y);
        GL.Vertex2(v2.X, v2.Y);
        GL.Vertex2(v1.X, v2.Y);
        GL.End();
    }

    internal void MakeCurrent()
    {
        _gfxContext.MakeCurrent();
    }

    internal void MakeNoneCurrent()
    {
        _gfxContext.MakeNoneCurrent();
    }

    internal void DrawScene(Level lev, ElmaCamera cam, SceneSettings sceneSettings, RenderingSettings settings)
    {
        MakeCurrent();
        var aspectRatio = cam.AspectRatio;
        var centerX = cam.CenterX;
        var centerY = cam.CenterY;
        GL.LoadIdentity();
        GL.Ortho(cam.XMin, cam.XMax, cam.YMin, cam.YMax, ZNear, ZFar);

        GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit |
                 ClearBufferMask.ColorBufferBit);
        GL.Enable(EnableCap.StencilTest);
        GL.Disable(EnableCap.Texture2D);
        GL.Disable(EnableCap.Blend);
        GL.StencilOp(StencilOp.Incr, StencilOp.Keep, StencilOp.Decr);
        GL.StencilFunc(StencilFunction.Equal, GroundStencil, StencilMask);
        GL.ColorMask(false, false, false, false);
        GL.Begin(PrimitiveType.Triangles);
        foreach (var k in lev.Polygons.Concat(sceneSettings.TransientElements.Polygons))
            if (!k.IsGrass)
                DrawFilledTrianglesFast(k.Decomposition, ZFar - (ZFar - ZNear) * SkyDepth);
        GL.End();
        GL.ColorMask(true, true, true, true);
        GL.Enable(EnableCap.Texture2D);
        GL.Enable(EnableCap.DepthTest);
        GL.StencilFunc(StencilFunction.Equal, GroundStencil, StencilMask);
        GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
        var midX = (lev.Bounds.XMin + lev.Bounds.XMax) / 2;
        var midY = (lev.Bounds.YMin + lev.Bounds.YMax) / 2;
        var groundSky = OpenGlLgr?.GetGroundAndSky(lev, settings.DefaultGroundAndSky);
        if (settings.ShowGround)
        {
            const double depth = ZFar - (ZFar - ZNear) * GroundDepth;
            if (settings.GroundTextureEnabled && OpenGlLgr != null && groundSky is var (ground, _))
            {
                OpenGlLgr.DrawFullScreenTexture(cam, ground, midX, midY, depth, settings);
            }
            else
            {
                GL.Disable(EnableCap.Texture2D);
                GL.Color4(settings.GroundFillColor);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex3(midX - TextureVertexConst, midY - TextureVertexConst, depth);
                GL.Vertex3(midX + TextureVertexConst, midY - TextureVertexConst, depth);
                GL.Vertex3(midX + TextureVertexConst, midY + TextureVertexConst, depth);
                GL.Vertex3(midX - TextureVertexConst, midY + TextureVertexConst, depth);
                GL.End();
                GL.Enable(EnableCap.Texture2D);
            }
        }

        GL.StencilFunc(StencilFunction.Equal, SkyStencil, StencilMask);
        if (settings.SkyTextureEnabled && OpenGlLgr != null && groundSky is var (_, sky))
        {
            const double depth = ZFar - (ZFar - ZNear) * SkyDepth;
            GL.BindTexture(TextureTarget.Texture2D, sky.TextureId);
            if (settings.ZoomTextures)
            {
                GL.Begin(PrimitiveType.Quads);
                GL.TexCoord2(0, TextureCoordConst / sky.Width * sky.Width / sky.Height);
                GL.Vertex3(centerX / 2 - TextureVertexConst, centerY - TextureVertexConst, depth);
                GL.TexCoord2(TextureCoordConst / sky.Width,
                    TextureCoordConst / sky.Width * sky.Width / sky.Height);
                GL.Vertex3(centerX / 2 + TextureVertexConst, centerY - TextureVertexConst, depth);
                GL.TexCoord2(TextureCoordConst / sky.Width, 0);
                GL.Vertex3(centerX / 2 + TextureVertexConst, centerY + TextureVertexConst, depth);
                GL.TexCoord2(0, 0);
                GL.Vertex3(centerX / 2 - TextureVertexConst, centerY + TextureVertexConst, depth);
                GL.End();
            }
            else
            {
                var xdelta = centerX / sky.Width;
                var skyAspectRatio = sky.AspectRatio;
                const int xRepeat = 3;
                GL.PushMatrix();
                GL.LoadIdentity();
                GL.Ortho(0, 1, 0, 1, ZNear, ZFar);
                GL.Begin(PrimitiveType.Quads);
                GL.TexCoord2(xdelta, 0);
                GL.Vertex3(0, 1, depth);
                GL.TexCoord2(xRepeat + xdelta, 0);
                GL.Vertex3(1, 1, depth);
                GL.TexCoord2(xRepeat + xdelta,
                    xRepeat * skyAspectRatio / aspectRatio);
                GL.Vertex3(1, 0, depth);
                GL.TexCoord2(xdelta,
                    xRepeat * skyAspectRatio / aspectRatio);
                GL.Vertex3(0, 0, depth);
                GL.End();
                GL.PopMatrix();
            }
        }

        GL.Enable(EnableCap.AlphaTest);
        if (OpenGlLgr != null)
        {
            GL.DepthFunc(DepthFunction.Gequal);
            GL.Enable(EnableCap.ScissorTest);
            var graphicElements = lev.GraphicElements.Concat(sceneSettings.TransientElements.GraphicElements)
                .AsEnumerable().Reverse();
            DrawPictures(graphicElements.Where(p => p.Clipping == ClippingType.Unclipped), cam, settings);
            DrawPictures(graphicElements.Where(p => p.Clipping != ClippingType.Unclipped), cam, settings);

            foreach (var picture in lev.GraphicElements.Concat(sceneSettings.TransientElements.GraphicElements))
            {
                if (picture is GraphicElement.Texture t && settings.ShowTextures)
                {
                    DoPictureScissor(picture, cam);
                    GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Invert);
                    switch (picture.Clipping)
                    {
                        case ClippingType.Ground:
                            GL.StencilFunc(StencilFunction.Equal, GroundStencil, StencilMask);
                            break;
                        case ClippingType.Sky:
                            GL.StencilFunc(StencilFunction.Equal, SkyStencil, StencilMask);
                            break;
                        case ClippingType.Unclipped:
                            GL.StencilFunc(StencilFunction.Gequal, 5, StencilMask);
                            break;
                    }

                    var depth = (picture.Distance / 1000.0 * (ZFar - ZNear)) + ZNear;
                    OpenGlLgr.DrawPicture(t.MaskInfo.TextureId, picture.Position.X, picture.Position.Y, picture.Width, -picture.Height,
                        depth + 0.001, TexCoord.Default);

                    var info = t.TextureInfo;
                    GL.StencilFunc(StencilFunction.Lequal, 5, StencilMask);
                    OpenGlLgr.DrawFullScreenTexture(cam, info, midX, midY, depth, settings);
                }
            }

            GL.Disable(EnableCap.ScissorTest);
            GL.DepthFunc(DepthFunction.Gequal);
            if (settings.ShowGrass)
            {
                OpenGlLgr.DrawGrass(lev, cam, midX, midY, settings, sceneSettings);
            }
        }

        GL.Disable(EnableCap.StencilTest);
        if (settings.ShowObjects && OpenGlLgr != null)
        {
            var depth = sceneSettings.PicturesInBackground ? 0 : 0.5 * (ZFar - ZNear) + ZNear;
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusDstColor);
            foreach (var (x, i) in GetVisibleObjects(lev, sceneSettings))
            {
                if (sceneSettings.FadedObjectIndices.Contains(i))
                {
                    GL.Enable(EnableCap.Blend);
                }
                else
                {
                    GL.Disable(EnableCap.Blend);
                }
                switch (x.Type)
                {
                    case ObjectType.Flower:
                        OpenGlLgr.DrawFlower(x.Position, depth);
                        break;
                    case ObjectType.Killer:
                        OpenGlLgr.DrawKiller(x.Position, depth);
                        break;
                    case ObjectType.Apple:
                        OpenGlLgr.DrawApple(x.Position, x.AnimationNumber, depth);
                        break;
                    case ObjectType.Start:
                        DrawDummyPlayer(x.Position.X, x.Position.Y, sceneSettings,
                            new PlayerRenderOpts { IsActive = true, UseTransparency = false, UseGraphics = true },
                            settings);
                        GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusDstColor);
                        break;
                }
            }
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        GL.Disable(EnableCap.Texture2D);
        GL.Disable(EnableCap.StencilTest);
        GL.Disable(EnableCap.DepthTest);
        GL.Disable(EnableCap.AlphaTest);

        GL.Enable(EnableCap.Blend);
        if (settings.ShowGrid)
        {
            DrawGrid(cam, sceneSettings, settings);
        }
        
        if (settings.ShowMaxDimensions)
        {
            GL.Enable(EnableCap.LineStipple);
            GL.LineStipple((int)settings.LineWidth * 2, LinePattern);
            var levCenterX = (lev.Bounds.XMin + lev.Bounds.XMax) / 2;
            var levCenterY = (lev.Bounds.YMin + lev.Bounds.YMax) / 2;
            DrawRectangle(levCenterX - Level.MaximumSize / 2,
                levCenterY - Level.MaximumSize / 2,
                levCenterX + Level.MaximumSize / 2,
                levCenterY + Level.MaximumSize / 2,
                settings.MaxDimensionColor);
            GL.Disable(EnableCap.LineStipple);
            GL.LineWidth(settings.LineWidth);
        }

        if (settings.ShowObjectFrames)
            DrawObjectFrames(lev, sceneSettings, settings);
        if (settings.ShowObjectCenters)
            DrawObjectCenters(lev, sceneSettings, settings);
        if (settings.ShowGravityAppleArrows &&
            (settings.ShowObjectFrames || (OpenGlLgr != null && settings.ShowObjects)))
        {
            foreach (var (o, _) in GetVisibleObjects(lev, sceneSettings))
            {
                DrawGravityArrowMaybe(o, settings);
            }
        }

        foreach (var x in lev.Polygons)
        {
            if (x.IsGrass)
            {
                if (settings.ShowGrassEdges)
                {
                    var color = x.SlopeInfo?.HasError ?? false ? Color.Red : settings.GrassEdgeColor;
                    DrawGrassPolygon(x, color, settings.ShowInactiveGrassEdges, settings);
                }
            }
            else if (settings.ShowGroundEdges)
                DrawPolygon(x, settings.GroundEdgeColor);
        }

        GL.Color4(settings.TextureFrameColor);
        foreach (var z in lev.GraphicElements.Concat(sceneSettings.TransientElements.GraphicElements))
        {
            DrawGraphicElementFrame(z, settings, null);
        }

        if (settings.ShowVertices)
        {
            var showGrassVertices = settings.ShowGrassEdges || settings.ShowGrass;
            var showGroundVertices = settings.ShowGroundEdges || (settings.ShowGround && OpenGlLgr != null);
            GL.Color4(settings.VertexColor);
            if (settings.UseCirclesForVertices)
            {
                GL.Begin(PrimitiveType.Points);
                foreach (var x in lev.Polygons)
                    if ((showGrassVertices && x.IsGrass) || (showGroundVertices && !x.IsGrass))
                        foreach (var z in x.Vertices)
                            GL.Vertex3(z.X, z.Y, 0);
            }
            else
            {
                GL.Begin(PrimitiveType.Triangles);
                foreach (var x in lev.Polygons)
                    if ((showGrassVertices && x.IsGrass) || (showGroundVertices && !x.IsGrass))
                        foreach (var z in x.Vertices)
                            DrawEquilateralTriangleFast(z, cam.ZoomLevel * settings.VertexSize);
            }

            GL.End();
        }
    }

    private void DrawPictures(IEnumerable<GraphicElement> pics, ElmaCamera cam, RenderingSettings settings)
    {
        foreach (var picture in pics)
        {
            if (picture is GraphicElement.Picture p && settings.ShowPictures)
            {
                DoPictureScissor(picture, cam);
                GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
                switch (picture.Clipping)
                {
                    case ClippingType.Unclipped:
                        GL.StencilFunc(StencilFunction.Always, 0, StencilMask);
                        break;
                    case ClippingType.Sky:
                        GL.StencilFunc(StencilFunction.Equal, SkyStencil, StencilMask);
                        break;
                    case ClippingType.Ground:
                        GL.StencilFunc(StencilFunction.Equal, GroundStencil, StencilMask);
                        break;
                }

                OpenGlLgr.DrawPicture(p.PictureInfo.TextureId, picture.Position.X, picture.Position.Y, picture.Width,
                    -picture.Height,
                    (picture.Distance / 1000.0 * (ZFar - ZNear)) + ZNear, TexCoord.Default);
            }
        }
    }

    public void DrawGraphicElementFrame(GraphicElement e, RenderingSettings settings, Color? overrideColor)
    {
        var forceShow = overrideColor is not null;
        switch (e)
        {
            case GraphicElement.Picture:
                if (!settings.ShowPictureFrames && !forceShow)
                {
                    return;
                }
                GL.Color4(overrideColor ?? settings.PictureFrameColor);
                break;
            case GraphicElement.Texture:
                if (!settings.ShowTextureFrames && !forceShow)
                {
                    return;
                }
                GL.Color4(overrideColor ?? settings.TextureFrameColor);
                break;
            case GraphicElement.MissingPicture when settings.ShowPictureFrames || settings.ShowPictures || forceShow:
                DrawLine(e.Position.X, e.Position.Y, e.Position.X + e.Width, e.Position.Y - e.Height,
                    overrideColor ?? settings.PictureFrameColor);
                DrawLine(e.Position.X + e.Width, e.Position.Y, e.Position.X, e.Position.Y - e.Height,
                    overrideColor ?? settings.PictureFrameColor);
                break;
            case GraphicElement.MissingTexture when settings.ShowTextureFrames || settings.ShowTextures || forceShow:
                DrawLine(e.Position.X, e.Position.Y, e.Position.X + e.Width, e.Position.Y - e.Height,
                    overrideColor ?? settings.TextureFrameColor);
                DrawLine(e.Position.X + e.Width, e.Position.Y, e.Position.X, e.Position.Y - e.Height,
                    overrideColor ?? settings.TextureFrameColor);
                break;
            default:
                return;
        }

        DrawRectangle(e.Position.X, e.Position.Y, e.Position.X + e.Width, e.Position.Y - e.Height);
    }

    public void DrawGrassPolygon(Polygon polygon, Color color, bool drawInactiveGrassEdge, RenderingSettings settings)
    {
        GL.Color4(color);
        GL.Begin(PrimitiveType.LineStrip);
        for (var index = polygon.GrassStart; index < polygon.Vertices.Count; index++)
        {
            var z = polygon.Vertices[index];
            GL.Vertex3(z.X, z.Y, 0);
        }

        for (var index = 0; index < polygon.GrassStart; index++)
        {
            var z = polygon.Vertices[index];
            GL.Vertex3(z.X, z.Y, 0);
        }

        GL.End();
        if (drawInactiveGrassEdge)
        {
            DrawDashLine(polygon.Vertices[polygon.GrassStart == 0 ? ^1 : polygon.GrassStart - 1],
                polygon.Vertices[polygon.GrassStart],
                color, settings);
        }
    }

    internal void Swap()
    {
        _gfxContext.SwapBuffers();
    }

    private IEnumerable<(LevObject, int)> GetVisibleObjects(Level lev, SceneSettings sceneSettings)
    {
        return lev.Objects.Select((t, i) => (t, i)).Where(t => !sceneSettings.HiddenObjectIndices.Contains(t.i))
            .Concat(sceneSettings.TransientElements.Objects.Select(o => (o, -1)));
    }

    private void DoPictureScissor(GraphicElement graphicElement, ElmaCamera cam)
    {
        var x = (int)((graphicElement.Position.X - cam.XMin) / (cam.XMax - cam.XMin) * cam.ViewPortWidth);
        var y = (int)(((graphicElement.Position.Y - graphicElement.Height) - cam.YMin) / (cam.YMax - cam.YMin) * cam.ViewPortHeight);
        var w = (int)((graphicElement.Position.X + graphicElement.Width - cam.XMin) / (cam.XMax - cam.XMin) * cam.ViewPortWidth) - x;
        var h = (int)((graphicElement.Position.Y - cam.YMin) / (cam.YMax - cam.YMin) * cam.ViewPortHeight) - y;
        GL.Scissor(x, y, w, h);
    }

    private void DrawGravityArrowMaybe(LevObject o, RenderingSettings settings)
    {
        if (o.Type == ObjectType.Apple && o.AppleType != AppleType.Normal)
        {
            GL.Color4(settings.AppleGravityArrowColor);
            double arrowRotation = o.AppleType switch
            {
                AppleType.GravityUp => 0.0,
                AppleType.GravityDown => 180.0,
                AppleType.GravityLeft => 90.0,
                AppleType.GravityRight => 270.0,
                _ => throw new ArgumentOutOfRangeException()
            };

            const double arrowThickness = 0.4;
            GL.PushMatrix();
            GL.Translate(o.Position.X, o.Position.Y, 0.0);
            GL.Scale(0.5, 0.5, 1.0);
            GL.Rotate(arrowRotation, 0.0, 0.0, 1.0);
            GL.Translate(-0.5, -0.5, 0.0);
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex2((1 - arrowThickness) / 2.0, 0.0);
            GL.Vertex2((1 - arrowThickness) / 2.0, 0.5);
            GL.Vertex2(0.0, 0.5);
            GL.Vertex2(0.5, 1.0);
            GL.Vertex2(1.0, 0.5);
            GL.Vertex2(1.0 - (1 - arrowThickness) / 2.0, 0.5);
            GL.Vertex2(1.0 - (1 - arrowThickness) / 2.0, 0.0);
            GL.End();
            GL.PopMatrix();
        }
    }

    internal void DrawSquare(Vector v, double side, Color color)
    {
        DrawRectangle(v.X - side, v.Y - side, v.X + side, v.Y + side, color);
    }

    internal void DrawEquilateralTriangle(Vector center, double side, Color color)
    {
        GL.Color4(color);
        GL.Begin(PrimitiveType.Triangles);
        DrawEquilateralTriangleFast(center, side);
        GL.End();
    }

    private void DrawEquilateralTriangleFast(Vector center, double side)
    {
        const double factor = 1 / (1.7320508075688772935274463415059 * 2);
        GL.Vertex3(center.X + side / 2, center.Y - side * factor, 0);
        GL.Vertex3(center.X, center.Y + side / Math.Sqrt(3), 0);
        GL.Vertex3(center.X - side / 2, center.Y - side * factor, 0);
    }

    internal void InitializeLevel(Level lev, RenderingSettings settings)
    {
        lev.Objects = lev.Objects.OrderBy(o => o.Type switch
        {
            ObjectType.Flower => 3,
            ObjectType.Apple => 2,
            ObjectType.Killer => 1,
            ObjectType.Start => 4,
            _ => throw new ArgumentOutOfRangeException()
        }).ToList();
        lev.UpdateAllPolygons(settings.GrassZoom);
    }

    internal void ResetViewport(int width, int height)
    {
        GL.Viewport(0, 0, width, height);
    }

    internal RendererSettingsChangeResult UpdateSettings(Level lev, RenderingSettings newSettings)
    {
        var currentLgr = OpenGlLgr?.CurrentLgr.Path;
        var newLgr = newSettings.ResolveLgr(lev);
        var lgrUpdated = false;
        if (!currentLgr.EqualsIgnoreCase(newLgr))
        {
            if (File.Exists(newLgr))
            {
                var old = OpenGlLgr;
                try
                {
                    OpenGlLgr = new OpenGlLgr(lev, _lgrCache.GetOrLoadLgr(newLgr), newSettings);
                    lev.UpdateImages(OpenGlLgr.DrawableImages);
                    old?.Dispose();
                    lgrUpdated = true;
                }
                catch (Exception e)
                {
                    UiUtils.ShowError("Error occurred when loading LGR file! Reason:\r\n\r\n" + e.Message);
                }
            }
        }
        else if (OpenGlLgr != null)
        {
            lev.UpdateImages(OpenGlLgr.DrawableImages);
            OpenGlLgr.RefreshGrassPics(lev, newSettings);
        }

        GL.ClearColor(newSettings.SkyFillColor);
        GL.LineWidth(newSettings.LineWidth);
        GL.PointSize((float)(newSettings.VertexSize * 300));
        return new RendererSettingsChangeResult(lgrUpdated);
    }

    private void Dispose(bool disposing)
    {
        // Check to see if Dispose has already been called.
        if (!_disposed)
        {
            // If disposing equals true, dispose all managed
            // and unmanaged resources.
            if (disposing)
            {
                // Dispose managed resources.
            }

            // Release unmanaged resources. If disposing is false,
            // only the following code is executed.
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

        _disposed = true;
    }

    private static void DrawFilledTrianglesFast(IEnumerable<Vector[]> triangles, double depth = 0.0)
    {
        foreach (var triangle in triangles)
            foreach (var x in triangle)
                GL.Vertex3(x.X, x.Y, depth);
    }

    private void DrawGrid(ElmaCamera cam, SceneSettings sceneSettings, RenderingSettings settings)
    {
        var gridSize = settings.GridSize;
        var current = GetFirstGridLine(gridSize, sceneSettings.GridOffset.X, cam.XMin);
        EnableDashLines(LinePattern, settings);
        GL.Color4(settings.GridColor);
        GL.Begin(PrimitiveType.Lines);
        while (!(current > cam.XMax))
        {
            DrawLineFast(current, cam.YMin, current, cam.YMax);
            current += gridSize;
        }

        current = GetFirstGridLine(gridSize, sceneSettings.GridOffset.Y, cam.YMin);
        while (!(current > cam.YMax))
        {
            DrawLineFast(cam.XMin, current, cam.XMax, current);
            current += gridSize;
        }

        GL.End();
        GL.Disable(EnableCap.LineStipple);
        GL.LineWidth(settings.LineWidth);
    }

    private void EnableDashLines(short pattern, RenderingSettings settings)
    {
        GL.Enable(EnableCap.LineStipple);
        GL.LineStipple((int)settings.LineWidth, pattern);
    }

    private void DrawObjectCenters(Level lev, SceneSettings sceneSettings, RenderingSettings settings)
    {
        foreach (var (x, _) in GetVisibleObjects(lev, sceneSettings))
        {
            switch (x.Type)
            {
                case ObjectType.Flower:
                    DrawPoint(x.Position, settings.FlowerColor);
                    break;
                case ObjectType.Killer:
                    DrawPoint(x.Position, settings.KillerColor);
                    break;
                case ObjectType.Apple:
                    DrawPoint(x.Position, settings.AppleColor);
                    break;
                case ObjectType.Start:
                    DrawPoint(x.Position, settings.StartColor);
                    DrawPoint(x.Position.X + Level.RightWheelDifferenceFromLeftWheelX, x.Position.Y,
                        Global.AppSettings.LevelEditor.RenderingSettings.StartColor);
                    DrawPoint(x.Position.X + Level.HeadDifferenceFromLeftWheelX,
                        x.Position.Y + Level.HeadDifferenceFromLeftWheelY,
                        Global.AppSettings.LevelEditor.RenderingSettings.StartColor);
                    break;
            }
        }
    }

    private void DrawObjectFrames(Level lev, SceneSettings sceneSettings, RenderingSettings settings)
    {
        foreach (var (x, i) in GetVisibleObjects(lev, sceneSettings))
        {
            if (sceneSettings.FadedObjectIndices.Contains(i))
            {
                EnableDashLines(0b0110011001100110, settings);
            }
            else
            {
                GL.Disable(EnableCap.LineStipple);
            }
            switch (x.Type)
            {
                case ObjectType.Flower:
                    DrawCircle(x.Position, OpenGlLgr.ObjectRadius, settings.FlowerColor, settings.CircleDrawingAccuracy);
                    break;
                case ObjectType.Killer:
                    DrawCircle(x.Position, OpenGlLgr.ObjectRadius, settings.KillerColor, settings.CircleDrawingAccuracy);
                    break;
                case ObjectType.Apple:
                    DrawCircle(x.Position, OpenGlLgr.ObjectRadius, settings.AppleColor, settings.CircleDrawingAccuracy);
                    break;
                case ObjectType.Start:
                    DrawDummyPlayer(x.Position.X, x.Position.Y, sceneSettings,
                        new PlayerRenderOpts(settings.StartColor, true, false, false), settings);
                    break;
            }
        }
        GL.Disable(EnableCap.LineStipple);
    }

    internal void DrawPlayer(PlayerState player, PlayerRenderOpts opts, SceneSettings sceneSettings, RenderingSettings settings)
    {
        if (opts.UseGraphics && OpenGlLgr != null)
        {
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusDstColor);
            OpenGlLgr.DrawLgrPlayer(player, opts, sceneSettings);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }
        else
        {
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.DepthTest);
            if (!opts.IsActive)
            {
                GL.Enable(EnableCap.LineStipple);
                GL.LineStipple((int)settings.LineWidth, LinePattern);
            }

            DrawPlayerFrames(player, opts.Color, settings);
            if (!opts.IsActive)
            {
                GL.Disable(EnableCap.LineStipple);
            }
        }
    }

    private void DrawPlayerFrames(PlayerState player, Color playerColor, RenderingSettings settings)
    {
        var headCos = Math.Cos(player.BikeRotation * MathUtils.DegToRad);
        var headSin = Math.Sin(player.BikeRotation * MathUtils.DegToRad);
        var f = player.IsRight ? 1 : -1;
        var headLineEndPointX = player.HeadX + f * headCos * ElmaConstants.HeadDiameter / 2;
        var headLineEndPointY = player.HeadY + f * headSin * ElmaConstants.HeadDiameter / 2;
        DrawCircle(player.LeftWheelX, player.LeftWheelY, OpenGlLgr.ObjectRadius, playerColor, settings.CircleDrawingAccuracy);
        DrawCircle(player.RightWheelX, player.RightWheelY, OpenGlLgr.ObjectRadius, playerColor, settings.CircleDrawingAccuracy);
        GL.Begin(PrimitiveType.Lines);
        for (var k = 0; k < 2; k++)
        {
            for (var j = 0; j < 4; j++)
            {
                double wheelx;
                double wheely;
                double wheelrot;
                if (k == 0)
                {
                    wheelx = player.LeftWheelX;
                    wheely = player.LeftWheelY;
                    wheelrot = player.LeftWheelRotation;
                }
                else
                {
                    wheelx = player.RightWheelX;
                    wheely = player.RightWheelY;
                    wheelrot = player.RightWheelRotation;
                }

                GL.Vertex2(wheelx, wheely);
                GL.Vertex2(wheelx + OpenGlLgr.ObjectRadius * Math.Cos(wheelrot + j * Math.PI / 2),
                    wheely + OpenGlLgr.ObjectRadius * Math.Sin(wheelrot + j * Math.PI / 2));
            }
        }

        GL.End();
        DrawCircle(player.HeadX, player.HeadY, ElmaConstants.HeadDiameter / 2, playerColor, settings.CircleDrawingAccuracy);
        GL.Begin(PrimitiveType.Lines);
        GL.Vertex2(player.HeadX, player.HeadY);
        GL.Vertex2(headLineEndPointX, headLineEndPointY);
        GL.End();
    }

    private void InitializeOpengl(bool disableFrameBuffer)
    {
        _gfxContext.MakeCurrent();
        GL.MatrixMode(MatrixMode.Projection);
        GL.ClearDepth(GroundDepth);
        GL.StencilMask(255);
        GL.ClearStencil(GroundStencil);
        GL.DepthFunc(DepthFunction.Gequal);
        GL.AlphaFunc(AlphaFunction.Gequal, 0.9f);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Fastest);
        GL.Hint(HintTarget.TextureCompressionHint, HintMode.Fastest);
        GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Fastest);
        GL.Hint(HintTarget.LineSmoothHint, HintMode.Fastest);
        GL.Enable(EnableCap.PointSmooth);
        GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Replace);

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

    private void DrawDashLine(Vector v1, Vector v2, Color color, RenderingSettings settings)
    {
        DrawDashLine(v1.X, v1.Y, v2.X, v2.Y, color, settings);
    }

    public void DrawDashLine(double x1, double y1, double x2, double y2, Color color, RenderingSettings settings)
    {
        GL.Enable(EnableCap.LineStipple);
        GL.LineStipple((int)settings.LineWidth, LinePattern);
        DrawLine(x1, y1, x2, y2, color);
        GL.Disable(EnableCap.LineStipple);
        GL.LineWidth(settings.LineWidth);
    }

    public void SaveSnapShot(Level lev, string fileName, ZoomController zoomCtrl, SceneSettings sceneSettings, RenderingSettings settings)
    {
        using var bmp = GetSnapShot(lev, zoomCtrl, sceneSettings, settings);
        bmp.Save(fileName, ImageFormat.Png);
    }

    public void SaveSnapShotOfSelection(string fileName, ZoomController zoomCtrl)
    {
      using var bmp = GetSnapShotOfCurrent(zoomCtrl);
      bmp.Save(fileName, ImageFormat.Png);
    }

    public void SaveSnapShotForCustomShape(Level lev, string fileName, ZoomController zoomCtrl, SceneSettings sceneSettings, RenderingSettings settings)
    {
        using var snapShotBmp = GetSnapShotForCustomShape(lev, zoomCtrl, sceneSettings, settings);

        // Calculate the new dimensions while preserving the aspect ratio
        int width = snapShotBmp.Width;
        int height = snapShotBmp.Height;
        int maxWidth = 256;
        int maxHeight = 256;

        double aspectRatio = (double)width / height;
        int newWidth, newHeight;
        if (width > height)
        {
          newWidth = maxWidth;
          newHeight = (int)(maxWidth / aspectRatio);
        }
        else
        {
          newHeight = maxHeight;
          newWidth = (int)(maxHeight * aspectRatio);
        }

        // Resize the bitmap to the new dimensions
        using var resized = new Bitmap(newWidth, newHeight);
        using (var graphics = Graphics.FromImage(resized))
        {
          graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
          graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
          graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
          graphics.DrawImage(snapShotBmp, 0, 0, newWidth, newHeight);
        }
        resized.Save(fileName, ImageFormat.Png);
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