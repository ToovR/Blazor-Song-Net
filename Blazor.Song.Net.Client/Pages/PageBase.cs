using Blazor.Song.Net.Client.Interfaces;
using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Components;

namespace Blazor.Song.Net.Client.Pages
{
    public class PageBase : ComponentBase
    {
        public string Title { get; set; }

        [Inject]
        protected IDataManager Data { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Data.CurrentTrackChanged += CurrentTrackChanged;
            if (Data.CurrentTrack != null)
            {
                UpdateTitle(Data.CurrentTrack);
            }

            await base.OnInitializedAsync();
        }

        private Task CurrentTrackChanged(TrackInfo track)
        {
            UpdateTitle(track);
            this.StateHasChanged();
            return Task.CompletedTask;
        }

        private void UpdateTitle(TrackInfo info)
        {
            if (info != null)
                Title = $"{info.Title}, {info.Artist} - song.net";
            else
                Title = "song.net";
        }
    }
}