using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elmanager.SLE.E2ETests;

[TestClass]
public class BasicTest : SleTest
{
    [TestMethod]
    public async Task InitialLoad()
    {
        await Page.OpenAndWaitForFirstRender();
        await Page.ExpectScreenshotAsync("app-initial", new ToMatchScreenshotOptions
        {
            MaxDiffPixels = 2 // macOS runner has small difference here
        });
    }
}
