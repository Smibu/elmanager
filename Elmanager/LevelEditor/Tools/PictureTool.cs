using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Elmanager.Application;
using Elmanager.Geometry;
using Elmanager.Lev;
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
        UpdateHelp();
        if (_currentElem is { })
            AddCurrent(_currentElem);
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
        if (_currentElem is { })
            RemoveCurrent(_currentElem);
    }

    public void KeyDown(KeyEventArgs key)
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
    }

    public void MouseDown(MouseEventArgs mouseData)
    {
        switch (mouseData.Button)
        {
            case MouseButtons.Left:

                if (_currentElem is { })
                {
                    _currentElem = _currentElem with { };
                    Lev.GraphicElements.Add(_currentElem);
                    LevEditor.SetModified(LevModification.Decorations);
                }
                else
                    OpenDialog();

                break;
            case MouseButtons.Right:
                OpenDialog();
                break;
        }
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

    public void MouseMove(Vector p)
    {
        CurrentPos = p;
        AdjustForGrid(ref CurrentPos);
        UpdateCurrentElementPosition();
    }

    private void UpdateCurrentElementPosition()
    {
        if (_currentElem is { })
        {
            _currentElem.Position = CurrentPos + GetAnchorOffset(_currentElem);
        }
    }

    public void MouseOutOfEditor()
    {
    }

    public void MouseUp()
    {
    }

    public void UpdateHelp()
    {
        LevEditor.InfoLabel.Text =
            "LMouse: insert new element; RMouse: select element type; 1-5: change placement anchor.";
    }

    private void AddCurrent(GraphicElement currentGraphicElement)
    {
        Lev.GraphicElements.Insert(0, currentGraphicElement);
        Lev.SortPictures();
    }

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
            if (_currentElem is { })
            {
                AddCurrent(_currentElem);
            }
        }
        else
        {
            RemoveCurrent(_currentElem);
            var newElem = OpenDialogNow(picForm, setDefaultsAutomatically: Global.AppSettings.LevelEditor.AlwaysSetDefaultsInPictureTool);
            _currentElem = newElem ?? _currentElem;
            AddCurrent(_currentElem);
        }
    }

    private void RemoveCurrent(GraphicElement currentGraphicElement)
    {
        Lev.GraphicElements.Remove(currentGraphicElement);
    }

    public override bool Busy => false;
}