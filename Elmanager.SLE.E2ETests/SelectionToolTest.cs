using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elmanager.SLE.E2ETests;

[TestClass]
public class SelectionToolTest : SleTest
{
    [TestMethod]
    public async Task MovesRectangleSelectedVertex()
    {
        await CreateTriangleAndSelectTool();

        await DragLevelViewport((0.50, 0.24), (0.56, 0.34));
        await DragLevelViewport((0.53, 0.29), (0.57, 0.27), (0.60, 0.22));
        await MovePointerAway();

        await ExpectLevelViewportScreenshotAsync("selection-tool-rectangle-move");
    }

    [TestMethod]
    public async Task MovesLassoSelectedVertex()
    {
        await CreateTriangleAndSelectTool();

        await Page.Keyboard.DownAsync("Shift");
        await DragLevelViewport((0.42, 0.52), (0.48, 0.52), (0.48, 0.60), (0.42, 0.60), (0.42, 0.52));
        await Page.Keyboard.UpAsync("Shift");
        await DragLevelViewport((0.45, 0.56), (0.45, 0.61), (0.45, 0.66));
        await MovePointerAway();

        await ExpectLevelViewportScreenshotAsync("selection-tool-lasso-move");
    }

    [TestMethod]
    public async Task MovesVertexAlongLockedEdges()
    {
        await CreateTriangleAndSelectTool();

        await Page.Keyboard.DownAsync("Shift");
        await DragLevelViewport((0.53, 0.29), (0.57, 0.31), (0.62, 0.33));
        await Page.Keyboard.UpAsync("Shift");
        await MovePointerAway();

        await ExpectLevelViewportScreenshotAsync("selection-tool-locked-edges-move");
    }

    private async Task CreateTriangleAndSelectTool()
    {
        await Page.OpenAndWaitForFirstRender();
        await ClickVertexTool();
        await MoveLevelViewportPointer(0.34, 0.22);
        await ClickLevelViewport(0.38, 0.29);
        await ClickLevelViewport(0.53, 0.29);
        await ClickLevelViewport(0.45, 0.56);
        await RightClickLevelViewport(0.47, 0.59);
        await ClickSelectTool();
    }
}
