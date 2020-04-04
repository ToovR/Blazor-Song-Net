using System;
using System.Collections.Generic;
using System.Text;

namespace Blazor.Song.Net.Shared
{
    public class PodcastChannelResponse
    {
        public int ResultCount { get; set; }
    public PodcastChannel[] Results { get; set; }
    }
}
