using Elmanager.Geometry;
using Elmanager.Rendering;

namespace Elmanager.LevelEditor.Tools;

public interface IPictureDialogService
{
    GraphicElement? ShowPictureDialog(
        ElmaRenderer renderer,
        Vector currentPos,
        GraphicElement? currentElem,
        bool setDefaultsAutomatically);
}
