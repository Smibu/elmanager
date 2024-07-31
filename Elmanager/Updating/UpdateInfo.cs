using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Elmanager.Updating;

internal class Asset
{
    [JsonPropertyName("browser_download_url")]
    public required string BrowserDownloadUrl { get; set; }
}

internal class UpdateInfo
{
    [JsonPropertyName("tag_name")]
    public required string TagName { get; set; }

    [JsonPropertyName("assets")]
    public required List<Asset> Assets { get; set; }

#if WINE
    public string Link => Assets.Find(IsWineAsset)!.BrowserDownloadUrl;
#else
    public string Link => Assets.Find(a => !IsWineAsset(a))!.BrowserDownloadUrl;
#endif

    public DateTime Date => DateTime.ParseExact(TagName, "d.M.yyyy", null);

    private static bool IsWineAsset(Asset asset) => asset.BrowserDownloadUrl.Contains("Wine");
}