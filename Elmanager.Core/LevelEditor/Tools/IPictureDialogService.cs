using System.Collections.Generic;
using Elmanager.Geometry;
using Elmanager.Rendering;
using LgrFile = Elmanager.Lgr.Lgr;

namespace Elmanager.LevelEditor.Tools;

public interface IPictureDialogService
{
    GraphicElement? ShowPictureDialog(
        LgrFile? lgr,
        Vector currentPos,
        GraphicElement? currentElem,
        bool setDefaultsAutomatically);

    (ImageSelection.TextureSelection Selection, TexturizationOptions Options)? ShowTexturizeDialog(
        LgrFile? lgr,
        TexturizationOptions? existingOptions);

    ImageSelection? ShowPicturePropertiesDialog(
        LgrFile? lgr,
        List<GraphicElement> selectedElems,
        bool setDefaultsAutomatically,
        Vector? currentPos = null);

    ImageSelection? ShowConvertToPictureDialog(LgrFile? lgr);
}
