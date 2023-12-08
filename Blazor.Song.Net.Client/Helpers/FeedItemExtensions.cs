using Blazor.Song.Net.Shared;

namespace Blazor.Song.Net.Client.Helpers
{
    public static class FeedItemExtensions
    {
        public static TrackInfo ToTrackInfo(this FeedItem feedItem, PodcastChannel channel)
        {
            return new TrackInfo
            {
                Id = feedItem.Id,
                CollectionId = channel.CollectionId,
                Title = feedItem.Title,
                Path = feedItem.Uri,
                Duration = feedItem.Duration
            };
        }
    }
}