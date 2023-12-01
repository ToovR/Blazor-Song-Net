﻿using Blazor.Song.Net.Shared;

namespace Blazor.Song.Net.Client.Interfaces
{
    public delegate Task CurrentTrackChangedDelegate(TrackInfo trackInfo);

    public interface IDataManager
    {
        event CurrentTrackChangedDelegate CurrentTrackChanged;

        string CurrentRenderMode { get; }
        TrackInfo CurrentTrack { get; set; }
        string Filter { get; set; }
        bool IsPlaying { get; set; }
        bool IsPlayingEnabled { get; }

        Task DownloadTrack(TrackInfo trackInfo);

        Task<List<PodcastChannel>> GetChannels(string filter);

        Task<Feed> GetEpisodes(long collectionId);

        Task<List<PodcastChannel>> GetNewChannels(string filter);

        Task<List<TrackInfo>> GetSongs(string filter);

        Task<List<TrackInfo>> GetTracks(string idList);

        Task<bool> LoadLibrary();

        Task<string> LoadPlaylist();

        Task SavePlaylist(string idList);

        Task SubscribeToPodcast(PodcastChannel channel);
    }
}