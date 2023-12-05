using Blazor.Song.Net.Client.Interfaces;
using Blazor.Song.Net.Shared;
using Blazored.LocalStorage;
using System.Net.Http.Json;
using System.Web;

namespace Blazor.Song.Net.Client.Services
{
    public class ClientDataManager : IDataManager
    {
        private readonly HttpClient _client;
        private readonly ILocalStorageService _localStorage;
        private TrackInfo _currentTrack;

        public ClientDataManager(ILocalStorageService localStorage, HttpClient client)
        {
            _client = client;
            _localStorage = localStorage;
        }

        public delegate void PlaylistChangedDelegate();

        public event CurrentTrackChangedDelegate CurrentTrackChanged;

        public RenderModes CurrentRenderMode
        {
            get { return RenderModes.Client; }
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

        public bool IsPlayingEnabled
        {
            get
            {
                return CurrentTrack != null;
            }
        }

        public async Task DownloadTrack(TrackInfo trackInfo)
        {
            Console.WriteLine($" t : {trackInfo.Title}, c : {trackInfo.CollectionId}");
            if (trackInfo.CollectionId != null)
            {
                await _client.GetByteArrayAsync($"/api/Podcast/GetChannelEpisode?collectionId={trackInfo.CollectionId}&id={trackInfo.Id}");
            }
            else
            {
                var trackInfoPath = $"/{trackInfo.Path}".Replace("//", "/");
                await _client.GetByteArrayAsync($"/api/Library/Download?path={HttpUtility.UrlEncode(trackInfoPath)}");
            }
        }

        public async Task<TrackInfo[]> GetAllSongs()
        {
            TrackInfo[] allTracks;
            TrackInfo[] tracks = await _localStorage.GetItemAsync<TrackInfo[]>("ALLTRACKS");
            if (tracks == null)
            {
                allTracks = await GetSongs(null);
                if (allTracks != null && allTracks.Length > 0)
                {
                    await _localStorage.SetItemAsync("ALLTRACKS", allTracks);
                }
            }
            else
            {
                allTracks = tracks;
            }

            return allTracks;
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

        public async Task<string?> GetFilter()
        {
            return await _localStorage.GetItemAsync<string>("FILTER");
        }

        public async Task<List<PodcastChannel>> GetNewChannels(string filter)
        {
            return (await _client.GetFromJsonAsync<PodcastChannelResponse>($"api/Podcast/NewChannels?filter={filter ?? ""}")).Results.ToList();
        }

        public async Task<TrackInfo[]> GetSongs(string? filter)
        {
            return (await _client.GetFromJsonAsync<TrackInfo[]>($"api/Library/Tracks?filter={filter ?? ""}")).ToArray();
        }

        public async Task<List<TrackInfo>> GetTracks(string idList)
        {
            return (await _client.GetFromJsonAsync<TrackInfo[]>($"api/Track/Tracks?ids={idList ?? ""}")).ToList();
        }

        public async Task<bool> LoadLibrary()
        {
            return await _client.GetFromJsonAsync<bool>($"api/Track/LoadLibrary");
        }

        public async Task<string> LoadPlaylist()
        {
            return await _client.GetStringAsync($"api/Library/Playlist");
        }

        public async Task SavePlaylist(string idList)
        {
            await _client.PostAsJsonAsync($"api/Library/Playlist", idList);
        }

        public async Task SetFilter(string filter)
        {
            await _localStorage.SetItemAsync("FILTER", filter);
        }

        public async Task SubscribeToPodcast(PodcastChannel podcastChannel)
        {
            await _client.PostAsJsonAsync($"api/Podcast/NewChannels", podcastChannel);
        }
    }
}