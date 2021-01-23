using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Elmanager.Forms;
using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Buffer;
using EndCapStyle = NetTopologySuite.Operation.Buffer.EndCapStyle;
using JoinStyle = NetTopologySuite.Operation.Buffer.JoinStyle;

namespace Elmanager.EditorTools
{
    internal class PolyOpTool : ToolBase, IEditorTool
    {
        private PolygonOperationType _currentOpType = PolygonOperationType.Union;
        private Polygon _firstPolygon;

        internal PolyOpTool(LevelEditor editor)
            : base(editor)
        {
        }

        private bool FirstSelected { get; set; }

        public void Activate()
        {
            UpdateHelp();
        }

        public void UpdateHelp()
        {
            char polyChar = FirstSelected ? 'B' : 'A';
            LevEditor.InfoLabel.Text = _currentOpType switch
            {
                PolygonOperationType.Union => "Click the polygon " + polyChar + " (operation = A+B).",
                PolygonOperationType.Difference => "Click the polygon " + polyChar + " (operation = A-B).",
                PolygonOperationType.Intersection => "(This mode is not yet implemented.)",
                _ => LevEditor.InfoLabel.Text
            };

            if (!FirstSelected)
            {
                LevEditor.InfoLabel.Text += " Space: Change mode.";
            }
        }

        public void ExtraRendering()
        {
        }

        public List<Polygon> GetExtraPolygons()
        {
            return new();
        }

        public void InActivate()
        {
            if (!FirstSelected) return;
            FirstSelected = false;
            _firstPolygon.Mark = PolygonMark.None;
        }

        public void KeyDown(KeyEventArgs key)
        {
            if (key.KeyCode != Keys.Space || FirstSelected) return;
            switch (_currentOpType)
            {
                case PolygonOperationType.Union:
                    _currentOpType = PolygonOperationType.Difference;
                    break;
                case PolygonOperationType.Intersection:
                    break;

                case PolygonOperationType.Difference:
                    _currentOpType = PolygonOperationType.Union;
                    break;
            }

            UpdateHelp();
        }

        public void MouseDown(MouseEventArgs mouseData)
        {
            switch (mouseData.Button)
            {
                case MouseButtons.Left:
                    int nearestVertexIndex = GetNearestVertexIndex(CurrentPos);
                    if (FirstSelected)
                    {
                        if (nearestVertexIndex >= -1)
                        {
                            if (!NearestPolygon.Equals(_firstPolygon))
                            {
                                MarkAllAs(VectorMark.None);
                                FirstSelected = false;
                                try
                                {
                                    Lev.Polygons.AddRange(
                                        NearestPolygon.PolygonOperationWith(_firstPolygon, _currentOpType));
                                    Lev.Polygons.RemoveAll(p => p.Vertices.Count < 3);
                                    Lev.Polygons.Remove(_firstPolygon);
                                    Lev.Polygons.Remove(NearestPolygon);
                                    LevEditor.Modified = true;
                                }
                                catch (PolygonException e)
                                {
                                    Utils.ShowError(e.Message);
                                }

                                ResetPolygonMarks();
                                UpdateHelp();
                            }
                        }
                    }
                    else
                    {
                        if (nearestVertexIndex >= -1)
                        {
                            FirstSelected = true;
                            _firstPolygon = NearestPolygon;
                            _firstPolygon.Mark = PolygonMark.Selected;
                            UpdateHelp();
                        }
                    }

                    break;
                case MouseButtons.Right:
                    if (FirstSelected)
                    {
                        FirstSelected = false;
                        ResetPolygonMarks();
                        UpdateHelp();
                    }

                    break;
            }
        }

        public void MouseMove(Vector p)
        {
            CurrentPos = p;
            ResetHighlight();
            int nearestVertex = GetNearestVertexIndex(p);
            if (nearestVertex >= -1)
            {
                ChangeCursorToHand();
                if (NearestPolygon.Mark != PolygonMark.Selected)
                    NearestPolygon.Mark = PolygonMark.Highlight;
            }
            else
                ChangeToDefaultCursorIfHand();
        }

        public void MouseOutOfEditor()
        {
            ResetHighlight();
        }

        public void MouseUp()
        {
        }

        public static bool PolyOpSelected(PolygonOperationType opType, List<Polygon> polygons)
        {
            var polys = polygons.GetSelectedPolygons().ToList();
            Geometry SymDiff(Geometry g, Geometry p) => p.SymmetricDifference(g);
            var selection = polygons.GetSelectedPolygonsAsMultiPolygon();
            var touching = polygons.Where(p =>
            {
                if (p.IsGrass)
                    return false;
                var ip = p.ToIPolygon();
                return !polys.Contains(p) && ip.Intersects(selection) && !ip.Contains(selection);
            }).ToList();

            if (!touching.Any())
            {
                return false;
            }

            var others = touching.ToIPolygons().Aggregate((Func<Geometry, Geometry, Geometry>) SymDiff);
            var remaining = polygons.Where(p =>
            {
                if (p.IsGrass)
                    return false;
                var ipoly = p.ToIPolygon();
                return !touching.Contains(p) && !polys.Contains(p) && ipoly.Within(others);
            }).ToList();

            others = remaining.ToIPolygons().Aggregate(others, (Func<Geometry, Geometry, Geometry>) SymDiff);

            Geometry result;
            try
            {
                result = opType switch
                {
                    PolygonOperationType.Union => others.Union(selection),
                    PolygonOperationType.Difference => others.Difference(selection),
                    PolygonOperationType.Intersection => others.Intersection(selection),
                    PolygonOperationType.SymmetricDifference => others.SymmetricDifference(selection)
                        .Buffer(Polygon.BufferDistance, new BufferParameters(0, EndCapStyle.Flat, JoinStyle.Bevel, 1)),
                    _ => throw new Exception("Unknown operation type.")
                };
            }
            catch (TopologyException)
            {
                Utils.ShowError(
                    "Could not perform this operation. Make sure the polygons don't have self-intersections.");
                return false;
            }

            touching.ForEach(p => polygons.Remove(p));
            polys.ForEach(p => polygons.Remove(p));
            remaining.ForEach(p => polygons.Remove(p));
            switch (result)
            {
                case MultiPolygon polygon:
                    {
                        foreach (var geometry in polygon.Geometries.Cast<NetTopologySuite.Geometries.Polygon>().Where(p => !p.IsEmpty))
                        {
                            var newPolys = geometry.ToElmaPolygons().ToList();
                            newPolys.ForEach(p => p.RemoveDuplicateVertices());
                            polygons.AddRange(newPolys);
                        }

                        break;
                    }
                case NetTopologySuite.Geometries.Polygon polygon1 when !polygon1.IsEmpty:
                    {
                        var newPolys = polygon1.ToElmaPolygons().ToList();
                        newPolys.ForEach(p => p.RemoveDuplicateVertices());
                        polygons.AddRange(newPolys);
                        break;
                    }
            }

            if (!polygons.Any())
            {
                Utils.ShowError("The level would become empty after this operation.");
                polygons.AddRange(polys);
                polygons.AddRange(touching);
            }

            return true;
        }

        public override bool Busy => FirstSelected;
    }
}