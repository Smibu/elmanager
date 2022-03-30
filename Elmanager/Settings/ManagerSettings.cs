using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Elmanager.UI;

namespace Elmanager.Settings;

internal class ManagerSettings
{
    public bool ConfirmDelete { get; set; } = true;
    public Point Location { get; set; }
    public byte[]? ListState { get; set; }
    public string SearchPattern { get; set; } = string.Empty;
    public SearchOption RecDirSearchOption { get; set; } = SearchOption.AllDirectories;
    public bool ShowGridInList { get; set; } = true;
    public Size Size { get; set; } = new(800, 600);
    public FormWindowState WindowState { get; set; } = FormWindowState.Normal;
    public bool ShowTooltipInList { get; set; } = true;
    public SearchOption LevDirSearchOption { get; set; } = SearchOption.AllDirectories;

    public void SaveGui(IManagerGui m)
    {
        var f = m.Form;
        var list = m.ObjectList;
        Location = f.Location;
        Size = f.Size;
        WindowState = f.WindowState;
        ShowGridInList = list.GridLines;
        ListState = list.SaveState();
        SearchPattern = m.SearchPattern;
    }

    public void RestoreGui(IManagerGui m)
    {
        var f = m.Form;
        var list = m.ObjectList;
        f.Location = Location;
        f.Size = Size;
        f.WindowState = WindowState;
        list.GridLines = ShowGridInList;
        m.SearchPattern = SearchPattern;
        if (ListState != null)
        {
            list.RestoreState(ListState);
        }
    }
}