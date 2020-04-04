using System;
using System.Collections.Generic;
using System.Text;

namespace Blazor.Song.Net.Shared
{
    public class Feed
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public FeedItem[] Items { get; set; }
    }
}
