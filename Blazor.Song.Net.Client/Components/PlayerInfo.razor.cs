using Blazor.Song.Net.Client.Interfaces;
using Blazor.Song.Net.Client.Wrap;
using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Blazor.Song.Net.Client.Components
{
    public partial class PlayerInfo : ComponentBase
    {
        [Inject]
        public IAudioService AudioService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        public double TimeStatus { get; set; }

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

        public void Refresh(double timeStatus)
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