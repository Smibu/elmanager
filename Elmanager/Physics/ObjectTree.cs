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
        var sortedObjects = objects.OrderBy(x => LevObject.ObjSortOrder(x.Type));
        foreach (var (i, obj) in sortedObjects.Select((obj, i) => (i, obj)))
        {
            if (obj.Type == ObjectType.Start)
            {
                continue;
            }
            _tree.Insert(EdgeTree.EnvelopeFromCircle(obj.Position, OpenGlLgr.ObjectRadius),
                new IndexedObject(i, obj));
        }
    }

    public IEnumerable<IndexedObject> GetCollidingObjects(Vector center, double radius)
    {
        return _tree.Query(EdgeTree.EnvelopeFromCircle(center, radius))
            .Where(io => io.Obj.Position.Dist(center) < OpenGlLgr.ObjectRadius + radius);
    }
}