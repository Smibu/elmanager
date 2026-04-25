using Elmanager.LevelEditor.Tools;

namespace Elmanager.LevelEditor;

internal class SelectionFilter(ILevelEditor levelEditor) : ISelectionFilter
{
    public bool AppleFilter { get; set; } = true;
    public bool FlowerFilter { get; set; } = true;
    public bool KillerFilter { get; set; } = true;
    public bool StartFilter { get; set; } = true;
    public bool GrassFilter { get; set; } = true;
    public bool GroundFilter { get; set; } = true;
    public bool PictureFilter { get; set; } = true;
    public bool TextureFilter { get; set; } = true;

    public bool EffectiveAppleFilter => AppleFilter &&
                                          (levelEditor.ObjectFramesVisible ||
                                           levelEditor.ObjectsVisible);

    public bool EffectiveKillerFilter => KillerFilter &&
                                           (levelEditor.ObjectFramesVisible ||
                                            levelEditor.ObjectsVisible);

    public bool EffectiveFlowerFilter => FlowerFilter &&
                                           (levelEditor.ObjectFramesVisible ||
                                            levelEditor.ObjectsVisible);

    public bool EffectiveStartFilter => StartFilter &&
                                           (levelEditor.ObjectFramesVisible ||
                                            levelEditor.ObjectsVisible);

    public bool EffectiveGrassFilter => GrassFilter &&
                                          (levelEditor.GrassEdgesVisible ||
                                           levelEditor.GrassVisible);

    public bool EffectiveGroundFilter => GroundFilter &&
                                           (levelEditor.GroundEdgesVisible ||
                                            levelEditor.GroundVisible);

    public bool EffectiveTextureFilter => TextureFilter &&
                                            (levelEditor.TextureFramesVisible || levelEditor.TexturesVisible);

    public bool EffectivePictureFilter => PictureFilter &&
                                            (levelEditor.PictureFramesVisible || levelEditor.PicturesVisible);
}
