using System;
using System.Collections;
using System.Drawing;
using Elmanager.Geometry;
using Elmanager.Lev;
using vectrast;

namespace Elmanager.LevelEditor;

internal static class VectrastWrapper
{
    internal static Level LoadLevelFromImage(string imageFileName)
    {
        var lev = new Level();
        var vr = new VectRast(false, false);
        byte[,] pixelOn;
        Bitmap bmp;
        var transformMatrix = Matrix2D.identityM();
        const int numFlowers = 0;
        try
        {
            vr.loadAsBmp(imageFileName, out bmp, out pixelOn,
                Math.Abs(transformMatrix.elements[0, 0]) + Math.Abs(transformMatrix.elements[1, 1]),
                1, numFlowers);
        }
        catch (ArgumentException)
        {
            throw new VectrastException(string.Format("The image file {0} is invalid.", imageFileName));
        }

        try
        {
            vr.collapseVectors(vr.createVectors(pixelOn, bmp));
        }
        catch (Exception e)
        {
            throw new VectrastException(e.Message);
        }

        transformMatrix = Matrix2D.translationM(-bmp.Width / 2.0, -bmp.Height / 2.0) * transformMatrix;
        transformMatrix = transformMatrix * Matrix2D.scaleM(0.1, 0.1);
        bmp.Dispose();

        try
        {
            vr.transformVectors(transformMatrix);
        }
        catch (Exception e)
        {
            throw new VectrastException(e.Message);
        }

        if (vr.polygons.Count == 0)
        {
            throw new VectrastException(string.Format("Failed to vectorize the image file {0}.", imageFileName));
        }

        foreach (ArrayList polygon in vr.polygons)
        {
            var elmaPolygon = new Polygon();
            foreach (DoubleVector2 vertex in polygon)
            {
                elmaPolygon.Add(new Vector(vertex.x, vertex.y));
            }

            lev.Polygons.Add(elmaPolygon);
        }

        return lev;
    }
}