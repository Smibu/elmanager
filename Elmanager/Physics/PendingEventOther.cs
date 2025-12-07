namespace Elmanager.Physics;

internal class PendingEventOther : PendingEvent
{
    public EventType EventType;

    public PendingEventOther(EventType eventType)
    {
        this.EventType = eventType;
    }
}