using System.IO;
using BrightIdeasSoftware;
using Elmanager.Application;
using Elmanager.IO;
using Microsoft.VisualBasic.FileIO;

namespace Elmanager.UI
{
    internal class Manager<T> where T : ElmaFile
    {
        private ObjectListView ObjectList => _managerGui.ObjectList;
        public readonly TypedObjectListView<T> TypedList;
        private readonly IManagerGui _managerGui;
        private FileSystemWatcher _levWatcher;
        private FileSystemWatcher _recWatcher;

        public Manager(IManagerGui m)
        {
            _managerGui = m;
            TypedList = new TypedObjectListView<T>(ObjectList);
            UiUtils.ConfigureColumns<T>(ObjectList);
            return;
            _levWatcher = new FileSystemWatcher(Global.AppSettings.General.LevelDirectory, "*.lev");
            _recWatcher = new FileSystemWatcher(Global.AppSettings.General.ReplayDirectory, "*.rec");
            _levWatcher.IncludeSubdirectories = true;
            _recWatcher.IncludeSubdirectories = true;
            _levWatcher.EnableRaisingEvents = false;
            _recWatcher.EnableRaisingEvents = false;
            _levWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            _recWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            _levWatcher.Changed += WatcherOnChanged;
            _levWatcher.Created += WatcherOnChanged;
            _levWatcher.Deleted += WatcherOnChanged;
            _levWatcher.Renamed += WatcherOnChanged;
            _recWatcher.Changed += WatcherOnChanged;
            _recWatcher.Created += WatcherOnChanged;
            _recWatcher.Deleted += WatcherOnChanged;
            _recWatcher.Renamed += WatcherOnChanged;
        }

        private void WatcherOnChanged(object sender, FileSystemEventArgs e)
        {
            _managerGui.NotifyAboutModification();
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
}