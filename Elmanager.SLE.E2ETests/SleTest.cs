using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace Elmanager.SLE.E2ETests;

public class SleTest : PageTest
{
    private const int BrowserViewportWidth = 1440;
    private const int BrowserViewportHeight = 900;
    private const float ToolPanelWidth = 120;
    private const float EditorChromeHeight = 133;
    private const float StatusBarHeight = 24;
    private const float ToolButtonX = 60;
    private const float FirstToolButtonY = 148;
    private const float ToolButtonHeight = 30;

    public override BrowserNewContextOptions ContextOptions() => new()
    {
        BaseURL = BrowserAppFixture.BaseUrl,
        ViewportSize = new ViewportSize { Width = BrowserViewportWidth, Height = BrowserViewportHeight },
    };

    protected Task ClickSelectTool() => ClickToolButton(0);
    protected Task ClickVertexTool() => ClickToolButton(1);
    protected Task ClickDrawTool() => ClickToolButton(2);
    protected Task ClickObjectTool() => ClickToolButton(3);
    protected Task ClickPipeTool() => ClickToolButton(4);
    protected Task ClickEllipseTool() => ClickToolButton(5);
    protected Task ClickPolyOpTool() => ClickToolButton(6);
    protected Task ClickFrameTool() => ClickToolButton(7);
    protected Task ClickSmoothenTool() => ClickToolButton(8);
    protected Task ClickCutConnectTool() => ClickToolButton(9);
    protected Task ClickAutoGrassTool() => ClickToolButton(10);
    protected Task ClickPictureTool() => ClickToolButton(11);
    protected Task ClickTextTool() => ClickToolButton(12);
    protected Task ClickCustomShapeTool() => ClickToolButton(13);

    protected async Task ClickBrowserViewport(
        double x,
        double y,
        float offsetX = 0,
        float offsetY = 0)
    {
        var point = RelativePoint(await GetBrowserViewportBoundsAsync(), x, y, offsetX, offsetY);
        await Page.Mouse.ClickAsync(point.X, point.Y);
    }

    protected async Task MoveBrowserViewportPointer(
        double x,
        double y,
        float offsetX = 0,
        float offsetY = 0)
    {
        var point = RelativePoint(await GetBrowserViewportBoundsAsync(), x, y, offsetX, offsetY);
        await Page.Mouse.MoveAsync(point.X, point.Y);
        await Page.WaitForTimeoutAsync(50);
    }

    protected async Task MoveLevelViewportPointer(
        double x,
        double y,
        float offsetX = 0,
        float offsetY = 0)
    {
        var point = RelativePoint(await GetLevelViewportBoundsAsync(), x, y, offsetX, offsetY);
        await Page.Mouse.MoveAsync(point.X, point.Y);
        await Page.WaitForTimeoutAsync(50);
    }

    protected Task MovePointerAway() =>
        MoveBrowserViewportPointer(0, 1, ToolButtonX, -100);

    protected Task ClickLevelViewport(double x, double y) =>
        ClickLevelViewport(x, y, MouseButton.Left);

    protected Task RightClickLevelViewport(double x, double y) =>
        ClickLevelViewport(x, y, MouseButton.Right);

    protected async Task ExpectLevelViewportScreenshotAsync(string name)
    {
        await Page.ExpectScreenshotAsync(name, new ToMatchScreenshotOptions
        {
            Clip = await GetLevelViewportBoundsAsync(),
        });
    }

    protected async Task ClickLevelPolygon(params (double X, double Y)[] coords)
    {
        if (coords.Length < 3)
        {
            throw new ArgumentException("A polygon requires at least three vertices.", nameof(coords));
        }

        await MoveLevelViewportPointer(coords[0].X, coords[0].Y, -40, -40);
        foreach (var coord in coords)
        {
            await ClickLevelViewport(coord.X, coord.Y);
        }

        await ClickLevelViewport(coords[^1].X, coords[^1].Y, MouseButton.Right, 20, 20);
    }

    private Task ClickToolButton(int row) =>
        ClickBrowserViewport(0, 0, ToolButtonX, FirstToolButtonY + row * ToolButtonHeight);

    protected async Task<Clip> GetBrowserViewportClipAsync(
        double x,
        double y,
        float offsetX,
        float offsetY,
        float width,
        float height)
    {
        var point = RelativePoint(await GetBrowserViewportBoundsAsync(), x, y, offsetX, offsetY);
        return new Clip { X = point.X, Y = point.Y, Width = width, Height = height };
    }

    private async Task ClickLevelViewport(
        double x,
        double y,
        MouseButton button,
        float offsetX = 0,
        float offsetY = 0)
    {
        await MoveLevelViewportPointer(x, y, offsetX, offsetY);
        await Page.Mouse.DownAsync(new MouseDownOptions { Button = button });
        await Page.Mouse.UpAsync(new MouseUpOptions { Button = button });
    }

    private async Task<Clip> GetBrowserViewportBoundsAsync()
    {
        var values = await Page.EvaluateAsync<double[]>("() => [0, 0, window.innerWidth, window.innerHeight]");
        return ToClip(values, "browser viewport");
    }

    private async Task<Clip> GetLevelViewportBoundsAsync()
    {
        var browser = await GetBrowserViewportBoundsAsync();
        return ToClip(
            [
                ToolPanelWidth,
                EditorChromeHeight,
                browser.Width - ToolPanelWidth,
                browser.Height - EditorChromeHeight - StatusBarHeight,
            ],
            "level viewport");
    }

    private static Clip ToClip(double[] values, string name)
    {
        if (values.Length != 4 || values[2] <= 0 || values[3] <= 0)
        {
            throw new InvalidOperationException($"Invalid {name} bounds.");
        }

        return new Clip
        {
            X = (float)values[0],
            Y = (float)values[1],
            Width = (float)values[2],
            Height = (float)values[3],
        };
    }

    private static ScreenPoint RelativePoint(
        Clip bounds,
        double x,
        double y,
        float offsetX,
        float offsetY)
    {
        ValidateRelativeCoordinate(x, nameof(x));
        ValidateRelativeCoordinate(y, nameof(y));
        return new ScreenPoint(
            (float)(bounds.X + bounds.Width * x + offsetX),
            (float)(bounds.Y + bounds.Height * y + offsetY));
    }

    private static void ValidateRelativeCoordinate(double value, string parameterName)
    {
        if (double.IsNaN(value) || double.IsInfinity(value) || value is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                value,
                "Viewport coordinates must be between 0.0 and 1.0.");
        }
    }

    private readonly record struct ScreenPoint(float X, float Y);
}
