using System.Windows.Forms;
using Elmanager.Forms;

namespace Elmanager.EditorTools
{
    internal class ObjectTool : ToolBase, IEditorTool
    {
        private Level.ObjectType _currentObjectType = Level.ObjectType.Apple;
        private bool _hasFocus;

        internal ObjectTool(LevelEditor editor)
            : base(editor)
        {
        }

        public void Activate()
        {
            UpdateHelp();
            CurrentPos = new Vector(0, 0);
        }

        public void ExtraRendering()
        {
            if (!_hasFocus)
                return;
            if (Global.AppSettings.LevelEditor.RenderingSettings.ShowObjectFrames)
            {
                switch (_currentObjectType)
                {
                    case Level.ObjectType.Killer:
                        Renderer.DrawCircle(CurrentPos, ElmaRenderer.ObjectRadius,
                                            Global.AppSettings.LevelEditor.RenderingSettings.KillerColor);
                        break;
                    case Level.ObjectType.Apple:
                        Renderer.DrawCircle(CurrentPos, ElmaRenderer.ObjectRadius,
                                            Global.AppSettings.LevelEditor.RenderingSettings.AppleColor);
                        break;
                    case Level.ObjectType.Flower:
                        Renderer.DrawCircle(CurrentPos, ElmaRenderer.ObjectRadius,
                                            Global.AppSettings.LevelEditor.RenderingSettings.FlowerColor);
                        break;
                }
            }
            if (!Global.AppSettings.LevelEditor.RenderingSettings.ShowObjects)
                return;
            switch (_currentObjectType)
            {
                case Level.ObjectType.Killer:
                    Renderer.DrawKiller(CurrentPos);
                    break;
                case Level.ObjectType.Apple:
                    Renderer.DrawApple(CurrentPos);
                    break;
                case Level.ObjectType.Flower:
                    Renderer.DrawFlower(CurrentPos);
                    break;
            }
        }

        public void InActivate()
        {
        }

        public void KeyDown(KeyEventArgs key)
        {
            if (key.KeyCode != Keys.Space) return;
            switch (_currentObjectType)
            {
                case Level.ObjectType.Apple:
                    _currentObjectType = Level.ObjectType.Killer;
                    break;
                case Level.ObjectType.Killer:
                    _currentObjectType = Level.ObjectType.Flower;
                    break;
                case Level.ObjectType.Flower:
                    _currentObjectType = Level.ObjectType.Apple;
                    break;
            }
            UpdateHelp();
            Renderer.RedrawScene();
        }

        public void MouseDown(MouseEventArgs mouseData)
        {
            if (mouseData.Button != MouseButtons.Left) return;
            Lev.Objects.Add(new Level.Object(CurrentPos, _currentObjectType, Level.AppleTypes.Normal, 0));
            LevEditor.Modified = true;
        }

        public void MouseMove(Vector p)
        {
            CurrentPos = p;
            _hasFocus = true;
            AdjustForGrid(CurrentPos);
            Renderer.RedrawScene();
        }

        public void MouseOutOfEditor()
        {
            _hasFocus = false;
            Renderer.RedrawScene();
        }

        public void MouseUp(MouseEventArgs mouseData)
        {
        }

        public void UpdateHelp()
        {
            LevEditor.InfoLabel.Text = "Left mouse button: insert new ";
            switch (_currentObjectType)
            {
                case Level.ObjectType.Apple:
                    LevEditor.InfoLabel.Text += "apple;";
                    break;
                case Level.ObjectType.Killer:
                    LevEditor.InfoLabel.Text += "killer;";
                    break;
                case Level.ObjectType.Flower:
                    LevEditor.InfoLabel.Text += "flower;";
                    break;
            }
            LevEditor.InfoLabel.Text += " Space: change object type.";
        }
    }
}