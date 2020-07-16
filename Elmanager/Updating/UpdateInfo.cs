using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Elmanager.Updating
{
    public class Asset
    {
        public string browser_download_url;
    }

    public class UpdateInfo
    {
        [JsonConverter(typeof(UpdateInfoDateTimeConverter))]
        public DateTime tag_name { get; set; }

        public List<Asset> assets;

        public string Link => assets[0].browser_download_url;
        public DateTime Date => tag_name;
    }
}