using System.ComponentModel;
using Elmanager.ElmaPrimitives;

namespace Elmanager.Lev
{
    internal abstract class Top10Entry
    {
        public abstract string PlayerA { get; protected set; }
        public abstract string PlayerB { get; protected set; }
        internal int Time;

        internal void Init(string playerA, string playerB, int time)
        {
            PlayerA = playerA;
            PlayerB = playerB;
            Time = time;
        }

        public double TimeInSecs => Time / 100.0;

        [Description("Time")] public ElmaTime TimeA => TimeInSecs;

        public abstract string FormatEntry(int pad);
    }
}