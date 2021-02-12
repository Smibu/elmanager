using System.Collections.Generic;

namespace Elmanager
{
    internal class SceneSettings
    {
        internal readonly HashSet<int> HiddenObjectIndices = new();
        internal HashSet<int> FadedObjectIndices = new();
        internal Vector GridOffset = new();
        internal bool PicturesInBackground;
        internal List<Polygon> AdditionalPolys = new();
    }
}