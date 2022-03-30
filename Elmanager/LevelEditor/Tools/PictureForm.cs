using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Elmanager.Lgr;
using Elmanager.Rendering;
using Elmanager.UI;

namespace Elmanager.LevelEditor.Tools;

internal partial class PictureForm
{
    private const string MultipleValues = "<multiple>";
    internal ImageSelection? Selection;
    private readonly Lgr.Lgr _lgr;
    private bool _autoTextureMode;
    private static readonly HashSet<string> KnownMaskNames = new() {"maskbig", "maskhor", "masklitt"};

    internal PictureForm(Lgr.Lgr lgr, GraphicElement? elem)
    {
        InitializeComponent();
        _lgr = lgr;
        SetupUi();
        if (elem is {})
        {
            SelectElement(elem);
        }
        else
        {
            SetSelection(null);
        }
    }

    private void SetSelection(ImageSelection? sel)
    {
        switch (sel)
        {
            case ImageSelection.PictureSelection p:
                PictureButton.Checked = true;
                PictureComboBox.SelectedItem = p.Pic.Name;
                break;
            case ImageSelection.TextureSelection t:
                TextureButton.Checked = true;
                TextureComboBox.SelectedItem = t.Txt.Name;
                MaskComboBox.SelectedItem = t.Mask.Name;
                break;
        }

        if (sel is {Distance: { } d, Clipping: { } c})
        {
            DistanceBox.Text = d.ToString();
            ClippingComboBox.SelectedIndex = (int) c;
        }
        else
        {
            SetDefaultDistanceAndClipping();
        }
        TextureButtonCheckedChanged();
    }

    internal bool AutoTextureMode
    {
        get => _autoTextureMode;
        set
        {
            masksLabel.Visible = value;
            minCoverLabel.Visible = value;
            iterationsLabel.Visible = value;
            maskListBox.Visible = value;
            minCoverTextBox.Visible = value;
            iterationsTextBox.Visible = value;
            MaskComboBox.Visible = !value;
            Label1.Visible = !value;
            if (value)
                TextureButton.Checked = true;
            PictureButton.Enabled = !value;
            _autoTextureMode = value;
            if (ImageBox.Image is { })
            {
                UpdatePicture(ImageBox.Image);
            }
        }
    }

    internal TexturizationOptions TexturizationOptions
    {
        get => new(
            Selection as ImageSelection.TextureSelection ??
            throw new InvalidOperationException("Expected a TextureSelection"),
            MinCoverPercentage,
            IterationCount, maskListBox.CheckedItems.Cast<string>().ToList());
        set
        {
            minCoverTextBox.Text = value.MinCoverPercentage.ToString();
            iterationsTextBox.Text = value.Iterations.ToString();
            SetSelection(value.Texture);
            for (int i = 0; i < maskListBox.Items.Count; i++)
            {
                maskListBox.SetItemChecked(i, value.SelectedMasks.Contains(maskListBox.Items[i]));
            }
        }
    }

    private double MinCoverPercentage => minCoverTextBox.Value;

    private int IterationCount => iterationsTextBox.Value;

    internal bool AllowMultiple { get; set; }

    internal bool SetDefaultsAutomatically { get; set; }

    private bool MultiplePicturesSelected => PictureComboBox.SelectedItem.ToString() == MultipleValues;

    private bool MultipleTexturesSelected => TextureComboBox.SelectedItem.ToString() == MultipleValues;

    private bool MultipleMaskSelected => MaskComboBox.SelectedItem?.ToString() == MultipleValues;

    private bool MultipleDistanceSelected => DistanceBox.Text == MultipleValues;

    private bool MultipleClippingSelected
    {
        get
        {
            if (ClippingComboBox.SelectedItem == null)
                return false;
            return ClippingComboBox.SelectedItem.ToString() == MultipleValues;
        }
    }

    private object[] GetNamesOf(ImageType type) =>
        _lgr.ListedImagesExcludingSpecial.Where(i => i.Type == type).Select(i => i.Name)
            .Concat(new object[] {MultipleValues}).ToArray();

    private void SetupUi()
    {
        TextureComboBox.SelectedIndexChanged -= TextureComboBoxSelectedIndexChanged;
        PictureComboBox.SelectedIndexChanged -= PictureComboBoxSelectedIndexChanged;
        TextureButton.CheckedChanged -= TextureButtonCheckedChanged;
        ClippingComboBox.Items.AddRange(new object[] {"Unclipped", "Ground", "Sky", MultipleValues});
        ClippingComboBox.SelectedIndex = 0;
        var maskNames = GetNamesOf(ImageType.Mask);
        MaskComboBox.Items.AddRange(maskNames);
        maskListBox.Items.AddRange(maskNames.SkipLast(1).ToArray());
        PictureComboBox.Items.AddRange(GetNamesOf(ImageType.Picture));
        TextureComboBox.Items.AddRange(GetNamesOf(ImageType.Texture));
        for (int i = 0; i < maskNames.Length - 1; i++)
        {
            maskListBox.SetItemCheckState(i,
                KnownMaskNames.Contains(maskNames[i]) ? CheckState.Checked : CheckState.Unchecked);
        }

        if (MaskItemCount > 1)
            MaskComboBox.SelectedIndex = 1;
        else
        {
            MaskComboBox.Enabled = false;
        }

        if (PictureItemCount > 1)
        {
            PictureComboBox.SelectedIndex = 1;
            PictureButton.Enabled = true;
        }
        else
            PictureButton.Enabled = false;

        if (TextureItemCount > 1)
        {
            TextureComboBox.SelectedIndex = 1;
            TextureButton.Enabled = MaskComboBox.Items.Count > 1;
        }
        else
            TextureButton.Enabled = false;

        OKButton.Enabled = PictureButton.Enabled || TextureButton.Enabled;
        DistanceBox.Enabled = OKButton.Enabled;
        ClippingComboBox.Enabled = OKButton.Enabled;
        ImageBox.Image = null;
        TextureComboBox.SelectedIndexChanged += TextureComboBoxSelectedIndexChanged;
        PictureComboBox.SelectedIndexChanged += PictureComboBoxSelectedIndexChanged;
        TextureButton.CheckedChanged += TextureButtonCheckedChanged;
    }

    private static bool IsMultipleSelected(ComboBox c)
    {
        if (c.SelectedItem == null)
        {
            return false;
        }

        return c.SelectedItem.ToString() == MultipleValues;
    }

    internal void SelectElement(GraphicElement graphicElement)
    {
        switch (graphicElement)
        {
            case GraphicElement.Picture p:
                PictureButton.Checked = true;
                PictureComboBox.SelectedItem = p.PictureInfo.Name;
                break;
            case GraphicElement.Texture t:
                TextureButton.Checked = true;
                TextureComboBox.SelectedItem = t.TextureInfo.Name;
                MaskComboBox.SelectedItem = t.MaskInfo.Name;
                break;
        }

        DistanceBox.Text = graphicElement.Distance.ToString();
        ClippingComboBox.SelectedIndex = (int) graphicElement.Clipping;
        TextureButtonCheckedChanged();
    }

    internal void SelectMultiple(List<GraphicElement> pictures)
    {
        if (pictures.TrueForAll(p => p is GraphicElement.Picture))
        {
            PictureButton.Checked = true;
            var piclist = pictures.Select(p => (p as GraphicElement.Picture)!).ToList();
            PictureComboBox.SelectedItem = piclist.TrueForAll(p => p.PictureInfo.Name == piclist[0].PictureInfo.Name)
                ? piclist[0].PictureInfo.Name
                : MultipleValues;
        }
        else if (pictures.TrueForAll(p => p is GraphicElement.Texture))
        {
            TextureButton.Checked = true;
            var texlist = pictures.Select(p => (p as GraphicElement.Texture)!).ToList();
            TextureComboBox.SelectedItem = texlist.TrueForAll(p => p.TextureInfo.Name == texlist[0].TextureInfo.Name)
                ? texlist[0].TextureInfo.Name
                : MultipleValues;

            MaskComboBox.SelectedItem = texlist.TrueForAll(p => p.MaskInfo.Name == texlist[0].MaskInfo.Name)
                ? texlist[0].MaskInfo.Name
                : MultipleValues;
        }
        else
        {
            PictureComboBox.SelectedItem = MultipleValues;
            TextureComboBox.SelectedItem = MultipleValues;
            MaskComboBox.SelectedItem = MultipleValues;
            PictureButton.Checked = false;
            TextureButton.Checked = false;
        }

        DistanceBox.Text = pictures.TrueForAll(p => p.Distance == pictures[0].Distance)
            ? pictures[0].Distance.ToString()
            : MultipleValues;

        if (pictures.TrueForAll(p => p.Clipping == pictures[0].Clipping))
        {
            ClippingComboBox.SelectedIndex = (int) (pictures[0].Clipping);
        }
        else
        {
            ClippingComboBox.SelectedItem = MultipleValues;
        }
    }

    private void PictureComboBoxSelectedIndexChanged(object? sender = null, EventArgs? e = null)
    {
        if (PictureItemCount > 0 && GetSelectedPicture() is { } selectedPicture && !IsMultipleSelected(PictureComboBox))
        {
            if (SetDefaultsAutomatically)
            {
                SetDefaultDistanceAndClipping();
            }

            UpdatePicture(selectedPicture.Bmp);
        }
    }

    private LgrImage? GetSelectedPicture() => GetSelectedImage(PictureComboBox);

    private int PictureItemCount => PictureComboBox.Items.Count;

    private void TextureComboBoxSelectedIndexChanged(object? sender = null, EventArgs? e = null)
    {
        if (TextureItemCount > 0 && GetSelectedTexture() is { } selectedTexture && !IsMultipleSelected(TextureComboBox))
        {
            if (SetDefaultsAutomatically)
            {
                SetDefaultDistanceAndClipping();
            }

            UpdatePicture(selectedTexture.Bmp);
        }
    }

    private LgrImage? GetSelectedTexture() => GetSelectedImage(TextureComboBox);

    private LgrImage? GetSelectedImage(ComboBox c) =>
        c.SelectedItem is string selectedItem ? _lgr.ImageFromName(selectedItem) : null;

    private int TextureItemCount => TextureComboBox.Items.Count;

    private void TextureButtonCheckedChanged(object? sender = null, EventArgs? e = null)
    {
        MaskComboBox.Enabled = TextureButton.Checked && MaskItemCount > 0;
        TextureComboBox.Enabled = TextureButton.Checked && TextureItemCount > 0 &&
                                  MaskItemCount > 0;
        PictureComboBox.Enabled = !TextureButton.Checked && PictureItemCount > 0;
        if (TextureComboBox.Enabled)
            TextureComboBoxSelectedIndexChanged();
        else
            PictureComboBoxSelectedIndexChanged();
    }

    private int MaskItemCount => MaskComboBox.Items.Count;

    private void UpdatePicture(Image bmp)
    {
        ImageBox.Image = bmp;
        ImageBox.Width = bmp.Width;
        ImageBox.Height = bmp.Height;
    }

    private void ButtonClick(object sender, EventArgs e)
    {
        Selection = null;
        bool ok = sender.Equals(OKButton);
        if (ok)
        {
            if (AutoTextureMode)
            {
                if (maskListBox.CheckedItems.Count == 0)
                {
                    UiUtils.ShowError("You have to select at least one mask.");
                    return;
                }

                if (IterationCount <= 0)
                {
                    UiUtils.ShowError("Iteration count must be at least 1.");
                    return;
                }

                if (MinCoverPercentage <= 0 || MinCoverPercentage > 100)
                {
                    UiUtils.ShowError("Min cover % must be greater than 0 and less than or equal to 100.");
                    return;
                }
            }

            var clipping = MultipleClippingSelected
                ? (ClippingType?) null
                : (ClippingType) ClippingComboBox.SelectedIndex;

            int? distance;
            try
            {
                distance = MultipleDistanceSelected ? null : int.Parse(DistanceBox.Text);
            }
            catch (FormatException)
            {
                DistanceError();
                return;
            }

            if (distance is not (> 0 and < 1000))
            {
                DistanceError();
                return;
            }

            if (PictureButton.Checked && MultiplePicturesSelected)
            {
                Selection = ImageSelection.Mixed(clipping, distance);
            }
            else if (PictureButton.Checked && !MultiplePicturesSelected)
            {
                Selection = ImageSelection.Picture(GetSelectedPicture()!, clipping, distance);
            }
            else if (TextureButton.Checked && MultipleTexturesSelected && !MultipleMaskSelected)
            {
                Selection = ImageSelection.MaskWithMultipleTextures(GetSelectedImage(MaskComboBox)!, clipping,
                    distance);
            }
            else if (TextureButton.Checked && !MultipleTexturesSelected && MultipleMaskSelected)
            {
                Selection = ImageSelection.TextureWithMultipleMasks(GetSelectedTexture()!, clipping, distance);
            }
            else if (TextureButton.Checked && !MultipleTexturesSelected && !MultipleMaskSelected)
            {
                Selection = ImageSelection.Texture(GetSelectedTexture()!, GetSelectedImage(MaskComboBox)!, clipping,
                    distance);
            }
            else if (TextureButton.Checked && MultipleTexturesSelected && MultipleMaskSelected)
            {
                Selection = ImageSelection.Mixed(clipping, distance);
            }
            else
            {
                Selection = ImageSelection.Mixed(clipping, distance);
            }

            if (!AllowMultiple &&
                (Selection is ImageSelection.MixedSelection or
                     ImageSelection.TextureSelectionMultipleMasks or
                     ImageSelection.TextureSelectionMultipleTextures || Selection.Clipping is null ||
                 Selection.Distance is null))
            {
                UiUtils.ShowError("You cannot select multiple values at this point.");
                return;
            }

            DialogResult = DialogResult.OK;
        }
        else
        {
            DialogResult = DialogResult.Cancel;
        }
    }

    private static void DistanceError()
    {
        UiUtils.ShowError("Distance is not valid! It must be an integer in range 1-999.");
    }

    private void DistanceBox_KeyPress(object sender, KeyPressEventArgs e)
    {
        SetDefaultsAutomatically = false;
    }

    private void ClippingComboBox_Click(object sender, EventArgs e)
    {
        SetDefaultsAutomatically = false;
    }

    private void ClippingComboBox_KeyPress(object sender, KeyPressEventArgs e)
    {
        SetDefaultsAutomatically = false;
    }

    private void SetDefaultsClicked(object sender, EventArgs e)
    {
        SetDefaultsAutomatically = true;
        SetDefaultDistanceAndClipping();
    }

    public void SetDefaultDistanceAndClipping()
    {
        var element = TextureButton.Checked ? GetSelectedTexture() : GetSelectedPicture();
        // can be null if <multiple> option is selected or the LGR has no pictures (e.g. Haddock)
        if (element is { })
        {
            DistanceBox.Text = element.Distance.ToString();
            ClippingComboBox.SelectedIndex = (int) element.ClippingType;
        }
    }

    private void MaskListBox_ItemCheck(object sender, ItemCheckEventArgs e)
    {
        if (e.NewValue == CheckState.Checked && !KnownMaskNames.Contains(maskListBox.Items[e.Index]))
        {
            UiUtils.ShowError(
                "Custom masks may not work accurately with texturization, but you can still try to use them.",
                "Warning", MessageBoxIcon.Exclamation);
        }
    }
}