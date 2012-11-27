using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Elmanager
{
    internal enum ReplayEventType
    {
        GroundTouch = 1,
        AppleTake = 4,
        Turn = 5,
        RightVolt = 6,
        LeftVolt = 7,
        SuperVolt = 8
    }

    internal enum Direction
    {
        Left = 0,
        Right = 1
    }

    [Serializable]
    internal class Replay
    {
        [Description("Date modified")] public readonly DateTime DateModified;

        [Description("Finished")] public readonly bool Finished; //Is replay finished

        [Description("Multi")] public readonly bool IsMulti; //Is it single or multi replay

        [Description("Level exists")] public readonly bool LevelExists; //Does the replay's level exist

        [Description("Level")] public readonly string LevelFilename; //Filename of the level of the replay

        [Description("Size (kB)")] public readonly int Size;

        [Description("Time")] public readonly double Time; //Time of the replay

        [Description("Wrong version")] public readonly bool WrongLevelVersion; //Is the version of the level wrong
        internal readonly bool AcrossLevel; //Is the level across level
        internal readonly int InternalIndex;
        internal readonly bool IsInternal; //Is it internal or external replay
        internal readonly bool IsNitro; //Is it nitro replay

        internal string LevelPath; //Path of the level file if it is found
        internal string Path; //Path of the replay, this can change
        internal Player Player1;
        internal Player Player2;

        private readonly byte[] _rawData;
        private string _fileName; //Filename of the replay, this can change

        internal Replay(string replayPath)
        {
            _rawData = File.ReadAllBytes(replayPath);
            Path = replayPath;
            _fileName = System.IO.Path.GetFileName(Path);
            Size = _rawData.Count();
            DateModified = File.GetLastWriteTime(replayPath);
            IsNitro = BitConverter.ToInt32(_rawData, 4) != 0x83;
            if (IsNitro)
                return;
            LevelFilename = Utils.ReadNullTerminatedString(_rawData, 20, 12);
            IsInternal = Level.IsInternalLevel(LevelFilename);
            IsMulti = _rawData[8] == 1;
            Player1 = new Player(_rawData, false);
            if (IsMulti)
            {
                Player2 = new Player(_rawData, true);
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
                InternalIndex = int.Parse(LevelFilename.Substring(6, 2));
            }
            else
            {
                InternalIndex = -1;
                if (Global.LevelFiles == null)
                    Global.LevelFiles = Utils.GetLevelFiles(true);
                foreach (string levelFile in Global.LevelFiles)
                {
                    if (System.IO.Path.GetFileName(levelFile).CompareWith(LevelFilename))
                    {
                        LevelExists = true;
                        LevelPath = levelFile;
                        var levelStream = new FileStream(levelFile, FileMode.Open, FileAccess.Read);
                        levelStream.Seek(3, SeekOrigin.Begin);
                        //Check also the version of the level
                        if (levelStream.Length >= 900)
                        {
                            if (levelStream.ReadByte() == 49)
                                //If Level(3) = 49, it is Elma lev, otherwise (when 48) Across lev
                            {
                                AcrossLevel = false;
                                levelStream.Seek(7, SeekOrigin.Begin);
                            }
                            else
                            {
                                AcrossLevel = true;
                                levelStream.Seek(5, SeekOrigin.Begin);
                            }
                            for (int i = 0; i <= 3; i++)
                            {
                                if (levelStream.ReadByte() != _rawData[16 + i])
                                {
                                    WrongLevelVersion = true;
                                    break;
                                }
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

        [Description("File name")]
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value + Constants.RecExtension;
                Path = System.IO.Path.GetDirectoryName(Path) + "\\" + _fileName;
            }
        }

        internal Level GetLevel()
        {
            if (LevelExists)
            {
                if (IsInternal)
                {
                    while (Global.Internals == null)
                    {
                    }
                    return Global.Internals[InternalIndex - 1];
                }
                return new Level(LevelPath);
            }
            throw (new FileNotFoundException("The level file does not exist!", LevelFilename));
        }

        internal void InitializeFrameData()
        {
            Player1.InitializeFrameData();
            if (IsMulti)
                Player2.InitializeFrameData();
        }
    }

    [Serializable]
    internal class PlayerEvent
    {
        internal int Info;
        internal double Time;
        internal ReplayEventType Type;

        internal PlayerEvent(ReplayEventType eventType, double eventTime, int info = 0)
        {
            Type = eventType;
            Time = eventTime;
            Info = info;
        }
    }
}