﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Elmanager.UI;

internal partial class ProgressDialog : FormMod
{
    private readonly CancellationTokenSource _cancelSrc;
    private readonly Task _task;

    public ProgressDialog(Task task, CancellationTokenSource cancelSrc, Progress<double> progress)
    {
        InitializeComponent();
        _cancelSrc = cancelSrc;
        _task = task;
        progress.ProgressChanged += (_, d) => { progressBar1.Value = (int)(d * 1000); };
    }

    private void cancelButton_Click(object sender, EventArgs e)
    {
        _cancelSrc.Cancel();
        cancelButton.Enabled = false;
        cancelButton.Text = "Canceling...";
    }

    private void ProgressDialog_Shown(object sender, EventArgs e)
    {
        _task.ContinueWith(_ => { Close(); }, TaskScheduler.FromCurrentSynchronizationContext());
    }
}