using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Blazor.Song.Net.Client.Shared
{
    public class SongListComponent : ComponentBase
    {
        [Parameter]
        public TrackInfo CurrentTrack { get; set; }

        [Parameter]
        public RenderFragment<TrackInfo> RowTemplate { get; set; }

        [Parameter]
        public List<TrackInfo> Tracks { get; set; }
    }
}