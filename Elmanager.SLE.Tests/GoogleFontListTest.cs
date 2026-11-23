using System;
using System.IO;
using System.Linq;
using Elmanager.SLE.Editor.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elmanager.SLE.Tests;

[TestClass]
public class GoogleFontListTest
{
    [TestMethod]
    public void ParsesFamiliesAndStylePaths()
    {
        const string text = "Abel\tabel/v18/a.ttf\t\t\t\nRoboto\tr.ttf\tb.ttf\ti.ttf\tbi.ttf\nNoto Mono\n";

        var fonts = SleTypefaceProvider.ParseFontList(text);

        CollectionAssert.AreEqual(new[] { "Abel", "Roboto", "Noto Mono" }, fonts.Select(f => f.Family).ToArray());
        CollectionAssert.AreEqual(new[] { "abel/v18/a.ttf", null, null, null }, fonts[0].Paths);
        CollectionAssert.AreEqual(new[] { "r.ttf", "b.ttf", "i.ttf", "bi.ttf" }, fonts[1].Paths);
        CollectionAssert.AreEqual(new string?[] { null, null, null, null }, fonts[2].Paths);
    }

    [TestMethod]
    public void BundledFontListIsWellFormed()
    {
        var lines = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "TestData", "googlefonts.tsv"));

        Assert.IsTrue(lines.Length > 1000, "Expected the full Google Fonts family list.");
        foreach (var line in lines)
        {
            var columns = line.Split('\t');
            Assert.AreEqual(5, columns.Length, $"Expected family + 4 style columns: '{line}'");
            foreach (var path in columns.Skip(1).Where(p => p.Length > 0))
            {
                Assert.IsTrue(path.EndsWith(".ttf") || path.EndsWith(".otf"), $"Not a TTF/OTF path: '{path}'");
            }
        }

        var roboto = SleTypefaceProvider.ParseFontList(string.Join('\n', lines)).Single(f => f.Family == "Roboto");
        Assert.IsTrue(roboto.Paths.All(p => p != null), "Roboto should have all four styles.");
    }
}
