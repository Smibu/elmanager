using System.Collections.Generic;
using System.Threading.Tasks;
using Elmanager.Geometry;
using Elmanager.Rendering;
using LgrFile = Elmanager.Lgr.Lgr;

namespace Elmanager.LevelEditor.Tools;

public interface IPictureDialogService
{
    Task<GraphicElement?> ShowPictureDialog(
        LgrFile? lgr,
        Vector currentPos,
        GraphicElement? currentElem,
        bool setDefaultsAutomatically);

    Task<(ImageSelection.TextureSelection Selection, TexturizationOptions Options)?> ShowTexturizeDialog(
        LgrFile? lgr,
        TexturizationOptions? existingOptions);

    Task<ImageSelection?> ShowPicturePropertiesDialog(
        LgrFile? lgr,
        List<GraphicElement> selectedElems,
        bool setDefaultsAutomatically,
        Vector? currentPos = null);

    Task<ImageSelection?> ShowConvertToPictureDialog(LgrFile? lgr);
}
