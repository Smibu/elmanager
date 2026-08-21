using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elmanager.SLE.E2ETests;

[TestClass]
public class AutoGrassToolTest : SleTest
{
    [TestMethod]
    public async Task AddsVisibleGrassEdges()
    {
        await Page.OpenAndWaitForFirstRender();

        await ClickVertexTool();
        await ClickLevelPolygon(
            (0.33, 0.56),
            (0.36, 0.36),
            (0.44, 0.28),
            (0.55, 0.31),
            (0.61, 0.41),
            (0.60, 0.59));

        await ClickAutoGrassTool();
        await ClickLevelViewport(0.33, 0.56);
        await Page.Keyboard.PressAsync("+");
        await ClickLevelViewport(0.68, 0.70);

        await MovePointerAway();
        await ExpectLevelViewportScreenshotAsync("auto-grass-tool-visible-edges");
    }
}
