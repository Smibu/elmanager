using System.Drawing;
using System.Windows.Forms;
using System.Windows.Input;
using Elmanager.Application;
using Elmanager.Geometry;
using Elmanager.Lev;
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
        UpdateHelp();
    }

    public void UpdateHelp()
    {
        LevEditor.InfoLabel.Text =
            "LMouse: create vertex. Click near an edge of a polygon to add vertices to it. LShift + click: create rectangle.";
        if (_currentPolygon is { })
        {
            var lngth = (_currentPolygon.Vertices[^1] -
                         _currentPolygon.Vertices[^2]).Length;
            LevEditor.InfoLabel.Text += $" Edge length: {lngth:F3}";
        }
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

    public void InActivate()
    {
        FinishVertexCreation();
    }

    public void KeyDown(KeyEventArgs key)
    {
        if (key.KeyCode != Keys.Space || _currentPolygon is null) return;
        _currentPolygon.RemoveLastVertex();
        _currentPolygon.ChangeOrientation();
        _currentPolygon.Add(CurrentPos);
        _currentPolygon.UpdateDecompositionOrGrassSlopeInfo(Lev.GroundBounds, LevEditor.Settings.RenderingSettings.GrassZoom);
    }

    public void MouseDown(MouseEventArgs mouseData)
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
                    rect.UpdateDecompositionOrGrassSlopeInfo(Lev.GroundBounds, LevEditor.Settings.RenderingSettings.GrassZoom);
                    LevEditor.SetModified(LevModification.Ground);
                    _rectangleStart = null;
                    return;
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
                        _currentPolygon.UpdateDecompositionOrGrassSlopeInfo(Lev.GroundBounds, LevEditor.Settings.RenderingSettings.GrassZoom);
                    }
                }

                break;
            case MouseButtons.Right:
                FinishVertexCreation();
                break;
        }
    }

    public void MouseMove(Vector p)
    {
        CurrentPos = p;
        if (_currentPolygon is { })
        {
            AdjustForGrid(ref CurrentPos);
            _currentPolygon.Vertices[^1] = CurrentPos;
            if (_currentPolygon.Vertices.Count > 2)
                _currentPolygon.UpdateDecompositionOrGrassSlopeInfo(Lev.GroundBounds, LevEditor.Settings.RenderingSettings.GrassZoom);
            UpdateHelp();
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
    }

    public void MouseOutOfEditor()
    {
        ResetHighlight();
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
                _currentPolygon.UpdateDecompositionOrGrassSlopeInfo(Lev.GroundBounds, LevEditor.Settings.RenderingSettings.GrassZoom);
                LevEditor.SetModified(_currentPolygon.IsGrass ? LevModification.Decorations : LevModification.Ground);
            }
            else
                Lev.Polygons.Remove(_currentPolygon);

            _currentPolygon = null;
            UpdateHelp();
        }
        else if (_rectangleStart is { })
        {
            _rectangleStart = null;
        }
    }

    public override bool Busy => _currentPolygon is { } || _rectangleStart is { };
}