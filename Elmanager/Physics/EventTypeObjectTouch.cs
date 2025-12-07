namespace Elmanager.Physics;

internal class EventTypeObjectTouch : EventType
{
    public int ObjIndex;
    public EventTypeObjectTouch(int objIndex)
    {
        this.ObjIndex = objIndex;
    }
}