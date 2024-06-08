using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

namespace Elmanager.Lgr;

internal class LgrManager
{
    private readonly Dictionary<string, string> _knownLgrs;
    private readonly Dictionary<string, string> _lgrFileHashes;

    private static readonly Dictionary<string, Size> DefaultPictureSizes = new()
    {
        { "q1body", new Size(149, 101) },
        { "q2body", new Size(149, 101) },
        { "q1thigh", new Size(168, 60) },
        { "q2thigh", new Size(168, 60) },
        { "q1leg", new Size(159, 88) },
        { "q2leg", new Size(159, 88) },
        { "q1forarm", new Size(238, 68) },
        { "q2forarm", new Size(238, 68) },
        { "q1bike", new Size(380, 301) },
        { "q2bike", new Size(380, 301) },
        { "q1susp1", new Size(240, 19) },
        { "q2susp1", new Size(240, 19) },
        { "q1head", new Size(189, 189) },
        { "q2head", new Size(189, 189) },
        { "q1up_arm", new Size(180, 72) },
        { "q2up_arm", new Size(180, 72) },
        { "q1susp2", new Size(238, 30) },
        { "q2susp2", new Size(238, 30) },
        { "q1wheel", new Size(240, 240) },
        { "q2wheel", new Size(240, 240) },
        { "qflag", new Size(62, 39) },
        { "qexit", new Size(2000, 40) },
        { "qkiller", new Size(1320, 40) },
        { "qcolors", new Size(66, 109) },
        { "qframe", new Size(207, 254) },
        { "qgrass", new Size(46, 35) },
        { "qdown_1", new Size(8, 42) },
        { "qdown_14", new Size(10, 55) },
        { "qdown_5", new Size(11, 46) },
        { "qdown_9", new Size(14, 50) },
        { "qup_0", new Size(8, 41) },
        { "qup_1", new Size(8, 42) },
        { "qup_14", new Size(10, 55) },
        { "qup_5", new Size(11, 46) },
        { "qup_9", new Size(14, 50) },
        { "qup_18", new Size(10, 59) },
        { "qdown_18", new Size(10, 59) },
        { "cliff", new Size(60, 61) },
        { "stone1", new Size(193, 237) },
        { "stone2", new Size(207, 253) },
        { "stone3", new Size(138, 168) },
        { "st3top", new Size(138, 168) },
        { "brick", new Size(82, 117) },
        { "qfood1", new Size(1360, 40) },
        { "qfood2", new Size(2040, 40) },
        { "bridge", new Size(180, 119) },
        { "sky", new Size(640, 560) },
        { "tree2", new Size(275, 203) },
        { "bush3", new Size(140, 107) },
        { "tree4", new Size(340, 337) },
        { "tree5", new Size(140, 210) },
        { "log2", new Size(190, 138) },
        { "sedge", new Size(139, 91) },
        { "tree3", new Size(286, 227) },
        { "plantain", new Size(100, 75) },
        { "bush1", new Size(142, 80) },
        { "bush2", new Size(115, 96) },
        { "ground", new Size(280, 31) },
        { "flag", new Size(44, 111) },
        { "secret", new Size(115, 69) },
        { "hang", new Size(80, 89) },
        { "edge", new Size(60, 45) },
        { "mushroom", new Size(85, 80) },
        { "log1", new Size(94, 58) },
        { "tree1", new Size(100, 176) },
        { "maskbig", new Size(200, 306) },
        { "maskhor", new Size(339, 113) },
        { "masklitt", new Size(10, 10) },
        { "barrel", new Size(74, 76) },
        { "supphred", new Size(158, 31) },
        { "suppvred", new Size(31, 159) },
        { "support2", new Size(163, 161) },
        { "support3", new Size(163, 161) },
        { "support1", new Size(42, 204) },
        { "suspdown", new Size(40, 59) },
        { "suspup", new Size(40, 59) },
        { "susp", new Size(10, 100) },
    };

    public string LgrFolderPath { get; }

    public LgrManager(string lgrFolderPath)
    {
        LgrFolderPath = lgrFolderPath;
        var jsonString = Properties.Resources.Lgrs;
        var lgrs = JsonSerializer.Deserialize<List<KnownLgrEntry>>(jsonString)!;
        _knownLgrs = lgrs.ToDictionary(l => l.Hash, l => l.Name);
        _lgrFileHashes = ReadFileSha256Hashes(lgrFolderPath);
    }

    public static Size GetDefaultSize(string name) => name.StartsWith("zz")
        ? new Size(200, 200)
        : DefaultPictureSizes.GetValueOrDefault(name, new Size(10, 10));

    public IEnumerable<LgrEntry> GetLgrs() => _lgrFileHashes.Select(name =>
        new LgrEntry(name.Key, _knownLgrs.GetValueOrDefault(name.Value) ?? "Unknown"));

    private static Dictionary<string, string> ReadFileSha256Hashes(string lgrFolderPath)
    {
        var files = new Dictionary<string, string>();
        using var sha256Hash = SHA256.Create();
        foreach (var file in Directory.EnumerateFiles(lgrFolderPath, "*.lgr", SearchOption.TopDirectoryOnly)
                     .Where(f => Path.GetFileNameWithoutExtension(f).Length <= 8))
        {
            using var stream = File.OpenRead(file);
            var hash = sha256Hash.ComputeHash(stream);
            files.Add(Path.GetFileNameWithoutExtension(file).ToLower(), BitConverter.ToString(hash).Replace("-", ""));
        }

        return files;
    }
}