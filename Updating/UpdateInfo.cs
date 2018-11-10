using System;
using Newtonsoft.Json;

namespace Elmanager.Updating
{
    public class UpdateInfo
    {
        public string Link { get; set; }

        [JsonConverter(typeof(UpdateInfoDateTimeConverter))]
        public DateTime Date { get; set; }
    }
}