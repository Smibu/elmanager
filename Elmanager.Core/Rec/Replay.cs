using System;
using System.Collections.Generic;
using System.IO;
using Elmanager.IO;
using Elmanager.Lev;
using Elmanager.Utilities;

namespace Elmanager.Rec;

public class Replay
{
    public readonly bool Finished;

    public readonly bool IsMulti;

    public bool LevelExists => LevelPath is not null || IsInternal;

    public readonly string LevelFilename;

    public readonly double Time;

    public readonly bool WrongLevelVersion;
    internal readonly bool AcrossLevel;
    public bool IsInternal => _internalLevel is not null;

    public readonly string? LevelPath;
    public Player Player1 => Players[0];
    public Player Player2 => Players[1];
    public readonly List<Player> Players = new(2);
    private readonly Level? _internalLevel;

    private Replay(string replayPath, IReadOnlyList<string> levelFiles, IReadOnlyList<Level> internals)
    {
        var isInternal = false;
        using (var stream = File.OpenRead(replayPath))
        {
            var rec = new BinaryReader(stream);
            try
            {
                var frames = rec.ReadInt32();
                var magic = rec.ReadInt32();
                if (magic != 0x83)
                    throw new BadFileException($"Unexpected magic ({magic}) in replay file: {replayPath}");
                IsMulti = rec.ReadInt32() == 1;
                var isFlagtag = rec.ReadInt32() == 1;
                LevId = rec.ReadInt32();
                LevelFilename = rec.ReadNullTerminatedString(12);
                rec.ReadInt32();
                isInternal = Level.IsInternalLevel(LevelFilename);
                Players.Add(new Player(rec, frames));
                if (IsMulti)
                {
                    frames = rec.ReadInt32();
                    rec.BaseStream.Seek(32, SeekOrigin.Current);
                    Players.Add(new Player(rec, frames));
                }
            }
            catch (EndOfStreamException)
            {
                throw new BadFileException($"Corrupted replay file: {replayPath}");
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

        if (isInternal)
        {
            WrongLevelVersion = false;
            AcrossLevel = false;
            var internalIndex = int.Parse(LevelFilename.Substring(6, 2));
            _internalLevel = internals[internalIndex - 1];
        }
        else
        {
            foreach (var levelFile in levelFiles)
            {
                if (Path.GetFileName(levelFile).EqualsIgnoreCase(LevelFilename))
                {
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

                    levelStream.Close();
                    break;
                }
            }
        }
    }

    public static ElmaFileObject<Replay> FromPath(string replayPath, IReadOnlyList<string> levelFiles, IReadOnlyList<Level> internals) =>
        ElmaFileObject<Replay>.FromPath(replayPath, new Replay(replayPath, levelFiles, internals));

    public int LevId { get; }

    public Level GetLevel()
    {
        if (LevelPath is not null)
        {
            return Level.FromPath(LevelPath).Obj;
        }

        if (_internalLevel is not null)
        {
            return _internalLevel;
        }

        throw new FileNotFoundException("The level file does not exist!", LevelFilename);
    }
}
