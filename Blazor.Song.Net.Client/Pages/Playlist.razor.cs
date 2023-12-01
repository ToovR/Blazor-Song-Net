using Blazor.Song.Net.Client.Helpers;
using Blazor.Song.Net.Client.Interfaces;
using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Components;
using System.Collections.Specialized;

namespace Blazor.Song.Net.Client.Pages
{
    public partial class Playlist : ComponentBase
    {
        [CascadingParameter]
        public ObservableList<TrackInfo> PlaylistTracks { get; set; }

        [Inject]
        protected IDataManager Data { get; set; }

        public void RemovePlaylistTrack(Int64 trackInfoId)
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

        protected override async Task OnInitializedAsync()
        {
            if (PlaylistTracks == null)
            {
                PlaylistTracks = new ObservableList<TrackInfo>();
            }
            else
            {
                PlaylistTracks.CollectionChanged -= PlaylistChanged;
            }
            await LoadPlaylist();
            Data.CurrentTrackChanged += CurrentTrackChanged;
            PlaylistTracks.CollectionChanged += PlaylistChanged;

            await base.OnInitializedAsync();
        }

        protected void PlaylistRowClick(Int64 id)
        {
            if (Data.CurrentTrack != null && Data.CurrentTrack.Id == id || !PlaylistTracks.Any(t => t.Id == id))
                return;
            Data.CurrentTrack = PlaylistTracks.First(t => t.Id == id);
            this.StateHasChanged();
        }

        protected void PlaylistRowDoubleClick(Int64 id)
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

        protected void PlaylistRowRemoveClick(Int64 id)
        {
            if (!PlaylistTracks.Any(t => t.Id == id))
                return;
            if (Data.CurrentTrack != null && Data.CurrentTrack.Id == id)
                SetCurrentTrackNext();

            RemovePlaylistTrack(id);
        }

        private async Task CurrentTrackChanged(TrackInfo track)
        {
            this.StateHasChanged();
        }

        private async Task LoadPlaylist()
        {
            string sidList = await Data.LoadPlaylist();
            if (sidList == null)
            {
                return;
            }
            if (sidList != null)
            {
                PlaylistTracks.CollectionChanged -= PlaylistChanged;
                (await Data.GetTracks(sidList)).ForEach(t =>
                {
                    if (!PlaylistTracks.Any(p => p.Id == t.Id))
                        PlaylistTracks.Add(t);
                });
                PlaylistTracks.CollectionChanged += PlaylistChanged;
            }
            if (PlaylistTracks.Count > 0 && Data.CurrentTrack == null)
            {
                Data.CurrentTrack = PlaylistTracks.First();
            }
            this.StateHasChanged();
        }

        private void PlaylistChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Task.Run(async () =>
            {
                await SavePlaylist();
                this.StateHasChanged();
            });
        }

        private async Task SavePlaylist()
        {
            string idList = string.Join("|", PlaylistTracks.Select(p => p.Id));
            await Data.SavePlaylist(idList);
        }
    }
}