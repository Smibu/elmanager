using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elmanager.Geometry;
using Elmanager.LevelEditor.Tools;
using Elmanager.Rendering;
using Elmanager.SLE.Dialogs;
using LgrFile = Elmanager.Lgr.Lgr;

namespace Elmanager.SLE.Platform;

internal class AvaloniaPictureDialogService : IPictureDialogService
{
    private readonly ElmaRenderer _renderer;

    internal AvaloniaPictureDialogService(ElmaRenderer renderer) => _renderer = renderer;

    public async Task<GraphicElement?> ShowPictureDialog(
        LgrFile? lgr,
        Vector currentPos,
        GraphicElement? currentElem,
        bool setDefaultsAutomatically)
    {
        if (lgr is null || _renderer.OpenGlLgr is null)
        {
            return null;
        }

        var dialog = new PictureDialog(lgr, setDefaultsAutomatically);
        if (currentElem is not null)
        {
            dialog.SelectElement(currentElem);
        }

        var result = await dialog.ShowAsync();
        if (!result.HasValue)
        {
            return null;
        }

        var sel = result.Value;
        var clipping = sel.Clipping!.Value;
        var distance = sel.Distance!.Value;
        return sel switch
        {
            ImageSelection.TextureSelection t => GraphicElement.Text(clipping, distance,
                currentPos, _renderer.OpenGlLgr.DrawableImageFromLgrImage(t.Txt),
                _renderer.OpenGlLgr.DrawableImageFromLgrImage(t.Mask)),
            ImageSelection.PictureSelection p => GraphicElement.Pic(
                _renderer.OpenGlLgr.DrawableImageFromLgrImage(p.Pic),
                currentPos, distance, clipping),
            _ => throw new Exception("Unexpected")
        };
    }

    public async Task<(ImageSelection.TextureSelection Selection, TexturizationOptions Options)?> ShowTexturizeDialog(
        LgrFile? lgr,
        TexturizationOptions? existingOptions)
    {
        if (lgr is null)
        {
            return null;
        }

        var dialog = new TexturizeDialog(lgr, existingOptions);
        var result = await dialog.ShowAsync();
        return result.HasValue ? result.Value : null;
    }

    public async Task<ImageSelection?> ShowPicturePropertiesDialog(
        LgrFile? lgr,
        List<GraphicElement> selectedElems,
        bool setDefaultsAutomatically,
        Vector? currentPos = null)
    {
        if (lgr is null)
        {
            return null;
        }

        var dialog = new PictureDialog(lgr, setDefaultsAutomatically);
        if (selectedElems is { Count: > 1 })
        {
            dialog.SelectMultiple(selectedElems);
        }
        else if (selectedElems is { Count: 1 })
        {
            dialog.SelectElement(selectedElems[0]);
        }

        var result = await dialog.ShowAsync();
        return result.HasValue ? result.Value : null;
    }

    public async Task<ImageSelection?> ShowConvertToPictureDialog(LgrFile? lgr) =>
        await ShowPicturePropertiesDialog(lgr, new List<GraphicElement>(), true);
}
