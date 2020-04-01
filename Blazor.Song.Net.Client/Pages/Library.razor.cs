using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components;
using Blazor.Song.Net.Shared;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Web;
using Blazor.Song.Net.Client.Shared;
using System.Linq;

namespace Blazor.Song.Net.Client
{
    public class LibraryComponent : ComponentBase
    {
        [Inject]
        Services.IDataManager Data { get; set; }

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

        public TrackInfo CurrentTrack { get; set; }
        public List<TrackInfo> TrackListFiltered { get; set; }

        public void DoubleclickPlaylistRow(TrackInfo track)
        {
            if (PlaylistTracks.Any(t => t.Id == track.Id))
                return;
            PlaylistTracks.Add(track);
        }

        protected async Task FilterClick()
        {
            string filter = Data.Filter;
            await UpdateLibrary(filter);
        }

        protected override async Task OnInitializedAsync()
        {
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

        async Task UpdateLibrary(string filter)
        {
            TrackListFiltered = await Data.GetTracks(filter);
        }
    }
}
