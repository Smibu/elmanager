using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elmanager.SLE.E2ETests;

[TestClass]
public class DrawToolTest : SleTest
{
    [TestMethod]
    public async Task CreatesFreehandPolygon()
    {
        await Page.OpenAndWaitForFirstRender();

        await ClickDrawTool();
        await MoveLevelViewportPointer(0.38, 0.36);
        await Page.Mouse.DownAsync();
        await MoveLevelViewportPointer(0.42, 0.27);
        await MoveLevelViewportPointer(0.48, 0.32);
        await MoveLevelViewportPointer(0.53, 0.25);
        await MoveLevelViewportPointer(0.58, 0.37);
        await MoveLevelViewportPointer(0.52, 0.47);
        await MoveLevelViewportPointer(0.55, 0.59);
        await MoveLevelViewportPointer(0.47, 0.52);
        await MoveLevelViewportPointer(0.41, 0.60);
        await MoveLevelViewportPointer(0.42, 0.47);
        await MoveLevelViewportPointer(0.36, 0.43);
        await MoveLevelViewportPointer(0.38, 0.36);
        await Page.Mouse.UpAsync();

        await ExpectLevelViewportScreenshotAsync("draw-tool-freehand-polygon");
    }
}
