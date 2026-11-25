using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;

namespace Elmanager.SLE.Editor;

public partial class MainView
{
    private void OnDragOver(object? _, DragEventArgs e)
    {
        if (!e.DataTransfer.Contains(DataFormat.File))
        {
            HideDropOverlay();
            e.DragEffects = DragDropEffects.None;
            e.Handled = true;
            return;
        }

        var files = GetDroppedFiles(e);
        var mode = GetFileDropMode(GetFileDrop(files));
        var browserFileCount = 0;
        if (mode == FileDropMode.None && files.Count == 0)
        {
            browserFileCount = e.DataTransfer.GetItems(DataFormat.File).Count();
            mode = browserFileCount switch
            {
                1 => FileDropMode.OpenOrImport,
                > 1 => FileDropMode.Import,
                _ => FileDropMode.None
            };
        }

        var open = mode == FileDropMode.OpenOrImport && IsOpenDropPosition(e);

        UpdateDropOverlay(mode, open);
        if (browserFileCount == 1)
        {
            DropPrimaryText.Text = "OPEN .LEV";
            DropPrimaryDescription.Text = "Only a level file can be opened";
        }

        e.DragEffects = mode == FileDropMode.None ? DragDropEffects.None : DragDropEffects.Copy;
        e.Handled = true;
    }

    private void OnDragLeave(object? _, DragEventArgs e)
    {
        HideDropOverlay();
        e.Handled = true;
    }

    private async void OnDrop(object? _, DragEventArgs e)
    {
        var drop = GetFileDrop(GetDroppedFiles(e));
        var open = drop is FileDrop.LevelFile && IsOpenDropPosition(e);

        HideDropOverlay();
        e.Handled = true;

        await (drop switch
        {
            null => Task.CompletedTask,
            FileDrop.LevelFile level when open => OpenDroppedLevel(level.Lev),
            FileDrop.LevelFile level => ImportFiles([level.Lev]),
            FileDrop.ImportFiles import => ImportFiles(import.Files),
            FileDrop.LgrFile lgr => AddDroppedLgr(lgr.Lgr)
        });
    }

    private async Task OpenDroppedLevel(IStorageFile lev)
    {
        if (await ConfirmDiscardUnsavedChanges())
        {
            await OpenLevelFromStorageFile(lev);
        }
    }

    private static List<IStorageFile> GetDroppedFiles(DragEventArgs e) =>
        e.DataTransfer.TryGetFiles()?.OfType<IStorageFile>().ToList() ?? [];

    private static FileDrop? GetFileDrop(IReadOnlyList<IStorageFile> files)
    {
        if (files.FirstOrDefault(file => file.Name.EndsWith(".lgr", StringComparison.OrdinalIgnoreCase)) is { } lgr)
        {
            return new FileDrop.LgrFile(lgr);
        }

        if (files.Count == 1 && files[0].Name.EndsWith(".lev", StringComparison.OrdinalIgnoreCase))
        {
            return new FileDrop.LevelFile(files[0]);
        }

        if (files.Count > 0 && files.All(IsImportable))
        {
            return new FileDrop.ImportFiles(files);
        }

        return null;
    }

    private static FileDropMode GetFileDropMode(FileDrop? drop) => drop switch
    {
        null => FileDropMode.None,
        FileDrop.LevelFile => FileDropMode.OpenOrImport,
        FileDrop.ImportFiles => FileDropMode.Import,
        FileDrop.LgrFile => FileDropMode.UseLgr
    };

    private bool IsOpenDropPosition(DragEventArgs e) =>
        e.GetPosition(ViewportDropTarget).X < ViewportDropTarget.Bounds.Width / 2;

    private void UpdateDropOverlay(FileDropMode mode, bool open)
    {
        DropOverlay.IsVisible = mode != FileDropMode.None;
        var hasChoice = mode == FileDropMode.OpenOrImport;
        DropSecondaryRegion.IsVisible = hasChoice;
        Grid.SetColumnSpan(DropPrimaryRegion, hasChoice ? 1 : 2);

        if (hasChoice)
        {
            DropPrimaryText.Text = "OPEN LEVEL";
            DropPrimaryDescription.Text = "Replace the current level";
            DropPrimaryRegion.Opacity = open ? 1 : 0.45;
            DropSecondaryRegion.Opacity = open ? 0.45 : 1;
        }
        else if (mode == FileDropMode.Import)
        {
            DropPrimaryText.Text = "DROP TO IMPORT";
            DropPrimaryDescription.Text = "Add the dropped files to the current level";
            DropPrimaryRegion.Opacity = 1;
        }
        else if (mode == FileDropMode.UseLgr)
        {
            DropPrimaryText.Text = "DROP TO USE LGR";
            DropPrimaryDescription.Text = "Apply the dropped graphics to the current level";
            DropPrimaryRegion.Opacity = 1;
        }
    }

    private void HideDropOverlay() => DropOverlay.IsVisible = false;

    private union FileDrop(FileDrop.LevelFile, FileDrop.ImportFiles, FileDrop.LgrFile)
    {
        public record LevelFile(IStorageFile Lev);

        public record ImportFiles(IReadOnlyList<IStorageFile> Files);

        public record LgrFile(IStorageFile Lgr);
    }

    private enum FileDropMode
    {
        None,
        OpenOrImport,
        Import,
        UseLgr
    }
}
