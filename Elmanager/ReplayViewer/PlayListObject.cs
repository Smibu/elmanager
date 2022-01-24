using System.ComponentModel;
using System.Drawing;
using Elmanager.Rec;

namespace Elmanager.ReplayViewer;

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