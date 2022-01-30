using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Elmanager.UI;

namespace Elmanager.IO;

internal static class NetUtils
{
    internal static async Task DownloadAndOpenFile(string uri, string destFile)
    {
        var client = new HttpClient();
        try
        {
            {
                var result = await client.GetStreamAsync(uri);
                await using var fs = File.Create(destFile);
                await result.CopyToAsync(fs);
            }
            OsUtils.ShellExecute(destFile);
        }
        catch (HttpRequestException)
        {
            UiUtils.ShowError("Failed to download file " + uri);
        }
    }
}