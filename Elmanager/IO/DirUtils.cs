using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elmanager.Application;

namespace Elmanager.IO;

internal static class DirUtils
{
    internal static List<string> GetLevelFiles(SearchOption searchSubDirs)
    {
        if (GetLevDir() is { } levDir)
        {
            string[] files = Directory.GetFiles(levDir, AllLevs,
                searchSubDirs);
            Array.Sort(files);
            return files.ToList();
        }

        return new List<string>();
    }

    internal static string? GetLevDir() =>
        Directory.Exists(Global.AppSettings.General.LevelDirectory)
            ? Global.AppSettings.General.LevelDirectory
            : null;

    internal static string? GetRecDir() =>
        Directory.Exists(Global.AppSettings.General.ReplayDirectory)
            ? Global.AppSettings.General.ReplayDirectory
            : null;

    private const string AllLevs = "*" + LevExtension;
    internal const string LevExtension = ".lev";
    internal const string LebExtension = ".leb";

    internal const string LevOrRecDirNotFound =
        "Replay or level directory are not specified or they doesn\'t exist!";

    internal const string LevDirNotFound = "Level directory is not specified or it doesn\'t exist!";
    internal const string RecDirNotFound = "Replay directory is not specified or it doesn\'t exist!";
    internal const string RecExtension = ".rec";
}