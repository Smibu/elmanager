using Elmanager.IO;

namespace Elmanager.LevelEditor.Shapes
{
    internal record ShapeSelection(ElmaFileObject<SleShape> Shape, SleShape Original);
}
