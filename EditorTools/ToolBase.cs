using System;
using System.Linq;
using System.Windows.Forms;
using Elmanager.Forms;

namespace Elmanager.EditorTools
{
    internal class ToolBase : IEditorToolBase
    {
        protected Vector CurrentPos;
        private Control EditorControl;
        protected LevelEditor LevEditor;
        internal Polygon NearestPolygon;
        protected ElmaRenderer Renderer;
        protected bool _Busy;

        protected ToolBase(LevelEditor editor)
        {
            LevEditor = editor;
            Renderer = editor.Renderer;
            EditorControl = editor.EditorControl;
        }

        protected Level Lev
        {
            get { return LevEditor.Lev; }
        }

        public bool Busy
        {
            get { return _Busy; }
        }

        internal int GetNearestObjectIndex(Vector p)
        {
            int index = -1;
            double smallest = double.MaxValue;
            for (int i = 0; i < Lev.Objects.Count; i++)
            {
                Vector z = Lev.Objects[i].Position;
                Level.ObjectType type = Lev.Objects[i].Type;
                if (((type == Level.ObjectType.Apple && LevEditor.AppleFilter) ||
                     (type == Level.ObjectType.Killer && LevEditor.KillerFilter) ||
                     (type == Level.ObjectType.Flower && LevEditor.FlowerFilter) || type == Level.ObjectType.Start) &&
                    (p - z).LengthSquared < smallest)
                {
                    smallest = (p - z).LengthSquared;
                    index = i;
                }
                if (Lev.Objects[i].Type != Level.ObjectType.Start) continue;
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
                Math.Max(Renderer.ZoomLevel * Global.AppSettings.LevelEditor.CaptureRadius, ElmaRenderer.ObjectRadius))
                return index;
            return -1;
        }

        internal int GetNearestPictureIndex(Vector p)
        {
            for (int j = 0; j < Lev.Pictures.Count; j++)
            {
                Level.Picture z = Lev.Pictures[j];
                if ((z.IsPicture && LevEditor.PictureFilter) || (!z.IsPicture && LevEditor.TextureFilter))
                {
                    if (p.X > z.Position.X && p.X < z.Position.X + z.Width && p.Y > z.Position.Y &&
                        p.Y < z.Position.Y + z.Height)
                    {
                        return j;
                    }
                }
            }
            return -1;
        }

        internal int GetNearestVertexIndex(Vector p)
        {
            bool nearestVertexFound = false;
            int nearestIndex = -1;
            foreach (Polygon x in Lev.Polygons)
            {
                if (((x.IsGrass && LevEditor.GrassFilter) || (!x.IsGrass && LevEditor.GroundFilter)) &&
                    x.GetNearestVertexDistance(p) < Renderer.ZoomLevel * Global.AppSettings.LevelEditor.CaptureRadius)
                {
                    NearestPolygon = x;
                    nearestIndex = x.GetNearestVertexIndex(p);
                    nearestVertexFound = true;

                    //prefer selected vectors
                    if (x[nearestIndex].Mark == Geometry.VectorMark.Selected)
                    {
                        return nearestIndex;
                    }
                }
            }
            if (nearestVertexFound)
            {
                return nearestIndex;
            }

            foreach (Polygon x in Lev.Polygons)
            {
                if (((x.IsGrass && LevEditor.GrassFilter) || (!x.IsGrass && LevEditor.GroundFilter)) &&
                    x.DistanceFromPoint(p) < Renderer.ZoomLevel * Global.AppSettings.LevelEditor.CaptureRadius)
                {
                    NearestPolygon = x;
                    return -1; //Indicates that mouse was near an edge but not vertex
                }
            }
            return -2; //Indicates that mouse wasn't near any edge nor vertex
        }

        protected virtual void ResetHighlight()
        {
            if (!Global.AppSettings.LevelEditor.UseHighlight)
                return;
            foreach (Polygon x in Lev.Polygons)
            {
                if (x.Mark == PolygonMark.Highlight)
                    x.Mark = PolygonMark.None;
                foreach (Vector z in x.Vertices)
                    if (z.Mark != Geometry.VectorMark.Selected)
                        z.Mark = Geometry.VectorMark.None;
            }
            foreach (Level.Object x in Lev.Objects)
                if (x.Position.Mark == Geometry.VectorMark.Highlight)
                    x.Position.Mark = Geometry.VectorMark.None;
            foreach (Level.Picture z in Lev.Pictures)
                if (z.Position.Mark == Geometry.VectorMark.Highlight)
                    z.Position.Mark = Geometry.VectorMark.None;
        }

        protected virtual void ResetPolygonMarks()
        {
            foreach (Polygon x in Lev.Polygons)
                x.Mark = PolygonMark.None;
        }

        protected void AdjustForGrid(Vector p)
        {
            if (!Global.AppSettings.LevelEditor.RenderingSettings.ShowGrid || !Global.AppSettings.LevelEditor.SnapToGrid)
                return;
            double x = p.X % Global.AppSettings.LevelEditor.RenderingSettings.GridSize;
            if (Math.Abs(x) > Global.AppSettings.LevelEditor.RenderingSettings.GridSize / 2)
            {
                x -= Global.AppSettings.LevelEditor.RenderingSettings.GridSize * Math.Sign(x);
            }
            p.X -= x;
            double y = p.Y % Global.AppSettings.LevelEditor.RenderingSettings.GridSize;
            if (Math.Abs(y) > Global.AppSettings.LevelEditor.RenderingSettings.GridSize / 2)
            {
                y -= Global.AppSettings.LevelEditor.RenderingSettings.GridSize * Math.Sign(y);
            }
            p.Y -= y;
        }

        protected void ChangeCursorToHand()
        {
            if (Global.AppSettings.LevelEditor.UseHighlight)
                EditorControl.Cursor = Cursors.Hand;
        }

        protected void ChangeToDefaultCursor()
        {
            if (EditorControl.Cursor == Cursors.Hand)
                EditorControl.Cursor = Cursors.Default;
        }

        protected void MarkAllAs(Geometry.VectorMark mark)
        {
            foreach (Vector t in Lev.Polygons.SelectMany(x => x.Vertices))
            {
                t.Mark = mark;
            }
            foreach (Level.Object x in Lev.Objects)
                x.Position.Mark = mark;
            foreach (Level.Picture x in Lev.Pictures)
                x.Position.Mark = mark;
        }
    }
}