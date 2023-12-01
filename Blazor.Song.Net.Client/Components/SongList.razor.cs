using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Components;

namespace Blazor.Song.Net.Client.Components
{
    public partial class SongList : ComponentBase
    {
        [Parameter]
        public TrackInfo CurrentTrack { get; set; }

        [Parameter]
        public RenderFragment<TrackInfo> RowTemplate { get; set; }

        [Parameter]
        public List<TrackInfo> Tracks { get; set; }
    }
}