using Blazor.Song.Net.Shared;
using System.ServiceModel.Syndication;

namespace Blazor.Song.Net.Helpers
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