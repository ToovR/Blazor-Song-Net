using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Blazor.Song.Net.Shared
{
    public class FeedItem
    {
        public Int64 Id { get; set; }
        public string Title { get; set; }
        public string Uri { get; set; }
        [JsonIgnore]
        public TimeSpan Duration { get; set; }
        public string SDuration
        {
            get { return Duration.ToString(); }
            set { Duration = TimeSpan.Parse(value); }
        }
    }
}
