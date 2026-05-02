using System;
using System.Collections.Generic;
using Elmanager.Geometry;
using Elmanager.LevelEditor.Input;
using Elmanager.Rendering;

namespace Elmanager.LevelEditor.Tools;

public class PictureTool : ToolBase, IEditorTool
{
    private GraphicElement? _currentElem;
    private PlacementAnchor _anchor = PlacementAnchor.Center;

    public PictureTool(ILevelEditor editor)
        : base(editor)
    {
    }

    public void Activate()
    {
    }

    public void ExtraRendering()
    {
    }

    public TransientElements GetTransientElements(bool hasFocus) => _currentElem is not null && hasFocus
        ? TransientElements.FromGraphicElements(new List<GraphicElement> { _currentElem })
        : TransientElements.Empty;

    public LevVisualChange InActivate() => LevVisualChange.GraphicElements;

    public LevVisualChange KeyDown(EditorKeyEventArgs key)
    {
        switch (key.KeyCode)
        {
            case EditorKey.Space:
                OpenDialog();
                break;
            case EditorKey.D1:
                _anchor = PlacementAnchor.Center;
                break;
            case EditorKey.D2:
                _anchor = PlacementAnchor.TopLeft;
                break;
            case EditorKey.D3:
                _anchor = PlacementAnchor.TopRight;
                break;
            case EditorKey.D4:
                _anchor = PlacementAnchor.BottomLeft;
                break;
            case EditorKey.D5:
                _anchor = PlacementAnchor.BottomRight;
                break;
        }
        UpdateCurrentElementPosition();
        return LevVisualChange.GraphicElements;
    }

    public LevVisualChange MouseDown(EditorMouseEventArgs mouseData)
    {
        switch (mouseData.Button)
        {
            case EditorMouseButton.Left:

                if (_currentElem is { })
                {
                    Lev.GraphicElements.Add(_currentElem);
                    _currentElem = _currentElem with { };
                    LevEditor.SetModified(_currentElem is GraphicElement.Picture ? LevModification.Pictures : LevModification.Textures);
                }
                else
                    OpenDialog();

                break;
            case EditorMouseButton.Right:
                OpenDialog();
                break;
        }
        return LevVisualChange.Nothing;
    }

    private Vector GetAnchorOffset(GraphicElement elem) =>
        _anchor switch
        {
            PlacementAnchor.Center => new Vector(-elem.Width / 2, elem.Height / 2),
            PlacementAnchor.TopLeft => new Vector(0, 0),
            PlacementAnchor.TopRight => new Vector(-elem.Width, 0),
            PlacementAnchor.BottomLeft => new Vector(0, elem.Height),
            PlacementAnchor.BottomRight => new Vector(-elem.Width, elem.Height),
            _ => throw new ArgumentOutOfRangeException()
        };

    public LevVisualChange MouseMove(Vector p)
    {
        CurrentPos = p;
        AdjustForGrid(ref CurrentPos);
        UpdateCurrentElementPosition();
        return _currentElem is not null ? LevVisualChange.GraphicElements : LevVisualChange.Nothing;
    }

    private void UpdateCurrentElementPosition()
    {
        if (_currentElem is { })
        {
            _currentElem.Position = CurrentPos + GetAnchorOffset(_currentElem);
        }
    }

    public LevVisualChange MouseOutOfEditor() => LevVisualChange.GraphicElements;

    public void MouseUp()
    {
    }

    public string GetHelp() =>
        "LMouse: insert new element; RMouse: select element type; 1-5: change placement anchor.";

    private GraphicElement? OpenDialogNow(bool setDefaultsAutomatically)
    {
        return LevEditor.PictureDialogService.ShowPictureDialog(Renderer.OpenGlLgr?.CurrentLgr, CurrentPos,
            _currentElem, setDefaultsAutomatically);
    }

    private void OpenDialog()
    {
        if (_currentElem is null)
        {
            _currentElem = OpenDialogNow(setDefaultsAutomatically: true);
        }
        else
        {
            var newElem = OpenDialogNow(setDefaultsAutomatically: LevEditor.Settings.AlwaysSetDefaultsInPictureTool);
            _currentElem = newElem ?? _currentElem;
        }
    }

    public override bool Busy => false;
}
