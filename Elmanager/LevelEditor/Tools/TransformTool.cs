using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Input;
using Elmanager.Application;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.Rendering;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Elmanager.LevelEditor.Tools;

internal class TransformTool : ToolBase, IEditorTool
{
    private TransformState? _transformState;

    internal TransformTool(LevelEditorForm editor)
        : base(editor)
    {
    }

    private bool DisableRotation => Keyboard.IsKeyDown(Key.LeftShift);

    private bool DisableScaling => Keyboard.IsKeyDown(Key.LeftCtrl);

    private bool Transforming => _transformState?.TransformPolygonIndex is { };

    public void Activate()
    {
        List<LevObject> transformObjectReferences = new List<LevObject>();
        List<Polygon> transformPolygonReferences = new List<Polygon>();
        List<GraphicElement> transformTextureReferences = new List<GraphicElement>();
        int numberOfMarked = 0;
        var originalTransformPolygons = new List<Polygon>();
        var originalTransformObjects = new List<LevObject>();
        var originalTransformTextures = new List<GraphicElement>();
        double xMin = 0;
        double xMax = 0;
        double yMin = 0;
        double yMax = 0;

        void IncrementMarked(Vector z)
        {
            numberOfMarked++;
            if (numberOfMarked == 1)
            {
                xMin = z.X;
                xMax = z.X;
                yMin = z.Y;
                yMax = z.Y;
            }
            else
            {
                xMin = Math.Min(xMin, z.X);
                xMax = Math.Max(xMax, z.X);
                yMin = Math.Min(yMin, z.Y);
                yMax = Math.Max(yMax, z.Y);
            }
        }

        foreach (Polygon x in Lev.Polygons)
        {
            bool foundMarked = false;
            foreach (Vector z in x.Vertices)
            {
                if (z.Mark != VectorMark.Selected) continue;
                if (!foundMarked)
                {
                    transformPolygonReferences.Add(x);
                    originalTransformPolygons.Add(new Polygon(x));
                    foundMarked = true;
                }

                IncrementMarked(z);
            }
        }

        foreach (LevObject x in Lev.Objects)
        {
            if (x.Position.Mark != VectorMark.Selected) continue;
            originalTransformObjects.Add(new LevObject(x.Position, x.Type, x.AppleType,
                x.AnimationNumber));
            transformObjectReferences.Add(x);
            IncrementMarked(x.Position);
        }

        foreach (GraphicElement x in Lev.GraphicElements)
        {
            if (x.Position.Mark != VectorMark.Selected)
                continue;
            originalTransformTextures.Add(x with { });
            transformTextureReferences.Add(x);
            IncrementMarked(x.Position);
        }

        if (numberOfMarked > 1)
        {
            var transformRectangle = new Polygon();
            transformRectangle.Add(new Vector(xMin, yMin));
            transformRectangle.Add(new Vector(xMax, yMin));
            transformRectangle.Add(new Vector(xMax, yMax));
            transformRectangle.Add(new Vector(xMin, yMax));
            _transformState = new TransformState(originalTransformObjects, originalTransformPolygons,
                originalTransformTextures, transformRectangle);
            foreach (Polygon x in transformPolygonReferences)
            {
                Lev.Polygons.Remove(x);
                Lev.Polygons.Add(x);
            }

            foreach (LevObject x in transformObjectReferences)
            {
                Lev.Objects.Remove(x);
                Lev.Objects.Add(x);
            }

            foreach (GraphicElement x in transformTextureReferences)
            {
                Lev.GraphicElements.Remove(x);
                Lev.GraphicElements.Add(x);
            }

            UpdateHelp();
        }
        else
            InActivate();
    }

    public void ExtraRendering()
    {
        if (_transformState is null)
        {
            return;
        }

        var transformRectangle = _transformState.TransformRectangle;
        Renderer.DrawPolygon(transformRectangle, Color.Blue);
        Renderer.DrawLine((transformRectangle.Vertices[0] + transformRectangle.Vertices[1]) / 2,
            (transformRectangle.Vertices[2] + transformRectangle.Vertices[3]) / 2, Color.Blue);
        Renderer.DrawLine((transformRectangle.Vertices[1] + transformRectangle.Vertices[2]) / 2,
            (transformRectangle.Vertices[3] + transformRectangle.Vertices[0]) / 2, Color.Blue);
    }

    public void InActivate()
    {
        EndTransforming();
        LevEditor.SelectButton.Select();
        LevEditor.CurrentTool = LevEditor.Tools.SelectionTool;
        LevEditor.CurrentTool.Activate();
    }

    public void KeyDown(KeyEventArgs key)
    {
        if (key.KeyCode == Keys.Space)
        {
            InActivate();
        }
    }

    public void MouseDown(MouseEventArgs mouseData)
    {
        if (mouseData.Button != MouseButtons.Left
            || Transforming
            || _transformState is null)
        {
            return;
        }

        var transformRectangle = _transformState.TransformRectangle;
        for (int i = 0; i < 4; i++)
        {
            if ((transformRectangle[i] - CurrentPos).Length <
                Global.AppSettings.LevelEditor.CaptureRadius * ZoomCtrl.ZoomLevel)
            {
                _transformState.TransformPolygonIndex = i;
                break;
            }

            if (((transformRectangle[i] + transformRectangle[i + 1]) / 2 - CurrentPos).Length <
                Global.AppSettings.LevelEditor.CaptureRadius * ZoomCtrl.ZoomLevel)
            {
                _transformState.TransformPolygonIndex = i + 4;
                break;
            }
        }

        if (((transformRectangle[0] + transformRectangle[2]) / 2 - CurrentPos).Length <
            Global.AppSettings.LevelEditor.CaptureRadius * ZoomCtrl.ZoomLevel)
        {
            _transformState.TransformPolygonIndex = 8;
        }
    }

    public void MouseMove(Vector p)
    {
        CurrentPos = p;
        if (_transformState?.TransformPolygonIndex is not { } pi) return;
        var originalRectangle = _transformState.OriginalRectangle;
        Vector center = (originalRectangle.Vertices[0] + originalRectangle.Vertices[2]) / 2;
        Matrix transformMatrix = Matrix.Identity;
        transformMatrix.Translate(-center.X, -center.Y);
        Vector z;
        double scaleFactor;
        if (pi < 4)
        {
            z = originalRectangle.Vertices[pi];
            if (!DisableScaling)
            {
                scaleFactor = (p - center).Length / (z - center).Length;
                transformMatrix.Scale(scaleFactor, scaleFactor);
            }

            if (!DisableRotation)
                transformMatrix.Rotate(-(p - center).AngleBetween(z - center));
        }
        else if (_transformState.TransformPolygonIndex != 8)
        {
            z = (originalRectangle[pi - 4] +
                 originalRectangle[pi - 3]) / 2;
            double rotated =
                (originalRectangle.Vertices[1] - originalRectangle.Vertices[0]).Angle;
            if (!DisableScaling)
            {
                scaleFactor = (p - center).Length / (z - center).Length;
                transformMatrix.Rotate(rotated);
                if (pi % 2 == 0)
                    transformMatrix.Scale(1, scaleFactor);
                else
                    transformMatrix.Scale(scaleFactor, 1);
                transformMatrix.Rotate(-rotated);
            }

            if (!DisableRotation)
                transformMatrix.Rotate(-(p - center).AngleBetween(z - center));
        }
        else
        {
            var v = p - center;
            transformMatrix.Translate(v.X, v.Y);
        }

        transformMatrix.Translate(center.X, center.Y);
        _transformState.TransformRectangle = originalRectangle.ApplyTransformation(transformMatrix);
        for (int i = 0; i < _transformState.OriginalTransformPolygons.Count; i++)
        {
            Lev.Polygons[^(i + 1)] =
                _transformState.OriginalTransformPolygons[^(i + 1)].ApplyTransformation(transformMatrix, true);
            Lev.Polygons[^(i + 1)].UpdateDecompositionOrGrassSlopeInfo(Lev.GroundBounds,
                LevEditor.Settings.RenderingSettings.GrassZoom);
        }

        for (int i = 0; i < _transformState.OriginalTransformObjects.Count; i++)
            Lev.Objects[^(i + 1)].Position =
                _transformState.OriginalTransformObjects[^(i + 1)].Position * transformMatrix;
        for (int i = 0; i < _transformState.OriginalTransformTextures.Count; i++)
        {
            GraphicElement x = _transformState.OriginalTransformTextures[^(i + 1)];
            Lev.GraphicElements[^(i + 1)].Position = x.Position * transformMatrix;
        }
    }

    public void MouseOutOfEditor()
    {
    }

    public void MouseUp()
    {
        EndTransforming();
    }

    public void UpdateHelp()
    {
        LevEditor.InfoLabel.Text = "Space: done; Left Ctrl: rotate only; Left Shift: resize only";
    }

    private void EndTransforming()
    {
        if (_transformState?.TransformPolygonIndex is null) return;
        _transformState.TransformPolygonIndex = null;
        for (int i = 0; i < _transformState.OriginalTransformPolygons.Count; i++)
            _transformState.OriginalTransformPolygons[^(i + 1)] = new Polygon(Lev.Polygons[^(i + 1)]);
        for (int i = 0; i < _transformState.OriginalTransformObjects.Count; i++)
        {
            LevObject x = Lev.Objects[^(i + 1)];
            _transformState.OriginalTransformObjects[^(i + 1)] = new LevObject(
                x.Position, x.Type, x.AppleType, x.AnimationNumber);
        }

        for (int i = 0; i < _transformState.OriginalTransformTextures.Count; i++)
            _transformState.OriginalTransformTextures[^(i + 1)] = Lev.GraphicElements[^(i + 1)] with { };
        _transformState.OriginalRectangle = new Polygon(_transformState.TransformRectangle);
        LevEditor.SetModified(LevModification.Ground);
    }

    public override bool Busy => true;
}