using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace Elmanager.SLE.Platform;

internal static class StorageFileExtensions
{
    internal static async Task<IStorageFile> RenameAsync(
        this IStorageFile file,
        string newName,
        IStorageProvider storageProvider)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newName);
        ArgumentNullException.ThrowIfNull(storageProvider);

        if (newName is "." or ".." || newName.IndexOfAny(['/', '\\']) >= 0)
        {
            throw new ArgumentException("The new name must be a filename, not a path.", nameof(newName));
        }

        if (string.Equals(file.Name, newName, StringComparison.Ordinal))
        {
            throw new ArgumentException("The new name must be different from the current name.", nameof(newName));
        }

        var localPath = file.TryGetLocalPath();
        if (localPath != null)
        {
            return await RenameLocalFile(file, localPath, newName, storageProvider);
        }

        if (OperatingSystem.IsBrowser())
        {
            return await RenameBrowserFile(file, newName, storageProvider);
        }

        throw new PlatformNotSupportedException(
            "The current storage provider does not expose a native file rename operation.");
    }

    private static async Task<IStorageFile> RenameLocalFile(
        IStorageFile file,
        string currentPath,
        string newName,
        IStorageProvider storageProvider)
    {
        var parentPath = Path.GetDirectoryName(currentPath);
        if (parentPath == null)
        {
            throw new IOException("The file's parent folder could not be determined.");
        }

        var renamedPath = Path.Combine(parentPath, newName);
        Stream? securityScopeStream = null;
        try
        {
            if (OperatingSystem.IsMacOS() ||
                OperatingSystem.IsIOS() ||
                OperatingSystem.IsMacCatalyst())
            {
                securityScopeStream = await file.OpenReadAsync();
            }

            File.Move(currentPath, renamedPath);
            try
            {
                return await storageProvider.TryGetFileFromPathAsync(renamedPath) ??
                       throw new IOException("The renamed file could not be reopened.");
            }
            catch (Exception reopenException)
            {
                try
                {
                    File.Move(renamedPath, currentPath);
                }
                catch (Exception rollbackException)
                {
                    throw new IOException(
                        "The file was renamed, but it could not be reopened or restored to its original name.",
                        new AggregateException(reopenException, rollbackException));
                }

                throw;
            }
        }
        finally
        {
            if (securityScopeStream != null)
            {
                await securityScopeStream.DisposeAsync();
            }
        }
    }

    private static async Task<IStorageFile> RenameBrowserFile(
        IStorageFile file,
        string newName,
        IStorageProvider storageProvider)
    {
        var oldName = file.Name;
        var bookmark = await file.SaveBookmarkAsync() ??
                       throw new PlatformNotSupportedException(
                           "This browser storage provider does not support persistent file handles.");

        await BrowserInterop.RenameBookmarkedFile(bookmark, newName);
        try
        {
            return await storageProvider.OpenFileBookmarkAsync(bookmark) ??
                   throw new IOException("The renamed file could not be reopened.");
        }
        catch (Exception reopenException)
        {
            try
            {
                await BrowserInterop.RenameBookmarkedFile(bookmark, oldName);
            }
            catch (Exception rollbackException)
            {
                throw new IOException(
                    "The file was renamed, but it could not be reopened or restored to its original name.",
                    new AggregateException(reopenException, rollbackException));
            }

            throw;
        }
    }
}
