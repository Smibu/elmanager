namespace Elmanager.LevelEditor.Tools;

public interface ISelectionFilter
{
    bool EffectiveAppleFilter { get; }
    bool EffectiveKillerFilter { get; }
    bool EffectiveFlowerFilter { get; }
    bool EffectiveStartFilter { get; }
    bool EffectiveGrassFilter { get; }
    bool EffectiveGroundFilter { get; }
    bool EffectivePictureFilter { get; }
    bool EffectiveTextureFilter { get; }
}
