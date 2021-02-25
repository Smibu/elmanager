using System;
using System.Net;
using System.Text.Json;
using Elmanager.Application;

namespace Elmanager.Updating
{
    internal static class UpdateChecker
    {
        /// <summary>
        ///   Checks if there are new updates for the program.
        /// </summary>
        internal static void CheckForUpdates()
        {
            using var wc = new WebClient();
            wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:78.0) Gecko/20100101 Firefox/78.0");
            try
            {
                var info = JsonSerializer.Deserialize<UpdateInfo>(wc.DownloadString(VersionUri));
                if (info is not null && info.Date > Global.Version)
                {
                    var newDlg = new NewVersionForm(info);
                    newDlg.ShowDialog();
                }
            }
            catch (WebException)
            {
            }
            catch (FormatException)
            {
            }
        }

        private const string VersionUri = "https://api.github.com/repos/Smibu/elmanager/releases/latest";
        internal const string ChangelogUri = "https://github.com/Smibu/elmanager/blob/master/Elmanager/changelog.md";
    }
}