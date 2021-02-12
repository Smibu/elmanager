using BrightIdeasSoftware;
using System.Windows.Forms;

namespace Elmanager.Forms
{
    internal interface IManagerGui
    {
        Form Form { get; }
        ObjectListView ObjectList { get; }
        string SearchPattern { get; set; }
        string EmptySelectionError { get; }
        bool Busy { get; }
        void DisplaySelectionInfo();
        bool ConfirmDeletion();
        void NotifyAboutModification();
    }
}