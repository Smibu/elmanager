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
using OpenTK.WinForms;
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

    internal Level Lev = new();

    private bool _disposed;

    private readonly IGraphicsContext _gfxContext;
    private RenderingSettings _settings = null!;
    private int _frameBuffer;
    private int _colorRenderBuffer;
    private int _depthStencilRenderBuffer;
    private int _maxRenderbufferSize;

    internal ElmaRenderer(GLControl renderingTarget, RenderingSettings settings)
    {
        _gfxContext = renderingTarget.Context;
        InitializeOpengl(disableFrameBuffer: settings.DisableFrameBuffer);
        UpdateSettings(settings);
    }

    public OpenGlLgr? OpenGlLgr { get; private set; }

    public void Dispose()
    {
        Dispose(true);
    }

    private Bitmap GetSnapShot(ZoomController zoomCtrl, SceneSettings sceneSettings)
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
            zoomCtrl.ZoomFill();
            DrawScene(zoomCtrl.Cam, sceneSettings);
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

    private void DrawCircle(double x, double y, double radius, Color circleColor)
    {
        var accuracy = _settings.CircleDrawingAccuracy;
        GL.Color4(circleColor);
        GL.Begin(PrimitiveType.LineLoop);
        for (var i = 0; i <= accuracy; i++)
            GL.Vertex2(x + Math.Cos(i * 360 / (double)accuracy * Math.PI / 180) * radius,
                y + Math.Sin(i * 360 / (double)accuracy * Math.PI / 180) * radius);
        GL.End();
    }

    internal void DrawCircle(Vector v, double radius, Color circleColor)
    {
        DrawCircle(v.X, v.Y, radius, circleColor);
    }

    internal void DrawDummyPlayer(double leftWheelx, double leftWheely, SceneSettings sceneSettings, PlayerRenderOpts opts)
    {
        DrawPlayer(new PlayerState(leftWheelx + Level.GlobalBodyDifferenceFromLeftWheelX,
            leftWheely + Level.GlobalBodyDifferenceFromLeftWheelY, leftWheelx, leftWheely,
            leftWheelx + Level.RightWheelDifferenceFromLeftWheelX, leftWheely, 0, 0,
            leftWheelx + Level.HeadDifferenceFromLeftWheelX, leftWheely + Level.HeadDifferenceFromLeftWheelY,
            0, Direction.Left, 0), opts, sceneSettings);
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

    internal void DrawScene(ElmaCamera cam, SceneSettings sceneSettings)
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
        GL.StencilOp(StencilOp.Incr, StencilOp.Keep, StencilOp.Decr);
        GL.StencilFunc(StencilFunction.Equal, GroundStencil, StencilMask);
        GL.ColorMask(false, false, false, false);
        GL.Begin(PrimitiveType.Triangles);
        foreach (var k in Lev.Polygons.Concat(sceneSettings.AdditionalPolys))
            if (!k.IsGrass)
                DrawFilledTrianglesFast(k.Decomposition, ZFar - (ZFar - ZNear) * SkyDepth);
        GL.End();
        GL.ColorMask(true, true, true, true);
        GL.Enable(EnableCap.Texture2D);
        GL.Enable(EnableCap.DepthTest);
        GL.StencilFunc(StencilFunction.Equal, GroundStencil, StencilMask);
        GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
        var midX = (Lev.Bounds.XMin + Lev.Bounds.XMax) / 2;
        var midY = (Lev.Bounds.YMin + Lev.Bounds.YMax) / 2;
        if (_settings.ShowGround)
        {
            const double depth = ZFar - (ZFar - ZNear) * GroundDepth;
            if (_settings.GroundTextureEnabled && OpenGlLgr != null)
            {
                OpenGlLgr.DrawFullScreenTexture(cam, OpenGlLgr.GroundTexture, midX, midY, depth, _settings);
            }
            else
            {
                GL.Disable(EnableCap.Texture2D);
                GL.Color4(_settings.GroundFillColor);
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
        if (_settings.SkyTextureEnabled && OpenGlLgr != null)
        {
            const double depth = ZFar - (ZFar - ZNear) * SkyDepth;
            GL.BindTexture(TextureTarget.Texture2D, OpenGlLgr.SkyTexture.TextureId);
            if (_settings.ZoomTextures)
            {
                GL.Begin(PrimitiveType.Quads);
                GL.TexCoord2(0, TextureCoordConst / OpenGlLgr.SkyTexture.Width * OpenGlLgr.SkyTexture.Width / OpenGlLgr.SkyTexture.Height);
                GL.Vertex3(centerX / 2 - TextureVertexConst, centerY - TextureVertexConst, depth);
                GL.TexCoord2(TextureCoordConst / OpenGlLgr.SkyTexture.Width,
                    TextureCoordConst / OpenGlLgr.SkyTexture.Width * OpenGlLgr.SkyTexture.Width / OpenGlLgr.SkyTexture.Height);
                GL.Vertex3(centerX / 2 + TextureVertexConst, centerY - TextureVertexConst, depth);
                GL.TexCoord2(TextureCoordConst / OpenGlLgr.SkyTexture.Width, 0);
                GL.Vertex3(centerX / 2 + TextureVertexConst, centerY + TextureVertexConst, depth);
                GL.TexCoord2(0, 0);
                GL.Vertex3(centerX / 2 - TextureVertexConst, centerY + TextureVertexConst, depth);
                GL.End();
            }
            else
            {
                var xdelta = centerX / OpenGlLgr.SkyTexture.Width;
                var skyAspectRatio = OpenGlLgr.SkyTexture.AspectRatio;
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
            for (var i = Lev.GraphicElements.Count - 1; i >= 0; --i)
            {
                var picture = Lev.GraphicElements[i];
                DoPictureScissor(picture, cam);
                if (picture is GraphicElement.Picture p && _settings.ShowPictures)
                {
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

                    OpenGlLgr.DrawPicture(p.PictureInfo.TextureId, picture.Position.X, picture.Position.Y, picture.Width, -picture.Height,
                        (picture.Distance / 1000.0 * (ZFar - ZNear)) + ZNear);
                }
            }

            foreach (var picture in Lev.GraphicElements)
            {
                DoPictureScissor(picture, cam);
                if (picture is GraphicElement.Texture t && _settings.ShowTextures)
                {
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
                        depth + 0.001);

                    var info = t.TextureInfo;
                    GL.StencilFunc(StencilFunction.Lequal, 5, StencilMask);
                    OpenGlLgr.DrawFullScreenTexture(cam, info, midX, midY, depth, _settings);
                }
            }

            GL.Disable(EnableCap.ScissorTest);
            GL.DepthFunc(DepthFunction.Gequal);
            if (_settings.ShowGrass)
            {
                OpenGlLgr.DrawGrass(Lev, cam, midX, midY, _settings, sceneSettings);
            }
        }

        GL.Disable(EnableCap.StencilTest);
        if (_settings.ShowObjects && OpenGlLgr != null)
        {
            var depth = sceneSettings.PicturesInBackground ? 0 : 0.5 * (ZFar - ZNear) + ZNear;
            foreach (var (x, i) in GetVisibleObjects(sceneSettings))
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
                        DrawDummyPlayer(x.Position.X, x.Position.Y, sceneSettings, new PlayerRenderOpts { IsActive = true, UseTransparency = false, UseGraphics = true });
                        break;
                }
            }
        }

        GL.Disable(EnableCap.Texture2D);
        GL.Disable(EnableCap.StencilTest);
        GL.Disable(EnableCap.DepthTest);
        GL.Disable(EnableCap.AlphaTest);
        GL.Disable(EnableCap.Blend);
        if (_settings.ShowGrid)
            DrawGrid(cam, sceneSettings.GridOffset, _settings.GridSize);
        if (_settings.ShowMaxDimensions)
        {
            GL.Enable(EnableCap.LineStipple);
            GL.LineStipple((int)_settings.LineWidth * 2, LinePattern);
            var levCenterX = (Lev.Bounds.XMin + Lev.Bounds.XMax) / 2;
            var levCenterY = (Lev.Bounds.YMin + Lev.Bounds.YMax) / 2;
            DrawRectangle(levCenterX - Level.MaximumSize / 2,
                levCenterY - Level.MaximumSize / 2,
                levCenterX + Level.MaximumSize / 2,
                levCenterY + Level.MaximumSize / 2,
                _settings.MaxDimensionColor);
            GL.Disable(EnableCap.LineStipple);
            GL.LineWidth(_settings.LineWidth);
        }

        if (_settings.ShowObjectFrames)
            DrawObjectFrames(sceneSettings);
        if (_settings.ShowObjectCenters)
            DrawObjectCenters(sceneSettings);
        if (_settings.ShowGravityAppleArrows &&
            (_settings.ShowObjectFrames || (OpenGlLgr != null && _settings.ShowObjects)))
        {
            foreach (var (o, _) in GetVisibleObjects(sceneSettings))
            {
                DrawGravityArrowMaybe(o);
            }
        }

        foreach (var x in Lev.Polygons)
        {
            if (x.IsGrass)
            {
                if (_settings.ShowGrassEdges)
                {
                    var color = x.SlopeInfo?.HasError ?? false ? Color.Red : _settings.GrassEdgeColor;
                    DrawGrassPolygon(x, color, _settings.ShowInactiveGrassEdges);
                }
            }
            else if (_settings.ShowGroundEdges)
                DrawPolygon(x, _settings.GroundEdgeColor);
        }

        GL.Color4(_settings.TextureFrameColor);
        foreach (var z in Lev.GraphicElements)
        {
            if (z is GraphicElement.Picture)
            {
                if (!_settings.ShowPictureFrames) continue;
                GL.Color4(_settings.PictureFrameColor);
            }
            else
            {
                if (!_settings.ShowTextureFrames)
                    continue;
                GL.Color4(_settings.TextureFrameColor);
            }

            DrawRectangle(z.Position.X, z.Position.Y, z.Position.X + z.Width, z.Position.Y - z.Height);
        }

        if (_settings.ShowVertices)
        {
            var showGrassVertices = _settings.ShowGrassEdges || _settings.ShowGrass;
            var showGroundVertices = _settings.ShowGroundEdges || (_settings.ShowGround && OpenGlLgr != null);
            GL.Color3(_settings.VertexColor);
            if (_settings.UseCirclesForVertices)
            {
                GL.Begin(PrimitiveType.Points);
                foreach (var x in Lev.Polygons)
                    if ((showGrassVertices && x.IsGrass) || (showGroundVertices && !x.IsGrass))
                        foreach (var z in x.Vertices)
                            GL.Vertex3(z.X, z.Y, 0);
            }
            else
            {
                GL.Begin(PrimitiveType.Triangles);
                foreach (var x in Lev.Polygons)
                    if ((showGrassVertices && x.IsGrass) || (showGroundVertices && !x.IsGrass))
                        foreach (var z in x.Vertices)
                            DrawEquilateralTriangleFast(z, cam.ZoomLevel * _settings.VertexSize);
            }

            GL.End();
        }
    }

    public void DrawGrassPolygon(Polygon polygon, Color color, bool drawInactiveGrassEdge)
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
                color);
        }
    }

    internal void Swap()
    {
        _gfxContext.SwapBuffers();
    }

    private IEnumerable<(LevObject, int)> GetVisibleObjects(SceneSettings sceneSettings)
    {
        return Lev.Objects.Select((t, i) => (t, i)).Where(t => !sceneSettings.HiddenObjectIndices.Contains(t.i));
    }

    private void DoPictureScissor(GraphicElement graphicElement, ElmaCamera cam)
    {
        var x = (int)((graphicElement.Position.X - cam.XMin) / (cam.XMax - cam.XMin) * cam.ViewPortWidth);
        var y = (int)(((graphicElement.Position.Y - graphicElement.Height) - cam.YMin) / (cam.YMax - cam.YMin) * cam.ViewPortHeight);
        var w = (int)((graphicElement.Position.X + graphicElement.Width - cam.XMin) / (cam.XMax - cam.XMin) * cam.ViewPortWidth) - x;
        var h = (int)((graphicElement.Position.Y - cam.YMin) / (cam.YMax - cam.YMin) * cam.ViewPortHeight) - y;
        GL.Scissor(x, y, w, h);
    }

    private void DrawGravityArrowMaybe(LevObject o)
    {
        if (o.Type == ObjectType.Apple && o.AppleType != AppleType.Normal)
        {
            GL.Color3(_settings.AppleGravityArrowColor);
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
        GL.Color3(color);
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

    internal void InitializeLevel(Level level)
    {
        Lev = level;
        Lev.Objects = Lev.Objects.OrderBy(o => o.Type switch
        {
            ObjectType.Flower => 3,
            ObjectType.Apple => 2,
            ObjectType.Killer => 1,
            ObjectType.Start => 4,
            _ => throw new ArgumentOutOfRangeException()
        }).ToList();
        Lev.UpdateAllPolygons(_settings.GrassZoom);
        if (OpenGlLgr != null)
        {
            Lev.UpdateImages(OpenGlLgr.DrawableImages);
            OpenGlLgr.UpdateGroundAndSky(Lev, _settings.DefaultGroundAndSky);
        }
    }

    internal void ResetViewport(int width, int height)
    {
        GL.Viewport(0, 0, width, height);
    }

    internal void UpdateSettings(RenderingSettings newSettings)
    {
        var currentLgr = OpenGlLgr?.CurrentLgr.Path;
        var newLgr = newSettings.ResolveLgr(Lev);
        if (!currentLgr.EqualsIgnoreCase(newLgr))
        {
            if (File.Exists(newLgr))
            {
                var old = OpenGlLgr;
                try
                {
                    OpenGlLgr = new OpenGlLgr(Lev, new Lgr.Lgr(newLgr), newSettings);
                    Lev.UpdateImages(OpenGlLgr.DrawableImages);
                    old?.Dispose();
                }
                catch (Exception e)
                {
                    UiUtils.ShowError("Error occurred when loading LGR file! Reason:\r\n\r\n" + e.Message);
                }
            }
        }
        else if (OpenGlLgr != null)
        {
            var grassZoomChanged = Math.Abs(_settings.GrassZoom - newSettings.GrassZoom) > double.Epsilon;
            if (grassZoomChanged)
            {
                OpenGlLgr.RefreshGrassPics(Lev, newSettings);
                foreach (var polygon in Lev.Polygons.Where(p => p.IsGrass))
                {
                    polygon.SlopeInfo = new GrassSlopeInfo(polygon, Lev.GroundBounds, newSettings.GrassZoom);
                }
            }
            if (_settings.DefaultGroundAndSky != newSettings.DefaultGroundAndSky)
            {
                OpenGlLgr.UpdateGroundAndSky(Lev, newSettings.DefaultGroundAndSky);
            }
        }

        GL.ClearColor(newSettings.SkyFillColor);
        GL.LineWidth(newSettings.LineWidth);
        GL.PointSize((float)(newSettings.VertexSize * 300));
        _settings = newSettings.Clone();
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

    private void DrawGrid(ElmaCamera cam, Vector gridOffset, double gridSize)
    {
        var current = GetFirstGridLine(gridSize, gridOffset.X, cam.XMin);
        EnableDashLines(LinePattern);
        GL.Color3(_settings.GridColor);
        GL.Begin(PrimitiveType.Lines);
        while (!(current > cam.XMax))
        {
            DrawLineFast(current, cam.YMin, current, cam.YMax);
            current += gridSize;
        }

        current = GetFirstGridLine(gridSize, gridOffset.Y, cam.YMin);
        while (!(current > cam.YMax))
        {
            DrawLineFast(cam.XMin, current, cam.XMax, current);
            current += gridSize;
        }

        GL.End();
        GL.Disable(EnableCap.LineStipple);
        GL.LineWidth(_settings.LineWidth);
    }

    private void EnableDashLines(short pattern)
    {
        GL.Enable(EnableCap.LineStipple);
        GL.LineStipple((int)_settings.LineWidth, pattern);
    }

    private void DrawObjectCenters(SceneSettings sceneSettings)
    {
        foreach (var (x, _) in GetVisibleObjects(sceneSettings))
        {
            switch (x.Type)
            {
                case ObjectType.Flower:
                    DrawPoint(x.Position, _settings.FlowerColor);
                    break;
                case ObjectType.Killer:
                    DrawPoint(x.Position, _settings.KillerColor);
                    break;
                case ObjectType.Apple:
                    DrawPoint(x.Position, _settings.AppleColor);
                    break;
                case ObjectType.Start:
                    DrawPoint(x.Position, _settings.StartColor);
                    DrawPoint(x.Position.X + Level.RightWheelDifferenceFromLeftWheelX, x.Position.Y,
                        Global.AppSettings.LevelEditor.RenderingSettings.StartColor);
                    DrawPoint(x.Position.X + Level.HeadDifferenceFromLeftWheelX,
                        x.Position.Y + Level.HeadDifferenceFromLeftWheelY,
                        Global.AppSettings.LevelEditor.RenderingSettings.StartColor);
                    break;
            }
        }
    }

    private void DrawObjectFrames(SceneSettings sceneSettings)
    {
        foreach (var (x, i) in GetVisibleObjects(sceneSettings))
        {
            if (sceneSettings.FadedObjectIndices.Contains(i))
            {
                EnableDashLines(0b0110011001100110);
            }
            else
            {
                GL.Disable(EnableCap.LineStipple);
            }
            switch (x.Type)
            {
                case ObjectType.Flower:
                    DrawCircle(x.Position, OpenGlLgr.ObjectRadius, _settings.FlowerColor);
                    break;
                case ObjectType.Killer:
                    DrawCircle(x.Position, OpenGlLgr.ObjectRadius, _settings.KillerColor);
                    break;
                case ObjectType.Apple:
                    DrawCircle(x.Position, OpenGlLgr.ObjectRadius, _settings.AppleColor);
                    break;
                case ObjectType.Start:
                    DrawDummyPlayer(x.Position.X, x.Position.Y, sceneSettings, new PlayerRenderOpts(_settings.StartColor, true, false, false));
                    break;
            }
        }
        GL.Disable(EnableCap.LineStipple);
    }

    internal void DrawPlayer(PlayerState player, PlayerRenderOpts opts, SceneSettings sceneSettings)
    {
        if (opts.UseGraphics && OpenGlLgr != null)
        {
            OpenGlLgr.DrawLgrPlayer(player, opts, sceneSettings);
        }
        else
        {
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.DepthTest);
            if (!opts.IsActive)
            {
                GL.Enable(EnableCap.LineStipple);
                GL.LineStipple((int)_settings.LineWidth, LinePattern);
            }

            DrawPlayerFrames(player, opts.Color);
            if (!opts.IsActive)
            {
                GL.Disable(EnableCap.LineStipple);
            }
        }
    }

    private void DrawPlayerFrames(PlayerState player, Color playerColor)
    {
        var headCos = Math.Cos(player.BikeRotation * MathUtils.DegToRad);
        var headSin = Math.Sin(player.BikeRotation * MathUtils.DegToRad);
        var f = player.IsRight ? 1 : -1;
        var headLineEndPointX = player.HeadX + f * headCos * ElmaConstants.HeadDiameter / 2;
        var headLineEndPointY = player.HeadY + f * headSin * ElmaConstants.HeadDiameter / 2;
        DrawCircle(player.LeftWheelX, player.LeftWheelY, OpenGlLgr.ObjectRadius, playerColor);
        DrawCircle(player.RightWheelX, player.RightWheelY, OpenGlLgr.ObjectRadius, playerColor);
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
        DrawCircle(player.HeadX, player.HeadY, ElmaConstants.HeadDiameter / 2, playerColor);
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
        GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusDstColor);
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

    public void DrawDashLine(Vector v1, Vector v2, Color color)
    {
        DrawDashLine(v1.X, v1.Y, v2.X, v2.Y, color);
    }

    public void DrawDashLine(double x1, double y1, double x2, double y2, Color color)
    {
        GL.Enable(EnableCap.LineStipple);
        GL.LineStipple((int)_settings.LineWidth, LinePattern);
        DrawLine(x1, y1, x2, y2, color);
        GL.Disable(EnableCap.LineStipple);
        GL.LineWidth(_settings.LineWidth);
    }

    public void SaveSnapShot(string fileName, ZoomController zoomCtrl, SceneSettings sceneSettings)
    {
        using var bmp = GetSnapShot(zoomCtrl, sceneSettings);
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