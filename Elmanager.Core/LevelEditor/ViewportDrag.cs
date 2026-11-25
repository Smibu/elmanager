using Elmanager.Geometry;

namespace Elmanager.LevelEditor;

public union ViewportDrag(ViewportDrag.PanningScreen, ViewportDrag.DraggingGrid)
{
    public record PanningScreen(Vector Start);

    public record DraggingGrid(Vector Start, Vector GridStartOffset);
}
