namespace Elmanager.LevelEditor;

internal class SelectionFilter(LevelEditorForm levelEditorForm)
{
    public bool AppleFilter { get; set; } = true;
    public bool FlowerFilter { get; set; } = true;
    public bool KillerFilter { get; set; } = true;
    public bool StartFilter { get; set; } = true;
    public bool GrassFilter { get; set; } = true;
    public bool GroundFilter { get; set; } = true;
    public bool PictureFilter { get; set; } = true;
    public bool TextureFilter { get; set; } = true;

    internal bool EffectiveAppleFilter => AppleFilter &&
                                          (levelEditorForm.ShowObjectFramesButton.Checked ||
                                           levelEditorForm.ShowObjectsButton.Checked);

    internal bool EffectiveKillerFilter => KillerFilter &&
                                           (levelEditorForm.ShowObjectFramesButton.Checked ||
                                            levelEditorForm.ShowObjectsButton.Checked);

    internal bool EffectiveFlowerFilter => FlowerFilter &&
                                           (levelEditorForm.ShowObjectFramesButton.Checked ||
                                            levelEditorForm.ShowObjectsButton.Checked);

    internal bool EffectiveStartFilter => StartFilter &&
                                           (levelEditorForm.ShowObjectFramesButton.Checked ||
                                            levelEditorForm.ShowObjectsButton.Checked);

    internal bool EffectiveGrassFilter => GrassFilter &&
                                          (levelEditorForm.ShowGrassEdgesButton.Checked ||
                                           levelEditorForm.showGrassButton.Checked);

    internal bool EffectiveGroundFilter => GroundFilter &&
                                           (levelEditorForm.ShowGroundEdgesButton.Checked ||
                                            levelEditorForm.ShowGroundButton.Checked);

    internal bool EffectiveTextureFilter => TextureFilter &&
                                            (levelEditorForm.ShowTextureFramesButton.Checked || levelEditorForm.ShowTexturesButton.Checked);

    internal bool EffectivePictureFilter => PictureFilter &&
                                            (levelEditorForm.ShowPictureFramesButton.Checked || levelEditorForm.ShowPicturesButton.Checked);
}