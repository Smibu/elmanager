using System;
using Elmanager.Lev;
using Elmanager.Rendering;
using SvgNet;
using System.Drawing;
using System.IO;
using System.Linq;
using Elmanager.Geometry;

namespace Elmanager.LevelEditor;

internal static class SvgExporter
{
    public static void ExportAsSvg(Level level, RenderingSettings settings, string fileName)
    {
        var g = new SvgGraphics(Color.LightGray);
        var blackPen = Pens.Black;
        var greenPen = Pens.Green;
        const int scale = 10;
        var m = Matrix.CreateTranslation(-level.XMin + 1, -level.YMin + 1) * Matrix.CreateScaling(scale, scale);
        var objOffset = new Vector(-0.4, -0.4);
        const float oSize = (float)0.8 * scale;
        level.Polygons.ForEach(p =>
        {
            if (p.IsGrass && settings.ShowGrassOrEdges)
            {
                g.DrawLines(greenPen, p
                    .ApplyTransformation(m)
                    .Vertices.Select(v => new PointF((float)v.X, (float)v.Y)).ToArray());
            }
            else if (!p.IsGrass && settings.ShowGroundOrEdges)
            {
                g.DrawPolygon(blackPen, p
                    .ApplyTransformation(m)
                    .Vertices.Select(v => new PointF((float)v.X, (float)v.Y)).ToArray());
            }
        });
        if (settings.ShowObjectsOrFrames)
        {
            level.Objects.ForEach(o =>
            {
                var pos = (o.Position + objOffset) * m;
                switch (o.Type)
                {
                    case ObjectType.Flower:
                        g.DrawEllipse(Pens.White, (float)pos.X, (float)pos.Y, oSize, oSize);
                        break;
                    case ObjectType.Apple:
                        g.DrawEllipse(Pens.Red, (float)pos.X, (float)pos.Y, oSize, oSize);
                        break;
                    case ObjectType.Killer:
                        g.DrawEllipse(Pens.Brown, (float)pos.X, (float)pos.Y, oSize, oSize);
                        break;
                    case ObjectType.Start:
                        g.DrawEllipse(Pens.Blue, (float)pos.X, (float)pos.Y, oSize, oSize);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        var svgBody = g.WriteSVGString();
        var width = (int)((level.Width + 2) * scale);
        var height = (int)((level.Height + 2) * scale);
        svgBody = svgBody.Replace("<svg ", $@"<svg width=""{width}"" height=""{height}"" ");
        File.WriteAllText(fileName, svgBody);
    }
}