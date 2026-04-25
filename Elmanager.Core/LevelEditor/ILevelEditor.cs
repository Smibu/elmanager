using Elmanager.Lev;
using Elmanager.LevelEditor.Input;
using Elmanager.LevelEditor.Playing;
using Elmanager.LevelEditor.Tools;
using Elmanager.Rendering;
using Elmanager.Rendering.Camera;

namespace Elmanager.LevelEditor;

public interface ILevelEditor
{
    Level Lev { get; }
    ElmaRenderer Renderer { get; }
    ZoomController ZoomCtrl { get; }
    SceneSettings SceneSettings { get; }
    LevelEditorRenderingSettings RenderingSettings { get; }
    LevelEditorSettings Settings { get; }
    IEditorCursorManager CursorManager { get; }
    IKeyboardState KeyboardState { get; }
    ISelectionFilter SelectionFilter { get; }
    PlayController PlayController { get; }
    HighlightTarget? CurrentHighlight { get; set; }
    string HighlightText { get; set; }

    void ShowError(string message, string caption = "Error");
    void SetModified(LevModification value);
    void PreserveSelection();
    void UpdateSelectionInfo();
    void RedrawScene();
    void ChangeToSelectionTool();
    void TransformMenuItemClick();

    IPictureDialogService PictureDialogService { get; }
    ICustomShapeService CustomShapeService { get; }

    bool ObjectFramesVisible { get; }
    bool ObjectsVisible { get; }
    bool GrassEdgesVisible { get; }
    bool GrassVisible { get; }
    bool GroundEdgesVisible { get; }
    bool GroundVisible { get; }
    bool TextureFramesVisible { get; }
    bool TexturesVisible { get; }
    bool PictureFramesVisible { get; }
    bool PicturesVisible { get; }
}
