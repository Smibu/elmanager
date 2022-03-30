using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Elmanager.IO;
using Elmanager.Utilities;

namespace Elmanager.UI;

internal static class UiUtils
{
    /// <summary>
    ///   Display a message box to indicate that an error occurred.
    /// </summary>
    /// <param name = "text">The text to display in the message box.</param>
    /// <param name = "caption">The title of the message box.</param>
    /// <param name = "icon">The icon to display in the message box.</param>
    /// <returns></returns>
    internal static void ShowError(string text, string caption = "Error", MessageBoxIcon icon = MessageBoxIcon.Hand)
    {
        MessageBox.Show(text, caption, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, 0, false);
    }

    public static void ConfigureColumns<T>(ObjectListView oList, bool addIndexColumn = false,
        IEnumerable<string>? hiddenColumns = null)
    {
        var members = typeof(T).GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        var cols = new List<OLVColumn>();
        if (addIndexColumn)
        {
            cols.Add(new OLVColumn("#", null));
            oList.FormatRow += (sender, args) => { args.Item.Text = (args.RowIndex + 1).ToString(); };
        }

        var hiddens = new HashSet<string>();
        if (hiddenColumns != null)
        {
            foreach (var c in hiddenColumns)
            {
                hiddens.Add(c);
            }
        }

        foreach (var m in members)
        {
            var descs = (DescriptionAttribute[]) m.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (descs.Length == 0)
            {
                continue;
            }

            var desc = descs[0];
            var col = new OLVColumn(desc.Description, m.Name);
            var t = m switch
            {
                FieldInfo fieldInfo => fieldInfo.FieldType,
                MethodInfo methodInfo => methodInfo.ReturnType,
                PropertyInfo propertyInfo => propertyInfo.PropertyType,
                _ => throw new ArgumentOutOfRangeException(nameof(m))
            };

            if (t == typeof(bool))
            {
                col.AspectToStringConverter = BoolUtils.BoolToString;
            }
            else if (t == typeof(double))
            {
                col.AspectToStringConverter = d => $"{d:F2}";
            }

            if (hiddens.Contains(col.Text))
            {
                col.IsVisible = false;
            }

            cols.Add(col);
        }

        var i = cols.FindIndex(c => c.AspectName == nameof(ElmaFile.FileNameNoExt));
        if (i >= 0)
        {
            var colf = cols[i];
            cols.RemoveAt(i);
            cols.Insert(0, colf);
        }

        oList.AllColumns = cols;
        oList.RebuildColumns();
    }

    public static void ConfigureTooltip(ToolTipControl toolTip)
    {
        toolTip.IsBalloon = true;
        toolTip.AutoPopDelay = 30000;
        toolTip.InitialDelay = 1000;
        toolTip.ReshowDelay = 0;
        toolTip.Title = "Details";
    }

    public static bool Confirm(string text)
    {
        return MessageBox.Show(text, "Elmanager", MessageBoxButtons.YesNo,
            MessageBoxIcon.Question) == DialogResult.Yes;
    }
}