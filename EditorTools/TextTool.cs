using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Media;
using Elmanager.Forms;
using Color = System.Drawing.Color;
using FontFamily = System.Drawing.FontFamily;
using Point = System.Windows.Point;

namespace Elmanager.EditorTools
{
    internal class TextTool : ToolBase, IEditorTool
    {
        private TextToolOptions _currentOptions = new TextToolOptions
        {
            Font = new Font(new FontFamily("Arial Unicode MS"), 8.0f),
            Smoothness = 0.03,
            Text = ""
        };

        private List<Polygon> _currentTextPolygons;
        private bool _writing;

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
                    var result = Prompt.ShowDefault(_currentOptions, HandleChange);
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
                Renderer.DrawLine(CurrentPos, CurrentPos + new Vector(1, 0), Color.Blue);

                _currentTextPolygons.ForEach(p => Renderer.DrawPolygon(p, Color.Blue));
            }
        }

        public void InActivate()
        {
        }

        public void Activate()
        {
        }

        private List<Polygon> RenderString(TextToolOptions options, Vector offset)
        {
            var fontfamily = new System.Windows.Media.FontFamily(options.Font.FontFamily.Name);

            var typeface = fontfamily.GetTypefaces().First();
            foreach (var familyTypeface in fontfamily.GetTypefaces())
            {
                var faceName = familyTypeface.FaceNames[XmlLanguage.GetLanguage("en-US")];
                var styleName = options.Font.Style.ToString().Replace(",", "");
                if (faceName == styleName)
                {
                    typeface = familyTypeface;
                    break;
                }
                if (faceName == styleName.Replace("Italic", "Oblique"))
                {
                    typeface = familyTypeface;
                    break;
                }
            }

            var polys = new List<Polygon>();

            GlyphTypeface glyphTypeface;
            var success = typeface.TryGetGlyphTypeface(out glyphTypeface);
            if (success)
            {
                const double size = 1;
                const double lineHeight = size;
                double ypos = 0;
                foreach (var line in options.Text.Split(new[] {"\r\n"}, StringSplitOptions.None))
                {
                    if (line != "")
                    {
                        Func<char, ushort> charToGlyphIndex = c =>
                        {
                            ushort index;
                            if (glyphTypeface.CharacterToGlyphMap.TryGetValue(c, out index))
                                return index;
                            return glyphTypeface.CharacterToGlyphMap[' '];
                        };
                        var glyphRun = new GlyphRun(glyphTypeface,
                            0,
                            false,
                            size,
                            line.Select(charToGlyphIndex).ToList(),
                            new Point(),
                            line.Select(c => glyphTypeface.AdvanceWidths[charToGlyphIndex(c)]*size)
                                .ToList(),
                            null, null, null, null, null, null);
                        var geom = glyphRun.BuildGeometry();
                        var poly = geom.GetOutlinedPathGeometry()
                            .GetFlattenedPathGeometry(options.Smoothness, ToleranceType.Absolute);
                        foreach (var figure in poly.Figures)
                        {
                            var polygon = new Polygon();
                            foreach (
                                var point in
                                    figure.Segments.Select(segment => segment as PolyLineSegment)
                                        .SelectMany(polysegment => polysegment.Points))
                            {
                                polygon.Add(new Vector(point.X + offset.X, point.Y + offset.Y + ypos,
                                    Geometry.VectorMark.Selected));
                            }
                            polygon.UpdateDecomposition();
                            polys.Add(polygon);
                        }
                    }
                    ypos += lineHeight;
                }
            }
            else
            {
                Utils.ShowError("Failed to get GlyphTypeface for this Typeface, sorry. Try some other font.");
            }
            return polys;
        }

        public void UpdateHelp()
        {
        }
    }
}