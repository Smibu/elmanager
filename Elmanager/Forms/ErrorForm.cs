using System;
using System.Collections.Generic;
using System.IO;
using Elmanager.CustomControls;

namespace Elmanager.Forms
{
    partial class ErrorForm : FormMod
    {
        internal ErrorForm(IEnumerable<string> recFiles)
        {
            InitializeComponent();
            foreach (var file in recFiles)
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