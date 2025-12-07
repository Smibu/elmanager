using System.Collections.Generic;
using System.Linq;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.Rendering;
using NetTopologySuite.Index.Quadtree;

namespace Elmanager.Physics;

internal class ObjectTree
{
    private readonly Quadtree<IndexedObject> _tree = new();

    public ObjectTree(List<LevObject> objects)
    {
        for (var i = 0; i < objects.Count; i++)
        {
            if (objects[i].Type == ObjectType.Start)
            {
                continue;
            }
            _tree.Insert(EdgeTree.EnvelopeFromCircle(objects[i].Position, OpenGlLgr.ObjectRadius),
                new IndexedObject(i, objects[i]));
        }
    }

    public IEnumerable<IndexedObject> GetCollidingObjects(Vector center, double radius)
    {
        return _tree.Query(EdgeTree.EnvelopeFromCircle(center, radius))
            .Where(io => io.Obj.Position.Dist(center) < OpenGlLgr.ObjectRadius + radius);
    }
}