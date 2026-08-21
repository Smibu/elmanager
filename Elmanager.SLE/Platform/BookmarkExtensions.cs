using System;
using System.Threading.Tasks;
using Elmanager.LevelEditor;

namespace Elmanager.SLE.Platform;

internal static class BookmarkExtensions
{
    internal static Task<bool> HasFileReadPermissionAsync(this Bookmark bookmark) =>
        OperatingSystem.IsBrowser()
            ? BrowserInterop.HasBookmarkedFileReadPermission(bookmark.Id)
            : Task.FromResult(true);
}
