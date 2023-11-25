using Blazor.Song.Net.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Server.Services
{
    public interface IPodcastStore
    {
        Task AddNewChannel(PodcastChannel podcast);

        Task<byte[]> GetChannelEpisodeFile(int collectionId, string link, long id);

        Task<Feed> GetChannelEpisodes(long collectionId);

        PodcastChannel[] GetChannels(string filter);

        Task<PodcastChannelResponse> GetNewChannels(string filter);

        IEnumerable<TrackInfo> GetTracks(IEnumerable<long> ids);
    }
}