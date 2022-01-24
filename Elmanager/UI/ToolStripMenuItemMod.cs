using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Elmanager.Utilities;

namespace Elmanager.UI;

internal class ToolStripMenuItemMod : ToolStripMenuItem
{
    private string _shortcutText;

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (Owner != null)
        {
            var renderer = Owner.Renderer;
            var rightToLeft = RightToLeft == RightToLeft.Yes;
            var textColor = SystemColors.MenuText;
            var textRect = (Rectangle) this.GetPropertyValue("InternalLayout").GetPropertyValue("TextRectangle");
            var g = e.Graphics;
            if ((DisplayStyle & ToolStripItemDisplayStyle.Text) == ToolStripItemDisplayStyle.Text)
            {
                bool showShortCut = ShowShortcutKeys;
                if (!DesignMode)
                {
                    showShortCut = showShortCut && !HasDropDownItems;
                }

                if (showShortCut)
                {
                    renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(g, this, ShortcutText,
                        textRect, textColor, Font,
                        rightToLeft ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleRight));
                }
            }
        }
    }

    [Description("Gets or sets the custom shortcut text.")]
    public string ShortcutText
    {
        get => _shortcutText;
        set
        {
            _shortcutText = value;
            Invalidate();
        }
    }
}