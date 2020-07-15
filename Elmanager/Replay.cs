using System;
using System.ComponentModel;
using System.IO;

namespace Elmanager
{
    internal enum ReplayEventType
    {
        ObjectTouch = 0,
        GroundTouch = 1,
        AppleTake = 4,
        Turn = 5,
        RightVolt = 6,
        LeftVolt = 7
    }

    internal enum LogicalEventType
    {
        Finish,
        FlowerTouch,
        KillerTouch,
        GroundTouch,
        AppleTake,
        Turn,
        RightVolt,
        LeftVolt,
        SuperVolt
    }

    internal enum Direction
    {
        Left = 0,
        Right = 1
    }

    internal class Replay : ElmaObject
    {
        [Description("Finished")] public readonly bool Finished; //Is replay finished

        [Description("Multi")] public readonly bool IsMulti; //Is it single or multi replay

        [Description("Level exists")] public readonly bool LevelExists; //Does the replay's level exist

        [Description("Level")] public string LevelDesc => Utils.GetPossiblyInternal(LevelFilename);

        public readonly string LevelFilename; //Filename of the level of the replay

        [Description("Time")] public string TimeStr => Time.ToTimeString();

        public readonly double Time; //Time of the replay

        [Description("Wrong version")] public readonly bool WrongLevelVersion; //Is the version of the level wrong
        internal readonly bool AcrossLevel; //Is the level across level
        private readonly int _internalIndex;
        internal readonly bool IsInternal; //Is it internal or external replay

        internal readonly string LevelPath; //Path of the level file if it is found
        internal readonly Player Player1;
        internal readonly Player Player2;

        internal Replay(string replayPath)
        {
            Path = replayPath;
            Size = (int) new FileInfo(replayPath).Length;
            DateModified = File.GetLastWriteTime(replayPath);
            using (var stream = File.OpenRead(replayPath))
            {
                var rec = new BinaryReader(stream);
                var frames = rec.ReadInt32();
                var invalid = rec.ReadInt32() != 0x83;
                if (invalid)
                    return;
                IsMulti = rec.ReadInt32() == 1;
                var isFlagtag = rec.ReadInt32() == 1;
                LevId = rec.ReadInt32();
                LevelFilename = rec.ReadNullTerminatedString(12);
                rec.ReadInt32();
                IsInternal = Level.IsInternalLevel(LevelFilename);
                Player1 = new Player(rec, frames);
                if (IsMulti)
                {
                    frames = rec.ReadInt32();
                    rec.BaseStream.Seek(32, SeekOrigin.Current);
                    Player2 = new Player(rec, frames);
                }
            }

            if (IsMulti)
            {
                if (!Player1.Finished)
                {
                    if (Player1.FakeFinish && Player2.IsLastEventApple)
                    {
                        Finished = true;
                        Time = Player1.Time;
                    }
                    else if (Player2.FakeFinish && Player1.IsLastEventApple)
                    {
                        Finished = true;
                        Time = Player2.Time;
                    }
                    else if (Player2.Finished)
                    {
                        Finished = true;
                        Time = Player2.Time;
                    }
                    else //In this case, neither of players finished
                        Time = Math.Max(Player1.Time, Player2.Time);
                }
                else
                {
                    Time = Player1.Time;
                    Finished = true;
                }
            }
            else
            {
                Finished = Player1.Finished;
                Time = Player1.Time;
            }

            if (IsInternal)
            {
                LevelExists = true;
                WrongLevelVersion = false;
                AcrossLevel = false;
                _internalIndex = int.Parse(LevelFilename.Substring(6, 2));
            }
            else
            {
                _internalIndex = -1;
                foreach (var levelFile in Global.GetLevelFiles())
                {
                    if (System.IO.Path.GetFileName(levelFile).CompareWith(LevelFilename))
                    {
                        LevelExists = true;
                        LevelPath = levelFile;
                        var fileStream = File.OpenRead(levelFile);
                        var levelStream = new BinaryReader(fileStream);
                        fileStream.Seek(3, SeekOrigin.Begin);
                        //Check also the version of the level
                        if (fileStream.Length > 0)
                        {
                            if (levelStream.ReadByte() == 49)
                                //If Level(3) = 49, it is Elma lev, otherwise (when 48) Across lev
                            {
                                AcrossLevel = false;
                                fileStream.Seek(7, SeekOrigin.Begin);
                            }
                            else
                            {
                                AcrossLevel = true;
                                fileStream.Seek(5, SeekOrigin.Begin);
                            }

                            if (levelStream.ReadInt32() != LevId)
                            {
                                WrongLevelVersion = true;
                                break;
                            }
                        }
                        else
                            LevelExists = false;

                        levelStream.Close();
                        break;
                    }
                }
            }
        }

        public int LevId { get; }

        internal Level GetLevel()
        {
            if (LevelExists)
            {
                if (IsInternal)
                {
                    while (Global.Internals == null)
                    {
                    }

                    return Global.Internals[_internalIndex - 1];
                }

                return Level.FromPath(LevelPath);
            }

            throw new FileNotFoundException("The level file does not exist!", LevelFilename);
        }
    }

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