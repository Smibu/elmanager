using System.Collections.Generic;

namespace Elmanager.LevelEditor.Tools;

public record TexturizationOptions(
    ImageSelection.TextureSelection Texture,
    double MinCoverPercentage,
    int Iterations,
    List<string> SelectedMasks
);
