using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elmanager.SLE.E2ETests;

[TestClass]
public class EllipseToolTest : SleTest
{
    [TestMethod]
    public async Task CreatesEllipseWithAdjustedSides()
    {
        await Page.OpenAndWaitForFirstRender();

        await ClickEllipseTool();
        await ClickLevelViewport(0.45, 0.40);
        await MoveLevelViewportPointer(0.61, 0.59);
        await Page.Keyboard.PressAsync("+");
        await Page.Keyboard.PressAsync("+");
        await Page.Keyboard.PressAsync("+");
        await Page.Keyboard.PressAsync("-");
        await ClickLevelViewport(0.61, 0.59);

        await ExpectLevelViewportScreenshotAsync("ellipse-tool-adjusted-sides");
    }
}
