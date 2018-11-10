using System.Windows.Forms;
using Elmanager.Forms;

namespace Elmanager.EditorTools
{
    internal class ObjectTool : ToolBase, IEditorTool
    {
        private Level.ObjectType _currentObjectType = Level.ObjectType.Apple;
        private bool _hasFocus;
        private int _animNum = 1;

        internal ObjectTool(LevelEditor editor)
            : base(editor)
        {
        }

        public void Activate()
        {
            UpdateHelp();
            CurrentPos = CurrentPos ?? new Vector(0, 0);
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

            if (!Global.AppSettings.LevelEditor.RenderingSettings.ShowObjects || !Renderer.LgrGraphicsLoaded)
                return;
            switch (_currentObjectType)
            {
                case Level.ObjectType.Killer:
                    Renderer.DrawKiller(CurrentPos);
                    break;
                case Level.ObjectType.Apple:
                    Renderer.DrawApple(CurrentPos, _animNum);
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
            switch (key.KeyCode)
            {
                case Keys.Space:
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
                    break;
                case Keys.D1:
                    _animNum = 1;
                    break;
                case Keys.D2:
                    _animNum = 2;
                    break;
                case Keys.D3:
                    _animNum = 3;
                    break;
                case Keys.D4:
                    _animNum = 4;
                    break;
                case Keys.D5:
                    _animNum = 5;
                    break;
                case Keys.D6:
                    _animNum = 6;
                    break;
                case Keys.D7:
                    _animNum = 7;
                    break;
                case Keys.D8:
                    _animNum = 8;
                    break;
                case Keys.D9:
                    _animNum = 9;
                    break;
            }
        }

        public void MouseDown(MouseEventArgs mouseData)
        {
            if (mouseData.Button != MouseButtons.Left) return;
            Lev.Objects.Add(new Level.Object(CurrentPos, _currentObjectType, Level.AppleTypes.Normal, _animNum));
            LevEditor.Modified = true;
        }

        public void MouseMove(Vector p)
        {
            CurrentPos = p;
            _hasFocus = true;
            AdjustForGrid(CurrentPos);
        }

        public void MouseOutOfEditor()
        {
            _hasFocus = false;
        }

        public void MouseUp()
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

        public override bool Busy => false;
    }
}