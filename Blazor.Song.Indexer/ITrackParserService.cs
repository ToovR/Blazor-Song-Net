using Blazor.Song.Net.Shared;
using System;
using System.Threading.Tasks;

namespace Blazor.Song.Indexer
{
    public interface ITrackParserService
    {
        Task<byte[]> Download(string path);

        Task<string> GetChannelEpisode(int collectionId, string link, long id);

        string GetPodcastChannelListContent();

        string GetPodcastDownloadedEpisodesContent();

        string GetTrackContent();

        TrackInfo GetTrackInfo(string musicFilePath, int index, Uri folderRoot = null);

        bool IsLibraryFileExists();

        void UpdateChannelFile(string content);

        void UpdateEpisodeFile(string episodeFileContent);

        void UpdateTrackData();
    }
}