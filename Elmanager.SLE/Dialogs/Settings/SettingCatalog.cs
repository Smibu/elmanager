using System;
using System.Collections.Generic;
using Avalonia.Platform.Storage;
using Elmanager.LevelEditor;
using Elmanager.LevelEditor.Playing;
using Elmanager.SLE.Platform;

namespace Elmanager.SLE.Dialogs.Settings;

internal static class SettingCatalog
{
    public static IReadOnlyList<SettingsCategoryViewModel> Create(
        LevelEditorSettings settings,
        IStorageProvider storageProvider,
        Action onChanged)
    {
        var rendering = settings.RenderingSettings;
        var playing = settings.PlayingSettings;
        var create = new SettingViewModelFactory(storageProvider, onChanged);

        return
        [
            new SettingsCategoryViewModel("General",
            [
                create.Folder(
                    "Level folder",
                    () => settings.LevelFolder,
                    value => settings.LevelFolder = value),
                create.Folder(
                    "LGR folder",
                    () => settings.LgrFolder,
                    value => settings.LgrFolder = value),
                create.Folder(
                    "Shape folder",
                    () => settings.ShapeFolder,
                    value => settings.ShapeFolder = value),
                create.File(
                    "New level template",
                    () => settings.LevelTemplate,
                    value => settings.LevelTemplate = value,
                    LevelFileTypes.LevType),
                create.Number(
                    "Toolbar icon size",
                    () => settings.ToolbarIconSize,
                    value => settings.ToolbarIconSize = value,
                    LevelEditorSettings.MinToolbarIconSize,
                    LevelEditorSettings.MaxToolbarIconSize,
                    LevelEditorSettings.ToolbarIconSizeStep,
                    "You can also adjust this with Ctrl + mouse wheel when hovering toolbar."),
                create.Number(
                    "Mouse capture radius",
                    () => settings.CaptureRadius,
                    value => settings.CaptureRadius = value,
                    1,
                    50,
                    1,
                    "Capture radius in pixels."),
                create.Text(
                    "Default title",
                    () => settings.DefaultTitle,
                    value => settings.DefaultTitle = value),
                create.Text(
                    "Default filename",
                    () => settings.DefaultFilename,
                    value => settings.DefaultFilename = value,
                    "Use ? characters for a zero-padded number, e.g. MyLev???",
                    FilenameSuggestion.ValidatePattern),
                create.Boolean(
                    "Check topology when saving",
                    () => settings.CheckTopologyWhenSaving,
                    value => settings.CheckTopologyWhenSaving = value),
                create.Boolean(
                    "Check topology after every change",
                    () => settings.CheckTopologyDynamically,
                    value => settings.CheckTopologyDynamically = value),
                create.Boolean(
                    "Use filename for title",
                    () => settings.UseFilenameForTitle,
                    value => settings.UseFilenameForTitle = value),
                create.Boolean(
                    "Capture pictures and textures from borders only",
                    () => settings.CapturePicturesAndTexturesFromBordersOnly,
                    value => settings.CapturePicturesAndTexturesFromBordersOnly = value),
                create.Boolean(
                    "Always set defaults in picture tool",
                    () => settings.AlwaysSetDefaultsInPictureTool,
                    value => settings.AlwaysSetDefaultsInPictureTool = value),
                create.Boolean(
                    "Enable start position feature",
                    () => settings.EnableStartPositionFeature,
                    value => settings.EnableStartPositionFeature = value)
            ]),
            new SettingsCategoryViewModel("Colors",
            [
                create.Color(
                    "Ground fill",
                    () => rendering.GroundFillColor,
                    value => rendering.GroundFillColor = value),
                create.Color(
                    "Ground edge",
                    () => rendering.GroundEdgeColor,
                    value => rendering.GroundEdgeColor = value),
                create.Color(
                    "Grass edge",
                    () => rendering.GrassEdgeColor,
                    value => rendering.GrassEdgeColor = value),
                create.Color(
                    "Sky fill",
                    () => rendering.SkyFillColor,
                    value => rendering.SkyFillColor = value),
                create.Color(
                    "Apple",
                    () => rendering.AppleColor,
                    value => rendering.AppleColor = value),
                create.Color(
                    "Flower",
                    () => rendering.FlowerColor,
                    value => rendering.FlowerColor = value),
                create.Color(
                    "Start",
                    () => rendering.StartColor,
                    value => rendering.StartColor = value),
                create.Color(
                    "Killer",
                    () => rendering.KillerColor,
                    value => rendering.KillerColor = value),
                create.Color(
                    "Grid",
                    () => rendering.GridColor,
                    value => rendering.GridColor = value),
                create.Color(
                    "Vertex",
                    () => rendering.VertexColor,
                    value => rendering.VertexColor = value),
                create.Color(
                    "Picture frame",
                    () => rendering.PictureFrameColor,
                    value => rendering.PictureFrameColor = value),
                create.Color(
                    "Texture frame",
                    () => rendering.TextureFrameColor,
                    value => rendering.TextureFrameColor = value),
                create.Color(
                    "Apple gravity arrow",
                    () => rendering.AppleGravityArrowColor,
                    value => rendering.AppleGravityArrowColor = value),
                create.Color(
                    "Crosshair",
                    () => rendering.CrosshairColor,
                    value => rendering.CrosshairColor = value),
                create.Color(
                    "Highlight",
                    () => rendering.HighlightColor,
                    value => rendering.HighlightColor = value),
                create.Color(
                    "Selection",
                    () => rendering.SelectionColor,
                    value => rendering.SelectionColor = value)
            ]),
            new SettingsCategoryViewModel("Display",
            [
                create.Boolean(
                    "Pictures in background",
                    () => rendering.PicturesInBackground,
                    value => rendering.PicturesInBackground = value),
                create.Number(
                    "Circle drawing accuracy",
                    () => rendering.CircleDrawingAccuracy,
                    value => rendering.CircleDrawingAccuracy = value,
                    3,
                    100,
                    1,
                    "The number of vertices used to draw a circle."),
                create.Number(
                    "Grid size",
                    () => rendering.GridSize,
                    value => rendering.GridSize = value,
                    double.Epsilon,
                    double.MaxValue,
                    1,
                    allowDecimalInput: true),
                create.Number(
                    "Line width",
                    () => rendering.LineWidth,
                    value => rendering.LineWidth = (float)value,
                    float.Epsilon,
                    float.MaxValue,
                    1,
                    allowDecimalInput: true),
                create.Number(
                    "Point size",
                    () => rendering.PointSize,
                    value => rendering.PointSize = (float)value,
                    float.MinValue,
                    float.MaxValue,
                    1,
                    allowDecimalInput: true),
                create.Number(
                    "Grass zoom",
                    () => rendering.GrassZoom,
                    value => rendering.GrassZoom = value,
                    1,
                    3,
                    0.05,
                    "Grass detail level. Set this the same as eolconf zoom to make grass look the same as in EOL.",
                    true)
            ]),
            new SettingsCategoryViewModel("Playing",
            [
                create.Key(
                    "Gas",
                    () => playing.Gas,
                    value => playing.Gas = value),
                create.Key(
                    "Brake",
                    () => playing.Brake,
                    value => playing.Brake = value),
                create.Key(
                    "Brake alias",
                    () => playing.BrakeAlias,
                    value => playing.BrakeAlias = value),
                create.Key(
                    "Left volt",
                    () => playing.LeftVolt,
                    value => playing.LeftVolt = value),
                create.Key(
                    "Right volt",
                    () => playing.RightVolt,
                    value => playing.RightVolt = value),
                create.Key(
                    "Alovolt",
                    () => playing.AloVolt,
                    value => playing.AloVolt = value),
                create.Key(
                    "Turn",
                    () => playing.Turn,
                    value => playing.Turn = value),
                create.Key(
                    "Esc alias",
                    () => playing.EscAlias,
                    value => playing.EscAlias = value),
                create.Choice(
                    "When dying",
                    () => (int)playing.DyingBehavior,
                    value => playing.DyingBehavior = (DyingBehavior)value,
                    ["Stop playing", "Pause playing", "Restart playing", "Be invulnerable"]),
                create.Choice(
                    "Follow driver",
                    () => (int)playing.FollowDriverOption,
                    value => playing.FollowDriverOption = (FollowDriverOption)value,
                    ["When pressing a playing key", "Never"]),
                create.Key(
                    "Save",
                    () => playing.Save,
                    value => playing.Save = value),
                create.Key(
                    "Load",
                    () => playing.Load,
                    value => playing.Load = value),
                create.Boolean(
                    "Disable shortcut keys",
                    () => playing.DisableShortcuts,
                    value => playing.DisableShortcuts = value),
                create.Number(
                    "Physics FPS",
                    () => playing.PhysicsFps,
                    value => playing.PhysicsFps = value,
                    79,
                    1000,
                    1),
                create.Boolean(
                    "Constant physics FPS",
                    () => playing.ConstantFps,
                    value => playing.ConstantFps = value,
                    "If enabled, physics uses the same timestep for every frame. This makes rides deterministic, " +
                    "but disabling it is closer to Elma's variable physics FPS."),
                create.Boolean(
                    "Toggle fullscreen on play/stop",
                    () => playing.ToggleFullscreen,
                    value => playing.ToggleFullscreen = value,
                    "Regardless of this option, you can use F11 to toggle fullscreen.")
            ])
        ];
    }
}
