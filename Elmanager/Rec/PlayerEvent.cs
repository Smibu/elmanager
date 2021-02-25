namespace Elmanager.Rec
{
    internal class PlayerEvent<T>
    {
        internal int Info;
        internal double Time;
        internal T Type;

        internal PlayerEvent(T eventType, double eventTime, int info = 0)
        {
            Type = eventType;
            Time = eventTime;
            Info = info;
        }
    }
}