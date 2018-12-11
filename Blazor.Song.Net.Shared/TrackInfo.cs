using System;

namespace Blazor.Song.Net.Shared
{
    public class TrackInfo
    {
        public string Album { get; set; }
        public string Artist { get; set; }
        public TimeSpan Duration { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
    }
}