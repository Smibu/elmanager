namespace Elmanager.Rec;

public class PlayerEvent<T>
{
    public readonly int Info;
    public readonly double Time;
    public readonly T Type;

    public PlayerEvent(T eventType, double eventTime, int info = 0)
    {
        Type = eventType;
        Time = eventTime;
        Info = info;
    }
}
