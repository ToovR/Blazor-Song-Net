using Blazor.Song.Net.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Server.Helpers
{
    public static class SyndicationFeedExtensions
    {
        public static Feed ToFeed(this SyndicationFeed syndicationFeed)
        {
            return new Feed
            {
                Title = syndicationFeed.Title.Text,
                Description = syndicationFeed.Description.Text,
                ImageUrl = syndicationFeed.ImageUrl?.AbsoluteUri,
                Items = syndicationFeed.Items.Select(i => i.ToFeedItem()).ToArray()
            };
        }
    }
}
