using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elmanager.SLE.E2ETests;

[TestClass]
public class TransformToolTest : SleTest
{
    [TestMethod]
    public async Task ScalesAndRotatesFromCorner()
    {
        await StartTransformingTriangle();

        await DragLevelViewport((0.53, 0.29), (0.56, 0.26), (0.58, 0.22));
        await MovePointerAway();

        await ExpectLevelViewportScreenshotAsync("transform-tool-corner");
    }

    [TestMethod]
    public async Task StretchesFromEdgeMidpoint()
    {
        await StartTransformingTriangle();

        await DragLevelViewport((0.53, 0.425), (0.57, 0.43), (0.60, 0.44));
        await MovePointerAway();

        await ExpectLevelViewportScreenshotAsync("transform-tool-edge-midpoint");
    }

    [TestMethod]
    public async Task MovesFromCenter()
    {
        await StartTransformingTriangle();

        await DragLevelViewport((0.455, 0.425), (0.48, 0.45), (0.50, 0.47));
        await MovePointerAway();

        await ExpectLevelViewportScreenshotAsync("transform-tool-center");
    }

    private async Task StartTransformingTriangle()
    {
        await Page.OpenAndWaitForFirstRender();
        await ClickVertexTool();
        await MoveLevelViewportPointer(0.34, 0.22);
        await ClickLevelViewport(0.38, 0.29);
        await ClickLevelViewport(0.53, 0.29);
        await ClickLevelViewport(0.45, 0.56);
        await RightClickLevelViewport(0.47, 0.59);
        await ClickSelectTool();
        await DragLevelViewport((0.34, 0.24), (0.57, 0.61));
        await Page.Keyboard.PressAsync("Space");
    }
}
