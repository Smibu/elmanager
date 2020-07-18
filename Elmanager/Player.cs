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
        internal int Apples; // Number of appletakes and bugapples
        internal List<PlayerEvent<ReplayEventType>> RawEvents = new List<PlayerEvent<ReplayEventType>>();
        internal List<PlayerEvent<LogicalEventType>> Events = new List<PlayerEvent<LogicalEventType>>();
        internal int GroundTouches;
        internal int LeftVolts;
        internal int RightVolts;
        internal int SuperVolts;
        internal double Time;
        private double? _topSpeed;
        private double? _trip;
        internal int Turns;
        private PlayerEvent<LogicalEventType>[] _voltEvents;
        private const double ArmForwardTime = 0.2;
        private const double ArmRotationDelay = 0.916;
        private const double HeadDiff = 0.0915;
        private const double HeightConst = 0.632;
        private const double MaxArmRotation = 95;
        private const double WheelRotationFactor = 1 / 250.0;

        private List<Vector> _globalBody = new List<Vector>();
        private List<double> _bikeRotation = new List<double>();
        private List<Direction> _direction = new List<Direction>();
        private List<Vector> _head = new List<Vector>();
        private List<Vector> _leftWheel = new List<Vector>();
        private List<double> _leftWheelRotation = new List<double>();
        private List<Vector> _rightWheel = new List<Vector>();
        private List<double> _rightWheelRotation = new List<double>();
        private int _currentFrameIndex;
        private double _currentTime;
        private double _interpolationStep;

        internal int FrameCount => _globalBody.Count;
        private int MaxFrameIndex => FrameCount - 1;

        internal Player(BinaryReader rec, int frames)
        {
            var frameCount = frames;

            for (int i = 0; i < frameCount; i++)
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

            for (int i = 0; i < frameCount; i++)
            {
                _bikeRotation.Add(rec.ReadInt16() / 10000.0 * 2 * Math.PI);
            }

            foreach (var part in new[] {_leftWheelRotation, _rightWheelRotation})
            {
                for (int i = 0; i < frameCount; i++)
                {
                    part.Add(rec.ReadByte() * 2 * Math.PI * WheelRotationFactor);
                }
            }

            for (int i = 0; i < frameCount; i++)
            {
                var dirData = rec.ReadByte();
                _direction.Add(dirData % 4 < 2 ? Direction.Left : Direction.Right);
            }

            // Compute final head position
            for (int i = 0; i < frameCount; i++)
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
            for (int j = 0; j < eventCount; j++)
            {
                double eventTime = rec.ReadDouble() * TimeConst;
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
                        int skips = 0;
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
                for (int i = Events.Count - 1; i >= 0; i--)
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
        }

        private void ComputeTripAndTopSpeed()
        {
            var trip = 0.0;
            var top = 0.0;
            for (int i = 0; i < _globalBody.Count - 2; i++)
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

        internal double ArmRotation
        {
            get
            {
                if (_voltEvents == null)
                {
                    _voltEvents = GetEvents(LogicalEventType.LeftVolt, LogicalEventType.RightVolt,
                        LogicalEventType.SuperVolt);
                }

                int upperIndex = _voltEvents.Length;
                int lowerIndex = 0;
                int lastIndex = -1;
                while (lowerIndex != upperIndex)
                {
                    int currIndex = (lowerIndex + upperIndex) / 2;
                    double currTime = _voltEvents[currIndex].Time;
                    double difference = CurrentTime - currTime;
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

                    if (currTime < CurrentTime)
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
        }

        internal double BikeRotation
        {
            get
            {
                double bikeRotationBegin = _bikeRotation[FirstInterpolationIndex];
                double bikeRotationEnd = _bikeRotation[SecondInterpolationIndex];
                if (bikeRotationEnd - bikeRotationBegin > 5 * Math.PI / 3)
                    bikeRotationEnd -= 2 * Math.PI + bikeRotationBegin;
                if (bikeRotationBegin - bikeRotationEnd > 5 * Math.PI / 3)
                    bikeRotationBegin -= 2 * Math.PI + bikeRotationEnd;
                return Interpolate(bikeRotationBegin, bikeRotationEnd);
            }
        }

        internal double BikeRotationDegrees => BikeRotation / (2 * Math.PI) * 360;

        internal double CurrentTime
        {
            set
            {
                _currentFrameIndex = (int) Math.Floor(value * 30);
                _interpolationStep = value * 30 - _currentFrameIndex;
                _currentTime = value;
            }
            get => _currentTime;
        }

        internal Direction Dir => _direction[FirstInterpolationIndex];

        internal double GlobalBodyX =>
            Interpolate(_globalBody[FirstInterpolationIndex].X, _globalBody[SecondInterpolationIndex].X);

        internal double GlobalBodyY =>
            Interpolate(_globalBody[FirstInterpolationIndex].Y, _globalBody[SecondInterpolationIndex].Y);

        internal double HeadX => Interpolate(_head[FirstInterpolationIndex].X, _head[SecondInterpolationIndex].X);

        internal double HeadY => Interpolate(_head[FirstInterpolationIndex].Y, _head[SecondInterpolationIndex].Y);

        internal double LeftWheelRotation
        {
            get
            {
                double lWheelRotateBegin = _leftWheelRotation[FirstInterpolationIndex];
                double lWheelRotateEnd = _leftWheelRotation[SecondInterpolationIndex];
                if (lWheelRotateEnd - lWheelRotateBegin > 4 / 3.0 * Math.PI)
                    lWheelRotateEnd -= 2 * Math.PI;
                if (lWheelRotateBegin - lWheelRotateEnd > 4 / 3.0 * Math.PI)
                    lWheelRotateBegin -= 2 * Math.PI;
                return Interpolate(lWheelRotateBegin, lWheelRotateEnd);
            }
        }

        internal double LeftWheelX =>
            Interpolate(_leftWheel[FirstInterpolationIndex].X, _leftWheel[SecondInterpolationIndex].X);

        internal double LeftWheelY =>
            Interpolate(_leftWheel[FirstInterpolationIndex].Y, _leftWheel[SecondInterpolationIndex].Y);

        internal double RightWheelRotation
        {
            get
            {
                double rWheelRotateBegin = _rightWheelRotation[FirstInterpolationIndex];
                double rWheelRotateEnd = _rightWheelRotation[SecondInterpolationIndex];
                if (rWheelRotateEnd - rWheelRotateBegin > 4 / 3.0 * Math.PI)
                    rWheelRotateEnd -= 2 * Math.PI;
                if (rWheelRotateBegin - rWheelRotateEnd > 4 / 3.0 * Math.PI)
                    rWheelRotateBegin -= 2 * Math.PI;
                return Interpolate(rWheelRotateBegin, rWheelRotateEnd);
            }
        }

        internal double RightWheelX =>
            Interpolate(_rightWheel[FirstInterpolationIndex].X, _rightWheel[SecondInterpolationIndex].X);

        internal double RightWheelY =>
            Interpolate(_rightWheel[FirstInterpolationIndex].Y, _rightWheel[SecondInterpolationIndex].Y);

        internal double Speed
        {
            get
            {
                if (_currentFrameIndex == 0 || _currentFrameIndex > MaxFrameIndex)
                    return 0.0;
                return
                    Math.Sqrt(Math.Pow((_globalBody[_currentFrameIndex].X - _globalBody[_currentFrameIndex - 1].X), 2) +
                              Math.Pow((_globalBody[_currentFrameIndex].Y - _globalBody[_currentFrameIndex - 1].Y),
                                  2)) *
                    Constants.SpeedConst;
            }
        }

        private int FirstInterpolationIndex =>
            _currentFrameIndex > MaxFrameIndex ? MaxFrameIndex : _currentFrameIndex;

        private int SecondInterpolationIndex
        {
            get
            {
                if (_currentFrameIndex < MaxFrameIndex)
                    return _currentFrameIndex + 1;
                return FirstInterpolationIndex;
            }
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

        internal PlayerEvent<LogicalEventType>[] GetEvents(params LogicalEventType[] eventTypes)
        {
            return Events.Where(x => eventTypes.Contains(x.Type)).ToArray();
        }

        internal Vector GlobalBodyFromIndex(int index)
        {
            return _globalBody[index];
        }

        private double Interpolate(double firstValue, double secondValue)
        {
            return firstValue + (secondValue - firstValue) * _interpolationStep;
        }
    }
}