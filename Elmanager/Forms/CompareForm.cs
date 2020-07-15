using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Elmanager.CustomControls;

namespace Elmanager.Forms
{
    partial class CompareForm : FormMod
    {
        private readonly List<Replay> _comparingRps;
        private readonly int[][] _touchCheckPoints;

        internal CompareForm(List<Replay> rps)
        {
            InitializeComponent();
            //Assuming replays are singleplayer replays and they have same level
            _comparingRps = rps;
            LevelLabel.Text = "Level: ";
            if (rps[0].IsInternal)
                LevelLabel.Text += "Internal " + rps[0].LevelFilename.Substring(6, 2);
            else
                LevelLabel.Text += rps[0].LevelFilename;
            foreach (var t in rps)
                CRBox.Items.Add(Path.GetFileName(t.Path));
            Array.Resize(ref _touchCheckPoints, rps.Count);
            for (int i = 0; i < rps.Count; i++)
                Array.Resize(ref _touchCheckPoints[i], 0);
            CRBox.SelectedIndex = 0;
        }

        private void RadioButton2CheckedChanged(object sender, EventArgs e)
        {
            Label3.Visible = GTButton.Checked;
            CPListBox.Visible = GTButton.Checked;
            CrBoxSelectedIndexChanged(null, null);
        }

        private void Compare(object sender, EventArgs e)
        {
            int[][] checkPoints;
            double[][] eventTimes = new double[_comparingRps.Count][];
            int[][] appleCheckPoints = new int[_comparingRps.Count][];
            for (int i = 0; i < _comparingRps.Count; i++)
            {
                if (AppleButton.Checked)
                    eventTimes[i] = _comparingRps[i].Player1.GetEventTimes(LogicalEventType.AppleTake);
                else
                    eventTimes[i] = _comparingRps[i].Player1.GetEventTimes(LogicalEventType.GroundTouch);
                if (_comparingRps[i].Finished)
                {
                    Array.Resize(ref eventTimes[i], eventTimes[i].Length + 1);
                    eventTimes[i][eventTimes[i].Length - 1] = _comparingRps[i].Time;
                }
            }

            if (AppleButton.Checked)
            {
                //Initialize CheckPoints()() automatically for apple comparison
                for (int i = 0; i < CRBox.Items.Count; i++)
                {
                    Array.Resize(ref appleCheckPoints[i], eventTimes[i].Length);
                    for (int j = 0; j < eventTimes[i].Length; j++)
                        appleCheckPoints[i][j] = j;
                }

                checkPoints = appleCheckPoints;
            }
            else
                checkPoints = _touchCheckPoints;

            //Find out maximum number of checkpoints
            int maxCheckPoints = 0;
            int k = 0;
            for (int i = 0; i < _comparingRps.Count; i++)
            {
                if (checkPoints[i].Length > maxCheckPoints)
                    maxCheckPoints = checkPoints[i].Length;
                else if (checkPoints[i].Length == 0)
                {
                    Utils.ShowError("Replay " + CRBox.Items[i] + " has no checkpoints!");
                    ResultsBox.Visible = false;
                    Label19.Visible = false;
                    Label4.Visible = false;
                    return;
                }
            }

            ResultsBox.Items.Clear();
            double combinedTime = 0;
            for (int i = 0; i < maxCheckPoints; i++)
            {
                double bestTimeBetweenEvents = 3600;
                for (var x = 0; x < _comparingRps.Count; x++)
                {
                    double time;
                    if (i == 0)
                        time = eventTimes[x][checkPoints[x][0]];
                    else
                    {
                        if (checkPoints[x].Length >= i + 1)
                            time = eventTimes[x][checkPoints[x][i]] - eventTimes[x][checkPoints[x][i - 1]];
                        else
                            time = 3600;
                    }

                    if (time < bestTimeBetweenEvents)
                    {
                        bestTimeBetweenEvents = time;
                        k = x; //Save the number of the best replay
                    }
                }

                combinedTime += bestTimeBetweenEvents;
                if (i == 0)
                    ResultsBox.Items.Add("Start to Checkpoint 1: " + bestTimeBetweenEvents.ToTimeString() +
                                         " in " + CRBox.Items[k]);
                else
                    ResultsBox.Items.Add("Checkpoint " + i + " to Checkpoint " + (i + 1) + ": " +
                                         bestTimeBetweenEvents.ToTimeString() + " in " + CRBox.Items[k]);
            }

            ResultsBox.Visible = true;
            Label19.Visible = true;
            Label4.Visible = true;
            Label4.Text = "Combined time: " + combinedTime.ToTimeString();
        }

        private void CrBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            int j = CRBox.SelectedIndex;
            if (GTButton.Checked && j >= 0)
            {
                CPListBox.Items.Clear();
                double[] groundTouches = _comparingRps[j].Player1.GetEventTimes(LogicalEventType.GroundTouch);
                for (int i = 0; i < groundTouches.Length; i++)
                    CPListBox.Items.Add("Touch " + (i + 1) + ": " + groundTouches[i].ToTimeString());
                if (_comparingRps[j].Finished)
                    CPListBox.Items.Add("Flower: " + _comparingRps[j].Time.ToTimeString());
                if (_touchCheckPoints[j].Length > 0)
                {
                    CPListBox.ItemCheck -= CpListBoxItemCheck;
                    for (int i = 0; i < _touchCheckPoints[j].Length; i++)
                        CPListBox.SetItemChecked(_touchCheckPoints[j][i], true);
                    CPListBox.ItemCheck += CpListBoxItemCheck;
                }
            }
        }

        private void CpListBoxItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (CPListBox.Items.Count > 0)
            {
                CPListBox.ItemCheck -= CpListBoxItemCheck;
                CPListBox.SetItemChecked(e.Index, e.NewValue == CheckState.Checked);
                CPListBox.ItemCheck += CpListBoxItemCheck;
                Array.Resize(ref _touchCheckPoints[CRBox.SelectedIndex], CPListBox.CheckedIndices.Count);
                for (int i = 0; i < CPListBox.CheckedIndices.Count; i++)
                    _touchCheckPoints[CRBox.SelectedIndex][i] = CPListBox.CheckedIndices[i];
            }
        }
    }
}