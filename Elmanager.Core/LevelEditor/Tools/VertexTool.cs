using System.Drawing;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.LevelEditor.Input;
using Elmanager.Rendering;

namespace Elmanager.LevelEditor.Tools;

public class VertexTool : ToolBase, IEditorTool
{
    private VertexToolState _state = new Idle(null);

    private union VertexToolState(Idle, Drawing, DrawingRectangle);

    private record Idle(NearestVertexInfo.EdgeInfo? HoveredEdge);

    private record Drawing(Polygon Polygon);

    private record DrawingRectangle(Vector Start);

    public VertexTool(ILevelEditor editor)
        : base(editor)
    {
    }

    public void Activate()
    {
    }

    public string GetHelp()
    {
        var text = "LMouse: create vertex. Click near an edge of a polygon to add vertices to it. LShift + click: create rectangle.";
        if (_state is Drawing { Polygon: var currentPolygon })
        {
            var lngth = (currentPolygon.Vertices[^1] -
                         currentPolygon.Vertices[^2]).Length;
            text += $" Edge length: {lngth:F3}";
        }
        return text;
    }

    public void ExtraRendering()
    {
        if (_state is Drawing { Polygon: var currentPolygon })
            Renderer.DrawLine(currentPolygon.GetLastVertex(), currentPolygon.Vertices[0], Color.Red);
        else if (_state is DrawingRectangle { Start: var r })
        {
            AdjustForGrid(ref CurrentPos);
            Renderer.DrawRectangle(r, CurrentPos, Color.Blue);
        }
        else if (_state is Idle { HoveredEdge: { } v })
        {
            Renderer.DrawLine(v.Polygon[v.StartIndex], v.Polygon[v.EndIndex],
                Color.Yellow);
        }
    }

    public LevVisualChange InActivate()
    {
        FinishVertexCreation();
        return LevVisualChange.Nothing;
    }

    public LevVisualChange KeyDown(EditorKeyEventArgs key)
    {
        if (key.KeyCode == EditorKey.Space && _state is Drawing { Polygon: var currentPolygon })
        {
            currentPolygon.RemoveLastVertex();
            currentPolygon.ChangeOrientation();
            currentPolygon.Add(CurrentPos);
            currentPolygon.UpdateGrassSlopeInfo(Lev.GroundBounds, LevEditor.RenderingSettings.GrassZoom);
        }

        return LevVisualChange.Nothing;
    }

    public LevVisualChange MouseDown(EditorMouseEventArgs mouseData)
    {
        switch (mouseData.Button)
        {
            case EditorMouseButton.Left:
                var nearestIndex = GetNearestSegmentInfo(CurrentPos);
                AdjustForGrid(ref CurrentPos);
                if (_state is DrawingRectangle { Start: var r })
                {
                    var rect = Polygon.Rectangle(r, CurrentPos);
                    Lev.Polygons.Add(rect);
                    rect.UpdateGrassSlopeInfo(Lev.GroundBounds, LevEditor.RenderingSettings.GrassZoom);
                    LevEditor.SetModified(LevModification.Ground);
                    _state = new Idle(null);
                    return LevVisualChange.Nothing;
                }

                if (_state is not Drawing { Polygon: var currentPolygon })
                {
                    if (Keyboard.IsKeyDown(ModifierKey.LeftShift))
                    {
                        _state = new DrawingRectangle(CurrentPos);
                    }
                    else
                    {
                        if (nearestIndex is { } v)
                        {
                            v.Polygon.SetBeginPoint(v.EndIndex);
                            v.Polygon.Add(CurrentPos);
                            _state = new Drawing(v.Polygon);
                            ChangeToDefaultCursorIfHand();
                        }
                        else
                            _state = new Drawing(new Polygon(CurrentPos, CurrentPos));
                    }
                }
                else
                {
                    currentPolygon.Add(CurrentPos);
                    if (currentPolygon.Vertices.Count == 3)
                    {
                        Lev.Polygons.Add(currentPolygon);
                        currentPolygon.UpdateGrassSlopeInfo(Lev.GroundBounds, LevEditor.RenderingSettings.GrassZoom);
                    }
                }

                break;
            case EditorMouseButton.Right:
                return FinishVertexCreation();
        }
        return LevVisualChange.Nothing;
    }

    public LevVisualChange MouseMove(Vector p)
    {
        CurrentPos = p;
        if (_state is Drawing { Polygon: var currentPolygon })
        {
            AdjustForGrid(ref CurrentPos);
            currentPolygon.Vertices[^1] = CurrentPos;
            if (currentPolygon.Vertices.Count > 2)
            {
                currentPolygon.UpdateGrassSlopeInfo(Lev.GroundBounds, LevEditor.RenderingSettings.GrassZoom);
                return currentPolygon.IsGrass ? LevVisualChange.Grass : LevVisualChange.Ground;
            }
        }
        else if (_state is Idle)
        {
            ResetHighlight();
            var nearestVertex = GetNearestSegmentInfo(CurrentPos);
            _state = new Idle(nearestVertex);
            if (nearestVertex is { })
            {
                ChangeCursorToHand();
            }
            else
            {
                ChangeToDefaultCursorIfHand();
            }
        }

        return LevVisualChange.Nothing;
    }

    public LevVisualChange MouseOutOfEditor()
    {
        ResetHighlight();
        return LevVisualChange.Nothing;
    }

    public void MouseUp()
    {
    }

    private LevVisualChange FinishVertexCreation()
    {
        if (_state is Drawing { Polygon: var poly })
        {
            _state = new Idle(null);
            poly.RemoveLastVertex();
            if (poly.Vertices.Count > 2)
            {
                poly.UpdateGrassSlopeInfo(Lev.GroundBounds, LevEditor.RenderingSettings.GrassZoom);
                LevEditor.SetModified(poly.IsGrass ? LevModification.Grass : LevModification.Ground);
            }
            else
            {
                Lev.Polygons.Remove(poly);
                return LevVisualChange.Ground;
            }
        }
        else if (_state is DrawingRectangle)
        {
            _state = new Idle(null);
        }

        return LevVisualChange.Nothing;
    }

    public override bool Busy => _state is not Idle;
}
