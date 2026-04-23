using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Elmanager.IO;

public static class NetUtils
{
    public static async Task DownloadAndOpenFile(string uri, string destFile)
    {
        var client = new HttpClient();
        {
            var result = await client.GetStreamAsync(uri);
            await using var fs = File.Create(destFile);
            await result.CopyToAsync(fs);
        }
        OsUtils.ShellExecute(destFile);
    }
}
