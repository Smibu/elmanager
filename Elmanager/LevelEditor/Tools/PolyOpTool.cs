using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.UI;
using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Buffer;
using EndCapStyle = NetTopologySuite.Operation.Buffer.EndCapStyle;
using JoinStyle = NetTopologySuite.Operation.Buffer.JoinStyle;
using Polygon = Elmanager.Lev.Polygon;
using NetGeometry = NetTopologySuite.Geometries.Geometry;

namespace Elmanager.LevelEditor.Tools;

internal class PolyOpTool : ToolBase, IEditorTool
{
    private PolygonOperationType _currentOpType = PolygonOperationType.Union;
    private Polygon? _firstPolygon;

    internal PolyOpTool(LevelEditorForm editor)
        : base(editor)
    {
    }

    private bool FirstSelected => _firstPolygon is { };

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
        if (_firstPolygon is null) return;
        _firstPolygon.Mark = PolygonMark.None;
        _firstPolygon = null;
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
                var info = GetNearestVertexInfo(CurrentPos);
                if (_firstPolygon is { })
                {
                    if (info is { } v)
                    {
                        if (!v.Polygon.Equals(_firstPolygon))
                        {
                            MarkAllAs(VectorMark.None);
                            try
                            {
                                Lev.Polygons.AddRange(
                                    v.Polygon.PolygonOperationWith(_firstPolygon, _currentOpType));
                                Lev.Polygons.RemoveAll(p => p.Vertices.Count < 3);
                                Lev.Polygons.Remove(_firstPolygon);
                                Lev.Polygons.Remove(v.Polygon);
                                LevEditor.SetModified(LevModification.Ground);
                            }
                            catch (PolygonException e)
                            {
                                UiUtils.ShowError(e.Message);
                            }

                            _firstPolygon = null;
                            ResetPolygonMarks();
                            UpdateHelp();
                        }
                    }
                }
                else
                {
                    if (info is { } v)
                    {
                        _firstPolygon = v.Polygon;
                        _firstPolygon.Mark = PolygonMark.Selected;
                        UpdateHelp();
                    }
                }

                break;
            case MouseButtons.Right:
                if (_firstPolygon is { })
                {
                    _firstPolygon = null;
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
        if (GetNearestVertexInfo(p) is { } v)
        {
            ChangeCursorToHand();
            if (v.Polygon.Mark != PolygonMark.Selected)
                v.Polygon.Mark = PolygonMark.Highlight;
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
        NetGeometry SymDiff(NetGeometry g, NetGeometry p) => p.SymmetricDifference(g);
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

        var others = touching.ToIPolygons().Aggregate((Func<NetGeometry, NetGeometry, NetGeometry>) SymDiff);
        var remaining = polygons.Where(p =>
        {
            if (p.IsGrass)
                return false;
            var ipoly = p.ToIPolygon();
            return !touching.Contains(p) && !polys.Contains(p) && ipoly.Within(others);
        }).ToList();

        others = remaining.ToIPolygons().Aggregate(others, (Func<NetGeometry, NetGeometry, NetGeometry>) SymDiff);

        NetGeometry result;
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
            UiUtils.ShowError(
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
            UiUtils.ShowError("The level would become empty after this operation.");
            polygons.AddRange(polys);
            polygons.AddRange(touching);
        }

        return true;
    }

    public override bool Busy => FirstSelected;
}