using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Blazor.Song.Net.Client.Shared;
using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazor.Song.Net.Client.Pages
{
    public class PlaylistComponent : ComponentBase
    {
        [Inject]
        IJSRuntime JsRuntime { get; set; }


        [Inject]
        protected Services.IDataManager Data { get; set; }


        [CascadingParameter]
        public ObservableList<TrackInfo> PlaylistTracks { get; set; }


        void CurrentTrackChanged(TrackInfo track)
        {
            this.StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadPlaylist();
            Data.CurrentTrackChanged += CurrentTrackChanged;
            PlaylistTracks.CollectionChanged += PlaylistChanged;

            await base.OnInitializedAsync();
        }

        async void PlaylistChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            await SavePlaylist();
            this.StateHasChanged();
        }

        async Task LoadPlaylist()
        {
            Wrap.Cookie playlistCookie = new Wrap.Cookie("playlist", JsRuntime);
            string sidList = await playlistCookie.Get();
            if (sidList != null)
                (await Data.GetTracks("id:/^(" + sidList + ")$/")).ForEach(t =>
                {
                    if (!PlaylistTracks.Any(p => p.Id == t.Id))
                        PlaylistTracks.Add(t);
                });
            if (PlaylistTracks.Count > 0)
                this.StateHasChanged();
        }

        async Task SavePlaylist()
        {
            string idList = string.Join("|", PlaylistTracks.Select(p => p.Id));
            Wrap.Cookie playlistCookie = new Wrap.Cookie("playlist", JsRuntime);
            await playlistCookie.Set(idList);
        }

        protected void PlaylistRowClick(int id)
        {
            if (Data.CurrentTrack != null && Data.CurrentTrack.Id == id || !PlaylistTracks.Any(t => t.Id == id))
                return;
            Data.CurrentTrack = PlaylistTracks.First(t => t.Id == id);
            this.StateHasChanged();
        }

        protected void PlaylistRowDoubleClick(int id)
        {
            if (Data.IsPlaying)
                return;
            Data.IsPlaying = true;
            if (Data.CurrentTrack.Id == id || !PlaylistTracks.Any(t => t.Id == id))
            {
                this.StateHasChanged();
                return;
            }
            Data.CurrentTrack = PlaylistTracks.First(t => t.Id == id);
            this.StateHasChanged();
        }

        protected void PlaylistRowRemoveClick(int id)
        {
            if (!PlaylistTracks.Any(t => t.Id == id))
                return;
            if (Data.CurrentTrack.Id == id)
                SetCurrentTrackNext();

            RemovePlaylistTrack(id);
        }

        public void RemovePlaylistTrack(int trackInfoId)
        {
            TrackInfo trackToRemove = PlaylistTracks.First(t => t.Id == trackInfoId);
            PlaylistTracks.Remove(trackToRemove);
            this.StateHasChanged();
        }

        public void SetCurrentTrackNext()
        {
            if (PlaylistTracks.Count <= 1)
                return;
            Data.CurrentTrack = PlaylistTracks[(PlaylistTracks.IndexOf(Data.CurrentTrack) + 1) % PlaylistTracks.Count];
        }
    }
}
