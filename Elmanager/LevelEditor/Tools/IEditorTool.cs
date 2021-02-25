using System.Collections.Generic;
using System.Windows.Forms;
using Elmanager.Geometry;
using Elmanager.Lev;

namespace Elmanager.LevelEditor.Tools
{
    internal interface IEditorTool : IEditorToolBase
    {
        void MouseDown(MouseEventArgs mouseData);
        void MouseUp();
        void KeyDown(KeyEventArgs key);
        void MouseMove(Vector p);
        void MouseOutOfEditor();
        void ExtraRendering();
        List<Polygon> GetExtraPolygons();
        void InActivate();
        void Activate();
        void UpdateHelp();
    }
}