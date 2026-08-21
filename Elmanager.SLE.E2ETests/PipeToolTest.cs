using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elmanager.SLE.E2ETests;

[TestClass]
public class PipeToolTest : SleTest
{
    [TestMethod]
    public async Task CreatesPipeWithApples()
    {
        await Page.OpenAndWaitForFirstRender();

        await ClickPipeTool();
        await ClickLevelViewport(0.34, 0.36);
        await Page.Keyboard.PressAsync("Space");
        await ClickLevelViewport(0.44, 0.27);
        await ClickLevelViewport(0.55, 0.35);
        await ClickLevelViewport(0.52, 0.52);
        await ClickLevelViewport(0.40, 0.56);
        await RightClickLevelViewport(0.36, 0.48);

        await ExpectLevelViewportScreenshotAsync("pipe-tool-with-apples");
    }
}
