using Blazor.Song.Net.Shared;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Blazor.Song.Indexer
{
    public interface ITrackParserService
    {
        Task<string> GetChannelEpisode(int collectionId, string link, long id);
        bool IsLibraryFileExists();
        void UpdateTrackData();
        string GetTrackContent();
        string GetPodcastChannelListContent();
        string GetPodcastDownloadedEpisodesContent();
        void UpdateChannelFile(string content);
        void UpdateEpisodeFile(string episodeFileContent);
    }
}