using Elmanager.LevelEditor.Tools;

namespace Elmanager.LevelEditor;

internal record WinFormsEditorTools(SelectionTool SelectionTool, VertexTool VertexTool, DrawTool DrawTool,
    ObjectTool ObjectTool, PipeTool PipeTool, EllipseTool EllipseTool, PolyOpTool PolyOpTool,
    FrameTool FrameTool, SmoothenTool SmoothenTool, CutConnectTool CutConnectTool, AutoGrassTool AutoGrassTool,
    TransformTool TransformTool, PictureTool PictureTool, TextTool TextTool, CustomShapeTool CustomShapeTool)
    : EditorTools(SelectionTool, VertexTool, DrawTool, ObjectTool, PipeTool, EllipseTool, PolyOpTool,
        FrameTool, SmoothenTool, CutConnectTool, AutoGrassTool, TransformTool, PictureTool);
