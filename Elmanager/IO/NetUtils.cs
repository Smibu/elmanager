using System.Net;
using Elmanager.UI;

namespace Elmanager.IO;

internal static class NetUtils
{
    internal static void DownloadAndOpenFile(string uri, string destFile, string username = null,
        string password = null)
    {
        var wc = new WebClient();
        if (username != null && password != null)
            wc.Credentials = new NetworkCredential(username, password);
        try
        {
            wc.DownloadFile(uri, destFile);
            OsUtils.ShellExecute(destFile);
        }
        catch (WebException)
        {
            UiUtils.ShowError("Failed to download file " + uri);
        }
    }
}