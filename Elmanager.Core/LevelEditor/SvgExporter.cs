using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.Rendering;

namespace Elmanager.LevelEditor;

public static class SvgExporter
{
    public static void ExportAsSvg(Level level, RenderingSettings settings, string fileName)
    {
        const int scale = 10;
        var m = Matrix.CreateTranslation(-level.Bounds.XMin + 1, -level.Bounds.YMax - 1) * Matrix.CreateScaling(scale, -scale);
        var objOffset = new Vector(-0.4, 0.4);
        const float oSize = (float)0.8 * scale;
        var width = (int)((level.Width + 2) * scale);
        var height = (int)((level.Height + 2) * scale);

        var sb = new StringBuilder();
        sb.AppendLine(CultureInfo.InvariantCulture, $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{width}"" height=""{height}"">");
        sb.AppendLine(@"<rect width=""100%"" height=""100%"" fill=""lightgray""/>");

        level.Polygons.ForEach(p =>
        {
            var points = string.Join(" ", p
                .ApplyTransformation(m)
                .Vertices.Select(v => FormattableString.Invariant($"{v.X:F2},{v.Y:F2}")));

            if (p.IsGrass && settings.ShowGrassOrEdges)
            {
                sb.AppendLine(CultureInfo.InvariantCulture, $@"<polyline points=""{points}"" fill=""none"" stroke=""green"" stroke-width=""1""/>");
            }
            else if (!p.IsGrass && settings.ShowGroundOrEdges)
            {
                sb.AppendLine(CultureInfo.InvariantCulture, $@"<polygon points=""{points}"" fill=""none"" stroke=""black"" stroke-width=""1""/>");
            }
        });

        if (settings.ShowObjectsOrFrames)
        {
            level.Objects.ForEach(o =>
            {
                var pos = (o.Position + objOffset) * m;
                var color = o.Type switch
                {
                    ObjectType.Flower => "white",
                    ObjectType.Apple => "red",
                    ObjectType.Killer => "brown",
                    ObjectType.Start => "blue",
                    _ => throw new ArgumentOutOfRangeException()
                };
                var cx = pos.X + oSize / 2;
                var cy = pos.Y + oSize / 2;
                var rx = oSize / 2;
                var ry = oSize / 2;
                sb.AppendLine(CultureInfo.InvariantCulture, $@"<ellipse cx=""{cx:F2}"" cy=""{cy:F2}"" rx=""{rx:F2}"" ry=""{ry:F2}"" fill=""none"" stroke=""{color}"" stroke-width=""1""/>");
            });
        }

        sb.AppendLine("</svg>");
        File.WriteAllText(fileName, sb.ToString());
    }
}
