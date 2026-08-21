using System;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace Elmanager.SLE.E2ETests;

public static class SlePageExtensions
{
    private static readonly float? ConsoleMessageTimeout =
        string.Equals(Environment.GetEnvironmentVariable("CI"), "true", StringComparison.OrdinalIgnoreCase)
            ? 60_000
            : null;

    public static async Task OpenAndWaitForFirstRender(this IPage page)
    {
        await page.GotoAsync("/");
        await page.WaitForConsoleMessageAsync(new PageWaitForConsoleMessageOptions
        {
            Predicate = msg => msg.Text == "First render done",
            Timeout = ConsoleMessageTimeout,
        });
        await page.WaitForTimeoutAsync(50);
    }

    public static async Task WaitForLgr(this IPage page)
    {
        await page.WaitForConsoleMessageAsync(new PageWaitForConsoleMessageOptions
        {
            Predicate = message => message.Text == "LGR load ready",
            Timeout = ConsoleMessageTimeout,
        });
        await page.WaitForTimeoutAsync(50);
    }

    public static async Task WaitForPictureDialog(this IPage page)
    {
        await page.WaitForConsoleMessageAsync(new PageWaitForConsoleMessageOptions
        {
            Predicate = message => message.Text == "Picture dialog ready",
            Timeout = ConsoleMessageTimeout,
        });
    }
}
