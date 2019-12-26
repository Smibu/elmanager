using System;
using System.Collections.Generic;
using System.Linq;

namespace Elmanager
{
    internal class Player
    {
        internal const double TimeConst = 625.0 / 273.0;
        internal int Apples; //Number of appletakes and bugapples
        internal int BugApples;
        internal int EventCount; //Number of events (appletake = 2 events)
        internal List<PlayerEvent> Events;
        internal bool FakeFinish;
        internal bool Finished;
        internal int FrameCount;
        internal int GroundTouches;
        internal bool IsLastEventApple;
        internal int LeftVolts;
        internal int RightVolts;
        internal int SuperVolts;
        internal double Time;
        internal double TopSpeed;
        internal double Trip;
        internal int Turns;
        internal PlayerEvent[] VoltEvents;
        private const double ArmForwardTime = 0.2;
        private const double ArmRotationDelay = 0.916;
        private const int EventSize = 16;
        private const double HeadDiff = 0.0915;
        private const double HeightConst = 0.632;
        private const double MaxArmRotation = 95;
        private const double WheelRotationFactor = 1 / 250.0;
        private readonly int _pOffset;
        private readonly byte[] _rawData;

        private double[] _bikeRotation;
        private int _currentFrameIndex;
        private double _currentTime;
        private Direction[] _direction;
        private bool _frameDataInitialized;
        private Vector[] _globalBody;
        private Vector[] _head;
        private double _interpolationStep;
        private Vector[] _leftWheel;
        private double[] _leftWheelRotation;
        private int _maxFrameIndex;
        private Vector[] _rightWheel;
        private double[] _rightWheelRotation;
        private bool _playingInitialized;

        internal Player(byte[] rec, bool p2)
        {
            _rawData = rec;
            int consecutiveSimultaneousAppletakes = 0;
            FrameCount = BitConverter.ToInt32(rec, 0);
            EventCount = BitConverter.ToInt32(rec, FrameCount * 27 + 36);
            if (p2)
            {
                _pOffset = EventCount * EventSize + FrameCount * 27 + 44;
                FrameCount = BitConverter.ToInt32(rec, _pOffset);
                EventCount = BitConverter.ToInt32(rec, _pOffset + FrameCount * 27 + 36);
            }

            Events = new List<PlayerEvent>(EventCount);
            if (EventCount > 0)
            {
                int sp = _pOffset + 50 + FrameCount * 27;
                for (int j = 0; j < EventCount; j++)
                {
                    double eventTime = BitConverter.ToDouble(rec, sp + EventSize * j - 10) * TimeConst;
                    bool lastEventSuperVolt = false;
                    bool someOtherEventAdded = true;
                    switch (rec[sp + EventSize * j])
                    {
                        case (byte) ReplayEventType.AppleTake:
                            consecutiveSimultaneousAppletakes++;
                            someOtherEventAdded = false;
                            if (j < EventCount - 1)
                                continue;
                            j++;
                            break;
                        case (byte) ReplayEventType.RightVolt:
                            if (rec.Length >= sp + EventSize * j + EventSize &&
                                (BitConverter.ToDouble(rec, sp + EventSize * j + 6) * TimeConst == eventTime &&
                                 rec[sp + EventSize * j + EventSize] == 7))
                            {
                                SuperVolts++;
                                lastEventSuperVolt = true;
                                Events.Add(new PlayerEvent(ReplayEventType.SuperVolt, eventTime));
                            }
                            else
                            {
                                RightVolts++;
                                Events.Add(new PlayerEvent(ReplayEventType.RightVolt, eventTime));
                            }

                            break;
                        case (byte) ReplayEventType.LeftVolt:
                            LeftVolts++;
                            Events.Add(new PlayerEvent(ReplayEventType.LeftVolt, eventTime));
                            break;
                        case (byte) ReplayEventType.Turn:
                            Turns++;
                            Events.Add(new PlayerEvent(ReplayEventType.Turn, eventTime));
                            break;
                        case (byte) ReplayEventType.GroundTouch:
                            GroundTouches++;
                            Events.Add(new PlayerEvent(ReplayEventType.GroundTouch, eventTime));
                            break;
                        default:
                            someOtherEventAdded = false;
                            break;
                    }

                    if (consecutiveSimultaneousAppletakes > 0)
                    {
                        double appleTime = BitConverter.ToDouble(rec, sp + EventSize * j - 26);
                        int k = 0;
                        while (
                            !(BitConverter.ToDouble(rec, sp + EventSize * j - 26 - EventSize * k) != appleTime ||
                              (rec[sp + EventSize * j - EventSize - EventSize * k] != 4 &&
                               rec[sp + EventSize * j - EventSize - EventSize * k] != 0)))
                            k++;
                        if (k > consecutiveSimultaneousAppletakes * 2 && j > 2)
                            //If j <= 2 then this appletake is first event, which means k must be 0
                        {
                            int biggestIndex = 0;
                            for (int i = 0; i < k - consecutiveSimultaneousAppletakes; i++)
                                if (rec[sp + EventSize * j - EventSize * k + i * EventSize - 2] > biggestIndex)
                                    biggestIndex = rec[sp + EventSize * j - EventSize * k + i * EventSize - 2];
                            k = 0;
                            while (rec[sp + EventSize * j - EventSize * k - 34] == biggestIndex)
                                k++;
                        }
                        else
                            k = 0;

                        int lastAppleIndex = -1;
                        for (int i = 0; i < consecutiveSimultaneousAppletakes; i++)
                        {
                            int currentAppleIndex =
                                rec[sp + EventSize * j - 18 - EventSize * (consecutiveSimultaneousAppletakes + i + k)];
                            if (currentAppleIndex != lastAppleIndex)
                            {
                                Apples++;
                                if (Events.Count > 0 && someOtherEventAdded)
                                    Events.Insert(Events.Count - 1,
                                        new PlayerEvent(ReplayEventType.AppleTake, appleTime * TimeConst,
                                            currentAppleIndex));
                                else
                                    Events.Add(new PlayerEvent(ReplayEventType.AppleTake, appleTime * TimeConst,
                                        currentAppleIndex));
                            }
                            else
                                BugApples++;

                            lastAppleIndex = currentAppleIndex;
                        }
                    }

                    consecutiveSimultaneousAppletakes = 0;
                    if (lastEventSuperVolt)
                        j++;
                }

                //Check if player finished
                sp = _pOffset + 27 * FrameCount + 24 + EventSize * EventCount; //Points to start of last event
                if (rec[sp + 10] != 0)
                {
                    //Check if the last event was appletaking. If so, check if the second-last event is finish and its time is same as appletake's
                    if (rec[sp + 10] == (byte) ReplayEventType.AppleTake)
                    {
                        IsLastEventApple = true;
                        if (rec[sp - 22] == 0 && EventCount > 2)
                            //There must be at least 3 events for this (appletake = 2 events)
                        {
                            Finished = BitConverter.ToDouble(rec, sp - 32) == BitConverter.ToDouble(rec, sp);
                            Time = BitConverter.ToDouble(rec, sp - 32) * TimeConst;
                            Time = Math.Floor(Time * 1000) / 1000;
                        }
                    }
                }
                else
                {
                    //Make sure the second-last event isn't finishing event too
                    if (rec[sp - 6] == 0 &&
                        BitConverter.ToDouble(rec, sp) != BitConverter.ToDouble(rec, sp - EventSize) &&
                        EventCount > 1) //Real finish can't have two finish events

                        FakeFinish = true; //If the finishevents' times are the same, player finished with two wheels
                    else
                        Finished = true;
                    Time = BitConverter.ToDouble(rec, sp) * TimeConst;
                    Time = Math.Floor(Time * 1000) / 1000;
                }
            }

            if ((!Finished && !FakeFinish) || Time == 0)
                Time = Math.Round(FrameCount / 30.0, 3);
            TopSpeed = 0;
            Trip = 0;
        }

        internal double ArmRotation
        {
            get
            {
                int upperIndex = VoltEvents.Length;
                int lowerIndex = 0;
                int lastIndex = -1;
                while (lowerIndex != upperIndex)
                {
                    int currIndex = (lowerIndex + upperIndex) / 2;
                    double currTime = VoltEvents[currIndex].Time;
                    double difference = CurrentTime - currTime;
                    if (difference > 0 && difference < ArmRotationDelay)
                    {
                        if (difference < ArmForwardTime)
                        {
                            if (VoltEvents[currIndex].Type == ReplayEventType.RightVolt)
                                return MaxArmRotation * difference / ArmForwardTime;
                            return -MaxArmRotation * difference / ArmForwardTime;
                        }

                        if (VoltEvents[currIndex].Type == ReplayEventType.RightVolt)
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
                if (bikeRotationEnd - bikeRotationBegin > 300)
                    bikeRotationEnd -= 360 + bikeRotationBegin;
                if (bikeRotationBegin - bikeRotationEnd > 300)
                    bikeRotationBegin -= 360 + bikeRotationEnd;
                return Interpolate(bikeRotationBegin, bikeRotationEnd);
            }
        }

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
                if (lWheelRotateEnd - lWheelRotateBegin > 4 / 3 * Math.PI)
                    lWheelRotateEnd -= 2 * Math.PI + lWheelRotateBegin;
                if (lWheelRotateBegin - lWheelRotateEnd > 4 / 3 * Math.PI)
                    lWheelRotateBegin -= 2 * Math.PI + lWheelRotateEnd;
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
                if (rWheelRotateEnd - rWheelRotateBegin > 4 / 3 * Math.PI)
                    rWheelRotateEnd -= 2 * Math.PI + rWheelRotateBegin;
                if (rWheelRotateBegin - rWheelRotateEnd > 4 / 3 * Math.PI)
                    rWheelRotateBegin -= 2 * Math.PI + rWheelRotateEnd;
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
                if (_currentFrameIndex == 0 || _currentFrameIndex > _maxFrameIndex)
                    return 0.0;
                return
                    Math.Sqrt(Math.Pow((_globalBody[_currentFrameIndex].X - _globalBody[_currentFrameIndex - 1].X), 2) +
                              Math.Pow((_globalBody[_currentFrameIndex].Y - _globalBody[_currentFrameIndex - 1].Y),
                                  2)) *
                    Constants.SpeedConst;
            }
        }

        private int FirstInterpolationIndex =>
            _currentFrameIndex > _maxFrameIndex ? _maxFrameIndex : _currentFrameIndex;

        private int SecondInterpolationIndex
        {
            get
            {
                if (_currentFrameIndex < _maxFrameIndex)
                    return _currentFrameIndex + 1;
                return FirstInterpolationIndex;
            }
        }

        internal double[] GetEventTimes(params ReplayEventType[] eventTypes)
        {
            return (from x in Events
                where eventTypes.Contains(x.Type)
                select x.Time).ToArray();
        }

        internal PlayerEvent[] GetEvents(params ReplayEventType[] eventTypes)
        {
            return Events.Where(x => eventTypes.Contains(x.Type)).ToArray();
        }

        internal Vector GlobalBodyFromIndex(int index)
        {
            return _globalBody[index];
        }

        internal void InitializeFrameData()
        {
            if (_frameDataInitialized)
                return;
            if (FrameCount > 1)
            {
                int sp = _pOffset + 36; //Pointer to bike's x-coordinates
                int sp2 = _pOffset + 36 + FrameCount * 4; //Pointer to bike's y-coordinates
                for (int i = 0; i <= FrameCount - 2; i++)
                {
                    double x1 = BitConverter.ToSingle(_rawData, sp + i * 4);
                    double x2 = BitConverter.ToSingle(_rawData, sp + i * 4 + 4);
                    double y1 = BitConverter.ToSingle(_rawData, sp2 + i * 4);
                    double y2 = BitConverter.ToSingle(_rawData, sp2 + i * 4 + 4);
                    double speed = Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2));
                    Trip += speed;
                    if (speed > TopSpeed)
                        TopSpeed = speed;
                }
            }

            TopSpeed *= Constants.SpeedConst;

            var globalBody = new Vector[FrameCount];
            var head = new Vector[FrameCount];
            var leftWheel = new Vector[FrameCount];
            var leftWheelRotation = new double[FrameCount];
            var rightWheel = new Vector[FrameCount];
            var rightWheelRotation = new double[FrameCount];
            var bikeRotation = new double[FrameCount];
            var directions = new Direction[FrameCount];
            for (int i = 0; i < FrameCount; i++)
            {
                globalBody[i] = new Vector(BitConverter.ToSingle(_rawData, _pOffset + 36 + i * 4),
                    BitConverter.ToSingle(_rawData, _pOffset + 36 + i * 4 + FrameCount * 4));
                leftWheel[i] =
                    new Vector(
                        globalBody[i].X +
                        (BitConverter.ToInt16(_rawData, _pOffset + 36 + FrameCount * 8 + i * 2) / 1000.0),
                        globalBody[i].Y +
                        (BitConverter.ToInt16(_rawData, _pOffset + 36 + FrameCount * 10 + i * 2) / 1000.0));
                rightWheel[i] =
                    new Vector(
                        globalBody[i].X +
                        (BitConverter.ToInt16(_rawData, _pOffset + 36 + FrameCount * 12 + i * 2) / 1000.0),
                        globalBody[i].Y +
                        (BitConverter.ToInt16(_rawData, _pOffset + 36 + FrameCount * 14 + i * 2) / 1000.0));
                leftWheelRotation[i] = _rawData[_pOffset + 36 + FrameCount * 22 + i] * 2 * Math.PI *
                                       WheelRotationFactor;
                rightWheelRotation[i] = _rawData[_pOffset + 36 + FrameCount * 23 + i] * 2 * Math.PI *
                                        WheelRotationFactor;
                if (leftWheelRotation[i] >= 2 * Math.PI)
                    leftWheelRotation[i] = 0;
                if (rightWheelRotation[i] >= 2 * Math.PI)
                    rightWheelRotation[i] = 0;
                double bikeRot = (BitConverter.ToInt16(_rawData, _pOffset + 36 + FrameCount * 20 + i * 2) / 10000.0 +
                                  1 / 4.0) * 2 * Math.PI;
                bikeRotation[i] = (BitConverter.ToInt16(_rawData, _pOffset + 36 + FrameCount * 20 + i * 2) / 10000.0) *
                                  360 %
                                  360;
                double headCos = Math.Cos(bikeRot - Math.PI / 2);
                double headSin = Math.Sin(bikeRot - Math.PI / 2);
                double headX = BitConverter.ToInt16(_rawData, _pOffset + 36 + FrameCount * 16 + i * 2) / 1000.0 +
                               globalBody[i].X + Math.Cos(bikeRot) * HeightConst;
                double headY = BitConverter.ToInt16(_rawData, _pOffset + 36 + FrameCount * 18 + i * 2) / 1000.0 +
                               globalBody[i].Y + Math.Sin(bikeRot) * HeightConst;
                if (_rawData[_pOffset + 36 + FrameCount * 24 + i] % 4 < 2)
                {
                    headX -= headCos * HeadDiff;
                    headY -= headSin * HeadDiff;
                    directions[i] = Direction.Left;
                }
                else
                {
                    headX += headCos * HeadDiff;
                    headY += headSin * HeadDiff;
                    directions[i] = Direction.Right;
                }

                head[i] = new Vector(headX, headY);
            }

            SetFrameData(globalBody, leftWheel, rightWheel, leftWheelRotation, rightWheelRotation,
                bikeRotation, head, directions);
            _frameDataInitialized = true;
        }

        private void InitializeVoltEvents()
        {
            VoltEvents = GetEvents(ReplayEventType.LeftVolt, ReplayEventType.RightVolt, ReplayEventType.SuperVolt);
        }

        internal void InitializeForPlaying(int killerObjectCount)
        {
            if (_playingInitialized)
                return;
            PlayerEvent[] appleEvents = GetEvents(ReplayEventType.AppleTake);
            foreach (PlayerEvent z in appleEvents)
                z.Info -= killerObjectCount;
            InitializeVoltEvents();
            _playingInitialized = true;
        }

        private void SetFrameData(Vector[] globalBody, Vector[] leftWheel, Vector[] rightWheel,
            double[] leftWheelRotation,
            double[] rightWheelRotation, double[] bikeRotation, Vector[] head,
            Direction[] directions)
        {
            _globalBody = globalBody;
            _leftWheel = leftWheel;
            _rightWheel = rightWheel;
            _leftWheelRotation = leftWheelRotation;
            _rightWheelRotation = rightWheelRotation;
            _bikeRotation = bikeRotation;
            _head = head;
            _direction = directions;
            _maxFrameIndex = globalBody.Length - 1;
        }

        private double Interpolate(double firstValue, double secondValue)
        {
            return firstValue + (secondValue - firstValue) * _interpolationStep;
        }
    }
}