using Blazor.Song.Net.Client.Interfaces;
using Blazor.Song.Net.Client.Services;
using Blazor.Song.Net.Client.Wrap;
using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Blazor.Song.Net.Client.Shared
{
    public partial class PlayerInfo : ComponentBase
    {
        [Inject]
        public IAudioService AudioService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        public int TimeStatus { get; set; }

        public string TrackFullTitle
        {
            get
            {
                if (CurrentTrack == null)
                    return "";
                else
                    return $"{CurrentTrack.Title}  - {CurrentTrack.Artist}";
            }
        }

        [Inject]
        protected IDataManager Data { get; set; }

        private TrackInfo CurrentTrack
        {
            get { return Data.CurrentTrack; }
        }

        public void Refresh(int timeStatus)
        {
            TimeStatus = timeStatus;
            this.StateHasChanged();
        }

        protected async void ProgressClick(MouseEventArgs e)
        {
            if (CurrentTrack == null)
                return;
            var element = new Element("songProgress", JsRuntime);
            int offsetWidth = await element.GetOffsetWidth();
            long newTime = (int)e.ClientX * ((int)CurrentTrack.Duration.TotalSeconds) / offsetWidth;
            await AudioService.SetTime((int)newTime, CurrentTrack.Duration.TotalSeconds);
        }
    }
}