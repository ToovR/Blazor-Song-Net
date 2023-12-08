using Blazor.Song.Net.Client.Components;
using Blazor.Song.Net.Client.Helpers;
using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Components;

namespace Blazor.Song.Net.Client.Pages
{
    public partial class Library : PageBase
    {
        private TrackInfo[] _allTracks = [];
        private string _filter = string.Empty;
        private PersistingComponentStateSubscription _persistingSubscription;
        private SongList _songList;
        public TrackInfo CurrentTrack { get; set; }

        public string Filter
        {
            get { return _filter; }
            set
            {
                string? decodedValue = null;
                if (value != null)
                {
                    decodedValue = Uri.UnescapeDataString(value);
                }
                if (_filter == decodedValue)
                {
                    return;
                }
                _filter = decodedValue ?? "";
                Data.SetFilter(_filter);
                UpdateLibrary(Filter);
            }
        }

        [Parameter]
        public string? FilterParameter
        { get; set; }

        public bool IsLibraryLoaded { get; private set; }

        [CascadingParameter]
        public ObservableList<TrackInfo> PlaylistTracks { get; set; }

        public TrackInfo[] TrackListFiltered { get; set; }

        [Inject]
        private PersistentComponentState ApplicationState { get; set; }

        public void AddPlaylistClick(TrackInfo track)
        {
            if (PlaylistTracks.Any(t => t.Id == track.Id))
            {
                return;
            }
            PlaylistTracks.Add(track);
            if (track.ClickMarker == null)
            {
                track.ClickMarker = true;
            }
            track.ClickMarker = !track.ClickMarker;
        }

        protected async Task LoadLibraryClick()
        {
            bool loaded = await Data.LoadLibrary();
            if (loaded)
            {
                IsLibraryLoaded = true;
                await UpdateLibrary(_filter);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            _persistingSubscription = ApplicationState.RegisterOnPersisting(PersistData);

            _allTracks = await Data.GetAllSongs();

            if (!string.IsNullOrEmpty(FilterParameter))
            {
                _filter = FilterParameter;
            }
            else
            {
                string? filter = await Data.GetFilter();
                if (filter != null)
                {
                    _filter = filter;
                }
            }

            if (_allTracks.Length == 0)
            {
                IsLibraryLoaded = false;
                return;
            }

            IsLibraryLoaded = true;
            await UpdateLibrary(_filter);
            await base.OnInitializedAsync();
        }

        private Task PersistData()
        {
            ApplicationState.PersistAsJson("ALLTRACKS", _allTracks);
            ApplicationState.PersistAsJson("FILTER", _filter);
            return Task.CompletedTask;
        }

        private Task UpdateLibrary(string filter)
        {
            TrackListFiltered = _allTracks.GetTracks(filter);
            return Task.CompletedTask;
        }
    }
}