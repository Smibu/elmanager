using Elmanager.LevelEditor.Input;

namespace Elmanager.Rendering.Camera;

public static class CameraUtils
{
    public static bool StepArrowScroll(long timeDeltaMs, IKeyboardState keyboard, ZoomController zoomCtrl)
    {
        bool anyDown = false;

        if (keyboard.IsKeyDown(EditorKey.Up))
        {
            zoomCtrl.CenterY += timeDeltaMs / 200.0 * zoomCtrl.ZoomLevel;
            anyDown = true;
        }

        if (keyboard.IsKeyDown(EditorKey.Down))
        {
            zoomCtrl.CenterY -= timeDeltaMs / 200.0 * zoomCtrl.ZoomLevel;
            anyDown = true;
        }

        if (keyboard.IsKeyDown(EditorKey.Right))
        {
            zoomCtrl.CenterX += timeDeltaMs / 200.0 * zoomCtrl.ZoomLevel;
            anyDown = true;
        }

        if (keyboard.IsKeyDown(EditorKey.Left))
        {
            zoomCtrl.CenterX -= timeDeltaMs / 200.0 * zoomCtrl.ZoomLevel;
            anyDown = true;
        }

        return anyDown;
    }
}
