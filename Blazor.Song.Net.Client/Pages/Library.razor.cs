using Blazor.Song.Net.Client.Shared;
using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Pages
{
    public partial class Library : ComponentBase
    {
        public TrackInfo CurrentTrack { get; set; }

        [Parameter]
        public string Filter
        {
            get { return Data.Filter; }
            set
            {
                string decodedValue = null;
                if (value != null)
                    decodedValue = Uri.UnescapeDataString(value);
                if (Data.Filter == decodedValue)
                    return;
                Data.Filter = decodedValue;
            }
        }

        [CascadingParameter]
        public ObservableList<TrackInfo> PlaylistTracks { get; set; }

        public List<TrackInfo> TrackListFiltered { get; set; }
        public bool IsLibraryLoaded { get; private set; }
        [Inject]
        private Services.IDataManager Data { get; set; }

        public void DoubleclickPlaylistRow(TrackInfo track)
        {
            if (PlaylistTracks.Any(t => t.Id == track.Id))
                return;
            PlaylistTracks.Add(track);
        }

        public async Task SearchKeyUp(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                Console.WriteLine($"current filter : {Filter}");
                await UpdateLibrary(Data.Filter);
            }
        }

        protected async Task FilterClick()
        {
            string filter = Data.Filter;
            await UpdateLibrary(filter);
        }

        protected async Task LoadLibraryClick()
        {
            bool loaded = await Data.LoadLibrary();
            if (loaded)
            {
                IsLibraryLoaded = true;
                await UpdateLibrary(Data.Filter);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            if ((await Data.GetSongs(null)).Count == 0)
            {
                IsLibraryLoaded = false;
                return;
            }
            IsLibraryLoaded = true;
            await UpdateLibrary(Data.Filter);
            await base.OnInitializedAsync();
        }

        protected async Task SearchInputKeyPressed(KeyboardEventArgs keybordEvent)
        {
            if (keybordEvent.Key == "Enter")
            {
                await FilterClick();
            }
        }

        private async Task UpdateLibrary(string filter)
        {

            TrackListFiltered = await Data.GetSongs(filter);
        }
    }
}