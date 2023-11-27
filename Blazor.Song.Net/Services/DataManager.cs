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

namespace Blazor.Song.Net.Services
{
    public class DataManager : IDataManager
    {
        private readonly ILibraryStore _libraryStore;

        private readonly IPodcastStore _podcastStore;

        private TrackInfo _currentTrack;

        public DataManager(ILibraryStore libraryStore, IPodcastStore podcastStore)
        {
            _libraryStore = libraryStore;
            _podcastStore = podcastStore;
        }

        public delegate void PlaylistChangedDelegate();

        public event CurrentTrackChangedDelegate CurrentTrackChanged;

        public string CurrentRenderMode
        {
            get { return "Server"; }
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
                byte[] file = await _podcastStore.GetChannelEpisodeFile(trackInfo.CollectionId.Value, trackInfo.Path, trackInfo.Id);
            }
            else
            {
                //await _client.GetByteArrayAsync(trackInfo.Path);
            }
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

        public async Task<List<PodcastChannel>> GetNewChannels(string filter)
        {
            PodcastChannelResponse channels = await _podcastStore.GetNewChannels(filter);
            return channels.Results.ToList();
        }

        public async Task<List<TrackInfo>> GetSongs(string filter)
        {
            try
            {
                TrackInfo[] filteredTracks = _libraryStore.GetTracks(filter);
                return filteredTracks.ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<TrackInfo>> GetTracks(string ids)
        {
            var idList = ids.Split("|", StringSplitOptions.RemoveEmptyEntries).Select(id => long.Parse(id));
            try
            {
                var songs = _libraryStore.GetTracks(idList);
                var podcastEpisodes = _podcastStore.GetTracks(idList.Except(songs.Select(song => song.Id)));
                var presentIds = idList.Where(id => songs.Any(song => song.Id == id) || podcastEpisodes.Any(episode => episode.Id == id));
                var tracks = presentIds.Select(id => songs.FirstOrDefault(song => song.Id == id) ?? podcastEpisodes.First(episode => episode.Id == id));
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

        public async Task SubscribeToPodcast(PodcastChannel podcastChannel)
        {
            await _podcastStore.AddNewChannel(podcastChannel);
        }
    }
}