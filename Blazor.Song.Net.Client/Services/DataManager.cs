using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Services
{
    public class DataManager : IDataManager
    {
        public DataManager(HttpClient client)
        {
            _client = client;
        }

        private readonly HttpClient _client;
        private TrackInfo _currentTrack;

        public delegate void PlaylistChangedDelegate();

        public event CurrentTrackChangedDelegate CurrentTrackChanged;

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

        public async void DownloadTrack(TrackInfo trackInfo)
        {
            if (!File.Exists(trackInfo.Path))
                return;
            await _client.GetByteArrayAsync(trackInfo.Path).ContinueWith(res => { });
        }

        public async Task<List<TrackInfo>> GetTracks(string filter)
        {
            return (await _client.GetJsonAsync<TrackInfo[]>($"api/Library/Tracks?filter={filter ?? ""}")).ToList();
        }
    }
}