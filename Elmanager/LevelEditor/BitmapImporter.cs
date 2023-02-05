using System;
using System.Drawing;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.Vectrast;

namespace Elmanager.LevelEditor;

internal static class BitmapImporter
{
    internal static Level FromPath(string imageFileName)
    {
        var lev = new Level();
        var vr = new VectRast();
        byte[,] pixelOn;
        Bitmap bmp;
        var transformMatrix = Matrix2D.ScaleM(1, -1);
        try
        {
            vr.LoadAsBmp(imageFileName, out bmp, out pixelOn, 1);
        }
        catch (ArgumentException)
        {
            throw new ImportException($"The image file {imageFileName} is invalid.");
        }

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
        bmp.Dispose();

        try
        {
            vr.TransformVectors(transformMatrix);
        }
        catch (Exception e)
        {
            throw new ImportException(e.Message);
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