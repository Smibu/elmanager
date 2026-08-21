using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elmanager.SLE.E2ETests;

[TestClass]
public class CutConnectToolTest : SleTest
{
    [TestMethod]
    public async Task ConnectsPolygons()
    {
        await Page.OpenAndWaitForFirstRender();

        await ClickVertexTool();
        await ClickLevelPolygon(
            (0.33, 0.32),
            (0.43, 0.32),
            (0.43, 0.56),
            (0.33, 0.56));
        await ClickLevelPolygon(
            (0.53, 0.32),
            (0.64, 0.32),
            (0.64, 0.56),
            (0.53, 0.56));

        await ClickCutConnectTool();
        await ClickLevelViewport(0.38, 0.44);
        await ClickLevelViewport(0.58, 0.44);

        await MovePointerAway();
        await ExpectLevelViewportScreenshotAsync("cut-connect-tool-connected-polygons");
    }

    [TestMethod]
    public async Task CutsPolygon()
    {
        await Page.OpenAndWaitForFirstRender();

        await ClickVertexTool();
        await ClickLevelPolygon(
            (0.34, 0.29),
            (0.61, 0.29),
            (0.61, 0.59),
            (0.34, 0.59));

        await ClickCutConnectTool();
        await ClickLevelViewport(0.47, 0.22);
        await ClickLevelViewport(0.47, 0.66);

        await MovePointerAway();
        await ExpectLevelViewportScreenshotAsync("cut-connect-tool-cut-polygon");
    }
}
