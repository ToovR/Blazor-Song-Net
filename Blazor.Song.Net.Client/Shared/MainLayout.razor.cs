using Blazor.Song.Net.Client.Helpers;
using Blazor.Song.Net.Client.Interfaces;
using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Components;
using System.Collections.Specialized;

namespace Blazor.Song.Net.Client.Shared
{
    public class MainLayoutComponent : LayoutComponentBase
    {
        protected string CurrentRenderMode
        {
            get
            {
                if (Data?.CurrentRenderMode == null)
                {
                    return string.Empty;
                }
                return Data?.CurrentRenderMode.ToString() ?? "";
            }
        }

        protected ObservableList<TrackInfo> PlaylistTracks { get; set; } = new ObservableList<TrackInfo>();

        [Inject]
        private IDataManager Data { get; set; }

        protected override async Task OnInitializedAsync()
        {
            PlaylistTracks.CollectionChanged += PlaylistTracksCollectionChanged;
            if (PlaylistTracks.Count > 0)
                Data.CurrentTrack = PlaylistTracks[0];

            await base.OnInitializedAsync();
        }

        private void PlaylistTracksCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0 & Data.CurrentRenderMode == RenderModes.Client)
            {
                Console.WriteLine(e.NewItems[0]);
                Data.DownloadTrack((TrackInfo)e.NewItems[0]);
            }
        }
    }
}