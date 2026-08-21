using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elmanager.SLE.E2ETests;

[TestClass]
public class PictureToolTest : SleTest
{
    private const string LgrDownloadUrl = "https://api.elma.online/api/lgr/get/fancy?dl";
    private const string BrowserLgrFolderName = "sle-e2e-lgr";
    private static readonly HttpClient HttpClient = new() { Timeout = TimeSpan.FromMinutes(1) };
    private static readonly SemaphoreSlim LgrDownloadLock = new(1, 1);

    [TestMethod]
    public async Task CreatesPictures()
    {
        await OpenWithLgr();

        await ClickBrowserViewport(0, 0, 485, 91);
        await ClickPictureTool();
        await ClickLevelViewport(0.45, 0.36);
        await Page.WaitForPictureDialog();
        await ClickBrowserViewport(0.5, 0.5, 75, 130);

        await ClickLevelViewport(0.38, 0.36);
        await Page.Keyboard.PressAsync("2");
        await ClickLevelViewport(0.53, 0.43);
        await Page.Keyboard.PressAsync("5");
        await ClickLevelViewport(0.45, 0.56);

        await MovePointerAway();
        await ExpectLevelViewportScreenshotAsync("picture-tool-pictures");
    }

    [TestMethod]
    public async Task ShowsProgressWhileTexturizing()
    {
        await OpenWithLgr();

        await ClickDrawTool();
        await MoveLevelViewportPointer(0.34, 0.29);
        await Page.Mouse.DownAsync();
        await MoveLevelViewportPointer(0.61, 0.29);
        await MoveLevelViewportPointer(0.61, 0.70);
        await MoveLevelViewportPointer(0.34, 0.70);
        await MoveLevelViewportPointer(0.34, 0.29);
        await Page.Mouse.UpAsync();
        await Page.Keyboard.PressAsync("Control+A");

        await ClickBrowserViewport(0, 0, 145, 15);
        await Page.WaitForTimeoutAsync(200);
        await ClickBrowserViewport(0, 0, 160, 376);
        await Page.WaitForTimeoutAsync(500);

        await ClickBrowserViewport(0.5, 0.5, -100, 200);
        await Page.Keyboard.PressAsync("Control+A");
        await Page.Keyboard.TypeAsync("100");
        await ClickBrowserViewport(0.5, 0.5, 84, 238);
        await Page.WaitForTimeoutAsync(100);

        await Page.ExpectScreenshotAsync("texturize-progress", new ToMatchScreenshotOptions
        {
            Clip = await GetBrowserViewportClipAsync(0.5, 0.5, -183, -84, 366, 53),
        });

        await ClickBrowserViewport(0.5, 0.5, 0, 45);
    }

    private async Task OpenWithLgr()
    {
        var lgrPath = await GetLgrPath();
        await Page.AddInitScriptAsync($$"""
            globalThis.showDirectoryPicker = async () => {
                const root = await navigator.storage.getDirectory();
                return await root.getDirectoryHandle("{{BrowserLgrFolderName}}", { create: true });
            };
            """);
        await Page.OpenAndWaitForFirstRender();
        await CopyLgrToBrowserFileSystem(lgrPath);

        await ClickBrowserViewport(0, 0, 800, 50);
        await Page.WaitForLgr();
    }

    private async Task CopyLgrToBrowserFileSystem(string lgrPath)
    {
        await Page.EvaluateAsync("""
            () => {
                const input = document.createElement("input");
                input.id = "lgr-fixture";
                input.type = "file";
                document.body.appendChild(input);
            }
            """);
        var fixtureInput = Page.Locator("#lgr-fixture");
        await fixtureInput.SetInputFilesAsync(lgrPath);
        await Page.EvaluateAsync($$"""
            async () => {
                const input = document.querySelector("#lgr-fixture");
                const source = input.files[0];
                const root = await navigator.storage.getDirectory();
                const directory = await root.getDirectoryHandle("{{BrowserLgrFolderName}}", { create: true });
                const file = await directory.getFileHandle("default.lgr", { create: true });
                const writer = await file.createWritable();
                await writer.write(source);
                await writer.close();
                input.remove();
            }
            """);
    }

    private static async Task<string> GetLgrPath()
    {
        var lgrFolder = Path.Combine(AppContext.BaseDirectory, "Lgr");
        var lgrPath = Path.Combine(lgrFolder, "default.lgr");
        if (File.Exists(lgrPath))
            return lgrPath;

        await LgrDownloadLock.WaitAsync();
        try
        {
            if (File.Exists(lgrPath))
                return lgrPath;

            Directory.CreateDirectory(lgrFolder);
            var temporaryPath = lgrPath + ".download";
            try
            {
                using var response = await HttpClient.GetAsync(
                    LgrDownloadUrl, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                await using (var source = await response.Content.ReadAsStreamAsync())
                await using (var destination = File.Create(temporaryPath))
                {
                    await source.CopyToAsync(destination);
                }

                File.Move(temporaryPath, lgrPath, overwrite: true);
            }
            finally
            {
                File.Delete(temporaryPath);
            }

            return lgrPath;
        }
        finally
        {
            LgrDownloadLock.Release();
        }
    }
}
