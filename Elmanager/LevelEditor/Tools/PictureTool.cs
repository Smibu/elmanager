using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Elmanager.Application;
using Elmanager.Geometry;
using Elmanager.Rendering;

namespace Elmanager.LevelEditor.Tools;

internal class PictureTool : ToolBase, IEditorTool
{
    private GraphicElement? _currentElem;
    private PlacementAnchor _anchor = PlacementAnchor.Center;

    internal PictureTool(LevelEditorForm editor)
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

    public LevVisualChange KeyDown(KeyEventArgs key)
    {
        switch (key.KeyCode)
        {
            case Keys.Space:
                OpenDialog();
                break;
            case Keys.D1:
                _anchor = PlacementAnchor.Center;
                break;
            case Keys.D2:
                _anchor = PlacementAnchor.TopLeft;
                break;
            case Keys.D3:
                _anchor = PlacementAnchor.TopRight;
                break;
            case Keys.D4:
                _anchor = PlacementAnchor.BottomLeft;
                break;
            case Keys.D5:
                _anchor = PlacementAnchor.BottomRight;
                break;
        }
        UpdateCurrentElementPosition();
        return LevVisualChange.GraphicElements;
    }

    public LevVisualChange MouseDown(MouseEventArgs mouseData)
    {
        switch (mouseData.Button)
        {
            case MouseButtons.Left:

                if (_currentElem is { })
                {
                    Lev.GraphicElements.Add(_currentElem);
                    _currentElem = _currentElem with { };
                    LevEditor.SetModified(_currentElem is GraphicElement.Picture ? LevModification.Pictures : LevModification.Textures);
                }
                else
                    OpenDialog();

                break;
            case MouseButtons.Right:
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

    private GraphicElement? OpenDialogNow(PictureForm picForm, bool setDefaultsAutomatically)
    {
        if (Renderer.OpenGlLgr == null)
        {
            return null;
        }
        picForm.Location = Control.MousePosition;
        picForm.AllowMultiple = false;
        picForm.AutoTextureMode = false;
        picForm.SetDefaultsAutomatically = setDefaultsAutomatically;
        picForm.ShowDialog();
        if (picForm.Selection is { } sel)
        {
            var clipping = sel.Clipping!.Value;
            var distance = sel.Distance!.Value;
            return sel switch
            {
                ImageSelection.TextureSelection t => GraphicElement.Text(clipping, distance,
                    CurrentPos, Renderer.OpenGlLgr.DrawableImageFromLgrImage(t.Txt),
                    Renderer.OpenGlLgr.DrawableImageFromLgrImage(t.Mask)),
                ImageSelection.PictureSelection p => GraphicElement.Pic(Renderer.OpenGlLgr.DrawableImageFromLgrImage(p.Pic),
                    CurrentPos, distance, clipping),
                _ => throw new Exception("Unexpected")
            };
        }

        return null;
    }

    private void OpenDialog()
    {
        var picForm = new PictureForm(LevEditor.EditorLgr!, _currentElem);
        if (_currentElem is null)
        {
            _currentElem = OpenDialogNow(picForm, setDefaultsAutomatically: true);
        }
        else
        {
            var newElem = OpenDialogNow(picForm, setDefaultsAutomatically: Global.AppSettings.LevelEditor.AlwaysSetDefaultsInPictureTool);
            _currentElem = newElem ?? _currentElem;
        }
    }

    public override bool Busy => false;
}
