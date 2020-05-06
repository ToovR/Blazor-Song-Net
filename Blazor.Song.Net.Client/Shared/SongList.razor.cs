using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Blazor.Song.Net.Client.Shared
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