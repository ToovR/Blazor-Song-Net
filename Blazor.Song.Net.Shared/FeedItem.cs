using System;
using System.Text.Json.Serialization;

namespace Blazor.Song.Net.Shared
{
    public class FeedItem
    {
        [JsonIgnore]
        public TimeSpan Duration { get; set; }

        public Int64 Id { get; set; }

        public string SDuration
        {
            get { return Duration.ToString(); }
            set { Duration = TimeSpan.Parse(value); }
        }

        public string Title { get; set; }
        public string Uri { get; set; }
    }
}