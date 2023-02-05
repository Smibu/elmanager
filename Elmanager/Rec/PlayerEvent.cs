namespace Elmanager.Rec;

internal class PlayerEvent<T>
{
    internal readonly int Info;
    internal readonly double Time;
    internal readonly T Type;

    internal PlayerEvent(T eventType, double eventTime, int info = 0)
    {
        Type = eventType;
        Time = eventTime;
        Info = info;
    }
}