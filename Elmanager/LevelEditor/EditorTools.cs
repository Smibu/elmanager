using Elmanager.LevelEditor.Tools;

namespace Elmanager.LevelEditor;

internal record EditorTools(SelectionTool SelectionTool, VertexTool VertexTool, DrawTool DrawTool,
    ObjectTool ObjectTool, PipeTool PipeTool, EllipseTool EllipseTool, PolyOpTool PolyOpTool,
    FrameTool FrameTool, SmoothenTool SmoothenTool, CutConnectTool CutConnectTool, AutoGrassTool AutoGrassTool,
    TransformTool TransformTool, PictureTool PictureTool, TextTool TextTool, CustomShapeTool CustomShapeTool);
