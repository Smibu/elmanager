using System;
using System.Net;
using Elmanager.Forms;
using Newtonsoft.Json;

namespace Elmanager.Updating
{
    static class UpdateChecker
    {
        /// <summary>
        ///   Checks if there are new updates for the program.
        /// </summary>
        internal static void CheckForUpdates()
        {
            using (var wc = new WebClient())
            {
                wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:78.0) Gecko/20100101 Firefox/78.0");
                try
                {
                    var info = JsonConvert.DeserializeObject<UpdateInfo>(wc.DownloadString(Constants.VersionUri));
                    if (info.Date > Global.Version)
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
        }
    }
}