using System;
using System.Collections.Generic;
using System.Linq;
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
        var mode = GetFileDropMode(files);
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
        var files = GetDroppedFiles(e);
        var mode = GetFileDropMode(files);
        var open = mode == FileDropMode.OpenOrImport && IsOpenDropPosition(e);

        HideDropOverlay();
        e.Handled = true;

        switch (mode)
        {
            case FileDropMode.OpenOrImport when open:
                if (await ConfirmDiscardUnsavedChanges())
                {
                    await OpenLevelFromStorageFile(files[0]);
                }

                break;
            case FileDropMode.OpenOrImport:
            case FileDropMode.Import:
                await ImportFiles(files);
                break;
            case FileDropMode.UseLgr:
                await AddDroppedLgr(files.First(file =>
                    file.Name.EndsWith(".lgr", StringComparison.OrdinalIgnoreCase)));
                break;
        }
    }

    private static List<IStorageFile> GetDroppedFiles(DragEventArgs e) =>
        e.DataTransfer.TryGetFiles()?.OfType<IStorageFile>().ToList() ?? [];

    private static FileDropMode GetFileDropMode(IReadOnlyList<IStorageFile> files)
    {
        if (files.Any(file => file.Name.EndsWith(".lgr", StringComparison.OrdinalIgnoreCase)))
        {
            return FileDropMode.UseLgr;
        }

        if (files.Count == 1 && files[0].Name.EndsWith(".lev", StringComparison.OrdinalIgnoreCase))
        {
            return FileDropMode.OpenOrImport;
        }

        return files.Count > 0 && files.All(IsImportable)
            ? FileDropMode.Import
            : FileDropMode.None;
    }

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

    private enum FileDropMode
    {
        None,
        OpenOrImport,
        Import,
        UseLgr
    }
}
