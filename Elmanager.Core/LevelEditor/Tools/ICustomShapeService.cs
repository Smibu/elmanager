using Elmanager.IO;
using Elmanager.LevelEditor.Shapes;
using Elmanager.Rendering;

namespace Elmanager.LevelEditor.Tools;

public interface ICustomShapeService
{
    ElmaFileObject<SleShape>? OpenShapeDialog(string? currentShapePath);
    string? SaveShape(ILevelEditor editor, ElmaRenderer renderer, string? lastUsedShapeFolder);
}
