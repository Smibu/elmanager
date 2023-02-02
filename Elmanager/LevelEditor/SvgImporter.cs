using System;
using System.IO;
using System.Linq;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.LevelEditor.Tools;
using NetTopologySuite.Geometries;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace Elmanager.LevelEditor;

internal static class SvgImporter
{
    public static Level FromStream(TextReader svgStream, SvgImportOptions newOpts)
    {
        var settings = new WpfDrawingSettings
        { IncludeRuntime = false, TextAsGeometry = true, IgnoreRootViewbox = true };
        using var converter = new FileSvgReader(settings);
        var drawingGroup = converter.Read(svgStream);

        var (polys, _) = TextTool.BuildPolygons(
            TextTool.CreateGeometry(drawingGroup, newOpts),
            new Vector(),
            newOpts.Smoothness,
            newOpts.UseOutlinedGeometry);

        try
        {
            TextTool.FinalizePolygons(polys);
        }
        catch (TopologyException)
        {
        }
        catch (ArgumentException)
        {
        }

        var m = Matrix.CreateScaling(1 / 10.0, 1 / 10.0);
        polys = polys.Select(p => p.ApplyTransformation(m)).ToList();
        var lev = new Level();
        lev.Polygons.AddRange(polys);
        return lev;
    }
}