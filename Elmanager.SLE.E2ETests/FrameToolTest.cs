using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elmanager.SLE.E2ETests;

[TestClass]
public class FrameToolTest : SleTest
{
    [TestMethod]
    public async Task CreatesAdjustedInwardFrame()
    {
        await Page.OpenAndWaitForFirstRender();

        await ClickVertexTool();
        await ClickLevelPolygon(
            (0.36, 0.29),
            (0.53, 0.25),
            (0.60, 0.45),
            (0.49, 0.62),
            (0.34, 0.52));

        await ClickFrameTool();
        await ClickLevelViewport(0.36, 0.29);
        await Page.Keyboard.PressAsync("+");
        await Page.Keyboard.PressAsync("Space");
        await ClickLevelViewport(0.68, 0.70);

        await MovePointerAway();
        await ExpectLevelViewportScreenshotAsync("frame-tool-inward");
    }
}
