using Elmanager.IO;

namespace Elmanager.LevelEditor.Shapes;

public record ShapeSelection(ElmaFileObject<SleShape> Shape, SleShape Original);
