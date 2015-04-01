using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Elmanager.Forms
{
    partial class PictureForm
    {
        private const string _multipleValues = "<multiple values>";
        internal Level.ClippingType Clipping;
        internal int Distance;
        internal Lgr.LgrImage Mask;
        internal bool OkButtonPressed;
        internal Lgr.LgrImage Picture;
        internal Lgr.LgrImage Texture;
        internal bool TextureSelected;
        private Lgr _currentLgr;

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

        internal bool AllowMultiple { get; set; }

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
            PictureComboBox.Items.Clear();
            TextureComboBox.Items.Clear();
            ClippingComboBox.Items.Clear();
            ClippingComboBox.Items.Add("Unclipped");
            ClippingComboBox.Items.Add("Ground");
            ClippingComboBox.Items.Add("Sky");
            foreach (Lgr.ListedImage x in _currentLgr.ListedImages)
            {
                if (x.Name[0] != 'q')
                {
                    switch (x.Type)
                    {
                        case Lgr.ImageType.Mask:
                            MaskComboBox.Items.Add(x.Name);
                            break;
                        case Lgr.ImageType.Picture:
                            PictureComboBox.Items.Add(x.Name);
                            break;
                        case Lgr.ImageType.Texture:
                            TextureComboBox.Items.Add(x.Name);
                            break;
                    }
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
                Lgr.LgrImage selectedPicture = _currentLgr.ImageFromName(PictureComboBox.SelectedItem.ToString());
                if (!MultipleDistanceSelected)
                {
                    DistanceBox.Text = selectedPicture.Distance.ToString();
                }
                if (!MultipleClippingSelected)
                {
                    ClippingComboBox.SelectedIndex = (int) (selectedPicture.ClippingType);
                }
                UpdatePicture(selectedPicture.Bmp);
            }
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
                Lgr.LgrImage selectedTexture = _currentLgr.ImageFromName(TextureComboBox.SelectedItem.ToString());
                if (!MultipleDistanceSelected)
                {
                    DistanceBox.Text = selectedTexture.Distance.ToString();
                }
                if (!MultipleClippingSelected)
                {
                    ClippingComboBox.SelectedIndex = (int) (selectedTexture.ClippingType);
                }
                UpdatePicture(selectedTexture.Bmp);
            }
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
            Width = ImageBox.Location.X + bmp.Width + 20;
            Height = Math.Max(ImageBox.Location.Y + bmp.Height + 40, 218);
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
            OkButtonPressed = sender.Equals(OKButton);
            if (OkButtonPressed)
            {
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
                            Close();
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
                    Close();
                }
                
            }
            else
                Close();
        }

        private static void DistanceError()
        {
            Utils.ShowError("Distance is not valid! It must be an integer in range 1-999.");
        }
    }
}