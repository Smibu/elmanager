using System.Collections.Generic;

namespace Elmanager
{
    internal class SceneSettings
    {
        internal readonly HashSet<int> HiddenObjectIndices = new HashSet<int>();
        internal Vector GridOffset = new Vector();
        internal bool PicturesInBackground;
        internal List<Polygon> AdditionalPolys = new List<Polygon>();
    }
}