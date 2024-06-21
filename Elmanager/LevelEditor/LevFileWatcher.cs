using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Elmanager.IO;
using Elmanager.Lev;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Elmanager.LevelEditor;

internal class LevFileWatcher
{
    private readonly FileSystemWatcher _fileWatcher = new();
    private readonly LevelEditorForm _levelEditorForm;
    private ElmaFileObject<Level>? _levOnDisk;

    public LevFileWatcher(LevelEditorForm levelEditorForm)
    {
        _levelEditorForm = levelEditorForm;
        _fileWatcher.Changed += (_, _) =>
        {
            _levelEditorForm.Invoke(LevelFileChangedHandler);
        };
        _fileWatcher.Renamed += (_, args) =>
        {
            if (_levOnDisk is null)
            {
                return;
            }
            _levOnDisk = _levOnDisk with { File = new ElmaFile(args.FullPath) };
            UpdateFilter(new ElmaFile(args.FullPath));
        };
    }

    private void UpdateFilter(ElmaFile file)
    {
        _fileWatcher.Path = file.FileInfo.DirectoryName!;
        _fileWatcher.Filter = file.FileInfo.Name;
    }

    private void LevelFileChangedHandler()
    {
        if (_levOnDisk is null)
        {
            return;
        }

        ElmaFileObject<Level> modifiedLev;
        var tries = 0;
        while (true)
        {
            try
            {
                modifiedLev = Level.FromPath(_levOnDisk.File.Path);
                break;
            }
            catch (Exception e) when (e is EndOfStreamException or BadFileException)
            {
                WithoutEvents(() => _levOnDisk.Obj.Save(_levOnDisk.File.Path));
                var msgs = new[]
                {
                    "The lev file was corrupted by another process (most likely Elma wrote top10 to wrong place in lev file).",
                    "But don't worry: SLE fixed the lev file automatically.",
                    "Keep in mind that if you edit and save the lev in SLE while having the lev open in Elma, " +
                    "you must reload the lev in Elma by selecting some other lev in external list and switching back. " +
                    "If you forget to do this, Elma will have wrong version of the lev in memory and will corrupt the lev file if you drive a top10 time.",
                    "(This warning may pop up several times in a row if you drive multiple top10 times.)"
                };
                MessageBox.Show(_levelEditorForm, string.Join("\n\n", msgs), "Warning", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
            catch (Exception)
            {
                tries++;
                if (tries > 100)
                {
                    MessageBox.Show(_levelEditorForm,
                        "The lev file was changed by another process but SLE is unable to read it.", "Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
                Thread.Sleep(50);
            }
        }

        if (!modifiedLev.Obj.EqualsIgnoringTop10(_levelEditorForm.Lev))
        {
            _levelEditorForm.UpdateLevel(modifiedLev.Obj);
            MessageBox.Show(
                _levelEditorForm,
                "The lev file was modified by another process. SLE reloaded the level, but you can undo it if needed.",
                "Notice",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
    }

    public void StoreLevDiskSnapshot(ElmaFileObject<Level> lev)
    {
        UpdateFilter(lev.File);
        _fileWatcher.EnableRaisingEvents = true;
        _levOnDisk = lev with { Obj = lev.Obj.Clone() };
    }

    public void ClearLevDiskSnapshot()
    {
        _fileWatcher.EnableRaisingEvents = false;
        _levOnDisk = null;
    }

    public T WithoutEvents<T>(Func<T> func)
    {
        _fileWatcher.EnableRaisingEvents = false;
        var r = func();
        _fileWatcher.EnableRaisingEvents = true;
        return r;
    }
}