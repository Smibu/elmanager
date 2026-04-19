using System.Drawing;
using System.Windows.Forms;
using System.Windows.Input;
using Elmanager.Application;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.Rendering;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Elmanager.LevelEditor.Tools;

internal class VertexTool : ToolBase, IEditorTool
{
    private Polygon? _currentPolygon;
    private NearestVertexInfo.EdgeInfo? _nearestVertexInfo;
    private Vector? _rectangleStart;

    internal VertexTool(LevelEditorForm editor)
        : base(editor)
    {
    }

    public void Activate()
    {
    }

    public string GetHelp()
    {
        var text = "LMouse: create vertex. Click near an edge of a polygon to add vertices to it. LShift + click: create rectangle.";
        if (_currentPolygon is { })
        {
            var lngth = (_currentPolygon.Vertices[^1] -
                         _currentPolygon.Vertices[^2]).Length;
            text += $" Edge length: {lngth:F3}";
        }
        return text;
    }

    public void ExtraRendering()
    {
        if (_currentPolygon is { })
            Renderer.DrawLine(_currentPolygon.GetLastVertex(), _currentPolygon.Vertices[0], Color.Red);
        else if (_rectangleStart is { } r)
        {
            AdjustForGrid(ref CurrentPos);
            Renderer.DrawRectangle(r, CurrentPos, Color.Blue);
        }
        else
        {
            if (Global.AppSettings.LevelEditor.UseHighlight && _nearestVertexInfo is { } v)
            {
                Renderer.DrawLine(v.Polygon[v.StartIndex], v.Polygon[v.EndIndex],
                    Color.Yellow);
            }
        }
    }

    public LevVisualChange InActivate()
    {
        FinishVertexCreation();
        return LevVisualChange.Nothing;
    }

    public LevVisualChange KeyDown(KeyEventArgs key)
    {
        if (key.KeyCode == Keys.Space && _currentPolygon is not null)
        {
            _currentPolygon.RemoveLastVertex();
            _currentPolygon.ChangeOrientation();
            _currentPolygon.Add(CurrentPos);
            _currentPolygon.UpdateGrassSlopeInfo(Lev.GroundBounds, LevEditor.Settings.RenderingSettings.GrassZoom);
        }

        return LevVisualChange.Nothing;
    }

    public LevVisualChange MouseDown(MouseEventArgs mouseData)
    {
        switch (mouseData.Button)
        {
            case MouseButtons.Left:
                var nearestIndex = GetNearestSegmentInfo(CurrentPos);
                AdjustForGrid(ref CurrentPos);
                if (_rectangleStart is { } r)
                {
                    var rect = Polygon.Rectangle(r, CurrentPos);
                    Lev.Polygons.Add(rect);
                    rect.UpdateGrassSlopeInfo(Lev.GroundBounds, LevEditor.Settings.RenderingSettings.GrassZoom);
                    LevEditor.SetModified(LevModification.Ground);
                    _rectangleStart = null;
                    return LevVisualChange.Nothing;
                }

                if (_currentPolygon is null)
                {
                    if (Keyboard.IsKeyDown(Key.LeftShift))
                    {
                        _rectangleStart = CurrentPos;
                    }
                    else
                    {
                        if (nearestIndex is { } v)
                        {
                            _nearestVertexInfo = nearestIndex;
                            _currentPolygon = v.Polygon;
                            _currentPolygon.SetBeginPoint(nearestIndex.EndIndex);
                            _currentPolygon.Add(CurrentPos);
                            ChangeToDefaultCursorIfHand();
                        }
                        else
                            _currentPolygon = new Polygon(CurrentPos, CurrentPos);
                    }
                }
                else
                {
                    _currentPolygon.Add(CurrentPos);
                    if (_currentPolygon.Vertices.Count == 3)
                    {
                        Lev.Polygons.Add(_currentPolygon);
                        _currentPolygon.UpdateGrassSlopeInfo(Lev.GroundBounds, LevEditor.Settings.RenderingSettings.GrassZoom);
                    }
                }

                break;
            case MouseButtons.Right:
                FinishVertexCreation();
                break;
        }
        return LevVisualChange.Nothing;
    }

    public LevVisualChange MouseMove(Vector p)
    {
        CurrentPos = p;
        if (_currentPolygon is { })
        {
            AdjustForGrid(ref CurrentPos);
            _currentPolygon.Vertices[^1] = CurrentPos;
            if (_currentPolygon.Vertices.Count > 2)
            {
                _currentPolygon.UpdateGrassSlopeInfo(Lev.GroundBounds, LevEditor.Settings.RenderingSettings.GrassZoom);
                return _currentPolygon.IsGrass ? LevVisualChange.Grass : LevVisualChange.Ground;
            }
        }
        else if (_rectangleStart is null)
        {
            ResetHighlight();
            var nearestVertex = GetNearestSegmentInfo(CurrentPos);
            _nearestVertexInfo = nearestVertex;
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

    private void FinishVertexCreation()
    {
        if (_currentPolygon is { })
        {
            _currentPolygon.RemoveLastVertex();
            if (_currentPolygon.Vertices.Count > 2)
            {
                _currentPolygon.UpdateGrassSlopeInfo(Lev.GroundBounds, LevEditor.Settings.RenderingSettings.GrassZoom);
                LevEditor.SetModified(_currentPolygon.IsGrass ? LevModification.Grass : LevModification.Ground);
            }
            else
                Lev.Polygons.Remove(_currentPolygon);

            _currentPolygon = null;
        }
        else if (_rectangleStart is { })
        {
            _rectangleStart = null;
        }
    }

    public override bool Busy => _currentPolygon is { } || _rectangleStart is { };
}