using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Elmanager.Application;
using Elmanager.IO;
using Elmanager.Rec;
using Elmanager.UI;
using Elmanager.Utilities;
using Microsoft.VisualBasic;

namespace Elmanager.ReplayManager
{
    internal partial class RenameForm : FormMod
    {
        private readonly IList<Replay> _replaysToRename;
        private readonly ReplayManagerForm _rm;

        internal RenameForm(IList<Replay> replays, ReplayManagerForm rm)
        {
            InitializeComponent();
            _replaysToRename = replays;
            _rm = rm;
        }

        private void LoadSettings(object sender, EventArgs e)
        {
            NickBox.Text = Global.AppSettings.ReplayManager.Nickname;
            PatternBox.Text = Global.AppSettings.ReplayManager.Pattern;
        }

        private void SaveSettings(object sender, FormClosingEventArgs e)
        {
            Global.AppSettings.ReplayManager.Nickname = NickBox.Text;
            Global.AppSettings.ReplayManager.Pattern = PatternBox.Text;
        }

        private void RenameSelected(object sender, EventArgs e)
        {
            if (PatternBox.TextLength > 0)
            {
                for (int i = 0; i < PatternBox.TextLength; i++)
                {
                    char c = PatternBox.Text[i];
                    if (c != 'N' && c != 'T' && c != 'L' && c != 'F')
                    {
                        UiUtils.ShowError("Pattern string contains invalid character \"" + c + "\"!");
                        return;
                    }
                }

                foreach (Replay rp in _replaysToRename)
                {
                    string timePart = rp.Time.ToTimeString();
                    timePart = timePart.Substring(0, timePart.Length - 1); //Delete 3rd decimal
                    int j;
                    for (j = 0; j <= 1; j++) //Remove leading zeroes (like from 00:23,56)
                    {
                        if (timePart[0] == '0')
                        {
                            timePart = timePart.Remove(0, 1);
                        }
                        else
                        {
                            break;
                        }
                    }

                    j = 0;
                    while (j < timePart.Length)
                    {
                        if (timePart[j] == ':' || timePart[j] == ',')
                        {
                            timePart = timePart.Remove(j, 1);
                        }

                        j++;
                    }

                    if (timePart[0] == '0')
                    {
                        timePart = timePart.Remove(0, 1);
                    }

                    string newName = string.Empty;
                    for (j = 0; j < PatternBox.TextLength; j++)
                    {
                        switch (PatternBox.Text[j])
                        {
                            case 'L':
                                if (rp.IsInternal)
                                {
                                    newName += Strings.Mid(rp.LevelFilename, 7, 2);
                                }
                                else
                                {
                                    newName += Strings.Left(rp.LevelFilename, rp.LevelFilename.Length - 4);
                                }

                                break;
                            case 'N':
                                newName += NickBox.Text;
                                break;
                            case 'T':
                                newName += timePart;
                                break;
                            case 'F':
                                newName += Path.GetFileNameWithoutExtension(rp.FileName);
                                break;
                        }
                    }

                    if (!rp.FileName.CompareWith(newName + DirUtils.RecExtension))
                    {
                        _rm.Rename(rp, newName);
                    }
                }

                Close();
            }
            else
            {
                UiUtils.ShowError("Pattern string must not be empty!");
            }
        }
    }
}