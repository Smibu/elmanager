using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Media;
using Elmanager.Forms;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Drawing.Color;
using FlowDirection = System.Windows.FlowDirection;
using FontFamily = System.Drawing.FontFamily;
using Point = System.Windows.Point;

namespace Elmanager.EditorTools
{
    internal class TextTool : ToolBase, IEditorTool
    {
        private TextToolOptions _currentOptions = new TextToolOptions
        {
            Font = new Font(new FontFamily("Arial"), 9.0f),
            Smoothness = 0.03,
            Text = "Type text here!",
            LineHeight = 1
        };

        private List<Polygon> _currentTextPolygons;
        private bool _writing;
        private static readonly Dictionary<string, string> EmptySynonymMap = new Dictionary<string, string>();

        private static readonly Dictionary<string, string> SynonymMap = new Dictionary<string, string>
        {
            {"Italic", "Oblique"},
            {"Demibold", "Bold"}
        };

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

        public void MouseUp(MouseEventArgs mouseData)
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

        private static List<Polygon> RenderString(TextToolOptions options, Vector offset)
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
                FlowDirection.LeftToRight, typeface, options.Font.SizeInPoints*sizeFactor, Brushes.Black)
            {
                LineHeight = options.LineHeight*options.Font.SizeInPoints*sizeFactor
            };
            formattedText.SetTextDecorations(decorations);
            var poly = formattedText.BuildGeometry(new Point(0, 0))
                .GetOutlinedPathGeometry(0.005, ToleranceType.Absolute)
                .GetFlattenedPathGeometry(options.Smoothness, ToleranceType.Absolute);
            foreach (var figure in poly.Figures)
            {
                var polygon = new Polygon();
                foreach (
                    var point in
                        figure.Segments.Select(segment => segment as PolyLineSegment)
                            .SelectMany(polysegment => polysegment.Points))
                {
                    polygon.Add(new Vector(point.X + offset.X, point.Y + offset.Y,
                        Geometry.VectorMark.Selected));
                }
                polygon.UpdateDecomposition();
                polys.Add(polygon);
            }
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
    }
}