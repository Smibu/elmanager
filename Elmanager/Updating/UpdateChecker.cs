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