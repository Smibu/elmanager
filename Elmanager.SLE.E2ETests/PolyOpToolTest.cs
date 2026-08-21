using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elmanager.SLE.E2ETests;

[TestClass]
public class PolyOpToolTest : SleTest
{
    [TestMethod]
    public async Task UnitesOverlappingPolygons()
    {
        await Page.OpenAndWaitForFirstRender();

        await ClickVertexTool();
        await ClickLevelPolygon(
            (0.34, 0.32),
            (0.49, 0.32),
            (0.49, 0.56),
            (0.34, 0.56));
        await ClickLevelPolygon(
            (0.44, 0.22),
            (0.59, 0.22),
            (0.59, 0.47),
            (0.44, 0.47));

        await ClickPolyOpTool();
        await ClickLevelViewport(0.34, 0.32);
        await ClickLevelViewport(0.59, 0.22);

        await MovePointerAway();
        await ExpectLevelViewportScreenshotAsync("poly-op-tool-union");
    }
}
