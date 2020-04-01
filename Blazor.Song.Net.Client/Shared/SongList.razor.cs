using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Shared
{
    public class SongListComponent : ComponentBase
    {
        [Parameter]
        public List<TrackInfo> Tracks { get; set; }

        [Parameter]
        public TrackInfo CurrentTrack { get; set; }

        [Parameter]
        public RenderFragment<TrackInfo> RowTemplate { get; set; }

    }
}
