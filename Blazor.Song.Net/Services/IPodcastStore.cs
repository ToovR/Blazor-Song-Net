using Blazor.Song.Net.Shared;

namespace Blazor.Song.Net.Services
{
    public interface IPodcastStore
    {
        Task AddNewChannel(PodcastChannel podcast);

        Task<byte[]> GetChannelEpisodeFile(int collectionId, long id);

        Task<Feed> GetChannelEpisodes(long collectionId);

        PodcastChannel[] GetChannels(string filter);

        Task<PodcastChannelResponse> GetNewChannels(string filter);

        IEnumerable<TrackInfo> GetTracks(IEnumerable<long> ids);
    }
}