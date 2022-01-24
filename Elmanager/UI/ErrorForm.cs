using System;
using System.Collections.Generic;
using System.IO;

namespace Elmanager.UI;

internal partial class ErrorForm : FormMod
{
    internal ErrorForm(IEnumerable<string> files)
    {
        InitializeComponent();
        foreach (var file in files)
            ErrorBox.Items.Add(file);
    }

    private void DeleteFiles(object sender, EventArgs e)
    {
        if (!UiUtils.Confirm($"Delete the {ErrorBox.Items.Count} files?"))
        {
            return;
        }
        foreach (string file in ErrorBox.Items)
            File.Delete(file);
        Close();
    }
}