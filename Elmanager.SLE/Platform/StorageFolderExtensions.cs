using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace Elmanager.SLE.Platform;

internal static class StorageFolderExtensions
{
    internal static async Task<IReadOnlyList<string>> GetFileNamesAsync(
        this IStorageFolder folder,
        string bookmark,
        string extension)
    {
        if (OperatingSystem.IsBrowser())
        {
            var browserFileNames = await BrowserInterop.GetBookmarkedFolderFileNames(bookmark);
            return FilterByExtension(browserFileNames, extension);
        }

        List<string> fileNames = [];
        await foreach (var item in folder.GetItemsAsync())
        {
            using (item)
            {
                if (item is IStorageFile file &&
                    file.Name.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                {
                    fileNames.Add(file.Name);
                }
            }
        }

        return fileNames;
    }

    private static IReadOnlyList<string> FilterByExtension(
        IEnumerable<string> fileNames,
        string extension)
    {
        List<string> matches = [];
        foreach (var fileName in fileNames)
        {
            if (fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
            {
                matches.Add(fileName);
            }
        }

        return matches;
    }
}
