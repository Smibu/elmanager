using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Elmanager
{
    internal class Player
    {
        internal const double TimeConst = 625.0 / 273.0;
        internal readonly int Apples; // Number of appletakes and bugapples
        internal readonly List<PlayerEvent<ReplayEventType>> RawEvents = new List<PlayerEvent<ReplayEventType>>();
        internal readonly List<PlayerEvent<LogicalEventType>> Events = new List<PlayerEvent<LogicalEventType>>();
        internal readonly int GroundTouches;
        internal readonly int LeftVolts;
        internal readonly int RightVolts;
        internal readonly int SuperVolts;
        internal readonly double Time;
        private double? _topSpeed;
        private double? _trip;
        internal readonly int Turns;
        private List<PlayerEvent<LogicalEventType>> _voltEvents;
        private const double ArmForwardTime = 0.2;
        private const double ArmRotationDelay = 0.916;
        private const double HeadDiff = 0.0915;
        private const double HeightConst = 0.632;
        private const double MaxArmRotation = 95;
        private const double WheelRotationFactor = 1 / 250.0;

        private readonly List<Vector> _globalBody = new List<Vector>();
        private readonly List<double> _bikeRotation = new List<double>();
        private readonly List<Direction> _direction = new List<Direction>();
        private readonly List<Vector> _head = new List<Vector>();
        private readonly List<Vector> _leftWheel = new List<Vector>();
        private readonly List<double> _leftWheelRotation = new List<double>();
        private readonly List<Vector> _rightWheel = new List<Vector>();
        private readonly List<double> _rightWheelRotation = new List<double>();

        internal int FrameCount => _globalBody.Count;

        internal Player(BinaryReader rec, int frames)
        {
            var frameCount = frames;

            for (var i = 0; i < frameCount; i++)
            {
                _globalBody.Add(new Vector(rec.ReadSingle(), 0));
            }

            foreach (var b in _globalBody)
            {
                b.Y = rec.ReadSingle();
            }

            foreach (var part in new[] {_leftWheel, _rightWheel, _head})
            {
                foreach (var b in _globalBody)
                {
                    part.Add(new Vector(b.X + rec.ReadInt16() / 1000.0, 0));
                }

                foreach (var (b, w) in _globalBody.Zip(part, (x, y) => (x, y)))
                {
                    w.Y = b.Y + rec.ReadInt16() / 1000.0;
                }
            }

            for (var i = 0; i < frameCount; i++)
            {
                _bikeRotation.Add(rec.ReadInt16() / 10000.0 * 2 * Math.PI);
            }

            foreach (var part in new[] {_leftWheelRotation, _rightWheelRotation})
            {
                for (var i = 0; i < frameCount; i++)
                {
                    part.Add(rec.ReadByte() * 2 * Math.PI * WheelRotationFactor);
                }
            }

            var gas = new List<bool>();
            for (var i = 0; i < frameCount; i++)
            {
                var dirData = rec.ReadByte();
                gas.Add(dirData % 2 == 1);
                _direction.Add(dirData % 4 < 2 ? Direction.Left : Direction.Right);
            }

            // Compute final head position
            for (var i = 0; i < frameCount; i++)
            {
                var dirf = 2 * (int) _direction[i] - 1; // 0 -> -1, 1 -> 1
                _head[i].X += Math.Cos(_bikeRotation[i] + Math.PI / 2) * HeightConst +
                              Math.Cos(_bikeRotation[i]) * HeadDiff * dirf;
                _head[i].Y += Math.Sin(_bikeRotation[i] + Math.PI / 2) * HeightConst +
                              Math.Sin(_bikeRotation[i]) * HeadDiff * dirf;
            }

            // Skip rotation speed and collision data - they're not needed for replay playing except for sounds.
            rec.BaseStream.Seek(frameCount * 2, SeekOrigin.Current);

            var eventCount = rec.ReadInt32();
            for (var j = 0; j < eventCount; j++)
            {
                var eventTime = rec.ReadDouble() * TimeConst;
                var info1 = rec.ReadByte();
                var info2 = rec.ReadByte();
                var eventType = (ReplayEventType) rec.ReadByte();
                rec.ReadByte();
                var info3 = rec.ReadSingle();
                RawEvents.Add(new PlayerEvent<ReplayEventType>(eventType, eventTime, info1));
            }

            if (rec.ReadInt32() != 0x492f75)
            {
                throw new Exception("Invalid magic in replay");
            }

            var objectIndexFreqs = new Dictionary<int, int>();
            var appleIndices = new HashSet<int>();

            foreach (var e in RawEvents)
            {
                if (e.Type == ReplayEventType.ObjectTouch)
                {
                    objectIndexFreqs.TryGetValue(e.Info, out var freq);
                    objectIndexFreqs[e.Info] = freq + 1;
                }
            }

            for (var index = 0; index < RawEvents.Count; index++)
            {
                var e = RawEvents[index];
                switch (e.Type)
                {
                    case ReplayEventType.ObjectTouch:
                        Events.Add(new PlayerEvent<LogicalEventType>(LogicalEventType.FlowerTouch, e.Time, e.Info));
                        break;
                    case ReplayEventType.AppleTake:
                        var consecutiveSimultaneousAppletakes = 1;
                        for (var i = index + 1; i < RawEvents.Count; i++)
                        {
                            if (RawEvents[i].Type != ReplayEventType.AppleTake)
                            {
                                break;
                            }

                            if (RawEvents[i].Time != e.Time)
                            {
                                throw new BadFileException("Expected same time");
                            }

                            consecutiveSimultaneousAppletakes++;
                        }

                        // We have to find the ObjectTouch event that corresponds to the AppleTake event (to get the apple index).
                        // That is not completely straightforward, as there may be interleaving flowertouch events
                        // (e.g. when the player finishes by taking the last apple while touching a flower).
                        // We use the objectIndexFreqs dictionary to determine which ObjectTouch events likely correspond to the flower touch,
                        // and skip those.
                        var skips = 0;
                        for (var i = 0; i < consecutiveSimultaneousAppletakes; i++)
                        {
                            var rawEvent = RawEvents[index - 1 - i - skips];
                            if (rawEvent.Type != ReplayEventType.ObjectTouch)
                            {
                                throw new BadFileException("Expected ObjectTouch");
                            }

                            objectIndexFreqs.TryGetValue(rawEvent.Info, out var f);
                            if (f > 2)
                            {
                                skips++;
                                i--;
                                continue;
                            }

                            appleIndices.Add(rawEvent.Info);
                            Apples++;
                            Events.Add(new PlayerEvent<LogicalEventType>(LogicalEventType.AppleTake, e.Time,
                                rawEvent.Info));
                        }

                        index += consecutiveSimultaneousAppletakes - 1;
                        break;
                    case ReplayEventType.RightVolt:
                        if (index + 1 < RawEvents.Count && RawEvents[index + 1].Type == ReplayEventType.LeftVolt &&
                            RawEvents[index + 1].Time == e.Time)
                        {
                            SuperVolts++;
                            index++;
                            Events.Add(new PlayerEvent<LogicalEventType>(LogicalEventType.SuperVolt, e.Time));
                        }
                        else
                        {
                            RightVolts++;
                            Events.Add(new PlayerEvent<LogicalEventType>(LogicalEventType.RightVolt, e.Time));
                        }

                        break;
                    case ReplayEventType.LeftVolt:
                        LeftVolts++;
                        Events.Add(new PlayerEvent<LogicalEventType>(LogicalEventType.LeftVolt, e.Time));
                        break;
                    case ReplayEventType.Turn:
                        Turns++;
                        Events.Add(new PlayerEvent<LogicalEventType>(LogicalEventType.Turn, e.Time));
                        break;
                    case ReplayEventType.GroundTouch:
                        GroundTouches++;
                        Events.Add(new PlayerEvent<LogicalEventType>(LogicalEventType.GroundTouch, e.Time));
                        break;
                }
            }

            Events.RemoveAll(e => e.Type == LogicalEventType.FlowerTouch && appleIndices.Contains(e.Info));

            // Try to detect whether the player finished or not.
            if (Events.Count > 0)
            {
                var last = Events.Last();

                var lastSameTimeFlowerEvents = 0;
                var lastSameTimeAppleEvents = 0;
                var flowerWaitAtEnd = false;
                for (var i = Events.Count - 1; i >= 0; i--)
                {
                    var curr = Events[i];
                    if (curr.Time == last.Time)
                    {
                        switch (curr.Type)
                        {
                            case LogicalEventType.FlowerTouch:
                                lastSameTimeFlowerEvents++;
                                break;
                            case LogicalEventType.AppleTake:
                                lastSameTimeAppleEvents++;
                                break;
                        }
                    }
                    else
                    {
                        flowerWaitAtEnd = curr.Type == LogicalEventType.FlowerTouch;
                        break;
                    }
                }

                if (lastSameTimeFlowerEvents > 0 && (lastSameTimeAppleEvents > 0 || !flowerWaitAtEnd))
                {
                    Events.Add(new PlayerEvent<LogicalEventType>(LogicalEventType.Finish, last.Time));
                }

                Time = Math.Floor(last.Time * 1000) / 1000;
            }

            if ((!Finished && !FakeFinish) || Time == 0)
                Time = Math.Round(frameCount / 30.0, 3);
            
            // calculating gas events
            bool gasOn = false;
            for (int i = 0; i < frameCount; i++)
            {
                if (gas[i] == gasOn)
                    continue;
                
                gasOn = gas[i];
                var eventTime = Math.Round(i / 30.0, 3);
                if (eventTime >= Time)
                    break;
                
                Events.Add(new PlayerEvent<LogicalEventType>(gasOn ? LogicalEventType.GasOn : LogicalEventType.GasOff, eventTime));
            }
            
            Events.Sort((a, b) => a.Time.CompareTo(b.Time));
        }

        private void ComputeTripAndTopSpeed()
        {
            var trip = 0.0;
            var top = 0.0;
            for (var i = 0; i < _globalBody.Count - 2; i++)
            {
                var curr = _globalBody[i].Dist(_globalBody[i + 1]);
                trip += curr;
                top = Math.Max(top, curr);
            }

            _trip = trip;
            _topSpeed = top * Constants.SpeedConst;
        }

        internal bool Finished => Events.Count > 0 && Events.Last().Type == LogicalEventType.Finish;
        internal bool FakeFinish => Events.Count > 0 && Events.Last().Type == LogicalEventType.FlowerTouch;
        internal bool IsLastEventApple => Events.Count > 0 && Events.Last().Type == LogicalEventType.AppleTake;

        private double GetArmRotation(double currentTime)
        {
            if (_voltEvents == null)
            {
                _voltEvents = GetEvents(LogicalEventType.LeftVolt, LogicalEventType.RightVolt,
                    LogicalEventType.SuperVolt);
            }

            var upperIndex = _voltEvents.Count;
            var lowerIndex = 0;
            var lastIndex = -1;
            while (lowerIndex != upperIndex)
            {
                var currIndex = (lowerIndex + upperIndex) / 2;
                var currTime = _voltEvents[currIndex].Time;
                var difference = currentTime - currTime;
                if (difference > 0 && difference < ArmRotationDelay)
                {
                    if (difference < ArmForwardTime)
                    {
                        if (_voltEvents[currIndex].Type == LogicalEventType.RightVolt)
                            return MaxArmRotation * difference / ArmForwardTime;
                        return -MaxArmRotation * difference / ArmForwardTime;
                    }

                    if (_voltEvents[currIndex].Type == LogicalEventType.RightVolt)
                        return (MaxArmRotation -
                                MaxArmRotation * (difference - ArmForwardTime) /
                                (ArmRotationDelay - ArmForwardTime));
                    return
                        -(MaxArmRotation -
                          MaxArmRotation * (difference - ArmForwardTime) /
                          (ArmRotationDelay - ArmForwardTime));
                }

                if (currTime < currentTime)
                {
                    lowerIndex = currIndex;
                }
                else
                {
                    upperIndex = currIndex;
                }

                if (lastIndex == currIndex)
                    lowerIndex++;
                lastIndex = currIndex;
            }

            return 0.0;
        }

        internal PlayerState GetInterpolatedState(double time)
        {
            var currIndex = (int)Math.Floor(time * 30);
            var step = time * 30 - currIndex;
            var maxFrameIndex = FrameCount - 1;
            var i1 = Math.Min(currIndex, maxFrameIndex);
            var i2 = Math.Min(currIndex + 1, maxFrameIndex);
            var head1 = _head[i1];
            var head2 = _head[i2];
            var lWheelRotate1 = _leftWheelRotation[i1];
            var lWheelRotate2 = _leftWheelRotation[i2];
            if (lWheelRotate2 - lWheelRotate1 > 4 / 3.0 * Math.PI)
                lWheelRotate2 -= 2 * Math.PI;
            if (lWheelRotate1 - lWheelRotate2 > 4 / 3.0 * Math.PI)
                lWheelRotate1 -= 2 * Math.PI;
            var rWheelRotate1 = _rightWheelRotation[i1];
            var rWheelRotate2 = _rightWheelRotation[i2];
            if (rWheelRotate2 - rWheelRotate1 > 4 / 3.0 * Math.PI)
                rWheelRotate2 -= 2 * Math.PI;
            if (rWheelRotate1 - rWheelRotate2 > 4 / 3.0 * Math.PI)
                rWheelRotate1 -= 2 * Math.PI;
            var bikeRotation1 = _bikeRotation[i1];
            var bikeRotation2 = _bikeRotation[i2];
            if (bikeRotation2 - bikeRotation1 > 5 * Math.PI / 3)
                bikeRotation2 -= 2 * Math.PI + bikeRotation1;
            if (bikeRotation1 - bikeRotation2 > 5 * Math.PI / 3)
                bikeRotation1 -= 2 * Math.PI + bikeRotation2;
            var leftwheel1 = _leftWheel[i1];
            var leftwheel2 = _leftWheel[i2];
            var rightwheel1 = _rightWheel[i1];
            var rightwheel2 = _rightWheel[i2];
            var global1 = _globalBody[i1];
            var global2 = _globalBody[i2];
            return new PlayerState(
                Interpolate(global1.X, global2.X, step), Interpolate(global1.Y, global2.Y, step),
                Interpolate(leftwheel1.X, leftwheel2.X, step), Interpolate(leftwheel1.Y, leftwheel2.Y, step),
                Interpolate(rightwheel1.X, rightwheel2.X, step), Interpolate(rightwheel1.Y, rightwheel2.Y, step),
                Interpolate(lWheelRotate1, lWheelRotate2, step), Interpolate(rWheelRotate1, rWheelRotate2, step),
                Interpolate(head1.X, head2.X, step), Interpolate(head1.Y, head2.Y, step),
                Interpolate(bikeRotation1, bikeRotation2, step) / (2 * Math.PI) * 360, _direction[i1],
                GetArmRotation(time));
        }

        internal double TopSpeed
        {
            get
            {
                if (_topSpeed == null)
                {
                    ComputeTripAndTopSpeed();
                }

                Debug.Assert(_topSpeed != null);
                return _topSpeed.Value;
            }
        }

        internal double Trip
        {
            get
            {
                if (_trip == null)
                {
                    ComputeTripAndTopSpeed();
                }

                Debug.Assert(_trip != null);
                return _trip.Value;
            }
        }

        internal double[] GetEventTimes(params LogicalEventType[] eventTypes)
        {
            return (from x in Events
                where eventTypes.Contains(x.Type)
                select x.Time).ToArray();
        }

        internal List<PlayerEvent<LogicalEventType>> GetEvents(params LogicalEventType[] eventTypes)
        {
            return Events.Where(x => eventTypes.Contains(x.Type)).ToList();
        }

        internal List<Vector> GlobalBody => _globalBody;

        private static double Interpolate(double firstValue, double secondValue, double step)
        {
            return firstValue + (secondValue - firstValue) * step;
        }
    }
}