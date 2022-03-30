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
        }
    }

    public void MouseDown(MouseEventArgs mouseData)
    {
        switch (mouseData.Button)
        {
            case MouseButtons.Left:

                if (_currentElem is { })
                {
                    _currentElem = _currentElem with {};
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

    public void MouseMove(Vector p)
    {
        CurrentPos = p;
        AdjustForGrid(ref CurrentPos);

        if (_currentElem is { })
        {
            _currentElem.Position = CurrentPos + new Vector(-_currentElem.Width / 2, _currentElem.Height / 2);
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
            "Left mouse button: insert new picture/texture, right mouse button: select picture type.";
    }

    private void AddCurrent(GraphicElement currentGraphicElement)
    {
        Lev.GraphicElements.Insert(0, currentGraphicElement);
        Lev.SortPictures();
    }

    private GraphicElement? OpenDialogNow(PictureForm picForm, bool setDefaultsAutomatically)
    {
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
                    CurrentPos, Renderer.DrawableImageFromName(t.Txt),
                    Renderer.DrawableImageFromName(t.Mask)),
                ImageSelection.PictureSelection p => GraphicElement.Pic(Renderer.DrawableImageFromName(p.Pic),
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