using System.Windows.Forms;
using Elmanager.Geometry;
using Elmanager.Rendering;

namespace Elmanager.LevelEditor.Tools;

internal interface IEditorTool : IEditorToolBase
{
    LevVisualChange MouseDown(MouseEventArgs mouseData);
    void MouseUp();
    LevVisualChange KeyDown(KeyEventArgs key);
    LevVisualChange MouseMove(Vector p);
    LevVisualChange MouseOutOfEditor();

    void ExtraRendering();
    TransientElements GetTransientElements(bool hasFocus) => TransientElements.Empty;
    LevVisualChange InActivate();
    void Activate();
    string GetHelp();
}