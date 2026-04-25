using System;
using System.Windows.Forms;
using Elmanager.Geometry;
using Elmanager.Rendering;

namespace Elmanager.LevelEditor.Tools.Platform;

internal class WinFormsPictureDialogService : IPictureDialogService
{
    private readonly Func<Lgr.Lgr?> _getEditorLgr;

    internal WinFormsPictureDialogService(Func<Lgr.Lgr?> getEditorLgr)
    {
        _getEditorLgr = getEditorLgr;
    }

    public GraphicElement? ShowPictureDialog(
        ElmaRenderer renderer,
        Vector currentPos,
        GraphicElement? currentElem,
        bool setDefaultsAutomatically)
    {
        if (renderer.OpenGlLgr == null || _getEditorLgr() == null)
        {
            return null;
        }

        var picForm = new PictureForm(_getEditorLgr()!, currentElem);
        picForm.Location = Control.MousePosition;
        picForm.AllowMultiple = false;
        picForm.AutoTextureMode = false;
        picForm.SetDefaultsAutomatically = setDefaultsAutomatically;
        picForm.ShowDialog();
        if (picForm.Selection is { } sel)
        {
            var clipping = sel.Clipping!.Value;
            var distance = sel.Distance!.Value;
            return sel switch
            {
                ImageSelection.TextureSelection t => GraphicElement.Text(clipping, distance,
                    currentPos, renderer.OpenGlLgr.DrawableImageFromLgrImage(t.Txt),
                    renderer.OpenGlLgr.DrawableImageFromLgrImage(t.Mask)),
                ImageSelection.PictureSelection p => GraphicElement.Pic(renderer.OpenGlLgr.DrawableImageFromLgrImage(p.Pic),
                    currentPos, distance, clipping),
                _ => throw new Exception("Unexpected")
            };
        }

        return null;
    }
}
