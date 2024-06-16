using System.Collections.Generic;
using Elmanager.Geometry;
using Elmanager.LevelEditor.Tools;

namespace Elmanager.Rendering;

internal class SceneSettings
{
    internal readonly HashSet<int> HiddenObjectIndices = new();
    internal HashSet<int> FadedObjectIndices = new();
    internal Vector GridOffset = new();
    internal bool PicturesInBackground;
    internal TransientElements TransientElements = TransientElements.Empty;
}