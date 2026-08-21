using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.LevelEditor;
using Elmanager.LevelEditor.Input;
using Elmanager.LevelEditor.Tools;
using Elmanager.Rendering;
using Elmanager.SLE.Dialogs;
using NetTopologySuite.Geometries;
using SkiaSharp;
using Polygon = Elmanager.Lev.Polygon;
using Vector = Elmanager.Geometry.Vector;

namespace Elmanager.SLE.Editor.Tools;

internal class TextTool : ToolBase, IEditorTool
{
    private const double SizeFactor = 0.1;

    private TextToolOptions _currentOptions = TextToolOptions.Default;
    private List<Polygon>? _currentTextPolygons;

    internal TextTool(ILevelEditor editor) : base(editor)
    {
    }

    public override bool Busy => false; // dialog is modal

    public void Activate()
    {
    }

    public LevVisualChange InActivate() => LevVisualChange.Nothing;

    public LevVisualChange MouseDown(EditorMouseEventArgs mouseData)
    {
        if (mouseData.Button == EditorMouseButton.Left)
        {
            _ = OpenDialogAndApply(CurrentPos);
        }

        return LevVisualChange.Nothing;
    }

    public void MouseUp()
    {
    }

    public LevVisualChange KeyDown(EditorKeyEventArgs key) => LevVisualChange.Nothing;

    public LevVisualChange MouseMove(Vector p)
    {
        CurrentPos = p;
        return LevVisualChange.Nothing;
    }

    public LevVisualChange MouseOutOfEditor() => LevVisualChange.Nothing;

    public void ExtraRendering()
    {
    }

    public TransientElements GetTransientElements(bool hasFocus) =>
        _currentTextPolygons is { } polys
            ? TransientElements.FromPolygons(polys)
            : TransientElements.Empty;

    public string GetHelp() => "LMouse: open text input dialog.";

    private async Task OpenDialogAndApply(Vector offset)
    {
        _currentTextPolygons = new List<Polygon>();
        LevEditor.RedrawScene();

        var dialog = new TextToolDialog(_currentOptions);
        dialog.OptionsChanged += options => HandleChange(options, offset);
        HandleChange(_currentOptions, offset);
        var result = await dialog.ShowAsync();

        if (result.HasValue)
        {
            _currentOptions = result.Value;
            MarkAllAs(VectorMark.None);
            var rendered = RenderString(_currentOptions, offset);
            Lev.Polygons.AddRange(rendered);
            if (rendered.Count > 0)
            {
                LevEditor.SetModified(LevModification.Ground);
            }

            LevEditor.UpdateSelectionInfo();
        }
        else
        {
            LevEditor.SignalVisualChange(LevVisualChange.Ground);
        }

        _currentTextPolygons = null;
        LevEditor.RedrawScene();
    }

    private void HandleChange(TextToolOptions options, Vector offset)
    {
        _currentTextPolygons = RenderString(options, offset);
        LevEditor.SignalVisualChange(LevVisualChange.Ground);
        LevEditor.RedrawScene();
    }

    private static List<Polygon> RenderString(TextToolOptions options, Vector offset)
    {
        var smoothness = options.Smoothness;
        using var glyphPath = BuildTextPath(options);
        List<Polygon> polys;
        try
        {
            (polys, _) = BuildPolygons(glyphPath, offset, smoothness, true);
        }
        catch (PolygonException)
        {
            var opt = TextToolOptions.Default;
            opt.Text = "Unable to render\nthis font without\ntopology errors.";
            return RenderString(opt, offset);
        }

        try
        {
            FinalizePolygons(polys);
            return polys;
        }
        catch (TopologyException)
        {
            return polys;
        }
    }

    internal static (List<Polygon> polys, double smoothness) BuildPolygons(SKPath path, Vector offset,
        double smoothness, bool useOutlinedGeometry)
    {
        using var simplified = useOutlinedGeometry ? path.Simplify() : null;
        var outline = simplified ?? path;
        List<Polygon> polys;
        do
        {
            if (smoothness < 0.0001)
            {
                throw new PolygonException("Smoothness limit reached.");
            }

            polys = Flatten(outline, smoothness, offset);
            smoothness *= 0.5;
        } while (polys.Any(p => p.Vertices.Count < 3));

        return (polys, smoothness);
    }

    private static List<Polygon> Flatten(SKPath path, double tolerance, Vector offset)
    {
        var result = new List<Polygon>();
        using var iterator = path.CreateRawIterator();
        var points = new SKPoint[4];
        var current = new List<Vector>();

        for (var verb = iterator.Next(points); verb != SKPathVerb.Done; verb = iterator.Next(points))
        {
            switch (verb)
            {
                case SKPathVerb.Move:
                    AddCurrentPolygon(result, current);
                    current.Clear();
                    current.Add(ToVector(points[0], offset));
                    break;
                case SKPathVerb.Line:
                    current.Add(ToVector(points[1], offset));
                    break;
                case SKPathVerb.Quad:
                    FlattenQuadratic(current, points[0], points[1], points[2], tolerance, offset);
                    break;
                case SKPathVerb.Conic:
                    FlattenConic(current, points[0], points[1], points[2], iterator.ConicWeight(), tolerance, offset);
                    break;
                case SKPathVerb.Cubic:
                    FlattenCubic(current, points[0], points[1], points[2], points[3], tolerance, offset);
                    break;
                case SKPathVerb.Close:
                    AddCurrentPolygon(result, current);
                    current.Clear();
                    break;
                default:
                    throw new TopologyException("Unexpected path verb.");
            }
        }

        AddCurrentPolygon(result, current);
        return result;
    }

    private static void AddCurrentPolygon(List<Polygon> result, List<Vector> current)
    {
        if (current.Count > 0)
        {
            result.Add(new Polygon(current));
        }
    }

    private static void FlattenQuadratic(List<Vector> result, SKPoint p0, SKPoint p1, SKPoint p2,
        double tolerance, Vector offset)
    {
        if (DistanceFromLine(p1, p0, p2) <= tolerance)
        {
            result.Add(ToVector(p2, offset));
            return;
        }

        var p01 = Midpoint(p0, p1);
        var p12 = Midpoint(p1, p2);
        var p012 = Midpoint(p01, p12);
        FlattenQuadratic(result, p0, p01, p012, tolerance, offset);
        FlattenQuadratic(result, p012, p12, p2, tolerance, offset);
    }

    private static void FlattenConic(List<Vector> result, SKPoint p0, SKPoint p1, SKPoint p2, float weight,
        double tolerance, Vector offset)
    {
        var segments = Math.Max(1, (int)Math.Ceiling(DistanceFromLine(p1, p0, p2) / tolerance));
        for (var i = 1; i <= segments; i++)
        {
            var t = (float)i / segments;
            result.Add(ToVector(EvaluateConic(p0, p1, p2, weight, t), offset));
        }
    }

    private static void FlattenCubic(List<Vector> result, SKPoint p0, SKPoint p1, SKPoint p2, SKPoint p3,
        double tolerance, Vector offset)
    {
        if (Math.Max(DistanceFromLine(p1, p0, p3), DistanceFromLine(p2, p0, p3)) <= tolerance)
        {
            result.Add(ToVector(p3, offset));
            return;
        }

        var p01 = Midpoint(p0, p1);
        var p12 = Midpoint(p1, p2);
        var p23 = Midpoint(p2, p3);
        var p012 = Midpoint(p01, p12);
        var p123 = Midpoint(p12, p23);
        var p0123 = Midpoint(p012, p123);
        FlattenCubic(result, p0, p01, p012, p0123, tolerance, offset);
        FlattenCubic(result, p0123, p123, p23, p3, tolerance, offset);
    }

    private static SKPoint EvaluateConic(SKPoint p0, SKPoint p1, SKPoint p2, float weight, float t)
    {
        var u = 1 - t;
        var a = u * u;
        var b = 2 * weight * u * t;
        var c = t * t;
        var denominator = a + b + c;
        return new SKPoint(((a * p0.X) + (b * p1.X) + (c * p2.X)) / denominator,
            ((a * p0.Y) + (b * p1.Y) + (c * p2.Y)) / denominator);
    }

    private static double DistanceFromLine(SKPoint point, SKPoint lineStart, SKPoint lineEnd)
    {
        var dx = lineEnd.X - lineStart.X;
        var dy = lineEnd.Y - lineStart.Y;
        var length = Math.Sqrt((dx * dx) + (dy * dy));
        if (length == 0)
        {
            dx = point.X - lineStart.X;
            dy = point.Y - lineStart.Y;
            return Math.Sqrt((dx * dx) + (dy * dy));
        }

        return Math.Abs((dy * point.X) - (dx * point.Y) + (lineEnd.X * lineStart.Y) - (lineEnd.Y * lineStart.X)) /
               length;
    }

    private static SKPoint Midpoint(SKPoint a, SKPoint b) => new((a.X + b.X) / 2, (a.Y + b.Y) / 2);

    private static Vector ToVector(SKPoint point, Vector offset) =>
        new(point.X + offset.X, -point.Y + offset.Y, VectorMark.Selected);

    /// <summary>
    ///     Cleans up the rendered polygons by removing duplicate vertices and resolving
    ///     self-intersections via a NetTopologySuite union.
    /// </summary>
    internal static void FinalizePolygons(List<Polygon> polys)
    {
        polys.ForEach(p => p.RemoveDuplicateVertices());
        polys.RemoveAll(p => p.Vertices.Count < 3);
        var intersections = GeometryUtils.GetIntersectionPoints(polys);
        if (intersections.Count > 0)
        {
            var f = GeometryFactory.Floating;
            var iarray = polys.Select(p => p.ToIPolygon()).ToArray();

            NetTopologySuite.Geometries.Geometry union = f.CreateMultiPolygon(iarray);
            union = intersections.Aggregate(union,
                (current, vector) =>
                    current.Union(f.CreatePoint(new Coordinate(vector.X, vector.Y)).Buffer(0.0001, 1)));
            polys.Clear();
            switch (union)
            {
                case NetTopologySuite.Geometries.Polygon polygon:
                    polys.AddRange(polygon.ToElmaPolygons());
                    break;
                case MultiPolygon multiPolygon:
                    polys.AddRange(multiPolygon.Geometries
                        .Select(geometry => geometry as NetTopologySuite.Geometries.Polygon)
                        .SelectMany(poly => poly!.ToElmaPolygons()));
                    break;
            }

            polys.ForEach(p => p.MarkVectorsAs(VectorMark.Selected));
        }
    }

    // The SkiaSharp docs recommend using typeface stuff from SKFont instead of SKPaint, but it causes buggy results
    // (wrong character spacing and random overlapping characters).
    // So disable "Type or member is obsolete" warning.
#pragma warning disable CS0618
    private static SKPath BuildTextPath(TextToolOptions options)
    {
        var typeface = SleTypefaceProvider.GetCached(options.FontFamily, options.Bold, options.Italic)
                       ?? SKTypeface.Default;

        var size = (float)(options.FontSize * SizeFactor);
        using var paint = new SKPaint();
        paint.Typeface = typeface;
        paint.TextSize = size;
        paint.IsAntialias = true;
        paint.FakeBoldText = options.Bold && !IsBold(typeface);
        paint.TextSkewX = options.Italic && !IsItalic(typeface) ? -0.25f : 0;

        var metrics = paint.FontMetrics;
        var lineSpacing = (float)(options.LineHeight * options.FontSize * SizeFactor);

        var fullPath = new SKPath { FillType = SKPathFillType.Winding };
        var lines = options.Text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            var baseline = -metrics.Ascent + (lineIndex * lineSpacing);
            AppendLine(fullPath, paint, metrics, lines[lineIndex], baseline, options, size);
        }

        return fullPath;
    }

    private static bool IsBold(SKTypeface typeface) =>
        typeface.FontStyle.Weight >= (int)SKFontStyleWeight.SemiBold;

    private static bool IsItalic(SKTypeface typeface) =>
        typeface.FontStyle.Slant == SKFontStyleSlant.Italic;

    private static void AppendLine(SKPath fullPath, SKPaint paint, SKFontMetrics metrics, string line,
        float baseline, TextToolOptions options, float size)
    {
        if (line.Length > 0)
        {
            using var linePath = paint.GetTextPath(line, 0, baseline);
            if (linePath is { IsEmpty: false })
            {
                fullPath.AddPath(linePath);
            }
        }

        var width = paint.MeasureText(line);
        if (width <= 0)
        {
            return;
        }

        if (options.Underline)
        {
            var thickness = metrics.UnderlineThickness ?? size * 0.05f;
            var pos = baseline + (metrics.UnderlinePosition ?? size * 0.1f);
            fullPath.AddRect(SKRect.Create(0, pos, width, thickness));
        }

        if (options.Strikeout)
        {
            var thickness = metrics.StrikeoutThickness ?? size * 0.05f;
            var pos = baseline + (metrics.StrikeoutPosition ?? -size * 0.25f);
            fullPath.AddRect(SKRect.Create(0, pos, width, thickness));
        }
    }
#pragma warning restore CS0618
}
