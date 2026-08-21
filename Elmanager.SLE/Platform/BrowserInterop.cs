using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace Elmanager.SLE.Platform;

internal static partial class BrowserInterop
{
    internal static async Task<string[]> GetBookmarkedFolderFileNames(string bookmark)
    {
        using var fileNames = await GetBookmarkedFolderFileNamesAsObject(bookmark);
        if (fileNames == null)
        {
            throw new InvalidOperationException("The browser returned no folder entries.");
        }

        return ToStringArray(fileNames);
    }

    internal static Task RenameBookmarkedFile(string bookmark, string newName) =>
        RenameBookmarkedFileCore(bookmark, newName);

    [JSImport("getBookmarkedFolderFileNames", "storage-interop.js")]
    private static partial Task<JSObject?> GetBookmarkedFolderFileNamesAsObject(string bookmark);

    [JSImport("hasBookmarkedFileReadPermission", "storage-interop.js")]
    internal static partial Task<bool> HasBookmarkedFileReadPermission(string bookmark);

    [JSImport("renameBookmarkedFile", "storage-interop.js")]
    private static partial Task RenameBookmarkedFileCore(string bookmark, string newName);

    [JSImport("syncToIndexedDb", "filesystem.js")]
    internal static partial Task SyncToIndexedDb();

    [JSImport("toStringArray", "storage-interop.js")]
    [return: JSMarshalAs<JSType.Array<JSType.String>>]
    private static partial string[] ToStringArray(JSObject value);

    [JSImport("setDocumentTitle", "browser.js")]
    internal static partial void SetDocumentTitle(string title);

    [JSImport("isChromiumBrowser", "browser.js")]
    internal static partial bool IsChromiumBrowser();

    [JSImport("setFullscreen", "browser.js")]
    internal static partial Task SetFullscreen(bool fullscreen);

    [JSImport("subscribeFullscreenChange", "browser.js")]
    internal static partial void SubscribeFullscreenChange(
        [JSMarshalAs<JSType.Function<JSType.Boolean>>] Action<bool> listener);
}
