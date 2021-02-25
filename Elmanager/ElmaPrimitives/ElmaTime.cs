using System;
using Elmanager.Utilities;

namespace Elmanager.ElmaPrimitives
{
    internal struct ElmaTime : IComparable
    {
        private readonly double _val;

        public ElmaTime(double value)
        {
            _val = value;
        }

        public static implicit operator ElmaTime(double value) => new(value);

        public static ElmaTime operator +(ElmaTime a, ElmaTime b)
        {
            return new(a._val + b._val);
        }

        public static ElmaTime operator -(ElmaTime a, ElmaTime b)
        {
            return new(a._val - b._val);
        }

        public static ElmaTime operator *(ElmaTime a, double b)
        {
            return new(a._val * b);
        }

        public static bool operator <(ElmaTime a, ElmaTime b)
        {
            return a._val < b._val;
        }

        public static bool operator >(ElmaTime a, ElmaTime b)
        {
            return a._val > b._val;
        }

        public double Value => _val;

        public override string ToString() => _val.ToTimeString(2);

        public int CompareTo(object obj) => _val.CompareTo(((ElmaTime)obj)._val);

        public static ElmaTime FromMilliSeconds(double stepM)
        {
            return new(stepM / TimeConst / 1000);
        }

        public double ToMilliSeconds()
        {
            return ToSeconds() * 1000;
        }

        public double ToSeconds()
        {
            return _val * TimeConst;
        }

        internal const double TimeConst = 625.0 / 273.0;
    }
}