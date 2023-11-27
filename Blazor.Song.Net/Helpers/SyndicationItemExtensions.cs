using Blazor.Song.Net.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Helpers
{
    public static class SyndicationItemExtensions
    {
        public static FeedItem ToFeedItem(this SyndicationItem syndicationItem)
        {
            string url = syndicationItem.Links.FirstOrDefault(l => l.MediaType!= null && l.MediaType.StartsWith("audio")).Uri.AbsoluteUri;
            return new FeedItem
            {
                Id = syndicationItem.Id.GetHashCode(),
                Title = syndicationItem.Title.Text,
                Uri = url
            };
        }

    }
}
