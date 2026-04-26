using Elmanager.Geometry;
using Elmanager.LevelEditor.Input;
using Elmanager.Rendering;

namespace Elmanager.LevelEditor.Tools;

public interface IEditorTool : IEditorToolBase
{
    LevVisualChange MouseDown(EditorMouseEventArgs mouseData);
    void MouseUp();
    LevVisualChange KeyDown(EditorKeyEventArgs key);
    LevVisualChange MouseMove(Vector p);
    LevVisualChange MouseOutOfEditor();

    void ExtraRendering();
    TransientElements GetTransientElements(bool hasFocus) => TransientElements.Empty;
    LevVisualChange InActivate();
    void Activate();
    string GetHelp();
}
