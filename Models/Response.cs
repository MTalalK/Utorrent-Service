using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WorkerServiceNew.Models
{
    internal record Response
    {
        [JsonPropertyName("build")]
        public int Build { get; set; }

        [JsonPropertyName("torrents")]
        public List<List<object>> Torrents { get; set; }

        [JsonPropertyName("label")]
        public List<object> Label { get; set; }

        [JsonPropertyName("torrentc")]
        public string Torrentc { get; set; }

        [JsonPropertyName("rssfeeds")]
        public List<object> RssFeeds { get; set; }

        [JsonPropertyName("rssfilters")]
        public List<object> RssFilters { get; set; }
    }
}
