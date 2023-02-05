using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Media;
using Elmanager.Geometry;
using Elmanager.Lev;
using NetTopologySuite.Geometries;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Drawing.Color;
using FlowDirection = System.Windows.FlowDirection;
using FontFamily = System.Windows.Media.FontFamily;
using LineSegment = System.Windows.Media.LineSegment;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;
using Polygon = Elmanager.Lev.Polygon;
using Vector = Elmanager.Geometry.Vector;

namespace Elmanager.LevelEditor.Tools;

internal class TextTool : ToolBase, IEditorTool
{
    private TextToolOptions _currentOptions = TextToolOptions.Default;

    private List<Polygon>? _currentTextPolygons;
    private static readonly Dictionary<string, string> EmptySynonymMap = new();

    private static readonly Dictionary<string, string> SynonymMap = new()
    {
        {"Italic", "Oblique"},
        {"Demibold", "Bold"}
    };

    internal TextTool(LevelEditorForm editor) : base(editor)
    {
    }

    public void MouseDown(MouseEventArgs mouseData)
    {
        switch (mouseData.Button)
        {
            case MouseButtons.Left:
                _currentTextPolygons = new List<Polygon>();
                LevEditor.RedrawScene();
                var result = TextToolForm.ShowDefault(_currentOptions, HandleChange);
                if (result is { })
                {
                    _currentOptions = result.Value;
                    MarkAllAs(VectorMark.None);
                    var rendered = RenderString(_currentOptions, CurrentPos);
                    Lev.Polygons.AddRange(rendered);
                    if (rendered.Count > 0)
                    {
                        LevEditor.SetModified(LevModification.Ground);
                    }

                    LevEditor.UpdateSelectionInfo();
                }

                _currentTextPolygons = null;

                LevEditor.RedrawScene();
                break;
            case MouseButtons.None:
                break;
            case MouseButtons.Right:
                break;
            case MouseButtons.Middle:
                break;
            case MouseButtons.XButton1:
                break;
            case MouseButtons.XButton2:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void HandleChange(TextToolOptions obj)
    {
        _currentTextPolygons = RenderString(obj, CurrentPos);
        LevEditor.RedrawScene();
    }

    public void MouseUp()
    {
    }

    public void KeyDown(KeyEventArgs key)
    {
    }

    public void MouseMove(Vector p)
    {
        CurrentPos = p;
    }

    public void MouseOutOfEditor()
    {
    }

    public void ExtraRendering()
    {
        if (_currentTextPolygons is { })
        {
            _currentTextPolygons.ForEach(p => Renderer.DrawPolygon(p, Color.Blue));
        }
    }

    public List<Polygon> GetExtraPolygons()
    {
        return new();
    }

    public void InActivate()
    {
    }

    public void Activate()
    {
        UpdateHelp();
    }

    private List<Polygon> RenderString(TextToolOptions options, Vector offset)
    {
        var fontfamily = new FontFamily(options.Font.FontFamily.Name);

        var typeface = FindTypeface(options, fontfamily, EmptySynonymMap) ??
                       FindTypeface(options, fontfamily, SynonymMap) ?? fontfamily.GetTypefaces().First();

        var decorations = new TextDecorationCollection();
        if (options.Font.Strikeout)
        {
            decorations.Add(TextDecorations.Strikethrough);
        }

        if (options.Font.Underline)
        {
            decorations.Add(TextDecorations.Underline);
        }

        const double sizeFactor = 0.1;
        var formattedText = new FormattedText(options.Text, CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight, typeface, options.Font.SizeInPoints * sizeFactor, Brushes.Black, 1)
        {
            LineHeight = options.LineHeight * options.Font.SizeInPoints * sizeFactor
        };
        formattedText.SetTextDecorations(decorations);

        var smoothness = options.Smoothness;
        var geom = formattedText.BuildGeometry(new Point(0, 0));

        List<Polygon> polys;
        try
        {
            (polys, smoothness) = BuildPolygons(geom, offset, smoothness);
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
            var opt = options;
            opt.Smoothness = smoothness;
            return RenderString(opt, offset);
        }
    }

    public static void FinalizePolygons(List<Polygon> polys)
    {
        polys.ForEach(p => p.RemoveDuplicateVertices());
        polys.RemoveAll(p => p.Count < 3);
        try
        {
            var isects = GeometryUtils.GetIntersectionPoints(polys);
            if (isects.Count > 0)
            {
                var f = GeometryFactory.Floating;
                var iarray = polys.Select(p => p.ToIPolygon()).ToArray();

                NetTopologySuite.Geometries.Geometry union = f.CreateMultiPolygon(iarray);
                union = isects.Aggregate(union,
                    (current, vector) => current.Union(f.CreatePoint(vector).Buffer(0.0001, 1)));
                polys.Clear();
                switch (union)
                {
                    case NetTopologySuite.Geometries.Polygon polygon:
                        polys.AddRange(polygon.ToElmaPolygons());
                        break;
                    case MultiPolygon multiPolygon:
                        polys.AddRange(multiPolygon.Geometries.Select(geometry => geometry as NetTopologySuite.Geometries.Polygon)
                            .SelectMany(poly => poly!.ToElmaPolygons()));
                        break;
                }

                polys.ForEach(p => p.MarkVectorsAs(VectorMark.Selected));
            }
        }
        finally
        {
            polys.ForEach(p => p.UpdateDecomposition());
        }
    }

    public static (List<Polygon> polys, double smoothness) BuildPolygons(
        System.Windows.Media.Geometry geom, Vector offset, double smoothness, bool useOutlined = true)
    {
        var outlinedGeometry = useOutlined ? geom.GetOutlinedPathGeometry(0.005, ToleranceType.Absolute) : geom;

        var polys = new List<Polygon>();
        do
        {
            if (smoothness < 0.0001)
            {
                throw new PolygonException("Smoothness limit reached.");
            }

            polys.Clear();
            var poly = outlinedGeometry.GetFlattenedPathGeometry(smoothness, ToleranceType.Absolute);
            polys.AddRange(
                poly.Figures
                    .Select(
                        figure => new Polygon(
                            figure.Segments
                                .SelectMany(segment => segment switch
                                {
                                    PolyLineSegment s => s.Points.ToArray(),
                                    LineSegment l => new[] { l.Point },
                                    _ => throw new TopologyException("Segment wasn't flattened?")
                                })
                                .Select(p =>
                                    new Vector(p.X + offset.X, -p.Y + offset.Y, VectorMark.Selected))
                        )
                    )
            );
            smoothness *= 0.5;
        } while (polys.Any(p => p.Count < 3));

        return (polys, smoothness);
    }

    private static Typeface? FindTypeface(TextToolOptions options, FontFamily fontfamily,
        Dictionary<string, string> synonymMap)
    {
        Typeface? typeface = null;
        foreach (var familyTypeface in fontfamily.GetTypefaces())
        {
            var faceName = familyTypeface.FaceNames[XmlLanguage.GetLanguage("en-US")];

            var styleName =
                options.Font.Style.ToString()
                    .Replace(",", "")
                    .Replace("Underline", "")
                    .Replace("Strikeout", "")
                    .Trim();
            var fixedStyleName = familyTypeface.Weight + " " + familyTypeface.Style;

            if (familyTypeface.Style.ToString() == styleName)
            {
                typeface = familyTypeface;
                break;
            }

            if (fixedStyleName == options.FontStyleName)
            {
                typeface = familyTypeface;
                break;
            }

            if (fixedStyleName == options.FontStyleName.Replace(faceName, "").Trim())
            {
                typeface = familyTypeface;
                break;
            }

            if (synonymMap.Any(v => faceName == styleName.Replace(v.Key, v.Value)))
            {
                typeface = familyTypeface;
                break;
            }
        }

        return typeface;
    }

    public static System.Windows.Media.Geometry CreateGeometry(DrawingGroup drawingGroup, SvgImportOptions opts)
    {
        var geometry = new GeometryGroup { FillRule = opts.FillRule };
        var defaultPen = new Pen(new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0)), 1);
        foreach (var drawing in drawingGroup.Children)
        {
            switch (drawing)
            {
                case GeometryDrawing gd:
                    HandleGeometry(gd.Geometry, geometry, gd, opts, defaultPen);
                    break;
                case GlyphRunDrawing grd:
                    geometry.Children.Add(grd.GlyphRun.BuildGeometry());
                    break;
                case DrawingGroup dg:
                    var c = new SvgImportOptions
                    {
                        FillRule = FillRule.EvenOdd,
                        Smoothness = opts.Smoothness,
                        UseOutlinedGeometry = opts.UseOutlinedGeometry,
                        NeverWidenClosedPaths = opts.NeverWidenClosedPaths
                    };
                    geometry.Children.Add(CreateGeometry(dg, c));
                    break;
            }
        }

        geometry.Transform = drawingGroup.Transform;
        return geometry;
    }

    private static void HandleGeometry(System.Windows.Media.Geometry g, GeometryGroup geometry, GeometryDrawing gd, SvgImportOptions opts, Pen defaultPen)
    {
        switch (g)
        {
            case CombinedGeometry cg:
                geometry.Children.Add(cg);
                break;
            case EllipseGeometry eg:
                if (gd.Pen is { } && !opts.NeverWidenClosedPaths)
                {
                    geometry.Children.Add(eg.GetWidenedPathGeometry(gd.Pen, opts.Smoothness, ToleranceType.Absolute).GetOutlinedPathGeometry(opts.Smoothness, ToleranceType.Absolute));
                }
                else
                {
                    geometry.Children.Add(eg);
                }

                break;
            case GeometryGroup gg:
                foreach (var c in gg.Children)
                {
                    HandleGeometry(c, geometry, gd, opts, defaultPen);
                }

                break;
            case LineGeometry lg:
                geometry.Children.Add(lg.GetWidenedPathGeometry(gd.Pen, opts.Smoothness, ToleranceType.Absolute));
                break;
            case PathGeometry pg:
                for (int i = pg.Figures.Count - 1; i >= 0; i--)
                {
                    if (!pg.Figures[i].IsClosed || (gd.Pen is { } && !opts.NeverWidenClosedPaths))
                    {
                        var penToUse = gd.Pen ?? defaultPen;
                        pg.AddGeometry(new PathGeometry(new List<PathFigure> { pg.Figures[i] })
                            .GetWidenedPathGeometry(penToUse, opts.Smoothness, ToleranceType.Absolute)
                            .GetOutlinedPathGeometry(opts.Smoothness, ToleranceType.Absolute));
                        pg.Figures.RemoveAt(i);
                    }
                }

                geometry.Children.Add(pg);
                break;
            case RectangleGeometry rg:
                if (gd.Pen is { } && !opts.NeverWidenClosedPaths)
                {
                    geometry.Children.Add(rg.GetWidenedPathGeometry(gd.Pen, opts.Smoothness, ToleranceType.Absolute).GetOutlinedPathGeometry(opts.Smoothness, ToleranceType.Absolute));
                }
                else
                {
                    geometry.Children.Add(rg);
                }
                break;
            case StreamGeometry sg:
                geometry.Children.Add(sg);
                break;
            default:
                geometry.Children.Add(gd.Geometry);
                break;
        }
    }

    public void UpdateHelp()
    {
        LevEditor.InfoLabel.Text = "LMouse: open text input dialog.";
    }

    public override bool Busy => false; // dialog is modal
}