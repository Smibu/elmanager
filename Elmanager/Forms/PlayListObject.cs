using System.ComponentModel;
using System.Drawing;

namespace Elmanager.Forms
{
    internal class PlayListObject
    {
        [Description("File name")]
        public readonly string FileName;
        [Description("#")]
        public readonly int PlayerNum;
        internal readonly Player Player;
        internal Color DrivingLineColor;

        internal PlayListObject(string fileName, int num, Player player)
        {
            FileName = fileName;
            PlayerNum = num;
            Player = player;
            DrivingLineColor = Color.Black;
        }
    }
}