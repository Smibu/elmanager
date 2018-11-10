using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Elmanager.Forms;
using GeoAPI.Geometries;
using GeoAPI.Operation.Buffer;
using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Buffer;

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
            switch (_currentOpType)
            {
                case PolygonOperationType.Union:
                    LevEditor.InfoLabel.Text = "Click the polygon " + polyChar + " (operation = A+B).";
                    break;
                case PolygonOperationType.Difference:
                    LevEditor.InfoLabel.Text = "Click the polygon " + polyChar + " (operation = A-B).";
                    break;
                case PolygonOperationType.Intersection:
                    LevEditor.InfoLabel.Text = "(This mode is not yet implemented.)";
                    break;
            }

            if (!FirstSelected)
            {
                LevEditor.InfoLabel.Text += " Space: Change mode.";
            }
        }

        public void ExtraRendering()
        {
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
                                MarkAllAs(Geometry.VectorMark.None);
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
            IGeometry SymDiff(IGeometry g, IGeometry p) => p.SymmetricDifference(g);
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

            var others = touching.ToIPolygons().Aggregate((Func<IGeometry, IGeometry, IGeometry>) SymDiff);
            var remaining = polygons.Where(p =>
            {
                if (p.IsGrass)
                    return false;
                var ipoly = p.ToIPolygon();
                return !touching.Contains(p) && !polys.Contains(p) && ipoly.Within(others);
            }).ToList();

            others = remaining.ToIPolygons().Aggregate(others, (Func<IGeometry, IGeometry, IGeometry>) SymDiff);

            IGeometry result;
            try
            {
                switch (opType)
                {
                    case PolygonOperationType.Union:
                        result = others.Union(selection);
                        break;
                    case PolygonOperationType.Difference:
                        result = others.Difference(selection);
                        break;
                    case PolygonOperationType.Intersection:
                        result = others.Intersection(selection);
                        break;
                    case PolygonOperationType.SymmetricDifference:
                        result = others.SymmetricDifference(selection)
                            .Buffer(-0.000001, new BufferParameters(0, EndCapStyle.Flat, JoinStyle.Bevel, 1));
                        break;
                    default:
                        throw new Exception("Unknown operation type.");
                }
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
            if (result is MultiPolygon polygon)
            {
                foreach (var geometry in polygon.Geometries.Cast<IPolygon>().Where(p => !p.IsEmpty))
                {
                    polygons.AddRange(geometry.ToElmaPolygons());
                }
            }
            else if (result is IPolygon polygon1 && !polygon1.IsEmpty)
            {
                polygons.AddRange(polygon1.ToElmaPolygons());
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