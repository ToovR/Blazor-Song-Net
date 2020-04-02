using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Components;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Shared
{
    public class MainLayoutComponent : LayoutComponentBase
    {
        protected ObservableList<TrackInfo> PlaylistTracks { get; set; } = new ObservableList<TrackInfo>();

        [Inject]
        private Services.IDataManager Data { get; set; }

        protected override async Task OnInitializedAsync()
        {
            PlaylistTracks.CollectionChanged += PlaylistTracksCollectionChanged;
            if (PlaylistTracks.Count > 0)
                Data.CurrentTrack = PlaylistTracks[0];
            await base.OnInitializedAsync();
        }

        private void PlaylistTracksCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
                Data.DownloadTrack(((TrackInfo)e.NewItems[0]));
        }
    }
}