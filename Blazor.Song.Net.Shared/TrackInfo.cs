using System;
using System.Text.Json.Serialization;

namespace Blazor.Song.Net.Shared
{
    public class TrackInfo
    {
        public string Album { get; set; }
        public string Artist { get; set; }

        [JsonIgnore]
        public TimeSpan Duration { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string SDuration
        {
            get { return Duration.ToString(); }
            set { Duration = TimeSpan.Parse(value); }
        }

        public string Title { get; set; }
    }
}