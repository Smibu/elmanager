using System.Drawing;
using System.Windows.Forms;
using Elmanager.Forms;

namespace Elmanager.EditorTools
{
    internal class VertexTool : ToolBase, IEditorTool
    {
        private bool _creatingVertex;
        private Polygon _currentPolygon;
        private bool _edgeClicked;

        internal VertexTool(LevelEditor editor)
            : base(editor)
        {
        }

        private bool CreatingVertex
        {
            get { return _creatingVertex; }
            set
            {
                _creatingVertex = value;
                _Busy = value;
            }
        }

        #region IEditorTool Members

        public void Activate()
        {
            UpdateHelp();
            Renderer.RedrawScene();
        }

        public void UpdateHelp()
        {
            LevEditor.InfoLabel.Text =
                "Left mouse button: create vertex. Click near a vertex of a polygon to add vertices to it.";
        }

        public void ExtraRendering()
        {
            if (CreatingVertex)
                Renderer.DrawLine(_currentPolygon.GetLastVertex(), _currentPolygon.Vertices[0], Color.Red);
        }

        public void InActivate()
        {
            FinishVertexCreation();
        }

        public void KeyDown(KeyEventArgs key)
        {
            if (key.KeyCode != Keys.Space || !CreatingVertex) return;
            if (_edgeClicked)
            {
                _currentPolygon.RemoveLastVertex();
                _currentPolygon.ChangeOrientation();
                _currentPolygon.Add(CurrentPos);
            }
            else
            {
                _currentPolygon.RemoveLastVertex();
                Vector old = _currentPolygon.GetLastVertex();
                _currentPolygon.RemoveLastVertex();
                _currentPolygon.ChangeOrientation();
                _currentPolygon.Add(old);
                _currentPolygon.Add(CurrentPos);
            }
            _currentPolygon.UpdateDecomposition(false);
            Renderer.RedrawScene();
        }

        public void MouseDown(MouseEventArgs mouseData)
        {
            switch (mouseData.Button)
            {
                case MouseButtons.Left:
                    if (!CreatingVertex)
                    {
                        CreatingVertex = true;
                        int nearestIndex = GetNearestVertexIndex(CurrentPos);
                        AdjustForGrid(CurrentPos);
                        if (nearestIndex >= 0)
                        {
                            _currentPolygon = NearestPolygon;
                            _currentPolygon.SetBeginPoint(nearestIndex + 1);
                            _currentPolygon.Add(CurrentPos);
                            ChangeToDefaultCursor();
                            _edgeClicked = false;
                        }
                        else if (nearestIndex == -1)
                        {
                            int nearestSegmentIndex = NearestPolygon.GetNearestSegmentIndex(CurrentPos);
                            _currentPolygon = NearestPolygon;
                            _currentPolygon.SetBeginPoint(nearestSegmentIndex + 1);
                            _currentPolygon.Add(CurrentPos);
                            ChangeToDefaultCursor();
                            _edgeClicked = true;
                        }
                        else
                            _currentPolygon = new Polygon(CurrentPos, CurrentPos);
                    }
                    else
                    {
                        _currentPolygon.Add(CurrentPos);
                        if (_currentPolygon.Count == 3)
                        {
                            Lev.Polygons.Add(_currentPolygon);
                            _currentPolygon.UpdateDecomposition(false);
                        }
                    }
                    Renderer.RedrawScene();
                    break;
                case MouseButtons.Right:
                    FinishVertexCreation();
                    break;
            }
        }

        public void MouseMove(Vector p)
        {
            CurrentPos = p;
            if (CreatingVertex)
            {
                AdjustForGrid(CurrentPos);
                _currentPolygon.Vertices[_currentPolygon.Count - 1] = p;
                if (_currentPolygon.Count > 2)
                    _currentPolygon.UpdateDecomposition(false);
            }
            else
            {
                ResetHighlight();
                int nearestVertex = GetNearestVertexIndex(p);
                if (nearestVertex >= 0)
                {
                    NearestPolygon.Vertices[nearestVertex].Mark = Geometry.VectorMark.Highlight;
                    ChangeCursorToHand();
                }
                else if (nearestVertex == -1)
                    ChangeCursorToHand();
                else
                    ChangeToDefaultCursor();
            }
            Renderer.RedrawScene();
        }

        public void MouseOutOfEditor()
        {
            ResetHighlight();
            Renderer.RedrawScene();
        }

        public void MouseUp(MouseEventArgs mouseData)
        {
        }

        #endregion

        private void FinishVertexCreation()
        {
            if (CreatingVertex)
            {
                CreatingVertex = false;
                LevEditor.InfoLabel.Text =
                    "Left mouse button: create vertex. Click near a vertex of a polygon to add vertices to it.";
                _currentPolygon.RemoveLastVertex();
                if (_currentPolygon.Count > 2)
                {
                    _currentPolygon.UpdateDecomposition();
                    LevEditor.Modified = true;
                }
                else
                    Lev.Polygons.Remove(_currentPolygon);
                Renderer.RedrawScene();
            }
        }
    }
}