using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Elmanager.Updating;

internal class Asset
{
    public Asset(string browserDownloadUrl)
    {
        BrowserDownloadUrl = browserDownloadUrl;
    }

    [JsonPropertyName("browser_download_url")]
    public string BrowserDownloadUrl { get; set; }
}

internal class UpdateInfo
{
    public UpdateInfo(List<Asset> assets, string tagName)
    {
        Assets = assets;
        TagName = tagName;
    }

    [JsonPropertyName("tag_name")]
    public string TagName { get; set; }

    [JsonPropertyName("assets")]
    public List<Asset> Assets { get; set; }

    public string Link => Assets[0].BrowserDownloadUrl;
    public DateTime Date => DateTime.ParseExact(TagName, "d.M.yyyy", null);
}