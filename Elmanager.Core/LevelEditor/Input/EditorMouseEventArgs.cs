namespace Elmanager.LevelEditor.Input;

public class EditorMouseEventArgs
{
    public EditorMouseButton Button { get; }

    public EditorMouseEventArgs(EditorMouseButton button)
    {
        Button = button;
    }
}
