using System;
using System.ComponentModel;
using Elmanager.IO;
using Elmanager.Lev;
using Elmanager.Rec;
using Elmanager.Utilities;

namespace Elmanager.ReplayManager;

internal record ReplayItem(ElmaFileObject<Replay> Efo) : IElmaFileObject
{
    public Replay Rec => Efo.Obj;

    [Description("Level")] public string LevelDesc => Level.GetPossiblyInternal(Rec.LevelFilename);

    [Description("Time")] public string TimeStr => Rec.Time.ToTimeString();

    [Description("Finished")] public bool Finished => Rec.Finished;

    [Description("Multi")] public bool IsMulti => Rec.IsMulti;

    [Description("Level exists")] public bool LevelExists => Rec.LevelExists;

    [Description("Wrong version")] public bool WrongLevelVersion => Rec.WrongLevelVersion;

    [Description("File name")] public string FileNameNoExt => Efo.File.FileNameNoExt;

    [Description("Date modified")] public DateTime DateModified => Efo.File.DateModified;

    [Description("Size (kB)")] public double SizeInKb => Efo.File.SizeInKb;

    public string Path => Efo.File.Path;

    public double Time => Rec.Time;

    public Player Player1 => Rec.Player1;
}