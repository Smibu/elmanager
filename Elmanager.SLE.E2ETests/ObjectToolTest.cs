using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elmanager.SLE.E2ETests;

[TestClass]
public class ObjectToolTest : SleTest
{
    [TestMethod]
    public async Task SwitchesObjectType()
    {
        await Page.OpenAndWaitForFirstRender();

        await ClickObjectTool();
        await ClickLevelViewport(0.38, 0.36);

        await Page.Keyboard.PressAsync("Space");
        await ClickLevelViewport(0.49, 0.36);

        await Page.Keyboard.PressAsync("Space");
        await ClickLevelViewport(0.61, 0.36);

        await Page.Keyboard.PressAsync("Space");
        await ClickLevelViewport(0.72, 0.36);

        await MovePointerAway();
        await ExpectLevelViewportScreenshotAsync("object-tool-types");
    }
}
