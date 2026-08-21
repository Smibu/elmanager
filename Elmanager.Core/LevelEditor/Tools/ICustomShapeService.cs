using System.Threading.Tasks;
using Elmanager.IO;
using Elmanager.LevelEditor.Shapes;
using Elmanager.Rendering;

namespace Elmanager.LevelEditor.Tools;

public interface ICustomShapeService
{
    Task<ElmaFileObject<SleShape>?> OpenShapeDialog(string? currentShapePath);
    Task<string?> SaveShape(ILevelEditor editor, ElmaRenderer renderer, string? lastUsedShapeFolder);
}
