using System.ComponentModel;
using System.Drawing;
using Elmanager.Rec;

namespace Elmanager.ReplayViewer;

internal class PlayListObject
{
    [Description("File name")]
    public string FileName { get; }
    [Description("#")]
    public int PlayerNum { get; }
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