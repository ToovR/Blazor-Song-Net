using Blazor.Song.Net.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Server.Helpers
{
    public static class SyndicationItemExtensions
    {
        public static FeedItem ToFeedItem(this SyndicationItem syndicationItem)
        {
            string url = syndicationItem.Links.FirstOrDefault(l => l.Uri.AbsoluteUri.Contains(".mp3")).Uri.AbsoluteUri;
            return new FeedItem
            {
                Id = syndicationItem.Id.GetHashCode(),
                Title = syndicationItem.Title.Text,
                Uri = url
            };
        }

    }
}
