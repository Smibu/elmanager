namespace Elmanager.Physics;

internal class PendingEventObject : PendingEvent
{
    public IndexedObject Obj;

    public PendingEventObject(IndexedObject obj)
    {
        this.Obj = obj;
    }
}