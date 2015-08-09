using System.Windows.Forms;
using Elmanager.Forms;

namespace Elmanager.EditorTools
{
    internal class PolyOpTool : ToolBase, IEditorTool
    {
        private PolygonOperationType _currentOpType = PolygonOperationType.Merge;
        private Polygon _firstPolygon;
        private bool _firstSelected;

        internal PolyOpTool(LevelEditor editor)
            : base(editor)
        {
        }

        private bool FirstSelected
        {
            get { return _firstSelected; }
            set
            {
                _firstSelected = value;
                _Busy = value;
            }
        }

        public void Activate()
        {
            UpdateHelp();
            Renderer.RedrawScene();
        }

        public void UpdateHelp()
        {
            char polyChar = FirstSelected ? 'B' : 'A';
            switch (_currentOpType)
            {
                case PolygonOperationType.Merge:
                    LevEditor.InfoLabel.Text = "Click the polygon " + polyChar + " (operation = A+B).";
                    break;
                case PolygonOperationType.Difference:
                    LevEditor.InfoLabel.Text = "Click the polygon " + polyChar + " (operation = A-B).";
                    break;
                case PolygonOperationType.Intersection:
                    LevEditor.InfoLabel.Text = "(This mode is not yet implemented.)";
                    break;
            }
            if (!FirstSelected)
            {
                LevEditor.InfoLabel.Text += " Space: Change mode.";
            }
        }

        public void ExtraRendering()
        {
        }

        public void InActivate()
        {
            if (!FirstSelected) return;
            FirstSelected = false;
            _firstPolygon.Mark = PolygonMark.None;
        }

        public void KeyDown(KeyEventArgs key)
        {
            if (key.KeyCode != Keys.Space || FirstSelected) return;
            switch (_currentOpType)
            {
                case PolygonOperationType.Merge:
                    _currentOpType = PolygonOperationType.Difference;
                    break;
                case PolygonOperationType.Intersection:
                    break;

                case PolygonOperationType.Difference:
                    _currentOpType = PolygonOperationType.Merge;
                    break;
            }
            UpdateHelp();
        }

        public void MouseDown(MouseEventArgs mouseData)
        {
            switch (mouseData.Button)
            {
                case MouseButtons.Left:
                    int nearestVertexIndex = GetNearestVertexIndex(CurrentPos);
                    if (FirstSelected)
                    {
                        if (nearestVertexIndex >= -1)
                        {
                            if (!NearestPolygon.Equals(_firstPolygon))
                            {
                                MarkAllAs(Geometry.VectorMark.None);
                                try
                                {
                                    Lev.Polygons.AddRange(NearestPolygon.PolygonOperationWith(_firstPolygon, _currentOpType));
                                    Lev.Polygons.Remove(_firstPolygon);
                                    Lev.Polygons.Remove(NearestPolygon);
                                    LevEditor.Modified = true;
                                }
                                catch (PolygonException e)
                                {
                                    Utils.ShowError(e.Message);
                                }
                                FirstSelected = false;
                                ResetPolygonMarks();
                                Renderer.RedrawScene();
                                UpdateHelp();
                            }
                        }
                    }
                    else
                    {
                        if (nearestVertexIndex >= -1)
                        {
                            FirstSelected = true;
                            _firstPolygon = NearestPolygon;
                            _firstPolygon.Mark = PolygonMark.Selected;
                            Renderer.RedrawScene();
                            UpdateHelp();
                        }
                    }
                    break;
                case MouseButtons.Right:
                    if (FirstSelected)
                    {
                        FirstSelected = false;
                        ResetPolygonMarks();
                        Renderer.RedrawScene();
                        UpdateHelp();
                    }
                    break;
            }
        }

        public void MouseMove(Vector p)
        {
            CurrentPos = p;
            ResetHighlight();
            int nearestVertex = GetNearestVertexIndex(p);
            if (nearestVertex >= -1)
            {
                ChangeCursorToHand();
                if (NearestPolygon.Mark != PolygonMark.Selected)
                    NearestPolygon.Mark = PolygonMark.Highlight;
            }
            else
                ChangeToDefaultCursor();
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
    }
}