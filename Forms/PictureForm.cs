using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Elmanager.Forms
{
    partial class PictureForm
    {
        private const string _multipleValues = "<multiple>";
        internal Level.ClippingType Clipping;
        internal int Distance;
        internal Lgr.LgrImage Mask;
        internal bool OkButtonPressed;
        internal Lgr.LgrImage Picture;
        internal Lgr.LgrImage Texture;
        internal bool TextureSelected;
        private Lgr _currentLgr;
        private bool _autoTextureMode;
        private bool _setDefaultsAutomatically;

        internal PictureForm(Lgr currentLgr)
        {
            InitializeComponent();
            UpdateLgr(currentLgr);
        }

        internal void UpdateLgr(Lgr newLgr)
        {
            _currentLgr = newLgr;
            UpdatePictureLists();
            TextureButtonCheckedChanged();
        }

        internal bool AutoTextureMode
        {
            get { return _autoTextureMode; }
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
                UpdatePicture(ImageBox.Image);
            }
        }

        internal IEnumerable<Lgr.LgrImage> SelectedMasks =>
            maskListBox.CheckedItems.Cast<string>()
                .Select(selectedItem => _currentLgr.ImageFromName(selectedItem.ToString()));

        internal double MinCoverPercentage => minCoverTextBox.Value;

        internal int IterationCount => iterationsTextBox.Value;

        internal int MinHeight => AutoTextureMode ? 293 : 185;

        internal bool AllowMultiple { get; set; }

        internal bool SetDefaultsAutomatically
        {
            get { return _setDefaultsAutomatically; }
            set
            {
                _setDefaultsAutomatically = value;
                if (value)
                {
                    SetDefaultDistanceAndClipping();
                }
            }
        }

        internal bool MultiplePicturesSelected
        {
            get { return PictureComboBox.SelectedItem.ToString() == _multipleValues; }
        }

        internal bool MultipleTexturesSelected
        {
            get { return TextureComboBox.SelectedItem.ToString() == _multipleValues; }
        }

        internal bool MultipleMaskSelected
        {
            get { return MaskComboBox.SelectedItem.ToString() == _multipleValues; }
        }

        internal bool MultipleDistanceSelected
        {
            get { return DistanceBox.Text == _multipleValues; }
        }

        internal bool MultipleClippingSelected
        {
            get
            {
                if (ClippingComboBox.SelectedItem == null)
                    return false;
                return ClippingComboBox.SelectedItem.ToString() == _multipleValues;
            }
        }

        internal bool AnyMultipleSelected
        {
            get
            {
                return MultipleClippingSelected || MultipleDistanceSelected || (MultipleMaskSelected && TextureSelected) ||
                       (MultiplePicturesSelected && !TextureSelected) || (MultipleTexturesSelected && TextureSelected);
            }
        }

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
            foreach (Lgr.ListedImage x in _currentLgr.ListedImagesIncludingGrass)
            {
                switch (x.Type)
                {
                    case Lgr.ImageType.Mask:
                        MaskComboBox.Items.Add(x.Name);
                        maskListBox.Items.Add(x.Name, true);
                        break;
                    case Lgr.ImageType.Picture:
                        PictureComboBox.Items.Add(x.Name);
                        break;
                    case Lgr.ImageType.Texture:
                        TextureComboBox.Items.Add(x.Name);
                        break;
                }
            }
            foreach (var c in new[] { MaskComboBox, PictureComboBox, TextureComboBox, ClippingComboBox })
            {
                c.Items.Add(_multipleValues);
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
            return c.SelectedItem.ToString() == _multipleValues;
        }

        internal void SelectTexture(Level.Picture picture)
        {
            if (picture.IsPicture)
            {
                PictureButton.Checked = true;
                PictureComboBox.SelectedItem = picture.Name;
            }else
            {
                TextureButton.Checked = true;
                TextureComboBox.SelectedItem = picture.TextureName;
                MaskComboBox.SelectedItem = picture.Name;
            }

            DistanceBox.Text = picture.Distance.ToString();
            ClippingComboBox.SelectedIndex = (int) (picture.Clipping);
        }

        internal void SelectMultiple(List<Level.Picture> pictures)
        {
            if (pictures.TrueForAll(p => p.IsPicture))
            {
                PictureButton.Checked = true;
                if (pictures.TrueForAll(p => p.Name == pictures[0].Name))
                {
                    PictureComboBox.SelectedItem = pictures[0].Name;
                }
                else
                {
                    PictureComboBox.SelectedItem = _multipleValues;
                }
            }
            else if (pictures.TrueForAll(p => !p.IsPicture))
            {
                TextureButton.Checked = true;
                if (pictures.TrueForAll(p => p.TextureName == pictures[0].TextureName))
                {
                    TextureComboBox.SelectedItem = pictures[0].TextureName;
                }
                else
                {
                    TextureComboBox.SelectedItem = _multipleValues;
                }

                if (pictures.TrueForAll(p => p.Name == pictures[0].Name))
                {
                    MaskComboBox.SelectedItem = pictures[0].Name;
                }
                else
                {
                    TextureComboBox.SelectedItem = _multipleValues;
                }
            }
            else
            {
                PictureComboBox.SelectedItem = _multipleValues;
                TextureComboBox.SelectedItem = _multipleValues;
                MaskComboBox.SelectedItem = _multipleValues;
                PictureButton.Checked = false;
                TextureButton.Checked = false;
            }

            if (pictures.TrueForAll(p => p.Distance == pictures[0].Distance))
            {
                DistanceBox.Text = pictures[0].Distance.ToString();
            }
            else
            {
                DistanceBox.Text = _multipleValues;
            }

            if (pictures.TrueForAll(p => p.Clipping == pictures[0].Clipping))
            {
                ClippingComboBox.SelectedIndex = (int)(pictures[0].Clipping);
            }
            else
            {
                ClippingComboBox.SelectedItem = _multipleValues;
            }
        }

        private void PictureComboBoxSelectedIndexChanged(object sender = null, EventArgs e = null)
        {
            if (PictureItemCount > 0 && !IsNothingSelected(PictureComboBox) && !IsMultipleSelected(PictureComboBox))
            {
                var selectedPicture = GetSelectedPicture();
                if (SetDefaultsAutomatically)
                {
                    SetDefaultDistanceAndClipping();
                }
                UpdatePicture(selectedPicture.Bmp);
            }
        }

        private Lgr.LgrImage GetSelectedPicture()
        {
            return _currentLgr.ImageFromName(PictureComboBox.SelectedItem.ToString());
        }

        private bool IsNothingSelected(ComboBox comboBox)
        {
            return comboBox.SelectedItem == null;
        }

        private int PictureItemCount
        {
            get { return PictureComboBox.Items.Count; }
        }

        private void TextureComboBoxSelectedIndexChanged(object sender = null, EventArgs e = null)
        {
            if (TextureItemCount > 0 && !IsNothingSelected(TextureComboBox) && !IsMultipleSelected(TextureComboBox))
            {
                var selectedTexture = GetSelectedTexture();
                if (SetDefaultsAutomatically)
                {
                    SetDefaultDistanceAndClipping();
                }
                UpdatePicture(selectedTexture.Bmp);
            }
        }

        private Lgr.LgrImage GetSelectedTexture()
        {
            return _currentLgr.ImageFromName(TextureComboBox.SelectedItem.ToString());
        }

        private int TextureItemCount
        {
            get { return TextureComboBox.Items.Count; }
        }

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

        private int MaskItemCount
        {
            get { return MaskComboBox.Items.Count; }
        }

        private void UpdatePicture(Image bmp)
        {
            ImageBox.Image = bmp;
            ImageBox.Width = bmp.Width;
            ImageBox.Height = bmp.Height;
            SetClientSizeCore(ImageBox.Location.X + bmp.Width + 5, Math.Max(ImageBox.Location.Y + bmp.Height, MinHeight) + 5);
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
                        Utils.ShowError("You have to select at least one mask.");
                        return;
                    }
                    if (IterationCount <= 0)
                    {
                        Utils.ShowError("Iteration count must be at least 1.");
                        return;
                    }
                    if (MinCoverPercentage <= 0 || MinCoverPercentage > 100)
                    {
                        Utils.ShowError("Min cover % must be greater than 0 and less than or equal to 100.");
                        return;
                    }
                }
                TextureSelected = TextureButton.Checked;
                if (!AllowMultiple)
                {
                    if (AnyMultipleSelected)
                    {
                        Utils.ShowError("You cannot select multiple values at this point.");
                        return;
                    }
                    try
                    {
                        Distance = int.Parse(DistanceBox.Text);
                        if (Distance > 0 && Distance < 1000)
                        {
                            Clipping = (Level.ClippingType) ClippingComboBox.SelectedIndex;
                            Picture = _currentLgr.ImageFromName(PictureComboBox.SelectedItem.ToString());
                            Texture = _currentLgr.ImageFromName(TextureComboBox.SelectedItem.ToString());
                            Mask = _currentLgr.ImageFromName(MaskComboBox.SelectedItem.ToString());
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
                        Clipping = (Level.ClippingType)ClippingComboBox.SelectedIndex;
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
            Utils.ShowError("Distance is not valid! It must be an integer in range 1-999.");
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
        }

        private void SetDefaultDistanceAndClipping()
        {
            var element = TextureButton.Checked ? GetSelectedTexture() : GetSelectedPicture();
            // can be null if <multiple> option is selected
            if (element != null)
            {
                DistanceBox.Text = element.Distance.ToString();
                ClippingComboBox.SelectedIndex = (int)element.ClippingType;
            }
        }
    }
}