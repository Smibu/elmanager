using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Elmanager.Geometry;
using Elmanager.Rendering;
using LgrFile = Elmanager.Lgr.Lgr;

namespace Elmanager.LevelEditor.Tools.Platform;

internal class WinFormsPictureDialogService : IPictureDialogService
{
    private readonly ElmaRenderer _renderer;

    internal WinFormsPictureDialogService(ElmaRenderer renderer)
    {
        _renderer = renderer;
    }

    public GraphicElement? ShowPictureDialog(
        LgrFile? lgr,
        Vector currentPos,
        GraphicElement? currentElem,
        bool setDefaultsAutomatically)
    {
        var elems = currentElem is { } e ? new List<GraphicElement> { e } : new List<GraphicElement>();
        var sel = ShowPicturePropertiesDialog(lgr, elems, setDefaultsAutomatically, currentPos);
        if (sel is null || _renderer.OpenGlLgr is null)
        {
            return null;
        }

        var clipping = sel.Clipping!.Value;
        var distance = sel.Distance!.Value;
        return sel switch
        {
            ImageSelection.TextureSelection t => GraphicElement.Text(clipping, distance,
                currentPos, _renderer.OpenGlLgr.DrawableImageFromLgrImage(t.Txt),
                _renderer.OpenGlLgr.DrawableImageFromLgrImage(t.Mask)),
            ImageSelection.PictureSelection p => GraphicElement.Pic(_renderer.OpenGlLgr.DrawableImageFromLgrImage(p.Pic),
                currentPos, distance, clipping),
            _ => throw new Exception("Unexpected")
        };
    }

    public (ImageSelection.TextureSelection Selection, TexturizationOptions Options)? ShowTexturizeDialog(
        LgrFile? lgr,
        TexturizationOptions? existingOptions)
    {
        var result = ShowPictureDialogCore(lgr, autoTextureMode: true, setDefaultsAutomatically: true,
            existingTexturizationOptions: existingOptions);
        if (result is not (ImageSelection.TextureSelection sel, { } opts))
        {
            return null;
        }

        return (sel, opts);
    }

    public ImageSelection? ShowPicturePropertiesDialog(
        LgrFile? lgr,
        List<GraphicElement> selectedElems,
        bool setDefaultsAutomatically,
        Vector? currentPos = null)
    {
        var (sel, _) = ShowPictureDialogCore(lgr, autoTextureMode: false,
            setDefaultsAutomatically: setDefaultsAutomatically,
            selectedElems: selectedElems, currentPos: currentPos) ?? default;
        return sel;
    }

    private (ImageSelection Selection, TexturizationOptions? TexturizationOptions)? ShowPictureDialogCore(
        LgrFile? lgr,
        bool autoTextureMode,
        bool setDefaultsAutomatically,
        List<GraphicElement>? selectedElems = null,
        Vector? currentPos = null,
        TexturizationOptions? existingTexturizationOptions = null)
    {
        if (lgr == null)
        {
            return null;
        }

        var picForm = new PictureForm(lgr, null)
        {
            AutoTextureMode = autoTextureMode,
            AllowMultiple = false,
            SetDefaultsAutomatically = setDefaultsAutomatically
        };
        if (currentPos is { })
        {
            picForm.Location = Control.MousePosition;
        }

        if (existingTexturizationOptions is { })
        {
            picForm.TexturizationOptions = existingTexturizationOptions;
        }

        if (selectedElems is { Count: > 1 })
        {
            picForm.AllowMultiple = true;
            picForm.SelectMultiple(selectedElems);
        }
        else if (selectedElems is { Count: 1 })
        {
            picForm.AllowMultiple = false;
            picForm.SelectElement(selectedElems[0]);
        }
        else
        {
            picForm.AllowMultiple = false;
            picForm.SetDefaultDistanceAndClipping();
        }

        picForm.ShowDialog();
        if (picForm.Selection is not { } selection)
        {
            return null;
        }

        var texturizationOptions = autoTextureMode ? picForm.TexturizationOptions : null;
        return (selection, texturizationOptions);
    }

    public ImageSelection? ShowConvertToPictureDialog(LgrFile? lgr) =>
        ShowPicturePropertiesDialog(lgr, new List<GraphicElement>(), setDefaultsAutomatically: true);
}
