using System.IO;
using BrightIdeasSoftware;
using Elmanager.IO;
using Microsoft.VisualBasic.FileIO;

namespace Elmanager.UI;

internal class Manager<T> where T : class, IElmaFileObject
{
    private ObjectListView ObjectList => _managerGui.ObjectList;
    public readonly TypedObjectListView<T> TypedList;
    private readonly IManagerGui _managerGui;

    public Manager(IManagerGui m)
    {
        _managerGui = m;
        TypedList = new TypedObjectListView<T>(ObjectList);
        UiUtils.ConfigureColumns<T>(ObjectList);
    }

    public void DeleteItems()
    {
        if (TypedList.SelectedObjects.Count == 0)
        {
            UiUtils.ShowError(_managerGui.EmptySelectionError);
            return;
        }

        if (_managerGui.Busy || !_managerGui.ConfirmDeletion())
            return;
        foreach (var x in TypedList.SelectedObjects)
        {
            try
            {
                FileSystem.DeleteFile(x.Path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }
            catch (FileNotFoundException)
            {
            }
        }
        RemoveReplays();
    }

    public void RemoveReplays()
    {
        if (ObjectList.SelectedObjects.Count <= 0)
        {
            UiUtils.ShowError(_managerGui.EmptySelectionError);
            return;
        }

        if (_managerGui.Busy)
            return;
        var index = ObjectList.SelectedIndices[0];
        ObjectList.RemoveObjects(ObjectList.SelectedObjects);
        if (ObjectList.Items.Count > 0)
        {
            if (index >= ObjectList.Items.Count)
            {
                index--;
            }

            ObjectList.Items[index].Selected = true;
        }

        _managerGui.DisplaySelectionInfo();
    }
}