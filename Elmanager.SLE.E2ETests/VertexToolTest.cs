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
}
