using Blazor.Song.Net.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Services
{
    public delegate Task CurrentTrackChangedDelegate(TrackInfo trackInfo);

    public interface IDataManager
    {
        event CurrentTrackChangedDelegate CurrentTrackChanged;

        TrackInfo CurrentTrack { get; set; }
        string Filter { get; set; }
        bool IsPlaying { get; set; }

        void DownloadTrack(TrackInfo trackInfo);

        Task<List<TrackInfo>> GetTracks(string filter);
    }
}