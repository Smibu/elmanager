using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elmanager.SLE.E2ETests;

[TestClass]
public class VertexToolTest : SleTest
{
    [TestMethod]
    public async Task CreatesTriangle()
    {
        await Page.OpenAndWaitForFirstRender();

        await ClickVertexTool();
        await MoveLevelViewportPointer(0.34, 0.22);
        await ClickLevelViewport(0.38, 0.29);
        await ClickLevelViewport(0.53, 0.29);
        await ClickLevelViewport(0.45, 0.56);
        await RightClickLevelViewport(0.47, 0.59);

        await ExpectLevelViewportScreenshotAsync("vertex-tool-triangle");
    }

    [TestMethod]
    public async Task CreatesRectangle()
    {
        await Page.OpenAndWaitForFirstRender();

        await ClickVertexTool();
        await MoveLevelViewportPointer(0.34, 0.22);
        await Page.Keyboard.DownAsync("Shift");
        await ClickLevelViewport(0.40, 0.30);
        await Page.Keyboard.UpAsync("Shift");
        await MoveLevelViewportPointer(0.50, 0.42);
        await ClickLevelViewport(0.55, 0.50);
        await MovePointerAway();

        await ExpectLevelViewportScreenshotAsync("vertex-tool-rectangle");
    }

    [TestMethod]
    public async Task AddsVerticesToExistingEdge()
    {
        await Page.OpenAndWaitForFirstRender();

        await ClickVertexTool();
        await MoveLevelViewportPointer(0.34, 0.22);
        await ClickLevelViewport(0.38, 0.29);
        await ClickLevelViewport(0.53, 0.29);
        await ClickLevelViewport(0.45, 0.56);
        await RightClickLevelViewport(0.47, 0.59);
        await MoveLevelViewportPointer(0.455, 0.29);
        await ClickLevelViewport(0.455, 0.29);
        await ClickLevelViewport(0.455, 0.20);
        await RightClickLevelViewport(0.47, 0.18);
        await MovePointerAway();

        await ExpectLevelViewportScreenshotAsync("vertex-tool-extended-edge");
    }
}
