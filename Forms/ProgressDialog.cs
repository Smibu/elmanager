using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft;

namespace Elmanager.Forms
{
    public partial class ProgressDialog : Form
    {
        private readonly CancellationTokenSource cancelSrc;
        private readonly Task task;

        public ProgressDialog()
        {
            InitializeComponent();
        }

        public ProgressDialog(Task task, CancellationTokenSource cancelSrc, Progress<double> progress) : this()
        {
            this.cancelSrc = cancelSrc;
            this.task = task;
            progress.ProgressChanged += (sender, d) => { progressBar1.Value = (int) (d*1000); };
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            cancelSrc.Cancel();
            cancelButton.Enabled = false;
            cancelButton.Text = "Canceling...";
        }

        private void ProgressDialog_Shown(object sender, EventArgs e)
        {
            task.ContinueWith(t => { Close(); }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}