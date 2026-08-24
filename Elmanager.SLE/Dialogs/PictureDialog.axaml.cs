using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaDialogs.Views;
using Elmanager.LevelEditor.Tools;
using Elmanager.Lgr;
using Elmanager.Rendering;
using SkiaSharp;

namespace Elmanager.SLE.Dialogs;

internal sealed record ImageChoice(LgrImage? Image, Bitmap? Preview)
{
    public const string MultipleValues = "<multiple>";

    public string Name => Image?.Name ?? MultipleValues;

    public bool IsMultiple => Image is null;

    public static ImageChoice Multiple { get; } = new(null, null);

    public static ImageChoice Create(LgrImage image) =>
        new(image, SkBitmapToAvaloniaBitmap(image.Bmp));

    private static Bitmap SkBitmapToAvaloniaBitmap(SKBitmap skBitmap)
        => new(
            PixelFormat.Rgba8888,
            AlphaFormat.Unpremul,
            skBitmap.GetPixels(),
            new PixelSize(skBitmap.Width, skBitmap.Height),
            new Vector(96, 96),
            skBitmap.RowBytes);
}

internal partial class PictureDialog : BaseDialog<ImageSelection>
{
    private readonly Lgr.Lgr _lgr;
    private readonly bool _setDefaultsAutomatically;
    private bool _allowMultiple;
    private ComboBoxItem? _multipleClippingItem;

    public PictureDialog()
    {
        InitializeComponent();
        _lgr = null!;
    }

    public PictureDialog(Lgr.Lgr lgr, bool setDefaultsAutomatically)
    {
        InitializeComponent();
        _lgr = lgr;
        _setDefaultsAutomatically = setDefaultsAutomatically;

        SetupUi();

        PictureRadio.IsCheckedChanged += OnImageTypeChanged;
        TextureRadio.IsCheckedChanged += OnImageTypeChanged;
        PictureComboBox.SelectionChanged += OnPictureSelectionChanged;
        TextureComboBox.SelectionChanged += OnTextureSelectionChanged;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        TopLevel.GetTopLevel(this)?.RequestAnimationFrame(_ => Console.WriteLine("Picture dialog ready"));
    }

    private void SetupUi()
    {
        var pictures = _lgr.ListedImagesExcludingSpecial
            .Where(i => i.Type == ImageType.Picture)
            .OrderBy(i => i.Name, StringComparer.OrdinalIgnoreCase)
            .Select(i => _lgr.ImageFromName(i.Name))
            .OfType<LgrImage>()
            .Select(ImageChoice.Create)
            .ToArray();
        var textures = _lgr.ListedImagesExcludingSpecial
            .Where(i => i.Type == ImageType.Texture)
            .OrderBy(i => i.Name, StringComparer.OrdinalIgnoreCase)
            .Select(i => _lgr.ImageFromName(i.Name))
            .OfType<LgrImage>()
            .Select(ImageChoice.Create)
            .ToArray();
        var masks = _lgr.ListedImagesExcludingSpecial
            .Where(i => i.Type == ImageType.Mask)
            .Select(i => i.Name)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        PictureComboBox.ItemsSource = pictures;
        TextureComboBox.ItemsSource = textures;
        MaskComboBox.ItemsSource = masks;

        if (pictures.Length > 0)
        {
            PictureComboBox.SelectedIndex = 0;
        }
        else
        {
            PictureRadio.IsEnabled = false;
        }

        if (textures.Length > 0)
        {
            TextureComboBox.SelectedIndex = 0;
        }

        if (masks.Length > 0)
        {
            MaskComboBox.SelectedIndex = 0;
        }
        else
        {
            TextureRadio.IsEnabled = false;
        }

        if (!PictureRadio.IsEnabled && TextureRadio.IsEnabled)
        {
            TextureRadio.IsChecked = true;
        }

        if (_setDefaultsAutomatically)
        {
            SetDefaultDistanceAndClipping();
        }
    }

    public void SelectElement(GraphicElement element)
    {
        switch (element)
        {
            case GraphicElement.Picture p:
                PictureRadio.IsChecked = true;
                SelectImage(PictureComboBox, p.PictureInfo.Name);
                break;
            case GraphicElement.Texture t:
                TextureRadio.IsChecked = true;
                SelectImage(TextureComboBox, t.TextureInfo.Name);
                MaskComboBox.SelectedItem = t.MaskInfo.Name;
                break;
        }

        DistanceBox.Text = element.Distance.ToString();
        ClippingComboBox.SelectedIndex = (int)element.Clipping;
    }

    public void SelectMultiple(IReadOnlyList<GraphicElement> elements)
    {
        if (elements.Count < 2)
        {
            throw new ArgumentException("Multiple elements are required.", nameof(elements));
        }

        EnableMultipleValues();

        if (elements.All(element => element is GraphicElement.Picture))
        {
            PictureRadio.IsChecked = true;
            var pictures = elements.Cast<GraphicElement.Picture>().ToList();
            if (pictures.All(picture => picture.PictureInfo.Name == pictures[0].PictureInfo.Name))
            {
                SelectImage(PictureComboBox, pictures[0].PictureInfo.Name);
            }
            else
            {
                PictureComboBox.SelectedItem = ImageChoice.Multiple;
            }
        }
        else if (elements.All(element => element is GraphicElement.Texture))
        {
            TextureRadio.IsChecked = true;
            var textures = elements.Cast<GraphicElement.Texture>().ToList();
            if (textures.All(texture => texture.TextureInfo.Name == textures[0].TextureInfo.Name))
            {
                SelectImage(TextureComboBox, textures[0].TextureInfo.Name);
            }
            else
            {
                TextureComboBox.SelectedItem = ImageChoice.Multiple;
            }

            MaskComboBox.SelectedItem = textures.All(texture => texture.MaskInfo.Name == textures[0].MaskInfo.Name)
                ? textures[0].MaskInfo.Name
                : ImageChoice.MultipleValues;
        }
        else
        {
            PictureComboBox.SelectedItem = ImageChoice.Multiple;
            TextureComboBox.SelectedItem = ImageChoice.Multiple;
            MaskComboBox.SelectedItem = ImageChoice.MultipleValues;
            PictureRadio.IsChecked = false;
            TextureRadio.IsChecked = false;
        }

        DistanceBox.Text = elements.All(element => element.Distance == elements[0].Distance)
            ? elements[0].Distance.ToString()
            : ImageChoice.MultipleValues;

        if (elements.All(element => element.Clipping == elements[0].Clipping))
        {
            ClippingComboBox.SelectedIndex = (int)elements[0].Clipping;
        }
        else
        {
            ClippingComboBox.SelectedItem = _multipleClippingItem;
        }
    }

    private void EnableMultipleValues()
    {
        _allowMultiple = true;
        PictureComboBox.ItemsSource = PictureComboBox.ItemsSource?
            .Cast<ImageChoice>()
            .Append(ImageChoice.Multiple)
            .ToArray();
        TextureComboBox.ItemsSource = TextureComboBox.ItemsSource?
            .Cast<ImageChoice>()
            .Append(ImageChoice.Multiple)
            .ToArray();
        MaskComboBox.ItemsSource = MaskComboBox.ItemsSource?
            .Cast<string>()
            .Append(ImageChoice.MultipleValues)
            .ToArray();

        _multipleClippingItem = new ComboBoxItem { Content = ImageChoice.MultipleValues };
        ClippingComboBox.Items.Add(_multipleClippingItem);
    }

    private void OnImageTypeChanged(object? sender, RoutedEventArgs e)
    {
        if (_setDefaultsAutomatically)
        {
            SetDefaultDistanceAndClipping();
        }
    }

    private void OnPictureSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_setDefaultsAutomatically)
        {
            SetDefaultDistanceAndClipping();
        }
    }

    private void OnTextureSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_setDefaultsAutomatically)
        {
            SetDefaultDistanceAndClipping();
        }
    }

    private ImageChoice? GetCurrentImageChoice() =>
        (TextureRadio.IsChecked == true ? TextureComboBox : PictureComboBox).SelectedItem as ImageChoice;

    private LgrImage? GetCurrentLgrImage() => GetCurrentImageChoice()?.Image;

    private static void SelectImage(ComboBox comboBox, string name) =>
        comboBox.SelectedItem = comboBox.ItemsSource?
            .Cast<ImageChoice>()
            .FirstOrDefault(choice => string.Equals(choice.Name, name, StringComparison.OrdinalIgnoreCase));

    private void SetDefaultDistanceAndClipping()
    {
        var image = GetCurrentLgrImage();
        if (image is not null)
        {
            DistanceBox.Text = image.Distance.ToString();
            ClippingComboBox.SelectedIndex = (int)image.ClippingType;
        }
    }

    private void OkButton_Click(object? sender, RoutedEventArgs e)
    {
        int? distance;
        if (_allowMultiple && DistanceBox.Text == ImageChoice.MultipleValues)
        {
            distance = null;
        }
        else if (!int.TryParse(DistanceBox.Text, out var parsedDistance) ||
                 parsedDistance is not (> 0 and < 1000))
        {
            _ = new SingleActionDialog
            {
                Message = "Distance is not valid! It must be an integer in range 1-999.",
                ButtonText = "OK"
            }.ShowAsync();
            return;
        }
        else
        {
            distance = parsedDistance;
        }

        var clipping = _allowMultiple && ReferenceEquals(ClippingComboBox.SelectedItem, _multipleClippingItem)
            ? (ClippingType?)null
            : (ClippingType)ClippingComboBox.SelectedIndex;

        ImageSelection selection;

        if (TextureRadio.IsChecked == true)
        {
            var texture = TextureComboBox.SelectedItem is ImageChoice textureChoice ? textureChoice.Image : null;
            var multipleTextures = TextureComboBox.SelectedItem is ImageChoice { IsMultiple: true };
            var multipleMasks = MaskComboBox.SelectedItem is ImageChoice.MultipleValues;
            var mask = MaskComboBox.SelectedItem is string maskName && !multipleMasks
                ? _lgr.ImageFromName(maskName)
                : null;

            if (multipleTextures && multipleMasks)
            {
                selection = ImageSelection.Mixed(clipping, distance);
            }
            else if (multipleTextures && mask is not null)
            {
                selection = ImageSelection.MaskWithMultipleTextures(mask, clipping, distance);
            }
            else if (texture is not null && multipleMasks)
            {
                selection = ImageSelection.TextureWithMultipleMasks(texture, clipping, distance);
            }
            else if (texture is not null && mask is not null)
            {
                selection = ImageSelection.Texture(texture, mask, clipping, distance);
            }
            else
            {
                _ = new SingleActionDialog { Message = "Please select both a texture and a mask.", ButtonText = "OK" }
                    .ShowAsync();
                return;
            }
        }
        else if (PictureRadio.IsChecked == true)
        {
            var picture = PictureComboBox.SelectedItem is ImageChoice pictureChoice ? pictureChoice.Image : null;
            if (PictureComboBox.SelectedItem is ImageChoice { IsMultiple: true })
            {
                selection = ImageSelection.Mixed(clipping, distance);
            }
            else if (picture is not null)
            {
                selection = ImageSelection.Picture(picture, clipping, distance);
            }
            else
            {
                _ = new SingleActionDialog { Message = "Please select a picture.", ButtonText = "OK" }.ShowAsync();
                return;
            }
        }
        else if (_allowMultiple)
        {
            selection = ImageSelection.Mixed(clipping, distance);
        }
        else
        {
            _ = new SingleActionDialog { Message = "Please select a picture or texture.", ButtonText = "OK" }
                .ShowAsync();
            return;
        }

        Close(selection);
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e) => Close();
}
