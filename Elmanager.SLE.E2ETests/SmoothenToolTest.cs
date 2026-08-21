using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elmanager.SLE.E2ETests;

[TestClass]
public class SmoothenToolTest : SleTest
{
    [TestMethod]
    public async Task SmoothsPolygon()
    {
        await Page.OpenAndWaitForFirstRender();

        await ClickVertexTool();
        await ClickLevelPolygon(
            (0.34, 0.36),
            (0.42, 0.24),
            (0.55, 0.27),
            (0.61, 0.47),
            (0.52, 0.62),
            (0.38, 0.56));

        await ClickSmoothenTool();
        await ClickLevelViewport(0.34, 0.36);
        await Page.Keyboard.PressAsync("+");
        await ClickLevelViewport(0.68, 0.70);

        await MovePointerAway();
        await ExpectLevelViewportScreenshotAsync("smoothen-tool-polygon");
    }
}
