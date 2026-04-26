using Elmanager.LevelEditor.Tools;

namespace Elmanager.LevelEditor;

public record EditorTools(SelectionTool SelectionTool, VertexTool VertexTool, DrawTool DrawTool,
    ObjectTool ObjectTool, PipeTool PipeTool, EllipseTool EllipseTool, PolyOpTool PolyOpTool,
    FrameTool FrameTool, SmoothenTool SmoothenTool, CutConnectTool CutConnectTool, AutoGrassTool AutoGrassTool,
    TransformTool TransformTool, PictureTool PictureTool);
