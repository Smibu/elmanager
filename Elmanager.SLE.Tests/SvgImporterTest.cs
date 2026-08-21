using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.LevelEditor;
using Elmanager.SLE.Editor.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elmanager.SLE.Tests;

[TestClass]
public class SvgImporterTest
{
    [TestMethod]
    public void ImportsTransformedGeometryAtWinFormsScale()
    {
        var level = Import("transformed-rectangle.svg");

        Assert.HasCount(1, level.Polygons);
        AssertBounds(level, 3, 6, -5.5, -3.5);
    }

    [TestMethod]
    public void WidensOpenStrokes()
    {
        var level = Import("open-stroke.svg");

        Assert.HasCount(1, level.Polygons);
        AssertBounds(level, 1, 5, -2.5, -1.5);
    }

    [TestMethod]
    public void HonorsNeverWidenClosedPaths()
    {
        var widened = Import("closed-stroke.svg");
        var options = SvgImportOptions.Default;
        options.NeverWidenClosedPaths = true;
        var notWidened = Import("closed-stroke.svg", options);

        AssertBounds(widened, 0.5, 3.5, -3.5, -0.5);
        AssertBounds(notWidened, 1, 3, -3, -1);
    }

    [TestMethod]
    public void AppliesSelectedFillRule()
    {
        var evenOddOptions = SvgImportOptions.Default;
        evenOddOptions.UseOutlinedGeometry = true;
        evenOddOptions.FillRule = FillRule.EvenOdd;
        var nonzeroOptions = evenOddOptions;
        nonzeroOptions.FillRule = FillRule.Nonzero;

        var evenOdd = Import("fill-rule.svg", evenOddOptions);
        var nonzero = Import("fill-rule.svg", nonzeroOptions);

        Assert.HasCount(2, evenOdd.Polygons);
        Assert.HasCount(1, nonzero.Polygons);
    }

    [TestMethod]
    public void ImportsGzipCompressedSvg()
    {
        var regular = Import("transformed-rectangle.svg");
        using var source = File.OpenRead(FixturePath("transformed-rectangle.svg"));
        using var compressed = new MemoryStream();
        using (var gzip = new GZipStream(compressed, CompressionLevel.SmallestSize, leaveOpen: true))
        {
            source.CopyTo(gzip);
        }

        compressed.Position = 0;
        var imported = Import(compressed, compressed: true);

        Assert.HasCount(regular.Polygons.Count, imported.Polygons);
        AssertBounds(imported, GetBounds(regular));
    }

    [TestMethod]
    public void RejectsAnimatedSvg()
    {
        Assert.ThrowsExactly<ImportException>(() => Import("animated.svg"));
    }

    [TestMethod]
    public void ImportsCurvesAndShapePrimitives()
    {
        var level = Import("curves-and-primitives.svg");

        Assert.HasCount(6, level.Polygons);
        Assert.IsTrue(level.Polygons.All(polygon => polygon.Vertices.Count >= 3));
        Assert.IsTrue(level.Polygons.Any(polygon => polygon.Vertices.Count > 4));
        AssertBounds(level, 1, 12, -7, -1);
    }

    [TestMethod]
    public void AppliesNestedTransformsWithoutLeakingToSiblings()
    {
        var level = Import("nested-transforms.svg");
        var bounds = level.Polygons
            .Select(polygon => polygon.Bounds)
            .OrderBy(item => item.XMin)
            .ToList();

        Assert.HasCount(2, bounds);
        AssertBounds(bounds[0], 3, 5, -4, -2);
        AssertBounds(bounds[1], 10, 11, -3, -2);
    }

    [TestMethod]
    public void ImportsFillAndStrokeFromSameElement()
    {
        var strokeOnly = Import("closed-stroke.svg");
        var fillAndStroke = Import("fill-and-stroke.svg");

        Assert.AreEqual(strokeOnly.Polygons.Count + 1, fillAndStroke.Polygons.Count);
        AssertBounds(fillAndStroke, 0.5, 3.5, -3.5, -0.5);
    }

    [TestMethod]
    public void ResolvesGroupedUseElements()
    {
        var level = Import("uses-and-groups.svg");

        Assert.HasCount(2, level.Polygons);
        AssertBounds(level, 1, 5, -3, -1);
    }

    [TestMethod]
    public void AppliesRoundStrokeCaps()
    {
        var level = Import("round-cap.svg");

        Assert.HasCount(1, level.Polygons);
        AssertBounds(level, 0.5, 5.5, -2.5, -1.5);
    }

    [TestMethod]
    public void AppliesStrokeJoinStyles()
    {
        var level = Import("stroke-joins.svg");
        var bounds = level.Polygons
            .Select(polygon => polygon.Bounds)
            .OrderBy(item => item.XMin)
            .ToList();

        Assert.HasCount(2, bounds);
        Assert.IsTrue(
            bounds[0].YMax > bounds[1].YMax + 0.5,
            "The miter join should extend farther than the bevel join.");
    }

    [TestMethod]
    public void ConvertsTextToPolygons()
    {
        var level = Import("text.svg");

        Assert.IsNotEmpty(level.Polygons);
        Assert.IsTrue(level.Polygons.Sum(polygon => polygon.Vertices.Count) > 10);
    }

    private static Level Import(string fileName, SvgImportOptions? options = null)
    {
        using var stream = File.OpenRead(FixturePath(fileName));
        return Import(stream, compressed: false, options);
    }

    private static Level Import(Stream stream, bool compressed, SvgImportOptions? options = null)
    {
        var level = SvgImporter.FromStream(stream, compressed, options ?? SvgImportOptions.Default);
        AssertNoRepeatedClosingVertices(level);
        return level;
    }

    private static string FixturePath(string fileName) =>
        Path.Combine(AppContext.BaseDirectory, "TestData", "SvgImporter", fileName);

    private static void AssertBounds(
        Level level,
        double minX,
        double maxX,
        double minY,
        double maxY)
    {
        AssertBounds(GetBounds(level), minX, maxX, minY, maxY);
    }

    private static void AssertBounds(Level level, Bounds expected)
    {
        AssertBounds(GetBounds(level), expected);
    }

    private static void AssertBounds(Bounds actual, Bounds expected)
    {
        AssertBounds(actual, expected.XMin, expected.XMax, expected.YMin, expected.YMax);
    }

    private static void AssertBounds(
        Bounds actual,
        double minX,
        double maxX,
        double minY,
        double maxY)
    {
        Assert.AreEqual(minX, actual.XMin, 0.001);
        Assert.AreEqual(maxX, actual.XMax, 0.001);
        Assert.AreEqual(minY, actual.YMin, 0.001);
        Assert.AreEqual(maxY, actual.YMax, 0.001);
    }

    private static Bounds GetBounds(Level level)
    {
        Assert.IsNotEmpty(level.Polygons);
        return level.Polygons
            .Select(polygon => polygon.Bounds)
            .Aggregate((bounds, polygonBounds) => bounds.Max(polygonBounds));
    }

    private static void AssertNoRepeatedClosingVertices(Level level)
    {
        foreach (var polygon in level.Polygons)
        {
            Assert.IsNotEmpty(polygon.Vertices);
            var first = polygon.Vertices[0];
            var last = polygon.Vertices[^1];
            Assert.IsFalse(
                first.X == last.X && first.Y == last.Y,
                "An imported polygon must not repeat its first vertex as its last vertex.");
        }
    }
}
