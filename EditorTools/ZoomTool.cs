using System.Windows.Forms;
using Elmanager.Forms;

namespace Elmanager.EditorTools
{
    internal class ZoomTool : ToolBase, IEditorTool
    {
        internal ZoomTool(LevelEditor editor) : base(editor)
        {
        }

        public void Activate()
        {
            UpdateHelp();
            Renderer.RedrawScene();
        }

        public void ExtraRendering()
        {
        }

        public void InActivate()
        {
        }

        public void KeyDown(KeyEventArgs key)
        {
        }

        public void MouseDown(MouseEventArgs mouseData)
        {
            CurrentPos.Y *= -1.0;
            switch (mouseData.Button)
            {
                case MouseButtons.Left:
                case MouseButtons.Right:
                    Renderer.Zoom(CurrentPos, mouseData.Button == MouseButtons.Left,
                                  1 - Global.AppSettings.LevelEditor.MouseClickStep / 100.0);
                    break;
            }
        }

        public void MouseMove(Vector p)
        {
            CurrentPos = p;
        }

        public void MouseOutOfEditor()
        {
        }

        public void MouseUp(MouseEventArgs mouseData)
        {
        }

        public void UpdateHelp()
        {
            LevEditor.InfoLabel.Text = "Left mouse button: zoom in; right mouse button: zoom out.";
        }
    }
}