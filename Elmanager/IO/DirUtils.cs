using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elmanager.Application;

namespace Elmanager.IO
{
    internal static class DirUtils
    {
        internal static List<string> GetLevelFiles(SearchOption searchSubDirs)
        {
            if (Directory.Exists(Global.AppSettings.General.LevelDirectory))
            {
                string[] files = Directory.GetFiles(Global.AppSettings.General.LevelDirectory, AllLevs,
                    searchSubDirs);
                Array.Sort(files);
                return files.ToList();
            }

            return new List<string>();
        }

        internal static bool LevDirectoryExists()
        {
            return Directory.Exists(Global.AppSettings.General.LevelDirectory);
        }

        internal static bool LevRecDirectoriesExist()
        {
            return RecDirectoryExists() && LevDirectoryExists();
        }

        internal static bool RecDirectoryExists()
        {
            return Directory.Exists(Global.AppSettings.General.ReplayDirectory);
        }

        private const string AllLevs = "*" + LevExtension;
        internal const string LevExtension = ".lev";
        internal const string LebExtension = ".leb";
        internal static readonly string[] LevLebExtensions = {LevExtension, LebExtension};

        internal const string LevOrRecDirNotFound =
            "Replay or level directory are not specified or they doesn\'t exist!";

        internal const string LevDirNotFound = "Level directory is not specified or it doesn\'t exist!";
        internal const string RecDirNotFound = "Replay directory is not specified or it doesn\'t exist!";
        internal const string RecExtension = ".rec";
    }
}