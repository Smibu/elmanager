using System;
using Avalonia.Controls;
using Avalonia.Input;
using Elmanager.LevelEditor.Input;

namespace Elmanager.SLE.Platform;

internal class AvaloniaCursorManager(Control control, Func<bool> isCursorBlocked) : IEditorCursorManager
{
    public void ChangeCursorToHand() =>
        control.Cursor = isCursorBlocked()
            ? Cursor.Default
            : new Cursor(StandardCursorType.Hand);

    public void ChangeToDefaultCursorIfHand()
    {
        if (control.Cursor?.ToString() == "Hand")
        {
            control.Cursor = Cursor.Default;
        }
    }
}
