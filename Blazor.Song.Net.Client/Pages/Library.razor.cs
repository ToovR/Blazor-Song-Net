using Blazor.Song.Net.Client.Components;
using Blazor.Song.Net.Client.Helpers;
using Blazor.Song.Net.Shared;
using Blazored.LocalStorage;
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
                LocalStorage.SetItemAsync("FILTER", _filter);
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

        [Inject]
        private ILocalStorageService LocalStorage { get; set; }

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

            if (Data.CurrentRenderMode == RenderModes.Server)
            {
                if (!ApplicationState.TryTakeFromJson("ALLTRACKS", out TrackInfo[]? allTracks))
                {
                    _allTracks = await Data.GetSongs(null);
                }
                else
                {
                    _allTracks = allTracks!;
                }
            }
            else
            {
                TrackInfo[] tracks = await LocalStorage.GetItemAsync<TrackInfo[]>("ALLTRACKS");
                if (tracks == null)
                {
                    _allTracks = await Data.GetSongs(null);
                    await LocalStorage.SetItemAsync("ALLTRACKS", _allTracks);
                }
                else
                {
                    _allTracks = tracks;
                }

                if (!string.IsNullOrEmpty(FilterParameter))
                {
                    _filter = FilterParameter;
                }
                else
                {
                    string filter = await LocalStorage.GetItemAsync<string>("FILTER");

                    if (filter != null)
                    {
                        _filter = filter ?? "";
                    }
                }
            }

            if (!string.IsNullOrEmpty(FilterParameter))
            {
                _filter = FilterParameter;
            }
            else if (ApplicationState.TryTakeFromJson("FILTER", out string? filter))
            {
                _filter = filter ?? "";
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