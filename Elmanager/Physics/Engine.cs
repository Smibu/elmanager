using System.Collections.Generic;
using Elmanager.ElmaPrimitives;
using Elmanager.Geometry;
using Elmanager.Lev;

namespace Elmanager.Physics
{
    internal class Engine
    {
        public readonly HashSet<int> TakenApples = new();
        private Vector _startLocation;

        public Engine(List<Polygon> polys, List<LevObject> objects)
        {
            InitPolysAndObjects(polys, objects);
        }

        public void InitPolysAndObjects(List<Polygon> polys, List<LevObject> objects)
        {

        }

        public Driver init_driver()
        {
            TakenApples.Clear();
            return new(_startLocation);
        }

        public void next_frame(
            Driver driver,
            InputKeys keys,
            RideRecorder recorder,
            ElmaTime timeStep
        )
        {

        }

        public bool Invulnerable { get; set; }
    }
}