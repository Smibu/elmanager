using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    internal ClippingType Clipping;
    internal int Distance;
    internal LgrImage Mask;
    internal bool OkButtonPressed;
    internal LgrImage Picture;
    internal LgrImage Texture;
    internal bool TextureSelected;
    private Lgr.Lgr _currentLgr;
    private bool _autoTextureMode;
    private static HashSet<string> _knownMaskNames = new() {"maskbig", "maskhor", "masklitt"};

    internal PictureForm(Lgr.Lgr currentLgr)
    {
        InitializeComponent();
        UpdateLgr(currentLgr);
    }

    internal void UpdateLgr(Lgr.Lgr newLgr)
    {
        _currentLgr = newLgr;
        UpdatePictureLists();
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
            if (ImageBox.Image != null)
            {
                UpdatePicture(ImageBox.Image);
            }
        }
    }

    internal IEnumerable<LgrImage> SelectedMasks =>
        maskListBox.CheckedItems.Cast<string>()
            .Select(selectedItem => _currentLgr.ImageFromName(selectedItem.ToString()));

    internal double MinCoverPercentage => minCoverTextBox.Value;

    internal int IterationCount => iterationsTextBox.Value;

    internal bool AllowMultiple { get; set; }

    internal bool SetDefaultsAutomatically { get; set; }

    internal bool MultiplePicturesSelected => PictureComboBox.SelectedItem.ToString() == MultipleValues;

    internal bool MultipleTexturesSelected => TextureComboBox.SelectedItem.ToString() == MultipleValues;

    internal bool MultipleMaskSelected => MaskComboBox.SelectedItem?.ToString() == MultipleValues;

    internal bool MultipleDistanceSelected => DistanceBox.Text == MultipleValues;

    internal bool MultipleClippingSelected
    {
        get
        {
            if (ClippingComboBox.SelectedItem == null)
                return false;
            return ClippingComboBox.SelectedItem.ToString() == MultipleValues;
        }
    }

    internal bool AnyMultipleSelected => MultipleClippingSelected || MultipleDistanceSelected ||
                                         (MultipleMaskSelected && TextureSelected) ||
                                         (MultiplePicturesSelected && !TextureSelected) ||
                                         (MultipleTexturesSelected && TextureSelected);

    private void UpdatePictureLists()
    {
        TextureComboBox.SelectedIndexChanged -= TextureComboBoxSelectedIndexChanged;
        PictureComboBox.SelectedIndexChanged -= PictureComboBoxSelectedIndexChanged;
        TextureButton.CheckedChanged -= TextureButtonCheckedChanged;
        MaskComboBox.Items.Clear();
        maskListBox.Items.Clear();
        PictureComboBox.Items.Clear();
        TextureComboBox.Items.Clear();
        ClippingComboBox.Items.Clear();
        ClippingComboBox.Items.Add("Unclipped");
        ClippingComboBox.Items.Add("Ground");
        ClippingComboBox.Items.Add("Sky");
        ClippingComboBox.SelectedIndex = 0;
        foreach (ListedImage x in _currentLgr.ListedImagesExcludingSpecial)
        {
            switch (x.Type)
            {
                case ImageType.Mask:
                    MaskComboBox.Items.Add(x.Name);
                    maskListBox.Items.Add(x.Name, _knownMaskNames.Contains(x.Name));
                    break;
                case ImageType.Picture:
                    PictureComboBox.Items.Add(x.Name);
                    break;
                case ImageType.Texture:
                    TextureComboBox.Items.Add(x.Name);
                    break;
            }
        }

        foreach (var c in new[] {MaskComboBox, PictureComboBox, TextureComboBox, ClippingComboBox})
        {
            c.Items.Add(MultipleValues);
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

    internal void SelectElement(Picture picture)
    {
        if (picture.IsPicture)
        {
            PictureButton.Checked = true;
            PictureComboBox.SelectedItem = picture.Name;
        }
        else
        {
            TextureButton.Checked = true;
            TextureComboBox.SelectedItem = picture.TextureName;
            MaskComboBox.SelectedItem = picture.Name;
        }

        DistanceBox.Text = picture.Distance.ToString();
        ClippingComboBox.SelectedIndex = (int) picture.Clipping;
    }

    internal void SelectMultiple(List<Picture> pictures)
    {
        if (pictures.TrueForAll(p => p.IsPicture))
        {
            PictureButton.Checked = true;
            PictureComboBox.SelectedItem = pictures.TrueForAll(p => p.Name == pictures[0].Name)
                ? pictures[0].Name
                : MultipleValues;
        }
        else if (pictures.TrueForAll(p => !p.IsPicture))
        {
            TextureButton.Checked = true;
            TextureComboBox.SelectedItem = pictures.TrueForAll(p => p.TextureName == pictures[0].TextureName)
                ? pictures[0].TextureName
                : MultipleValues;

            if (pictures.TrueForAll(p => p.Name == pictures[0].Name))
            {
                MaskComboBox.SelectedItem = pictures[0].Name;
            }
            else
            {
                TextureComboBox.SelectedItem = MultipleValues;
            }
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

    private void PictureComboBoxSelectedIndexChanged(object sender = null, EventArgs e = null)
    {
        if (PictureItemCount > 0 && IsAnythingSelected(PictureComboBox) && !IsMultipleSelected(PictureComboBox))
        {
            var selectedPicture = GetSelectedPicture();
            if (SetDefaultsAutomatically)
            {
                SetDefaultDistanceAndClipping();
            }

            UpdatePicture(selectedPicture.Bmp);
        }
    }

    private LgrImage GetSelectedPicture()
    {
        return _currentLgr.ImageFromName(PictureComboBox.SelectedItem?.ToString());
    }

    private static bool IsAnythingSelected(ComboBox comboBox)
    {
        return comboBox.SelectedItem != null;
    }

    private int PictureItemCount => PictureComboBox.Items.Count;

    private void TextureComboBoxSelectedIndexChanged(object sender = null, EventArgs e = null)
    {
        if (TextureItemCount > 0 && IsAnythingSelected(TextureComboBox) && !IsMultipleSelected(TextureComboBox))
        {
            var selectedTexture = GetSelectedTexture();
            if (SetDefaultsAutomatically)
            {
                SetDefaultDistanceAndClipping();
            }

            UpdatePicture(selectedTexture.Bmp);
        }
    }

    private LgrImage GetSelectedTexture()
    {
        return _currentLgr.ImageFromName(TextureComboBox.SelectedItem?.ToString());
    }

    private int TextureItemCount => TextureComboBox.Items.Count;

    private void TextureButtonCheckedChanged(object sender = null, EventArgs e = null)
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

    private void PreventClose(object sender, CancelEventArgs e)
    {
        e.Cancel = true;
        Visible = false;
    }

    private void WhenShown(object sender, EventArgs e)
    {
        if (Visible)
            OkButtonPressed = false;
    }

    private void ButtonClick(object sender, EventArgs e)
    {
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

            TextureSelected = TextureButton.Checked;
            if (!AllowMultiple)
            {
                if (AnyMultipleSelected)
                {
                    UiUtils.ShowError("You cannot select multiple values at this point.");
                    return;
                }

                try
                {
                    Distance = int.Parse(DistanceBox.Text);
                    if (Distance > 0 && Distance < 1000)
                    {
                        Clipping = (ClippingType) ClippingComboBox.SelectedIndex;
                        Picture = _currentLgr.ImageFromName(PictureComboBox.SelectedItem.ToString());
                        Texture = _currentLgr.ImageFromName(TextureComboBox.SelectedItem.ToString());
                        Mask = MaskComboBox.SelectedItem != null ? _currentLgr.ImageFromName(MaskComboBox.SelectedItem.ToString()) : null;
                        CloseAndAccept(true);
                    }
                    else
                        DistanceError();
                }
                catch (FormatException)
                {
                    DistanceError();
                }
            }
            else
            {
                if (!MultiplePicturesSelected)
                {
                    Picture = _currentLgr.ImageFromName(PictureComboBox.SelectedItem.ToString());
                }

                if (!MultipleTexturesSelected)
                {
                    Texture = _currentLgr.ImageFromName(TextureComboBox.SelectedItem.ToString());
                }

                if (!MultipleMaskSelected)
                {
                    Mask = _currentLgr.ImageFromName(MaskComboBox.SelectedItem.ToString());
                }

                if (!MultipleClippingSelected)
                {
                    Clipping = (ClippingType) ClippingComboBox.SelectedIndex;
                }

                if (!MultipleDistanceSelected)
                {
                    try
                    {
                        Distance = int.Parse(DistanceBox.Text);
                    }
                    catch (FormatException)
                    {
                        DistanceError();
                        return;
                    }
                }

                CloseAndAccept(true);
            }
        }
        else
        {
            CloseAndAccept(false);
        }
    }

    private void CloseAndAccept(bool accept)
    {
        Close();
        OkButtonPressed = accept;
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
        if (element != null)
        {
            DistanceBox.Text = element.Distance.ToString();
            ClippingComboBox.SelectedIndex = (int) element.ClippingType;
        }
    }

    private void MaskListBox_ItemCheck(object sender, ItemCheckEventArgs e)
    {
        if (e.NewValue == CheckState.Checked && !_knownMaskNames.Contains(maskListBox.Items[e.Index]))
        {
            UiUtils.ShowError("Custom masks may not work accurately with texturization, but you can still try to use them.", "Warning", MessageBoxIcon.Exclamation);
        }
    }
}