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

        internal const double RadToDeg = 180 / Math.PI;
        internal const string RecDirNotFound = "Replay directory is not specified or it doesn\'t exist!";
        internal const string RecExtension = ".rec";
        internal const double SpeedConst = 10000.0 / 23.0;

        internal const string VersionUri = "https://mkl.io/Elma/elmanager/check_update";
        internal const string ChangelogUri = "https://gitlab.com/Smibu/elmanager/blob/master/changelog.md";
        public const double TOLERANCE = 0.00000001;
    }
}