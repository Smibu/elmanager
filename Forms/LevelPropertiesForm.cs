﻿using System;

namespace Elmanager.Forms
{
    public sealed partial class LevelPropertiesForm
    {
        private readonly Level _level;

        internal LevelPropertiesForm(Level lev)
        {
            InitializeComponent();
            _level = lev;
            if (_level.Path != null)
                Text = "Level properties - " + _level.Path;
            else
                Text = "Level properties - New";
            PropertiesLabel.Text = "Polygons: " + _level.Polygons.Count + "\r\n" +
                                   "Vertices: " + _level.VertexCount + "\r\n" +
                                   "Ground polygons: " + _level.GroundPolygonCount + "\r\n" +
                                   "Ground vertices: " + _level.GroundVertexCount + "\r\n" +
                                   "Grass polygons: " + _level.GrassPolygonCount + "\r\n" +
                                   "Grass vertices: " + _level.GrassVertexCount + "\r\n" +
                                   "Objects: " + _level.Objects.Count + "\r\n" +
                                   "Apples: " + _level.AppleObjectCount + "\r\n" +
                                   "Killers: " + _level.KillerObjectCount + "\r\n" +
                                   "Flowers: " + _level.ExitObjectCount + "\r\n" +
                                   "Pictures: " + _level.PictureCount + "\r\n" +
                                   "Textures: " + _level.MaskCount;
            SinglePlayerTimesBox.Text = "";
            for (int i = 0; i <= 9; i++)
            {
                if (i < 9)
                    SinglePlayerTimesBox.Text += " ";
                SinglePlayerTimesBox.Text += (i + 1) + ". " + _level.Top10.GetSinglePlayerString(i) + "\r\n";
            }
            SinglePlayerTimesBox.Text += "Average: " + _level.Top10.GetSinglePlayerAverage().ToTimeString();
            MultiPlayerTimesBox.Text = "";
            for (int i = 0; i <= 9; i++)
            {
                if (i < 9)
                    MultiPlayerTimesBox.Text += " ";
                MultiPlayerTimesBox.Text += (i + 1) + ". " + _level.Top10.GetMultiPlayerString(i) + "\r\n";
            }
            MultiPlayerTimesBox.Text += "Average: " + _level.Top10.GetMultiPlayerAverage().ToTimeString();
        }

        private void OkButtonClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}