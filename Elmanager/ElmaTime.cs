using System;

namespace Elmanager
{
    internal struct ElmaTime : IComparable
    {
        private readonly double _val;

        private ElmaTime(double value)
        {
            _val = value;
        }

        public static implicit operator ElmaTime(double value) => new ElmaTime(value);

        public override string ToString() => _val.ToTimeString(2);

        public int CompareTo(object obj) => _val.CompareTo(((ElmaTime)obj)._val);
    }
}