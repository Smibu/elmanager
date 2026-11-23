using System.Collections.Generic;
using Elmanager.Lgr;

namespace Elmanager.LevelEditor.Tools;

public record TexturizationOptions(
    ImageSelection.TextureSelection Texture,
    ClippingType Clipping,
    int Distance,
    double MinCoverPercentage,
    int Iterations,
    List<string> SelectedMasks
);
