using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Elmanager.Utilities.Json;

namespace Elmanager.Updating;

public static class UpdateChecker
{
    public static async Task<UpdateInfo?> CheckForUpdates(DateTime currentVersion)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:78.0) Gecko/20100101 Firefox/78.0");
        try
        {
            var result = await client.GetStreamAsync(VersionUri);
            if (await JsonSerializer.DeserializeAsync(result, typeof(UpdateInfo), SourceGenerationContext.GetOptions()) is UpdateInfo info && info.Date > currentVersion)
            {
                return info;
            }
        }
        catch (HttpRequestException)
        {
        }
        catch (FormatException)
        {
        }

        return null;
    }

    private const string VersionUri = "https://api.github.com/repos/Smibu/elmanager/releases/latest";
    public const string ChangelogUri = "https://github.com/Smibu/elmanager/blob/master/Elmanager/changelog.md";
}
