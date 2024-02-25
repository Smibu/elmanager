using System.Drawing;
using System.IO;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using Elmanager.UI;

namespace Elmanager.Settings;

internal class ManagerSettings
{
    [JsonPropertyName("ConfirmDelete")]
    public bool ConfirmDelete { get; set; } = true;
    [JsonPropertyName("Location")]
    public Point Location { get; set; }
    [JsonPropertyName("ListState")]
    public byte[]? ListState { get; set; }
    [JsonPropertyName("SearchPattern")]
    public string SearchPattern { get; set; } = string.Empty;
    [JsonPropertyName("RecDirSearchOption")]
    public SearchOption RecDirSearchOption { get; set; } = SearchOption.AllDirectories;
    [JsonPropertyName("ShowGridInList")]
    public bool ShowGridInList { get; set; } = true;
    [JsonPropertyName("Size")]
    public Size Size { get; set; } = new(800, 600);
    [JsonPropertyName("WindowState")]
    public FormWindowState WindowState { get; set; } = FormWindowState.Normal;
    [JsonPropertyName("ShowTooltipInList")]
    public bool ShowTooltipInList { get; set; } = true;
    [JsonPropertyName("LevDirSearchOption")]
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