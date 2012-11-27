using System;
using System.ComponentModel;
using System.Drawing;
using Microsoft.VisualBasic;

namespace Elmanager.Forms
{
    partial class PictureForm
    {
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

        private void UpdatePictureLists()
        {
            TextureComboBox.SelectedIndexChanged -= TextureComboBoxSelectedIndexChanged;
            PictureComboBox.SelectedIndexChanged -= PictureComboBoxSelectedIndexChanged;
            TextureButton.CheckedChanged -= TextureButtonCheckedChanged;
            MaskComboBox.Items.Clear();
            PictureComboBox.Items.Clear();
            TextureComboBox.Items.Clear();
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
            if (MaskComboBox.Items.Count > 0)
                MaskComboBox.SelectedIndex = 0;
            if (PictureComboBox.Items.Count > 0)
            {
                PictureComboBox.SelectedIndex = 0;
                PictureButton.Enabled = true;
            }
            else
                PictureButton.Enabled = false;
            if (TextureComboBox.Items.Count > 0)
            {
                TextureComboBox.SelectedIndex = 0;
                TextureButton.Enabled = MaskComboBox.Items.Count > 0;
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

        private void PictureComboBoxSelectedIndexChanged(object sender = null, EventArgs e = null)
        {
            if (PictureComboBox.Items.Count > 0)
            {
                Lgr.LgrImage selectedPicture = _currentLgr.ImageFromName(PictureComboBox.SelectedItem.ToString());
                DistanceBox.Text = selectedPicture.Distance.ToString();
                ClippingComboBox.SelectedIndex = (int) (selectedPicture.ClippingType);
                UpdatePicture(selectedPicture.Bmp);
            }
        }

        private void TextureComboBoxSelectedIndexChanged(object sender = null, EventArgs e = null)
        {
            if (TextureComboBox.Items.Count > 0)
            {
                Lgr.LgrImage selectedTexture = _currentLgr.ImageFromName(TextureComboBox.SelectedItem.ToString());
                DistanceBox.Text = selectedTexture.Distance.ToString();
                ClippingComboBox.SelectedIndex = (int) (selectedTexture.ClippingType);
                UpdatePicture(selectedTexture.Bmp);
            }
        }

        private void TextureButtonCheckedChanged(object sender = null, EventArgs e = null)
        {
            MaskComboBox.Enabled = TextureButton.Checked && MaskComboBox.Items.Count > 0;
            TextureComboBox.Enabled = TextureButton.Checked && TextureComboBox.Items.Count > 0 &&
                                      MaskComboBox.Items.Count > 0;
            PictureComboBox.Enabled = !TextureButton.Checked && PictureComboBox.Items.Count > 0;
            if (TextureComboBox.Enabled)
                TextureComboBoxSelectedIndexChanged();
            else
                PictureComboBoxSelectedIndexChanged();
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
                if (Information.IsNumeric(DistanceBox.Text))
                {
                    try
                    {
                        Distance = int.Parse(DistanceBox.Text);
                        if (Distance > 0 && Distance < 1000)
                        {
                            Clipping = (Level.ClippingType) ClippingComboBox.SelectedIndex;
                            Picture = _currentLgr.ImageFromName(PictureComboBox.SelectedItem.ToString());
                            Texture = _currentLgr.ImageFromName(TextureComboBox.SelectedItem.ToString());
                            Mask = _currentLgr.ImageFromName(MaskComboBox.SelectedItem.ToString());
                            TextureSelected = TextureButton.Checked;
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
                    DistanceError();
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