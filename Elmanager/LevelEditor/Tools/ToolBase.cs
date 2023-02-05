using System;
using System.Windows.Forms;
using Elmanager.Application;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.Rendering;
using Elmanager.Rendering.Camera;

namespace Elmanager.LevelEditor.Tools;

internal abstract class ToolBase : IEditorToolBase
{
    protected Vector CurrentPos;
    private readonly Control _editorControl;
    protected readonly LevelEditorForm LevEditor;
    protected readonly ElmaRenderer Renderer;

    protected ToolBase(LevelEditorForm editor)
    {
        LevEditor = editor;
        Renderer = editor.Renderer;
        _editorControl = editor.EditorControl;
    }

    protected Level Lev => LevEditor.Lev;

    protected ZoomController ZoomCtrl => LevEditor.ZoomCtrl;
    private SceneSettings SceneSettings => LevEditor.SceneSettings;

    public abstract bool Busy { get; }

    internal int GetNearestObjectIndex(Vector p)
    {
        int index = -1;
        double smallest = double.MaxValue;
        var appleFilter = LevEditor.EffectiveAppleFilter;
        var killerFilter = LevEditor.EffectiveKillerFilter;
        var flowerFilter = LevEditor.EffectiveFlowerFilter;
        for (int i = 0; i < Lev.Objects.Count; i++)
        {
            Vector z = Lev.Objects[i].Position;
            ObjectType type = Lev.Objects[i].Type;
            if (((type == ObjectType.Apple && appleFilter) ||
                 (type == ObjectType.Killer && killerFilter) ||
                 (type == ObjectType.Flower && flowerFilter) || type == ObjectType.Start) &&
                (p - z).LengthSquared < smallest)
            {
                smallest = (p - z).LengthSquared;
                index = i;
            }

            if (Lev.Objects[i].Type != ObjectType.Start) continue;
            Vector v1 = p - new Vector(z.X + Level.RightWheelDifferenceFromLeftWheelX, z.Y);
            Vector v2 = p -
                        new Vector(z.X + Level.HeadDifferenceFromLeftWheelX,
                            z.Y + Level.HeadDifferenceFromLeftWheelY);
            if (v1.LengthSquared < smallest)
            {
                smallest = v1.LengthSquared;
                index = i;
            }

            if (v2.LengthSquared >= smallest) continue;
            smallest = v2.LengthSquared;
            index = i;
        }

        if (Math.Sqrt(smallest) <
            Math.Max(ZoomCtrl.ZoomLevel * Global.AppSettings.LevelEditor.CaptureRadius, ElmaRenderer.ObjectRadius))
            return index;
        return -1;
    }

    internal int GetNearestPictureIndex(Vector p)
    {
        var pictureFilter = LevEditor.EffectivePictureFilter;
        var textureFilter = LevEditor.EffectiveTextureFilter;
        int found = -1;
        if (Global.AppSettings.LevelEditor.CapturePicturesAndTexturesFromBordersOnly)
        {
            var limit = ZoomCtrl.ZoomLevel * Global.AppSettings.LevelEditor.CaptureRadius;
            for (int j = 0; j < Lev.GraphicElements.Count; j++)
            {
                GraphicElement z = Lev.GraphicElements[j];
                if ((z is GraphicElement.Picture && pictureFilter) || (z is GraphicElement.Texture && textureFilter))
                {
                    if (GeometryUtils.DistanceFromSegment(z.Position.X, z.Position.Y, z.Position.X + z.Width,
                            z.Position.Y, p.X, p.Y) < limit
                        || GeometryUtils.DistanceFromSegment(z.Position.X, z.Position.Y, z.Position.X,
                            z.Position.Y - z.Height, p.X, p.Y) < limit
                        || GeometryUtils.DistanceFromSegment(z.Position.X + z.Width, z.Position.Y,
                            z.Position.X + z.Width, z.Position.Y - z.Height, p.X, p.Y) < limit
                        || GeometryUtils.DistanceFromSegment(z.Position.X, z.Position.Y - z.Height,
                            z.Position.X + z.Width, z.Position.Y - z.Height, p.X, p.Y) < limit)
                    {
                        found = j;
                        if (z.Position.Mark == VectorMark.Selected)
                        {
                            return found;
                        }
                    }
                }
            }
        }
        else
        {
            for (int j = 0; j < Lev.GraphicElements.Count; j++)
            {
                GraphicElement z = Lev.GraphicElements[j];
                if ((z is GraphicElement.Picture && pictureFilter) || (z is GraphicElement.Texture && textureFilter))
                {
                    if (p.X > z.Position.X && p.X < z.Position.X + z.Width && p.Y < z.Position.Y &&
                        p.Y > z.Position.Y - z.Height)
                    {
                        found = j;
                        if (z.Position.Mark == VectorMark.Selected)
                        {
                            return found;
                        }
                    }
                }
            }
        }

        return found;
    }

    internal record NearestVertexInfo
    {
        internal record VertexInfo(int Index, Polygon P) : NearestVertexInfo;
        internal record EdgeInfo(int StartIndex, int EndIndex, Polygon P) : NearestVertexInfo;

        internal static VertexInfo Vertex(int index, Polygon p) => new(index, p);
        internal static EdgeInfo Edge(int start, int end, Polygon p) => new(start, end, p);

        internal Polygon Polygon => this switch
        {
            EdgeInfo edgeInfo => edgeInfo.P,
            VertexInfo vertexInfo => vertexInfo.P,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    internal NearestVertexInfo? GetNearestVertexInfo(Vector p)
    {
        int nearestIndex = -1;
        int nearestSelectedIndex = -1;
        double smallestDistance = double.MaxValue;
        double smallestSelectedDistance = double.MaxValue;
        Polygon? nearestSelectedPolygon = null;
        Polygon? nearestPolygon = null;
        var grassFilter = LevEditor.EffectiveGrassFilter;
        var groundFilter = LevEditor.EffectiveGroundFilter;
        foreach (Polygon poly in Lev.Polygons)
        {
            double currentDistance;
            if (((poly.IsGrass && grassFilter) || (!poly.IsGrass && groundFilter)) &&
                (currentDistance = poly.GetNearestVertexDistance(p)) <
                ZoomCtrl.ZoomLevel * Global.AppSettings.LevelEditor.CaptureRadius)
            {
                int currentIndex = poly.GetNearestVertexIndex(p);
                if (currentDistance < smallestDistance)
                {
                    nearestPolygon = poly;
                    nearestIndex = currentIndex;
                    smallestDistance = currentDistance;
                }

                //prefer selected vectors
                if (poly[currentIndex].Mark == VectorMark.Selected &&
                    currentDistance < smallestSelectedDistance)
                {
                    nearestSelectedPolygon = poly;
                    nearestSelectedIndex = currentIndex;
                    smallestSelectedDistance = currentDistance;
                }
            }
        }

        if (nearestSelectedPolygon is { })
        {
            return NearestVertexInfo.Vertex(nearestSelectedIndex, nearestSelectedPolygon);
        }

        if (nearestPolygon is { })
        {
            return NearestVertexInfo.Vertex(nearestIndex, nearestPolygon);
        }

        return GetNearestSegmentInfo(p);
    }

    internal NearestVertexInfo.EdgeInfo? GetNearestSegmentInfo(Vector p)
    {
        var smallestDistance = double.MaxValue;
        Polygon? nearestPolygon = null;
        var grassFilter = LevEditor.EffectiveGrassFilter;
        var groundFilter = LevEditor.EffectiveGroundFilter;
        foreach (Polygon x in Lev.Polygons)
        {
            double currentDistance;
            if (((x.IsGrass && grassFilter) || (!x.IsGrass && groundFilter)) &&
                (currentDistance = x.DistanceFromPoint(p)) <
                ZoomCtrl.ZoomLevel * Global.AppSettings.LevelEditor.CaptureRadius)
            {
                if (currentDistance < smallestDistance)
                {
                    smallestDistance = currentDistance;
                    nearestPolygon = x;
                }
            }
        }

        if (nearestPolygon is { })
        {
            var nearestSegmentIndex = nearestPolygon.GetNearestSegmentIndex(p);
            return NearestVertexInfo.Edge(nearestSegmentIndex, nearestSegmentIndex + 1 % nearestPolygon.Vertices.Count, nearestPolygon);
        }

        return null;
    }

    public double CaptureRadiusScaled => ZoomCtrl.ZoomLevel * Global.AppSettings.LevelEditor.CaptureRadius;

    protected void ResetHighlight()
    {
        if (!Global.AppSettings.LevelEditor.UseHighlight)
            return;
        if (LevEditor.PlayController.PlayerSelection == VectorMark.Highlight)
        {
            LevEditor.PlayController.PlayerSelection = VectorMark.None;
        }
        foreach (Polygon x in Lev.Polygons)
        {
            if (x.Mark == PolygonMark.Highlight)
                x.Mark = PolygonMark.None;
            for (var index = 0; index < x.Vertices.Count; index++)
            {
                Vector z = x.Vertices[index];
                if (z.Mark != VectorMark.Selected)
                    x.Vertices[index] = new Vector(z.X, z.Y, VectorMark.None);
            }
        }

        foreach (LevObject x in Lev.Objects)
            if (x.Mark == VectorMark.Highlight)
                x.Mark = VectorMark.None;
        foreach (GraphicElement z in Lev.GraphicElements)
            if (z.Mark == VectorMark.Highlight)
                z.Mark = VectorMark.None;
    }

    protected void ResetPolygonMarks()
    {
        foreach (Polygon x in Lev.Polygons)
            x.Mark = PolygonMark.None;
    }

    protected void AdjustForGrid(ref Vector p)
    {
        if (!Global.AppSettings.LevelEditor.RenderingSettings.ShowGrid ||
            !Global.AppSettings.LevelEditor.SnapToGrid)
            return;
        var gridSize = Global.AppSettings.LevelEditor.RenderingSettings.GridSize;
        double x = (p.X + SceneSettings.GridOffset.X) % gridSize;
        if (Math.Abs(x) > gridSize / 2)
        {
            x -= gridSize * Math.Sign(x);
        }

        p.X -= x;
        double y = (p.Y + SceneSettings.GridOffset.Y) % gridSize;
        if (Math.Abs(y) > gridSize / 2)
        {
            y -= gridSize * Math.Sign(y);
        }

        p.Y -= y;
    }

    protected void ChangeCursorToHand()
    {
        if (Global.AppSettings.LevelEditor.UseHighlight)
            _editorControl.Cursor = Cursors.Hand;
    }

    protected void ChangeToDefaultCursorIfHand()
    {
        if (_editorControl.Cursor == Cursors.Hand)
            LevEditor.ChangeToDefaultCursor();
    }

    protected void MarkAllAs(VectorMark mark)
    {
        LevEditor.PlayController.PlayerSelection = mark;
        foreach (var p in Lev.Polygons)
        {
            for (int j = 0; j < p.Vertices.Count; j++)
            {
                Vector v = p.Vertices[j];
                v.Mark = mark;
                p.Vertices[j] = v;
            }
        }

        foreach (LevObject x in Lev.Objects)
            x.Mark = mark;
        foreach (GraphicElement x in Lev.GraphicElements)
            x.Mark = mark;
    }
}