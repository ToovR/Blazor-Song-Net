using Blazor.Song.Net.Shared;

namespace Blazor.Song.Net.Services
{
    public interface ILibraryStore
    {
        Task<byte[]> Download(string path);

        TrackInfo[] GetTracks(string filter);

        IEnumerable<TrackInfo> GetTracks(IEnumerable<long> ids);

        bool LoadLibrary();

        Task<string> LoadPlaylist();

        Task SavePlaylist(string idList);
    }
}