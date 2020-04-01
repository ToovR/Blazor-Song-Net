using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Shared
{
    public class MainLayoutComponent : LayoutComponentBase
    {
        [Inject]
        Services.IDataManager Data { get; set; }


        protected ObservableList<TrackInfo> PlaylistTracks { get; set; } = new ObservableList<TrackInfo>();

        protected override async Task OnInitializedAsync()
        {
            PlaylistTracks.CollectionChanged += PlaylistTracksCollectionChanged;
            if (PlaylistTracks.Count > 0)
                Data.CurrentTrack = PlaylistTracks[0];
            await base.OnInitializedAsync();
        }

        void PlaylistTracksCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (e.NewItems != null && e.NewItems.Count > 0)
                Data.DownloadTrack(((TrackInfo)e.NewItems[0]));
        }

    }
}
