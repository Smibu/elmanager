using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Elmanager.Application;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.Utilities;

namespace Elmanager.LevelEditor.Tools;

internal class AutoGrassTool : ToolBase, IEditorTool
{
    private const double GrassHeight = 1.0;
    private const double MaximumGrassAngle = 60.0;
    private Polygon? _currentPolygon;
    private List<Polygon>? _currentAutograssPolys;

    internal AutoGrassTool(LevelEditorForm editor)
        : base(editor)
    {
    }

    private bool AutoGrassPolygonSelected => _currentPolygon is { };

    public void Activate()
    {
        UpdateHelp();
    }

    public void CancelAutoGrass()
    {
        if (!AutoGrassPolygonSelected) return;
        _currentPolygon = null;
        _currentAutograssPolys = null;
        ResetPolygonMarks();
    }

    public void ExtraRendering()
    {
        if (_currentAutograssPolys is { })
        {
            _currentAutograssPolys.ForEach(poly => Renderer.DrawLineStrip(poly, Color.Red));
        }
    }

    public List<Polygon> GetExtraPolygons()
    {
        return new();
    }

    public void InActivate()
    {
        CancelAutoGrass();
    }

    public void KeyDown(KeyEventArgs key)
    {
        if (_currentPolygon is { })
        {
            var changed = true;
            switch (key.KeyCode)
            {
                case KeyUtils.Increase:
                    Global.AppSettings.LevelEditor.AutoGrassThickness += 0.025;
                    break;
                case KeyUtils.Decrease:
                    if (Global.AppSettings.LevelEditor.AutoGrassThickness > 0.025)
                        Global.AppSettings.LevelEditor.AutoGrassThickness -= 0.025;
                    break;
                default:
                    changed = false;
                    break;
            }

            if (changed)
            {
                _currentAutograssPolys = AutoGrass(_currentPolygon);
                UpdateHelp();
            }
        }
    }

    public void MouseDown(MouseEventArgs mouseData)
    {
        switch (mouseData.Button)
        {
            case MouseButtons.Left:
                if (_currentPolygon is null)
                {
                    if (GetNearestVertexInfo(CurrentPos) is { } v && !v.Polygon.IsGrass)
                    {
                        _currentPolygon = v.Polygon;
                        _currentAutograssPolys = AutoGrass(_currentPolygon);
                    }
                }
                else
                {
                    Lev.Polygons.AddRange(AutoGrass(_currentPolygon));
                    _currentPolygon = null;
                    _currentAutograssPolys = null;
                    LevEditor.SetModified(LevModification.Decorations);
                }

                break;
            case MouseButtons.Right:
                CancelAutoGrass();
                break;
        }

        UpdateHelp();
    }

    public void MouseMove(Vector p)
    {
        CurrentPos = p;
        if (!AutoGrassPolygonSelected)
        {
            ResetHighlight();
            if (GetNearestVertexInfo(p) is { } v)
            {
                ChangeCursorToHand();
                v.Polygon.Mark = PolygonMark.Highlight;
            }
            else
                ChangeToDefaultCursorIfHand();
        }
        else
            ChangeToDefaultCursorIfHand();
    }

    public void MouseOutOfEditor()
    {
        if (AutoGrassPolygonSelected) return;
        ResetHighlight();
    }

    public void MouseUp()
    {
    }

    public void UpdateHelp()
    {
        LevEditor.InfoLabel.Text = AutoGrassPolygonSelected
            ? "Left mouse: apply AutoGrass, right mouse: cancel. Thickness: "
              + Global.AppSettings.LevelEditor.AutoGrassThickness.ToString("F3")
            : "Click the ground polygon to create grass polygon for.";
    }

    internal List<Polygon> AutoGrass(Polygon p)
    {
        double autoGrassThickness = Global.AppSettings.LevelEditor.AutoGrassThickness;
        List<Polygon> grassPolys = new List<Polygon>();
        bool isSky = Lev.IsSky(p);
        // TODO: Make autograss work without the WithYNegated hack.
        p = p.WithYNegated();
        if (!(isSky ^ p.IsCounterClockwise))
            p.ChangeOrientation();
        int i;
        int j;
        for (i = 0; i < p.Vertices.Count; i++)
        {
            Vector v = p[i + 1] - p[i];
            if (Math.Abs(v.Angle) > MaximumGrassAngle)
                break;
        }

        for (j = i; j < p.Vertices.Count; j++)
            if (Math.Abs((p[j + 1] - p[j]).Angle) <= MaximumGrassAngle)
                break;
        var exploredVerts = new HashSet<int>();
        j = j % p.Vertices.Count;
        while (!exploredVerts.Contains(j))
        {
            while (!(Math.Abs((p[j + 1] - p[j]).Angle) <= MaximumGrassAngle))
            {
                exploredVerts.Add(j);
                j++;
                j = j % p.Vertices.Count;
                if (exploredVerts.Contains(j))
                    return grassPolys.Select(x => x.WithYNegated()).ToList();
            }

            exploredVerts.Add(j);
            Vector directionVector = p[j + 1] - p[j];
            Vector previousDirectionVector = p[j] - p[j - 1];
            Vector previousDirectionVectorNeg = -previousDirectionVector;
            Vector firstGrassVertex;
            Vector a = new Vector(p[j].X,
                p[j].Y + autoGrassThickness / Math.Cos(directionVector.Angle * MathUtils.DegToRad));
            Vector b = new Vector(p[j + 1].X,
                p[j + 1].Y +
                autoGrassThickness / Math.Cos(directionVector.Angle * MathUtils.DegToRad));
            if (previousDirectionVectorNeg.Angle > -90 && previousDirectionVectorNeg.Angle < directionVector.Angle)
            {
                Vector c = new Vector(p[j].X, p[j].Y + GrassHeight);
                double s = (Vector.CrossProduct(c, previousDirectionVectorNeg) -
                            Vector.CrossProduct(a, previousDirectionVectorNeg)) /
                           Vector.CrossProduct(directionVector, previousDirectionVectorNeg);
                double r = (Vector.CrossProduct(a, directionVector) - Vector.CrossProduct(c, directionVector)) /
                           Vector.CrossProduct(previousDirectionVectorNeg, directionVector);
                if (s > 1)
                {
                    if (b.X - p[j - 1].X > 0)
                        firstGrassVertex = a + directionVector.Unit() * (p[j - 1].X - a.X);
                    else
                    {
                        j++;
                        continue;
                    }
                }
                else
                {
                    if (r > 1)
                        firstGrassVertex = a + directionVector.Unit() * (p[j - 1].X - a.X);
                    else if (r > 0)
                        firstGrassVertex = a + s * directionVector;
                    else
                    {
                        j++;
                        continue;
                    }
                }
            }
            else if (previousDirectionVectorNeg.Angle < -90)
                firstGrassVertex =
                    new Vector(p[j].X,
                        p[j].Y + autoGrassThickness / Math.Cos(directionVector.Angle * MathUtils.DegToRad)) +
                    directionVector.Unit() * 0.0;
            else if (previousDirectionVectorNeg.Angle > 90)
            {
                Vector c = p[j].Clone();
                double r = (Vector.CrossProduct(a, directionVector) - Vector.CrossProduct(c, directionVector)) /
                           Vector.CrossProduct(previousDirectionVectorNeg, directionVector);
                firstGrassVertex = c + r * previousDirectionVectorNeg;
            }
            else
                firstGrassVertex =
                    new Vector(p[j].X,
                        p[j].Y + autoGrassThickness / Math.Cos(directionVector.Angle * MathUtils.DegToRad)) +
                    directionVector.Unit() * 0.0;

            grassPolys.Add(new Polygon());
            grassPolys[grassPolys.Count - 1].IsGrass = true;
            grassPolys[grassPolys.Count - 1].Add(firstGrassVertex);
            bool found = false;
            if (Math.Abs((p[j + 2] - p[j + 1]).Angle) <= MaximumGrassAngle)
            {
                while (!(Math.Abs((p[j + 2] - p[j + 1]).Angle) > MaximumGrassAngle))
                {
                    j++;
                    grassPolys[grassPolys.Count - 1].Add(GeometryUtils.FindPoint(p[j - 1], p[j], p[j + 1],
                        autoGrassThickness));
                }

                j++;
                found = true;
            }
            else
            {
                grassPolys[grassPolys.Count - 1].Add(firstGrassVertex + directionVector.Unit() * 0.1);
                j++;
            }

            j = j % p.Vertices.Count;
            directionVector = p[j] - p[j - 1];
            Vector nextDirectionVector = p[j + 1] - p[j];
            Vector lastGrassVertex;
            a = new Vector(p[j - 1].X,
                p[j - 1].Y + autoGrassThickness / Math.Cos(directionVector.Angle * MathUtils.DegToRad));
            b = new Vector(p[j].X,
                p[j].Y + autoGrassThickness / Math.Cos(directionVector.Angle * MathUtils.DegToRad));
            if ((nextDirectionVector.Angle < -90 || nextDirectionVector.Angle > 90) &&
                nextDirectionVector.AnglePositive > (-directionVector).AnglePositive)
            {
                Vector c = new Vector(p[j].X, p[j].Y + GrassHeight);
                double s = (Vector.CrossProduct(c, nextDirectionVector) -
                            Vector.CrossProduct(a, nextDirectionVector)) /
                           Vector.CrossProduct(directionVector, nextDirectionVector);
                if (s > 1)
                {
                    if (grassPolys[grassPolys.Count - 1].Count < 3)
                    {
                        if (found)
                        {
                            Polygon x = grassPolys[grassPolys.Count - 1];
                            Vector dirvect = x.Vertices[x.Count - 1] - x.Vertices[x.Count - 2];
                            lastGrassVertex = x.Vertices[x.Count - 1] - dirvect.Unit() * 0.1;
                        }
                        else
                        {
                            grassPolys.RemoveAt(grassPolys.Count - 1);
                            continue;
                        }
                    }
                    else
                        continue;
                }
                else
                {
                    double r = (Vector.CrossProduct(a, directionVector) - Vector.CrossProduct(c, directionVector)) /
                               Vector.CrossProduct(nextDirectionVector, directionVector);
                    if (r > 1)
                        lastGrassVertex = b - directionVector.Unit() * (b.X - p[j + 1].X);
                    else
                        lastGrassVertex = a + s * directionVector;
                }
            }
            else if (nextDirectionVector.AnglePositive > 270)
                lastGrassVertex =
                    new Vector(p[j].X,
                        p[j].Y + autoGrassThickness / Math.Cos(directionVector.Angle * MathUtils.DegToRad)) -
                    directionVector.Unit() * 0.1;
            else if (nextDirectionVector.AnglePositive < 90)
            {
                Vector c = p[j].Clone();
                double s = (Vector.CrossProduct(a, directionVector) - Vector.CrossProduct(c, directionVector)) /
                           Vector.CrossProduct(nextDirectionVector, directionVector);
                lastGrassVertex = c + s * nextDirectionVector;
            }
            else
                lastGrassVertex = new Vector(p[j].X,
                    p[j].Y +
                    autoGrassThickness / Math.Cos(directionVector.Angle * MathUtils.DegToRad));

            grassPolys[grassPolys.Count - 1].Add(lastGrassVertex);
        }

        return grassPolys.Select(x => x.WithYNegated()).ToList();
    }

    public override bool Busy => AutoGrassPolygonSelected;
}