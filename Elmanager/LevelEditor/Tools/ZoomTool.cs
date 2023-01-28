using System.Collections.Generic;
using System.Windows.Forms;
using Elmanager.Application;
using Elmanager.Geometry;
using Elmanager.Lev;

namespace Elmanager.LevelEditor.Tools;

internal class ZoomTool : ToolBase, IEditorTool
{
    internal ZoomTool(LevelEditorForm editor) : base(editor)
    {
    }

    public void Activate()
    {
        UpdateHelp();
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
    }

    public void KeyDown(KeyEventArgs key)
    {
    }

    public void MouseDown(MouseEventArgs mouseData)
    {
        CurrentPos.Y *= -1.0;
        switch (mouseData.Button)
        {
            case MouseButtons.Left:
            case MouseButtons.Right:
                ZoomCtrl.Zoom(CurrentPos, mouseData.Button == MouseButtons.Left,
                    1 - Global.AppSettings.LevelEditor.MouseClickStep / 100.0);
                break;
        }
    }

    public void MouseMove(Vector p)
    {
        CurrentPos = p;
    }

    public void MouseOutOfEditor()
    {
    }

    public void MouseUp()
    {
    }

    public void UpdateHelp()
    {
        LevEditor.InfoLabel.Text = "LMouse: zoom in; RMouse: zoom out.";
    }

    public override bool Busy => false;
}