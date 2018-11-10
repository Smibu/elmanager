using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Media;
using Elmanager.Forms;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Drawing.Color;
using FlowDirection = System.Windows.FlowDirection;
using Point = System.Windows.Point;

namespace Elmanager.EditorTools
{
    internal class TextTool : ToolBase, IEditorTool
    {
        private TextToolOptions _currentOptions = TextToolOptions.Default;

        private List<Polygon> _currentTextPolygons;
        private bool _writing;
        private static readonly Dictionary<string, string> EmptySynonymMap = new Dictionary<string, string>();

        private static readonly Dictionary<string, string> SynonymMap = new Dictionary<string, string>
        {
            {"Italic", "Oblique"},
            {"Demibold", "Bold"}
        };

        private Dictionary<Font, double> _minSmoothnesses = new Dictionary<Font, double>();

        internal TextTool(LevelEditor editor) : base(editor)
        {
        }

        public void MouseDown(MouseEventArgs mouseData)
        {
            switch (mouseData.Button)
            {
                case MouseButtons.Left:
                    _writing = true;
                    _currentTextPolygons = new List<Polygon>();
                    Renderer.RedrawScene();
                    var result = TextToolForm.ShowDefault(_currentOptions, HandleChange);
                    _writing = false;
                    if (result.HasValue)
                    {
                        _currentOptions = result.Value;
                        MarkAllAs(Geometry.VectorMark.None);
                        var rendered = RenderString(_currentOptions, CurrentPos);
                        Lev.Polygons.AddRange(rendered);
                        if (rendered.Count > 0)
                        {
                            LevEditor.Modified = true;
                        }

                        LevEditor.UpdateSelectionInfo();
                    }

                    Renderer.RedrawScene();
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
            Renderer.RedrawScene();
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
            if (_writing)
            {
                _currentTextPolygons.ForEach(p => Renderer.DrawPolygon(p, Color.Blue));
            }
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
            var fontfamily = new System.Windows.Media.FontFamily(options.Font.FontFamily.Name);

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

            var polys = new List<Polygon>();
            const double sizeFactor = 0.1;
            var formattedText = new FormattedText(options.Text, CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight, typeface, options.Font.SizeInPoints * sizeFactor, Brushes.Black)
            {
                LineHeight = options.LineHeight * options.Font.SizeInPoints * sizeFactor
            };
            formattedText.SetTextDecorations(decorations);
            var isCached = _minSmoothnesses.TryGetValue(options.Font, out var cached);
            if (!isCached)
            {
                cached = options.Smoothness;
            }

            var smoothness = Math.Min(options.Smoothness, cached);
            var outlinedGeometry = formattedText.BuildGeometry(new Point(0, 0))
                .GetOutlinedPathGeometry(0.005, ToleranceType.Absolute);
            int iterations = 0;
            do
            {
                if (smoothness < 0.0001)
                {
                    var opt = TextToolOptions.Default;
                    opt.Text = "Unable to render\nthis font without\ntopology errors.";
                    return RenderString(opt, offset);
                }

                polys.Clear();
                var poly = outlinedGeometry.GetFlattenedPathGeometry(smoothness, ToleranceType.Absolute);
                polys.AddRange(
                    poly.Figures.Select(
                        figure => new Polygon(
                            figure.Segments
                                .Select(segment => segment as PolyLineSegment)
                                .SelectMany(polysegment => polysegment.Points)
                                .Select(p => new Vector(p.X + offset.X, p.Y + offset.Y, Geometry.VectorMark.Selected))
                        )
                    )
                );
                smoothness *= 0.5;
                ++iterations;
            } while (polys.Any(p => p.Count < 3 || !p.IsSimple));

            if (iterations > 1)
            {
                _minSmoothnesses[options.Font] = smoothness * 2;
            }

            var isects = Geometry.GetIntersectionPoints(polys);
            if (isects.Count > 0)
            {
                var f = GeometryFactory.Floating;
                var iarray = polys.Select(p => p.ToIPolygon()).ToArray();
                polys.Clear();
                IGeometry union = f.CreateMultiPolygon(iarray);
                try
                {
                    union = isects.Aggregate(union,
                        (current, vector) => current.Union(f.CreatePoint(vector).Buffer(0.0001, 1)));
                }
                catch (TopologyException)
                {
                    var opt = options;
                    opt.Smoothness = smoothness;
                    _minSmoothnesses[options.Font] = smoothness;
                    return RenderString(opt, offset);
                }

                if (union is IPolygon polygon)
                {
                    polys.AddRange(polygon.ToElmaPolygons());
                }
                else
                {
                    if (union is IMultiPolygon multiPolygon)
                    {
                        polys.AddRange(multiPolygon.Geometries.Select(geometry => geometry as IPolygon)
                            .SelectMany(poly => poly.ToElmaPolygons()));
                    }
                }

                polys.ForEach(p => p.MarkVectorsAs(Geometry.VectorMark.Selected));
            }

            polys.ForEach(p => p.UpdateDecomposition());
            return polys;
        }

        private static Typeface FindTypeface(TextToolOptions options, System.Windows.Media.FontFamily fontfamily,
            Dictionary<string, string> synonymMap)
        {
            Typeface typeface = null;
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

        public void UpdateHelp()
        {
            LevEditor.InfoLabel.Text = "Left mouse button: open text input dialog.";
        }

        public override bool Busy => false; // dialog is modal
    }
}