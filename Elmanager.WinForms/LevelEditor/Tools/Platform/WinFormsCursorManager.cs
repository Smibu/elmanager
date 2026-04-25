using System.Windows.Forms;
using Elmanager.Application;
using Elmanager.LevelEditor.Input;

namespace Elmanager.LevelEditor.Tools.Platform;

internal class WinFormsCursorManager : IEditorCursorManager
{
    private readonly Control _editorControl;
    private readonly LevelEditorForm _levEditor;

    internal WinFormsCursorManager(Control editorControl, LevelEditorForm levEditor)
    {
        _editorControl = editorControl;
        _levEditor = levEditor;
    }

    public void ChangeCursorToHand()
    {
        if (Global.AppSettings.LevelEditor.UseHighlight)
            _editorControl.Cursor = Cursors.Hand;
    }

    public void ChangeToDefaultCursorIfHand()
    {
        if (_editorControl.Cursor == Cursors.Hand)
            _levEditor.ChangeToDefaultCursor();
    }
}
