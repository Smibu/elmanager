using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Elmanager.LevelEditor;
using Elmanager.SLE.Platform;
using CoreLgr = Elmanager.Lgr.Lgr;
using ILgrCache = Elmanager.Lgr.ILgrCache;

namespace Elmanager.SLE.LgrUtil;

internal class BookmarkLgrCache : ILgrCache
{
    private readonly Dictionary<string, CoreLgr> _loaded = new();
    private readonly Dictionary<string, string> _loadedKnownNames = new(StringComparer.OrdinalIgnoreCase);
    private readonly Func<IStorageProvider> _storageProviderFactory;
    private Dictionary<string, Bookmark> _droppedBookmarks = new(StringComparer.OrdinalIgnoreCase);

    private IStorageFolder? _folder;

    private Bookmark? _folderBookmark;

    public BookmarkLgrCache(Func<IStorageProvider> storageProviderFactory) =>
        _storageProviderFactory = storageProviderFactory;

    public bool HasFolder => _folderBookmark != null;

    public CoreLgr? TryGetLoaded(string lgrName) => _loaded.GetValueOrDefault(lgrName.ToLower());

    public async Task<CoreLgr?> GetOrLoadLgr(string lgrName)
    {
        var key = lgrName.ToLower();
        if (_loaded.TryGetValue(key, out var existing))
        {
            return existing;
        }

        var bytes = await LoadBytes(key);
        if (bytes == null)
        {
            return null;
        }

        _loadedKnownNames[key] = KnownLgrs.ResolveName(bytes);

        try
        {
            var lgr = new CoreLgr(new MemoryStream(bytes), key);
            _loaded[key] = lgr;
            return lgr;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to parse LGR '{key}': {ex.Message}");
            return null;
        }
    }

    public bool Configure(Bookmark? folderBookmark, IEnumerable<Bookmark> droppedBookmarks)
    {
        var indexedDroppedBookmarks = IndexDroppedBookmarks(droppedBookmarks);
        var folderChanged = folderBookmark != _folderBookmark;
        var droppedChanged = !DictionariesEqual(_droppedBookmarks, indexedDroppedBookmarks);
        var configurationChanged = folderChanged || droppedChanged;

        if (folderChanged)
        {
            _folderBookmark = folderBookmark;
            _folder = null;
        }

        if (droppedChanged)
        {
            _droppedBookmarks = indexedDroppedBookmarks;
        }

        if (configurationChanged)
        {
            _loaded.Clear();
            _loadedKnownNames.Clear();
        }

        return configurationChanged;
    }

    public bool IsDropped(string lgrName) => _droppedBookmarks.ContainsKey(lgrName);

    public bool TryGetLoadedKnownName(string lgrName, out string knownName)
    {
        if (_loadedKnownNames.TryGetValue(lgrName, out var loadedKnownName))
        {
            knownName = loadedKnownName;
            return true;
        }

        knownName = KnownLgrs.UnknownName;
        return false;
    }

    public async Task<List<LgrDropdownItem>> ListLgrs()
    {
        var entries = new Dictionary<string, LgrDropdownItem>(StringComparer.OrdinalIgnoreCase);

        var folder = await ResolveFolder();
        if (folder != null && _folderBookmark != null)
        {
            var fileNames = await folder.GetFileNamesAsync(_folderBookmark.Id, ".lgr");
            foreach (var fileName in fileNames)
            {
                var name = GetLgrName(fileName);
                if (name == null)
                {
                    continue;
                }

                using var file = await folder.GetFileAsync(fileName);
                if (file != null)
                {
                    entries[name] = await CreateFolderEntry(file, name);
                }
            }
        }

        foreach (var (name, bookmark) in _droppedBookmarks)
        {
            if (entries.ContainsKey(name))
            {
                continue;
            }

            entries[name] = await CreateDroppedEntry(bookmark, name);
        }

        return entries.Values.OrderBy(e => e.Filename, StringComparer.OrdinalIgnoreCase).ToList();
    }

    private async Task<byte[]?> LoadBytes(string name)
    {
        if (_droppedBookmarks.TryGetValue(name, out var bookmark))
        {
            using var droppedFile = await OpenFileBookmark(bookmark.Id);
            if (droppedFile != null)
            {
                return await ReadAll(droppedFile);
            }
        }

        var folder = await ResolveFolder();
        if (folder == null)
        {
            return null;
        }

        using var file = await folder.GetFileAsync(name + ".lgr");
        return file == null ? null : await ReadAll(file);
    }

    private async Task<IStorageFolder?> ResolveFolder()
    {
        if (_folder != null)
        {
            return _folder;
        }

        if (_folderBookmark == null)
        {
            return null;
        }

        var provider = _storageProviderFactory();

        try
        {
            _folder = await provider.OpenFolderBookmarkAsync(_folderBookmark.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to open LGR folder bookmark: {ex.Message}");
            _folder = null;
        }

        return _folder;
    }

    private static string? GetLgrName(string fileName)
    {
        var name = Path.GetFileNameWithoutExtension(fileName).ToLowerInvariant();
        return name.Length <= 8 ? name : null;
    }

    private async Task<LgrDropdownItem> CreateDroppedEntry(Bookmark bookmark, string name)
    {
        if (_loadedKnownNames.TryGetValue(name, out var knownName))
        {
            return new LgrDropdownItem(name, knownName, LgrSource.Dropped, LgrAvailability.Found);
        }

        using var file = await OpenFileBookmark(bookmark.Id);
        return file == null
            ? new LgrDropdownItem(name, KnownLgrs.UnknownName, LgrSource.Dropped, LgrAvailability.NotFound)
            : await CreateEntry(file, name, LgrSource.Dropped);
    }

    private static Task<LgrDropdownItem> CreateFolderEntry(IStorageFile file, string name) =>
        CreateEntry(file, name, LgrSource.Folder);

    private static async Task<LgrDropdownItem> CreateEntry(
        IStorageFile file,
        string name,
        LgrSource source)
    {
        try
        {
            var knownName = KnownLgrs.ResolveName(await ReadAll(file));
            return new LgrDropdownItem(name, knownName, source, LgrAvailability.Found);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to read LGR '{name}': {ex.Message}");
            return new LgrDropdownItem(name, KnownLgrs.UnknownName, source, LgrAvailability.NotFound);
        }
    }

    private async Task<IStorageFile?> OpenFileBookmark(string bookmark)
    {
        var provider = _storageProviderFactory();

        try
        {
            return await provider.OpenFileBookmarkAsync(bookmark);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to open dropped LGR bookmark: {ex.Message}");
            return null;
        }
    }

    private static async Task<byte[]> ReadAll(IStorageFile file)
    {
        await using var stream = await file.OpenReadAsync();
        using var mem = new MemoryStream();
        await stream.CopyToAsync(mem);
        return mem.ToArray();
    }

    private static Dictionary<string, Bookmark> IndexDroppedBookmarks(IEnumerable<Bookmark> bookmarks)
    {
        var indexed = new Dictionary<string, Bookmark>(StringComparer.OrdinalIgnoreCase);
        foreach (var bookmark in bookmarks)
        {
            var name = Path.GetFileNameWithoutExtension(bookmark.DisplayName).ToLowerInvariant();
            indexed[name] = bookmark;
        }

        return indexed;
    }

    private static bool DictionariesEqual(
        IReadOnlyDictionary<string, Bookmark> a,
        IReadOnlyDictionary<string, Bookmark> b)
    {
        if (a.Count != b.Count)
        {
            return false;
        }

        foreach (var (key, value) in a)
        {
            if (!b.TryGetValue(key, out var other) || other != value)
            {
                return false;
            }
        }

        return true;
    }
}
