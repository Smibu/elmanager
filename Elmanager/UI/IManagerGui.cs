using System.Windows.Forms;
using BrightIdeasSoftware;

namespace Elmanager.UI;

internal interface IManagerGui
{
    Form Form { get; }
    ObjectListView ObjectList { get; }
    string SearchPattern { get; set; }
    string EmptySelectionError { get; }
    bool Busy { get; }
    void DisplaySelectionInfo();
    bool ConfirmDeletion();
}