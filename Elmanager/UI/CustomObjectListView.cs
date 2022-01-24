using System;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace Elmanager.UI;

internal class CustomObjectListView : ObjectListView
{
    protected override void WndProc(ref Message m)
    {
        // Workaround to avoid exception when clicking the first column of an ObjectListView. Seems to happen on .NET Core/.NET 5.
        try
        {
            base.WndProc(ref m);
        }
        catch (ArgumentNullException e) when (e.ParamName == "owningItem")
        {
        }
    }
}