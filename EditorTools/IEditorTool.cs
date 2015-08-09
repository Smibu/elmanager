using System.Windows.Forms;

namespace Elmanager.EditorTools
{
    internal interface IEditorTool : IEditorToolBase
    {
        void MouseDown(MouseEventArgs mouseData);
        void MouseUp(MouseEventArgs mouseData);
        void KeyDown(KeyEventArgs key);
        void MouseMove(Vector p);
        void MouseOutOfEditor();
        void ExtraRendering();
        void InActivate();
        void Activate();
        void UpdateHelp();
    }
}