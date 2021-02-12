using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Elmanager.Updating
{
    internal class Asset
    {
        [JsonPropertyName("browser_download_url")]
        public string BrowserDownloadUrl { get; set; }
    }

    internal class UpdateInfo
    {
        [JsonPropertyName("tag_name")]
        public string TagName { get; set; }

        [JsonPropertyName("assets")]
        public List<Asset> Assets { get; set; }

        public string Link => Assets[0].BrowserDownloadUrl;
        public DateTime Date => DateTime.ParseExact(TagName, "d.M.yyyy", null);
    }
}