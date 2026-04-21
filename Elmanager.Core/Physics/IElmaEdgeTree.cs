using System.Collections.Generic;
using Elmanager.Geometry;

namespace Elmanager.Physics;

public interface IElmaEdgeTree
{
    (Vector?, Vector?) GetTouchingEdges(Vector location, double radius);
    void Init(IEnumerable<Edge> edges, double radius);
}
