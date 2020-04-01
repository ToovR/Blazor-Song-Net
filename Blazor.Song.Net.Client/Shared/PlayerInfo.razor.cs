using Blazor.Song.Net.Client.Wrap;
using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Shared
{
    public class PlayerInfoComponent : ComponentBase
    {
        [Inject]
        public IJSRuntime JsRuntime { get; set; }


        [Inject]
        protected Services.IDataManager Data { get; set; }

        [Parameter]
        public PlayerAudio PlayerAudio { get; set; }


        public int TimeStatus { get; set; }

        public string TrackFullTitle
        {
            get
            {
                if (CurrentTrack == null)
                    return "";
                else
                    return CurrentTrack.Title + " - " + CurrentTrack.Artist;
            }
        }

        TrackInfo CurrentTrack
        {
            get { return Data.CurrentTrack; }
        }

        protected async void ProgressClick(MouseEventArgs e)
        {
            if (CurrentTrack == null)
                return;
            var element = new Element("songProgress", JsRuntime);
            int offsetWidth = await element.GetOffsetWidth();
            long newTime = (int)e.ClientX * ((int)CurrentTrack.Duration.TotalSeconds) / offsetWidth;
            await PlayerAudio.SetTime((int)newTime);
        }

        public void Refresh(int timeStatus)
        {
            TimeStatus = timeStatus;
            this.StateHasChanged();
        }

    }
}
