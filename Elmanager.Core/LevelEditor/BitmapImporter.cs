using System;
using System.IO;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.Vectrast;
using SkiaSharp;

namespace Elmanager.LevelEditor;

public static class BitmapImporter
{
    public static Level FromPath(string imageFileName)
    {
        using var stream = File.OpenRead(imageFileName);
        return FromStream(stream, imageFileName);
    }

    public static Level FromStream(Stream imageStream, string imageFileName)
    {
        var lev = new Level();
        var vr = new VectRast();
        byte[,] pixelOn;
        SKBitmap bmp;
        var transformMatrix = Matrix2D.ScaleM(1, -1);
        try
        {
            vr.LoadAsBmp(imageStream, out bmp, out pixelOn, 1);
        }
        catch (ArgumentException)
        {
            throw new ImportException($"The image file {imageFileName} is invalid.");
        }

        try
        {
            try
            {
                vr.CollapseVectors(vr.CreateVectors(pixelOn, bmp));
            }
            catch (Exception e)
            {
                throw new ImportException(e.Message);
            }

            transformMatrix = Matrix2D.TranslationM(-bmp.Width / 2.0, -bmp.Height / 2.0) * transformMatrix;
            transformMatrix = transformMatrix * Matrix2D.ScaleM(0.1, 0.1);

            try
            {
                vr.TransformVectors(transformMatrix);
            }
            catch (Exception e)
            {
                throw new ImportException(e.Message);
            }
        }
        finally
        {
            bmp.Dispose();
        }

        if (vr.Polygons.Count == 0)
        {
            throw new ImportException($"Failed to vectorize the image file {imageFileName}.");
        }

        foreach (var polygon in vr.Polygons)
        {
            var elmaPolygon = new Polygon();
            foreach (var vertex in polygon)
            {
                elmaPolygon.Add(new Vector(vertex.X, vertex.Y));
            }

            lev.Polygons.Add(elmaPolygon);
        }

        return lev;
    }
}
