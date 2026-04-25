namespace Elmanager.LevelEditor.Input;

public class EditorKeyEventArgs
{
    public EditorKey KeyCode { get; }

    public EditorKeyEventArgs(EditorKey keyCode)
    {
        KeyCode = keyCode;
    }
}
