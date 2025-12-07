using Elmanager.Lev;

namespace Elmanager.Physics;

internal class IndexedObject
{
    public int Index;
    public LevObject Obj;

    public IndexedObject(int i, LevObject levObject)
    {
        Index = i;
        Obj = levObject;
    }
}