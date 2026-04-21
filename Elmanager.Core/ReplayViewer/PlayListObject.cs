using System.ComponentModel;
using System.Drawing;
using Elmanager.Rec;

namespace Elmanager.ReplayViewer;

public class PlayListObject
{
    [Description("File name")]
    public string FileName { get; }
    [Description("#")]
    public int PlayerNum { get; }

    public readonly Player Player;
    public Color DrivingLineColor;

    public PlayListObject(string fileName, int num, Player player)
    {
        FileName = fileName;
        PlayerNum = num;
        Player = player;
        DrivingLineColor = Color.Black;
    }
}
