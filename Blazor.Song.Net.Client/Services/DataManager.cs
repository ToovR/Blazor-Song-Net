using Blazor.Song.Net.Client.Interfaces;
using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;

namespace Blazor.Song.Net.Client.Services
{
    public class DataManager : IDataManager
    {
        private readonly HttpClient _client;

        private TrackInfo _currentTrack;

        public DataManager(HttpClient client)
        {
            _client = client;
        }

        public delegate void PlaylistChangedDelegate();

        public event CurrentTrackChangedDelegate CurrentTrackChanged;

        public string CurrentRenderMode
        {
            get { return "Client"; }
        }

        public TrackInfo CurrentTrack
        {
            get { return _currentTrack; }
            set
            {
                if (value == null && _currentTrack == null)
                    return;
                if (value != null && _currentTrack != null && value.Id == _currentTrack.Id)
                    return;
                _currentTrack = value;
                CurrentTrackChanged?.Invoke(_currentTrack);
            }
        }

        public string Filter { get; set; }
        public bool IsPlaying { get; set; }

        public async Task DownloadTrack(TrackInfo trackInfo)
        {
            Console.WriteLine($" t : {trackInfo.Title}, c : {trackInfo.CollectionId}");
            if (trackInfo.CollectionId != null)
            {
                await _client.GetByteArrayAsync($"api/Podcast/GetChannelEpisode?collectionId={trackInfo.CollectionId}&link={HttpUtility.UrlEncode(trackInfo.Path)}&id={trackInfo.Id}");
            }
            else
            {
                await _client.GetByteArrayAsync(trackInfo.Path);
            }
        }

        public async Task<List<PodcastChannel>> GetChannels(string filter)
        {
            var channels = (await _client.GetFromJsonAsync<PodcastChannel[]>($"api/Podcast/Channels?filter={filter ?? ""}")).ToList();
            return channels;
        }

        public async Task<Feed> GetEpisodes(Int64 collectionId)
        {
            return await _client.GetFromJsonAsync<Feed>($"api/Podcast/GetChannelEpisodes?collectionId={collectionId}");
        }

        public async Task<List<PodcastChannel>> GetNewChannels(string filter)
        {
            return (await _client.GetFromJsonAsync<PodcastChannelResponse>($"api/Podcast/NewChannels?filter={filter ?? ""}")).Results.ToList();
        }

        public async Task<List<TrackInfo>> GetSongs(string filter)
        {
            return (await _client.GetFromJsonAsync<TrackInfo[]>($"api/Library/Tracks?filter={filter ?? ""}")).ToList();
        }

        public async Task<List<TrackInfo>> GetTracks(string idList)
        {
            return (await _client.GetFromJsonAsync<TrackInfo[]>($"api/Track/Tracks?ids={idList ?? ""}")).ToList();
        }

        public async Task<bool> LoadLibrary()
        {
            return await _client.GetFromJsonAsync<bool>($"api/Track/LoadLibrary");
        }

        public async Task SubscribeToPodcast(PodcastChannel podcastChannel)
        {
            await _client.PostAsJsonAsync($"api/Podcast/NewChannels", podcastChannel);
        }
    }
}