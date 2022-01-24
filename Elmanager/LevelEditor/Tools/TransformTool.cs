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
    private Polygon _originalRectangle;
    private List<LevObject> _originalTransformObjects;
    private List<Polygon> _originalTransformPolygons;
    private List<Picture> _originalTransformTextures;
    private int _transformPolygonIndex;
    private Polygon _transformRectangle;

    internal TransformTool(LevelEditorForm editor)
        : base(editor)
    {
    }

    private bool DisableRotation => Keyboard.IsKeyDown(Key.LeftShift);

    private bool DisableScaling => Keyboard.IsKeyDown(Key.LeftCtrl);

    private bool Transforming { get; set; }

    public void Activate()
    {
        List<LevObject> transformObjectReferences = new List<LevObject>();
        List<Polygon> transformPolygonReferences = new List<Polygon>();
        List<Picture> transformTextureReferences = new List<Picture>();
        int numberOfMarked = 0;
        _originalTransformPolygons = new List<Polygon>();
        _originalTransformObjects = new List<LevObject>();
        _originalTransformTextures = new List<Picture>();
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
                    _originalTransformPolygons.Add(new Polygon(x));
                    foundMarked = true;
                }

                IncrementMarked(z);
            }
        }

        foreach (LevObject x in Lev.Objects)
        {
            if (x.Position.Mark != VectorMark.Selected) continue;
            _originalTransformObjects.Add(new LevObject(x.Position, x.Type, x.AppleType,
                x.AnimationNumber));
            transformObjectReferences.Add(x);
            IncrementMarked(x.Position);
        }

        foreach (Picture x in Lev.Pictures)
        {
            if (x.Position.Mark != VectorMark.Selected)
                continue;
            _originalTransformTextures.Add(x.Clone());
            transformTextureReferences.Add(x);
            IncrementMarked(x.Position);
        }

        if (numberOfMarked > 1)
        {
            _transformRectangle = new Polygon();
            _transformRectangle.Add(new Vector(xMin, yMin));
            _transformRectangle.Add(new Vector(xMax, yMin));
            _transformRectangle.Add(new Vector(xMax, yMax));
            _transformRectangle.Add(new Vector(xMin, yMax));
            _originalRectangle = new Polygon(_transformRectangle);
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

            foreach (Picture x in transformTextureReferences)
            {
                Lev.Pictures.Remove(x);
                Lev.Pictures.Add(x);
            }

            UpdateHelp();
        }
        else
            InActivate();
    }

    public void ExtraRendering()
    {
        if (_transformRectangle == null)
        {
            return;
        }
        Renderer.DrawPolygon(_transformRectangle, Color.Blue);
        Renderer.DrawLine((_transformRectangle.Vertices[0] + _transformRectangle.Vertices[1]) / 2,
            (_transformRectangle.Vertices[2] + _transformRectangle.Vertices[3]) / 2, Color.Blue);
        Renderer.DrawLine((_transformRectangle.Vertices[1] + _transformRectangle.Vertices[2]) / 2,
            (_transformRectangle.Vertices[3] + _transformRectangle.Vertices[0]) / 2, Color.Blue);
    }

    public List<Polygon> GetExtraPolygons()
    {
        return new();
    }

    public void InActivate()
    {
        EndTransforming();
        LevEditor.SelectButton.Select();
        LevEditor.CurrentTool = LevEditor.Tools[0];
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
            || _transformRectangle == null)
        {
            return;
        }

        for (int i = 0; i < 4; i++)
        {
            if ((_transformRectangle[i] - CurrentPos).Length <
                Global.AppSettings.LevelEditor.CaptureRadius * ZoomCtrl.ZoomLevel)
            {
                _transformPolygonIndex = i;
                Transforming = true;
                break;
            }

            if (((_transformRectangle[i] + _transformRectangle[i + 1]) / 2 - CurrentPos).Length <
                Global.AppSettings.LevelEditor.CaptureRadius * ZoomCtrl.ZoomLevel)
            {
                _transformPolygonIndex = i + 4;
                Transforming = true;
                break;
            }
        }

        if (((_transformRectangle[0] + _transformRectangle[2]) / 2 - CurrentPos).Length <
            Global.AppSettings.LevelEditor.CaptureRadius * ZoomCtrl.ZoomLevel)
        {
            _transformPolygonIndex = 8;
            Transforming = true;
        }
    }

    public void MouseMove(Vector p)
    {
        CurrentPos = p;
        if (!Transforming) return;
        Vector center = (_originalRectangle.Vertices[0] + _originalRectangle.Vertices[2]) / 2;
        Matrix transformMatrix = Matrix.Identity;
        transformMatrix.Translate(-center.X, -center.Y);
        Vector z;
        double scaleFactor;
        if (_transformPolygonIndex < 4)
        {
            z = _originalRectangle.Vertices[_transformPolygonIndex];
            if (!DisableScaling)
            {
                scaleFactor = (p - center).Length / (z - center).Length;
                transformMatrix.Scale(scaleFactor, scaleFactor);
            }

            if (!DisableRotation)
                transformMatrix.Rotate(-(p - center).AngleBetween(z - center));
        }
        else if (_transformPolygonIndex != 8)
        {
            z = (_originalRectangle[_transformPolygonIndex - 4] +
                 _originalRectangle[_transformPolygonIndex - 3]) / 2;
            double rotated =
                (_originalRectangle.Vertices[1] - _originalRectangle.Vertices[0]).Angle;
            if (!DisableScaling)
            {
                scaleFactor = (p - center).Length / (z - center).Length;
                transformMatrix.Rotate(rotated);
                if (_transformPolygonIndex % 2 == 0)
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
        _transformRectangle = _originalRectangle.ApplyTransformation(transformMatrix);
        for (int i = 0; i < _originalTransformPolygons.Count; i++)
        {
            Lev.Polygons[Lev.Polygons.Count - 1 - i] =
                _originalTransformPolygons[i].ApplyTransformation(transformMatrix, true);
            Lev.Polygons[Lev.Polygons.Count - 1 - i].UpdateDecomposition();
        }

        for (int i = 0; i < _originalTransformObjects.Count; i++)
            Lev.Objects[Lev.Objects.Count - 1 - i].Position =
                _originalTransformObjects[_originalTransformObjects.Count - 1 - i].Position * transformMatrix;
        for (int i = 0; i < _originalTransformTextures.Count; i++)
        {
            Picture x = _originalTransformTextures[_originalTransformTextures.Count - 1 - i];
            Lev.Pictures[Lev.Pictures.Count - 1 - i].Position =
                x.Position * transformMatrix;
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
        if (!Transforming) return;
        Transforming = false;
        for (int i = 0; i < _originalTransformPolygons.Count; i++)
            _originalTransformPolygons[i] = new Polygon(Lev.Polygons[Lev.Polygons.Count - 1 - i]);
        for (int i = 0; i < _originalTransformObjects.Count; i++)
        {
            LevObject x = Lev.Objects[Lev.Objects.Count - 1 - i];
            _originalTransformObjects[_originalTransformObjects.Count - 1 - i] = new LevObject(
                x.Position, x.Type, x.AppleType, x.AnimationNumber);
        }

        for (int i = 0; i < _originalTransformTextures.Count; i++)
            _originalTransformTextures[i] = Lev.Pictures[Lev.Pictures.Count - 1 - i].Clone();
        _originalRectangle = new Polygon(_transformRectangle);
        LevEditor.SetModified(LevModification.Ground);
    }

    public override bool Busy => true;
}