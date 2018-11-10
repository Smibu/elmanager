using System.Windows.Forms;
using Elmanager.Forms;

namespace Elmanager.EditorTools
{
    internal class PictureTool : ToolBase, IEditorTool
    {
        private Level.Picture _currentPicture;

        internal PictureTool(LevelEditor editor)
            : base(editor)
        {
        }

        public void Activate()
        {
            UpdateHelp();
            if (_currentPicture != null)
                AddCurrent();
        }

        public void ExtraRendering()
        {
        }

        public void InActivate()
        {
            if (_currentPicture != null)
                RemoveCurrent();
        }

        public void KeyDown(KeyEventArgs key)
        {
            switch (key.KeyCode)
            {
                case Keys.Space:
                    OpenDialog();
                    break;
            }
        }

        public void MouseDown(MouseEventArgs mouseData)
        {
            switch (mouseData.Button)
            {
                case MouseButtons.Left:

                    if (_currentPicture != null)
                    {
                        
                        _currentPicture = _currentPicture.Clone();
                        Lev.Pictures.Add(_currentPicture);
                        LevEditor.Modified = true;
                    }
                    else
                        OpenDialog();
                    break;
                case MouseButtons.Right:
                    OpenDialog();
                    break;
            }
        }

        public void MouseMove(Vector p)
        {
            CurrentPos = p;
            AdjustForGrid(CurrentPos);

            if (_currentPicture != null)
            {
                _currentPicture.Position = CurrentPos;
                _currentPicture.Position.X -= _currentPicture.Width / 2;
                _currentPicture.Position.Y -= _currentPicture.Height / 2;
            }
        }

        public void MouseOutOfEditor()
        {
        }

        public void MouseUp()
        {
        }

        public void UpdateHelp()
        {
            LevEditor.InfoLabel.Text =
                "Left mouse button: insert new picture/texture, right mouse button: select picture type.";
        }

        private void AddCurrent()
        {
            Lev.Pictures.Insert(0, _currentPicture);
            Lev.SortPictures();
        }

        private void OpenDialogNow(bool setDefaultsAutomatically)
        {
            LevEditor.PicForm.Location = Control.MousePosition;
            LevEditor.PicForm.AllowMultiple = false;
            LevEditor.PicForm.AutoTextureMode = false;
            LevEditor.PicForm.SetDefaultsAutomatically = setDefaultsAutomatically;
            LevEditor.PicForm.ShowDialog();
            if (LevEditor.PicForm.OkButtonPressed)
            {
                if (LevEditor.PicForm.TextureSelected)
                {
                    _currentPicture = new Level.Picture(LevEditor.PicForm.Clipping, LevEditor.PicForm.Distance,
                                                        CurrentPos,
                                                        Renderer.DrawableImageFromName(LevEditor.PicForm.Texture.Name),
                                                        Renderer.DrawableImageFromName(LevEditor.PicForm.Mask.Name));
                }
                else
                {
                    _currentPicture = new Level.Picture(Renderer.DrawableImageFromName(LevEditor.PicForm.Picture.Name),
                                                        CurrentPos, LevEditor.PicForm.Distance,
                                                        LevEditor.PicForm.Clipping);
                }
                AddCurrent();
            }
        }

        private void OpenDialog()
        {
            if (_currentPicture == null)
            {
                LevEditor.PicForm.SetDefaultDistanceAndClipping();
                OpenDialogNow(setDefaultsAutomatically: true);
            }
            else
            {
                RemoveCurrent();
                LevEditor.PicForm.SelectElement(_currentPicture);
                OpenDialogNow(setDefaultsAutomatically: Global.AppSettings.LevelEditor.AlwaysSetDefaultsInPictureTool);
                if (!LevEditor.PicForm.OkButtonPressed)
                    AddCurrent();
            }
        }

        private void RemoveCurrent()
        {
            Lev.Pictures.Remove(_currentPicture);
        }

        public override bool Busy => false;
    }
}