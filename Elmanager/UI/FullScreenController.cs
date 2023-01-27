using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Elmanager.UI;

internal class FullScreenController
{
    private readonly Form _form;
    private readonly EventHandler _viewerResized;
    private readonly List<Control> _controlsToHide;
    private FormWindowState _previousWindowState;
    public bool IsFullScreen { get; private set; }

    public FullScreenController(Form form, EventHandler viewerResized, List<Control> controlsToHide)
    {
        _form = form;
        _viewerResized = viewerResized;
        _controlsToHide = controlsToHide;
        _previousWindowState = form.WindowState;
    }

    public void Toggle()
    {
        if (IsFullScreen)
        {
            Restore();
        }
        else
        {
            FullScreen();
        }
    }

    public void FullScreen()
    {
        if (IsFullScreen)
        {
            return;
        }

        _form.Resize -= _viewerResized;
        _previousWindowState = _form.WindowState;
        _form.WindowState = FormWindowState.Normal;
        _controlsToHide.ForEach(c => c.Visible = false);
        _form.FormBorderStyle = FormBorderStyle.None;
        _form.TopMost = true;
        _form.WindowState = FormWindowState.Maximized;
        _viewerResized(null, EventArgs.Empty);
        _form.Resize += _viewerResized;
        IsFullScreen = true;
    }

    public void Restore()
    {
        if (!IsFullScreen)
        {
            return;
        }

        IsFullScreen = false;
        _form.Resize -= _viewerResized;
        _form.FormBorderStyle = FormBorderStyle.Sizable;
        _form.TopMost = false;
        _form.WindowState = _previousWindowState;
        _controlsToHide.ForEach(c => c.Visible = true);
        _form.Resize += _viewerResized;
        _viewerResized(null, EventArgs.Empty);
    }
}