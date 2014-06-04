using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShopifyHelper.Model
{
    [JsonObject]
    public class KrakenResponse
    {
        [JsonProperty(PropertyName = "success")]
        public bool status { get; set; }

        [JsonProperty(PropertyName = "file_name")]
        public string fileName { get; set; }

        [JsonProperty(PropertyName = "original_size")]
        public double? originalSize { get; set; }

        [JsonProperty(PropertyName = "kraked_size")]
        public double? krakedSize { get; set; }

        [JsonProperty(PropertyName = "saved_bytes")]
        public double? savedBytes { get; set; }

        [JsonProperty(PropertyName = "kraked_url")]
        public string krakedURL { get; set; }

    }
}
