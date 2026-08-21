using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.LevelEditor;
using NetTopologySuite.Geometries;
using ShimSkiaSharp;
using Svg.Skia;
using ModelMatrix = ShimSkiaSharp.SKMatrix;
using ModelPaintStyle = ShimSkiaSharp.SKPaintStyle;
using ModelPicture = ShimSkiaSharp.SKPicture;
using SKMatrix = SkiaSharp.SKMatrix;
using SKPaint = SkiaSharp.SKPaint;
using SKPaintStyle = SkiaSharp.SKPaintStyle;
using SKPath = SkiaSharp.SKPath;
using SKPathFillType = SkiaSharp.SKPathFillType;
using SKPathVerb = SkiaSharp.SKPathVerb;
using SKPoint = SkiaSharp.SKPoint;

namespace Elmanager.SLE.Editor.Tools;

internal static class SvgImporter
{
    private const float SizeFactor = 0.1f;

    public static Level FromStream(Stream stream, bool compressed, SvgImportOptions options)
    {
        if (compressed)
        {
            using var gzip = new GZipStream(stream, CompressionMode.Decompress, true);
            return FromSvgStream(gzip, options);
        }

        return FromSvgStream(stream, options);
    }

    private static Level FromSvgStream(Stream stream, SvgImportOptions options)
    {
        using var svg = new SKSvg();
        svg.Load(stream);

        if (svg.Model is not { } model)
        {
            throw new ImportException("The SVG file does not contain importable geometry.");
        }

        if (svg.HasAnimations)
        {
            throw new ImportException("Animated SVGs are not supported.");
        }

        using var combinedPath = new SKPath
        {
            FillType = options.FillRule == FillRule.EvenOdd
                ? SKPathFillType.EvenOdd
                : SKPathFillType.Winding
        };
        var matrix = ModelMatrix.Identity;
        var matrixStack = new Stack<ModelMatrix>();
        AppendPicture(model, combinedPath, svg.SkiaModel, options, ref matrix, matrixStack);

        if (combinedPath.IsEmpty)
        {
            throw new ImportException("The SVG file does not contain importable vector paths.");
        }

        combinedPath.Transform(SKMatrix.CreateScale(SizeFactor, SizeFactor));
        var (polygons, _) = TextTool.BuildPolygons(
            combinedPath,
            new Vector(),
            options.Smoothness,
            options.UseOutlinedGeometry);

        try
        {
            TextTool.FinalizePolygons(polygons);
        }
        catch (TopologyException)
        {
        }
        catch (ArgumentException)
        {
        }

        var level = new Level();
        level.Polygons.AddRange(polygons);
        return level;
    }

    private static void AppendPaintedPath(
        SKPath sourcePath,
        ShimSkiaSharp.SKPaint sourcePaint,
        SKPath combinedPath,
        SkiaModel skiaModel,
        SvgImportOptions options,
        ModelMatrix matrix)
    {
        var nativeMatrix = skiaModel.ToSKMatrix(matrix);
        if (sourcePaint.Style is ModelPaintStyle.Fill or ModelPaintStyle.StrokeAndFill)
        {
            using var fill = new SKPath(sourcePath)
            {
                FillType = options.FillRule == FillRule.EvenOdd
                    ? SKPathFillType.EvenOdd
                    : SKPathFillType.Winding
            };
            fill.Transform(nativeMatrix);
            combinedPath.AddPath(fill);
        }

        if (sourcePaint.Style is not (ModelPaintStyle.Stroke or ModelPaintStyle.StrokeAndFill))
        {
            return;
        }

        using var strokePaint = skiaModel.ToSKPaint(sourcePaint);
        if (strokePaint is null)
        {
            return;
        }

        strokePaint.Style = SKPaintStyle.Stroke;

        foreach (var contour in SplitContours(sourcePath))
        {
            using (contour.Path)
            {
                if (contour.Closed && options.NeverWidenClosedPaths)
                {
                    contour.Path.Transform(nativeMatrix);
                    combinedPath.AddPath(contour.Path);
                    continue;
                }

                using var widened = new SKPath();
                if (!strokePaint.GetFillPath(contour.Path, widened))
                {
                    widened.AddPath(contour.Path);
                }

                widened.Transform(nativeMatrix);
                combinedPath.AddPath(widened);
            }
        }
    }

    private static List<PathContour> SplitContours(SKPath path)
    {
        var contours = new List<PathContour>();
        using var iterator = path.CreateRawIterator();
        var points = new SKPoint[4];
        SKPath? current = null;

        for (var verb = iterator.Next(points); verb != SKPathVerb.Done; verb = iterator.Next(points))
        {
            switch (verb)
            {
                case SKPathVerb.Move:
                    AddContour(contours, ref current, false);
                    current = new SKPath { FillType = path.FillType };
                    current.MoveTo(points[0]);
                    break;
                case SKPathVerb.Line:
                    (current ??= new SKPath { FillType = path.FillType }).LineTo(points[1]);
                    break;
                case SKPathVerb.Quad:
                    (current ??= new SKPath { FillType = path.FillType }).QuadTo(points[1], points[2]);
                    break;
                case SKPathVerb.Conic:
                    (current ??= new SKPath { FillType = path.FillType }).ConicTo(
                        points[1],
                        points[2],
                        iterator.ConicWeight());
                    break;
                case SKPathVerb.Cubic:
                    (current ??= new SKPath { FillType = path.FillType }).CubicTo(
                        points[1],
                        points[2],
                        points[3]);
                    break;
                case SKPathVerb.Close:
                    current?.Close();
                    AddContour(contours, ref current, true);
                    break;
            }
        }

        AddContour(contours, ref current, false);
        return contours;
    }

    private static void AddContour(List<PathContour> contours, ref SKPath? path, bool closed)
    {
        if (path is { IsEmpty: false })
        {
            contours.Add(new PathContour(path, closed));
        }
        else
        {
            path?.Dispose();
        }

        path = null;
    }

    private sealed record PathContour(SKPath Path, bool Closed);

#pragma warning disable CS0618
    private static void AppendPicture(
        ModelPicture picture,
        SKPath combinedPath,
        SkiaModel skiaModel,
        SvgImportOptions options,
        ref ModelMatrix matrix,
        Stack<ModelMatrix> matrixStack)
    {
        if (picture.Commands is not { } commands)
        {
            return;
        }

        foreach (var command in commands)
        {
            switch (command)
            {
                case SaveCanvasCommand:
                case SaveLayerCanvasCommand:
                    matrixStack.Push(matrix);
                    break;
                case RestoreCanvasCommand:
                    if (matrixStack.Count > 0)
                    {
                        matrix = matrixStack.Pop();
                    }

                    break;
                case SetMatrixCanvasCommand setMatrix:
                    matrix = matrix.PreConcat(setMatrix.DeltaMatrix);
                    break;
                case DrawPictureCanvasCommand { Picture: { } nestedPicture }:
                    AppendPicture(nestedPicture, combinedPath, skiaModel, options, ref matrix, matrixStack);
                    break;
                case DrawPathCanvasCommand { Path: { } modelPath, Paint: { } modelPaint }:
                    using (var path = skiaModel.ToSKPath(modelPath))
                    {
                        AppendPaintedPath(path, modelPaint, combinedPath, skiaModel, options, matrix);
                    }

                    break;
                case DrawTextCanvasCommand { Paint: { } textPaint } text:
                    using (var paint = CreateTextPaint(skiaModel, textPaint))
                    using (var path = paint.GetTextPath(text.Text, text.X, text.Y))
                    {
                        AppendPaintedPath(path, textPaint, combinedPath, skiaModel, options, matrix);
                    }

                    break;
                case DrawPositionedTextRunCanvasCommand { Paint: not null, Fragments: { } fragments } positioned:
                    using (var paint = CreateTextPaint(skiaModel, positioned.Paint))
                    {
                        foreach (var fragment in fragments)
                        {
                            using var path = paint.GetTextPath(fragment.Text, fragment.Point.X, fragment.Point.Y);
                            var fragmentMatrix = matrix;
                            if (fragment.RotationDegrees != 0)
                            {
                                fragmentMatrix = fragmentMatrix.PreConcat(ModelMatrix.CreateRotationDegrees(
                                    fragment.RotationDegrees,
                                    fragment.Point.X,
                                    fragment.Point.Y));
                            }

                            if (fragment.ScaleX != 1)
                            {
                                fragmentMatrix = fragmentMatrix.PreConcat(ModelMatrix.CreateScale(
                                    fragment.ScaleX,
                                    1,
                                    fragment.ScaleOriginX,
                                    fragment.Point.Y));
                            }

                            AppendPaintedPath(
                                path,
                                positioned.Paint,
                                combinedPath,
                                skiaModel,
                                options,
                                fragmentMatrix);
                        }
                    }

                    break;
                case DrawTextOnPathCanvasCommand:
                    throw new ImportException("SVG text on a path is not supported.");
            }
        }
    }

    private static SKPaint CreateTextPaint(SkiaModel skiaModel, ShimSkiaSharp.SKPaint source)
    {
        var paint = skiaModel.ToSKPaint(source) ?? new SKPaint();
        paint.Typeface = skiaModel.ToSKTypeface(source.Typeface);
        paint.TextSize = source.TextSize;
        return paint;
    }
#pragma warning restore CS0618
}
