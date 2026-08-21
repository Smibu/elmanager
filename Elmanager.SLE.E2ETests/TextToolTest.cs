using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elmanager.SLE.E2ETests;

[TestClass]
public class TextToolTest : SleTest
{
    [TestMethod]
    public async Task CreatesTextPolygons()
    {
        await Page.OpenAndWaitForFirstRender();

        await ClickTextTool();
        await ClickLevelViewport(0.45, 0.40);
        await Page.WaitForTimeoutAsync(200);
        await Page.Keyboard.PressAsync("Control+A");
        await Page.Keyboard.TypeAsync("First line");
        await Page.Keyboard.PressAsync("Enter");
        await Page.Keyboard.TypeAsync("Second line");

        await Page.Keyboard.PressAsync("Tab");
        await Page.Keyboard.PressAsync("Control+A");
        await Page.Keyboard.TypeAsync("Roboto");
        await Page.Keyboard.PressAsync("Enter");
        await Page.WaitForTimeoutAsync(3000);

        await Page.Keyboard.PressAsync("Tab");
        await Page.Keyboard.PressAsync("Control+A");
        await Page.Keyboard.TypeAsync("24");
        await Page.Keyboard.PressAsync("Enter");

        await Page.Keyboard.PressAsync("Tab");
        await Page.Keyboard.PressAsync("Tab");
        await Page.Keyboard.PressAsync("Tab");
        await Page.Keyboard.PressAsync("ArrowDown");
        await Page.WaitForTimeoutAsync(2000);

        await Page.Keyboard.PressAsync("Tab");
        for (var i = 0; i < 5; i++)
        {
            await Page.Keyboard.PressAsync("ArrowRight");
        }

        await Page.Keyboard.PressAsync("Tab");
        for (var i = 0; i < 10; i++)
        {
            await Page.Keyboard.PressAsync("PageUp");
        }

        await Page.Keyboard.PressAsync("Control+Enter");

        await ClickSelectTool();
        await ClickLevelViewport(0.68, 0.76);
        await MovePointerAway();
        await ExpectLevelViewportScreenshotAsync("text-tool-polygons");
    }
}
