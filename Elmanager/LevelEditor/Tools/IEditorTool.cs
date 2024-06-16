using System.Windows.Forms;
using Elmanager.Geometry;

namespace Elmanager.LevelEditor.Tools;

internal interface IEditorTool : IEditorToolBase
{
    void MouseDown(MouseEventArgs mouseData);
    void MouseUp();
    void KeyDown(KeyEventArgs key);
    void MouseMove(Vector p);
    void MouseOutOfEditor();
    void ExtraRendering();
    TransientElements GetTransientElements() => TransientElements.Empty;
    void InActivate();
    void Activate();
    void UpdateHelp();
}