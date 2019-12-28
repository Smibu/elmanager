using System;
using System.Collections.Generic;
using System.IO;
using Elmanager.CustomControls;

namespace Elmanager.Forms
{
    partial class ErrorForm : FormMod
    {
        internal ErrorForm(IEnumerable<string> files)
        {
            InitializeComponent();
            foreach (var file in files)
                ErrorBox.Items.Add(file);
        }

        private void DeleteFiles(object sender, EventArgs e)
        {
            if (!Utils.Confirm($"Delete the {ErrorBox.Items.Count} files?"))
            {
                return;
            }
            foreach (string file in ErrorBox.Items)
                File.Delete(file);
            Close();
        }
    }
}