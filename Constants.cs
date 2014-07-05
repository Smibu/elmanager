using System;
using System.Windows.Forms;

namespace Elmanager
{
    static class Constants
    {
        internal const string AllLevs = "*" + LevExtension;
        internal const string AllRecs = "*" + RecExtension;
        internal const Keys Decrease = Keys.OemMinus;
        internal const Keys DecreaseBig = Keys.PageDown;
        internal const double DegToRad = Math.PI / 180;
        internal const double HeadDiameter = 0.476;
        internal const double HeadRadius = HeadDiameter / 2;
        internal const Keys Increase = Keys.Oemplus;
        internal const Keys IncreaseBig = Keys.PageUp;
        internal const string LevExtension = ".lev";

        internal const string LevOrRecDirNotFound =
            "Replay or level directory are not specified or they doesn\'t exist!";

        internal const string ProgramUri = "http://users.jyu.fi/~mikkalle/Elma/Elmanager.zip";
        internal const double RadToDeg = 180 / Math.PI;
        internal const string RecDirNotFound = "Replay directory is not specified or it doesn\'t exist!";
        internal const string RecExtension = ".rec";
        internal const double SpeedConst = 10000.0 / 23.0;

        internal const string VersionUri = "http://users.jyu.fi/~mikkalle/Elma/version.txt";
        internal const string ChangelogUri = "http://users.jyu.fi/~mikkalle/Elma/em_changelog.txt";
    }
}