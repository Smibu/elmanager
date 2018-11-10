using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft;

namespace Elmanager.Forms
{
    public partial class ProgressDialog : Form
    {
        private readonly CancellationTokenSource _cancelSrc;
        private readonly Task _task;

        public ProgressDialog()
        {
            InitializeComponent();
        }

        public ProgressDialog(Task task, CancellationTokenSource cancelSrc, Progress<double> progress) : this()
        {
            _cancelSrc = cancelSrc;
            _task = task;
            progress.ProgressChanged += (sender, d) => { progressBar1.Value = (int) (d * 1000); };
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            _cancelSrc.Cancel();
            cancelButton.Enabled = false;
            cancelButton.Text = "Canceling...";
        }

        private void ProgressDialog_Shown(object sender, EventArgs e)
        {
            _task.ContinueWith(t => { Close(); }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}