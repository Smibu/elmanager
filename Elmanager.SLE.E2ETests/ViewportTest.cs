using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elmanager.SLE.E2ETests;

[TestClass]
public class ViewportTest : SleTest
{
    [TestMethod]
    public async Task PansWithMiddleButtonDrag()
    {
        await Page.OpenAndWaitForFirstRender();
        await ClickVertexTool();
        await MoveLevelViewportPointer(0.34, 0.22);
        await ClickLevelViewport(0.38, 0.29);
        await ClickLevelViewport(0.53, 0.29);
        await ClickLevelViewport(0.45, 0.56);
        await RightClickLevelViewport(0.47, 0.59);

        await MoveLevelViewportPointer(0.60, 0.60);
        await Page.Mouse.DownAsync(new MouseDownOptions { Button = MouseButton.Middle });
        await MoveLevelViewportPointer(0.55, 0.55);
        await MoveLevelViewportPointer(0.48, 0.50);
        await Page.Mouse.UpAsync(new MouseUpOptions { Button = MouseButton.Middle });
        await MovePointerAway();

        await ExpectLevelViewportScreenshotAsync("viewport-middle-drag-pan");
    }
}
