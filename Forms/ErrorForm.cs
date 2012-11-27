using System;
using System.Collections.Generic;
using System.IO;

namespace Elmanager.Forms
{
    partial class ErrorForm
    {
        internal ErrorForm(IEnumerable<string> recFiles)
        {
            InitializeComponent();
            foreach (string file in recFiles)
                ErrorBox.Items.Add(file);
        }

        private void DeleteReplays(object sender, EventArgs e)
        {
            foreach (string file in ErrorBox.Items)
                File.Delete(file);
            Close();
        }
    }
}