using Blazor.Song.Net.Client.Interfaces;
using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Components;

namespace Blazor.Song.Net.Services
{
    public class ServerDataManager : IDataManager
    {
        private readonly PersistentComponentState _applicationState;
        private readonly ILibraryStore _libraryStore;
        private readonly IPodcastStore _podcastStore;
        private TrackInfo _currentTrack;
        private PersistingComponentStateSubscription _persistingSubscription;

        public ServerDataManager(ILibraryStore libraryStore, IPodcastStore podcastStore, PersistentComponentState applicationState)
        {
            _applicationState = applicationState;
            _libraryStore = libraryStore;
            _podcastStore = podcastStore;
        }

        public delegate void PlaylistChangedDelegate();

        public event CurrentTrackChangedDelegate CurrentTrackChanged;

        public RenderModes CurrentRenderMode
        {
            get { return RenderModes.Server; }
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
            get { return false; }
        }

        public async Task DownloadTrack(TrackInfo trackInfo)
        {
            Console.WriteLine($" t : {trackInfo.Title}, c : {trackInfo.CollectionId}");
            if (trackInfo.CollectionId != null)
            {
                byte[] file = await _podcastStore.GetChannelEpisodeFile(trackInfo.CollectionId.Value, trackInfo.Id);
            }
            else
            {
                //await _client.GetByteArrayAsync(trackInfo.Path);
            }
        }

        public async Task<TrackInfo[]> GetAllSongs()
        {
            TrackInfo[] allTracks;
            if (!_applicationState.TryTakeFromJson("ALLTRACKS", out TrackInfo[]? tracks))
            {
                allTracks = await GetSongs(null);
            }
            else
            {
                allTracks = tracks!;
            }
            return allTracks;
        }

        public Task<List<PodcastChannel>> GetChannels(string filter)
        {
            PodcastChannel[] channels = _podcastStore.GetChannels(filter);
            return Task<List<PodcastChannel>>.FromResult(channels.ToList());
        }

        public async Task<Feed> GetEpisodes(Int64 collectionId)
        {
            Feed feed = await _podcastStore.GetChannelEpisodes(collectionId);
            return feed;
        }

        public Task<string?> GetFilter()
        {
            _applicationState.TryTakeFromJson("FILTER", out string? filter);
            return Task<string>.FromResult(filter);
        }

        public async Task<List<PodcastChannel>> GetNewChannels(string filter)
        {
            PodcastChannelResponse channels = await _podcastStore.GetNewChannels(filter);
            return channels.Results.ToList();
        }

        public async Task<TrackInfo[]> GetSongs(string? filter)
        {
            try
            {
                TrackInfo[] filteredTracks = _libraryStore.GetTracks(filter);
                return filteredTracks;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<TrackInfo>> GetTracks(string ids)
        {
            IEnumerable<long> idList = ids.Split("|", StringSplitOptions.RemoveEmptyEntries).Select(id => long.Parse(id));
            try
            {
                IEnumerable<TrackInfo> songs = _libraryStore.GetTracks(idList);
                IEnumerable<TrackInfo> podcastEpisodes = _podcastStore.GetTracks(idList.Except(songs.Select(song => song.Id)));
                IEnumerable<long> presentIds = idList.Where(id => songs.Any(song => song.Id == id) || podcastEpisodes.Any(episode => episode.Id == id));
                IEnumerable<TrackInfo> tracks = presentIds.Select(id => songs.FirstOrDefault(song => song.Id == id) ?? podcastEpisodes.First(episode => episode.Id == id));
                return tracks.ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> LoadLibrary()
        {
            try
            {
                return _libraryStore.LoadLibrary();
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> LoadPlaylist()
        {
            return await _libraryStore.LoadPlaylist();
        }

        public async Task SavePlaylist(string idList)
        {
            await _libraryStore.SavePlaylist(idList);
        }

        public Task SetFilter(string filter)
        {
            return Task.CompletedTask;
        }

        public async Task SubscribeToPodcast(PodcastChannel podcastChannel)
        {
            await _podcastStore.AddNewChannel(podcastChannel);
        }
    }
}