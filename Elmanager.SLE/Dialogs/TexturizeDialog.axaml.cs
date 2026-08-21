using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaDialogs.Views;
using Elmanager.LevelEditor.Tools;
using Elmanager.Lgr;

namespace Elmanager.SLE.Dialogs;

internal partial class
    TexturizeDialog : BaseDialog<(ImageSelection.TextureSelection Selection, TexturizationOptions Options)>
{
    private static readonly HashSet<string> KnownMaskNames = ["maskbig", "maskhor", "masklitt"];

    private readonly Lgr.Lgr _lgr;
    private readonly string[] _maskNames;

    public TexturizeDialog()
    {
        InitializeComponent();
        _lgr = null!;
        _maskNames = [];
    }

    public TexturizeDialog(Lgr.Lgr lgr, TexturizationOptions? existingOptions)
    {
        InitializeComponent();
        _lgr = lgr;

        var textures = lgr.ListedImagesExcludingSpecial
            .Where(i => i.Type == ImageType.Texture)
            .OrderBy(i => i.Name, StringComparer.OrdinalIgnoreCase)
            .Select(i => lgr.ImageFromName(i.Name))
            .OfType<LgrImage>()
            .Select(ImageChoice.Create)
            .ToArray();
        _maskNames = lgr.ListedImagesExcludingSpecial
            .Where(i => i.Type == ImageType.Mask)
            .Select(i => i.Name)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        TextureComboBox.ItemsSource = textures;
        MaskListBox.ItemsSource = _maskNames;

        if (textures.Length > 0)
        {
            TextureComboBox.SelectedIndex = 0;
        }

        SelectKnownMasks();

        TextureComboBox.SelectionChanged += OnTextureSelectionChanged;

        if (existingOptions is { } opts)
        {
            ApplyExistingOptions(opts);
        }
        else
        {
            SetDefaultDistanceAndClipping();
        }
    }

    private void SelectKnownMasks()
    {
        for (var i = 0; i < _maskNames.Length; i++)
        {
            if (KnownMaskNames.Contains(_maskNames[i]))
            {
                MaskListBox.Selection.Select(i);
            }
        }
    }

    private void ApplyExistingOptions(TexturizationOptions opts)
    {
        MinCoverBox.Text = opts.MinCoverPercentage.ToString();
        IterationsBox.Text = opts.Iterations.ToString();

        TextureComboBox.SelectedItem = TextureComboBox.ItemsSource?
            .Cast<ImageChoice>()
            .FirstOrDefault(choice => choice.Name.Equals(
                opts.Texture.Txt.Name,
                StringComparison.OrdinalIgnoreCase));
        DistanceBox.Text = (opts.Texture.Distance ?? 500).ToString();
        ClippingComboBox.SelectedIndex = (int)(opts.Texture.Clipping ?? ClippingType.Unclipped);

        MaskListBox.Selection.Clear();
        for (var i = 0; i < _maskNames.Length; i++)
        {
            if (opts.SelectedMasks.Contains(_maskNames[i]))
            {
                MaskListBox.Selection.Select(i);
            }
        }
    }

    private void OnTextureSelectionChanged(object? sender, SelectionChangedEventArgs e) =>
        SetDefaultDistanceAndClipping();

    private void SetDefaultDistanceAndClipping()
    {
        var image = GetSelectedTexture();
        if (image is not null)
        {
            DistanceBox.Text = image.Distance.ToString();
            ClippingComboBox.SelectedIndex = (int)image.ClippingType;
        }
    }

    private LgrImage? GetSelectedTexture() =>
        TextureComboBox.SelectedItem is ImageChoice choice ? choice.Image : null;

    private void OkButton_Click(object? sender, RoutedEventArgs e)
    {
        var selectedMasks = MaskListBox.Selection.SelectedItems
            .Cast<string>()
            .ToList();

        if (selectedMasks.Count == 0)
        {
            _ = new SingleActionDialog { Message = "You have to select at least one mask.", ButtonText = "OK" }
                .ShowAsync();
            return;
        }

        if (!int.TryParse(IterationsBox.Text, out var iterations) || iterations <= 0)
        {
            _ = new SingleActionDialog { Message = "Iteration count must be at least 1.", ButtonText = "OK" }
                .ShowAsync();
            return;
        }

        if (!double.TryParse(MinCoverBox.Text, out var minCover) || minCover is <= 0 or > 100)
        {
            _ = new SingleActionDialog
            {
                Message = "Min cover % must be greater than 0 and less than or equal to 100.",
                ButtonText = "OK"
            }.ShowAsync();
            return;
        }

        if (!int.TryParse(DistanceBox.Text, out var distance) || distance is not (> 0 and < 1000))
        {
            _ = new SingleActionDialog
            {
                Message = "Distance is not valid! It must be an integer in range 1-999.",
                ButtonText = "OK"
            }.ShowAsync();
            return;
        }

        var texture = GetSelectedTexture();
        var maskName = selectedMasks.First();
        var mask = _lgr.ImageFromName(maskName);
        if (texture is null || mask is null)
        {
            _ = new SingleActionDialog { Message = "Please select a texture.", ButtonText = "OK" }.ShowAsync();
            return;
        }

        var clipping = (ClippingType)ClippingComboBox.SelectedIndex;
        var selection = ImageSelection.Texture(texture, mask, clipping, distance);
        var options = new TexturizationOptions(selection, minCover, iterations, selectedMasks);
        Close((selection, options));
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e) => Close();
}
