namespace Elmanager.LevelEditor.Input;

public interface IKeyboardState
{
    bool IsKeyDown(ModifierKey key);
    bool IsKeyDown(EditorKey key);
}
