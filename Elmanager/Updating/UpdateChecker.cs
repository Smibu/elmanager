using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Elmanager.Application;
using Elmanager.Utilities.Json;

namespace Elmanager.Updating;

internal static class UpdateChecker
{
    /// <summary>
    ///   Checks if there are new updates for the program.
    /// </summary>
    internal static async Task<bool> CheckForUpdates()
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:78.0) Gecko/20100101 Firefox/78.0");
        try
        {
            var result = await client.GetStreamAsync(VersionUri);
            if (await JsonSerializer.DeserializeAsync(result, typeof(UpdateInfo), SourceGenerationContext.GetOptions()) is UpdateInfo info && info.Date > Global.Version)
            {
                var newDlg = new NewVersionForm(info);
                newDlg.ShowDialog();
                return true;
            }
        }
        catch (HttpRequestException)
        {
        }
        catch (FormatException)
        {
        }

        return false;
    }

    private const string VersionUri = "https://api.github.com/repos/Smibu/elmanager/releases/latest";
    internal const string ChangelogUri = "https://github.com/Smibu/elmanager/blob/master/Elmanager/changelog.md";
}