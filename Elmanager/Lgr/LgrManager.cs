using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

namespace Elmanager.Lgr;

internal class LgrManager
{
    private readonly Dictionary<string, string> _knownLgrs;
    private readonly Dictionary<string, string> _lgrFileHashes;

    public LgrManager(string lgrFolderPath)
    {
        var jsonString = Properties.Resources.Lgrs;
        var lgrs = JsonSerializer.Deserialize<List<KnownLgrEntry>>(jsonString)!;
        _knownLgrs = lgrs.ToDictionary(l => l.Hash, l => l.Name);
        _lgrFileHashes = ReadFileSha256Hashes(lgrFolderPath);
    }

    public IEnumerable<LgrEntry> GetLgrs() => _lgrFileHashes.Select(name =>
        new LgrEntry(name.Key, _knownLgrs.GetValueOrDefault(name.Value) ?? "Unknown"));

    private static Dictionary<string, string> ReadFileSha256Hashes(string folderPath)
    {
        var files = new Dictionary<string, string>();
        using var sha256Hash = SHA256.Create();
        foreach (var file in Directory.EnumerateFiles(folderPath, "*.lgr", SearchOption.AllDirectories))
        {
            using var stream = File.OpenRead(file);
            var hash = sha256Hash.ComputeHash(stream);
            files.Add(Path.GetFileNameWithoutExtension(file).ToLower(), BitConverter.ToString(hash).Replace("-", ""));
        }

        return files;
    }
}