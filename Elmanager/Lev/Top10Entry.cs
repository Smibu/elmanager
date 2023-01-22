namespace Elmanager.Lev;

internal abstract class Top10Entry
{
    public abstract string PlayerA { get; protected set; }
    public abstract string PlayerB { get; protected set; }
    public int Time { get; protected init; }

    public double TimeInSecs => Time / 100.0;
}