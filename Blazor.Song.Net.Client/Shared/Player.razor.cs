using Blazor.Song.Net.Client.Wrap;
using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Shared
{
    public partial class Player : ComponentBase
    {
        protected PlayerAudio playerAudio;
        protected PlayerInfo playerInfo;

        public Document Document { get; private set; }

        public Functions Functions { get; private set; }

        [Inject]
        public IJSRuntime JsRuntime { get; private set; }

        [Inject]
        protected Services.IDataManager Data { get; set; }

        protected bool IsPlaying { get; set; }

        [CascadingParameter]
        private ObservableList<TrackInfo> PlaylistTracks { get; set; }

        private int TimeStatus { get; set; }

        public void SetCurrentTrackNext()
        {
            if (PlaylistTracks.Count <= 1)
                return;
            Data.CurrentTrack = PlaylistTracks[(PlaylistTracks.IndexOf(Data.CurrentTrack) + 1) % PlaylistTracks.Count];
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
        }

        protected void NextTrackClick()
        {
            SetCurrentTrackNext();
        }

        protected override async Task OnInitializedAsync()
        {
            Document = new Document(JsRuntime);
            Functions = new Functions(JsRuntime);
            Functions.SetTimeout(RefreshTimeStatus, 900);
            if (Data.CurrentTrack != null)
                await UpdateTitle(Data.CurrentTrack);
            Data.CurrentTrackChanged += CurrentTrackChanged;
            await base.OnInitializedAsync();
        }

        protected void PreviousTrackClick()
        {
            if (PlaylistTracks.Count <= 1)
                return;
            if (PlaylistTracks.IndexOf(Data.CurrentTrack) == 0)
                Data.CurrentTrack = PlaylistTracks[PlaylistTracks.Count - 1];
            else
                Data.CurrentTrack = PlaylistTracks[(PlaylistTracks.ToList().IndexOf(Data.CurrentTrack) - 1) % PlaylistTracks.Count];
        }

        private async Task CurrentTrackChanged(TrackInfo info)
        {
            await UpdateTitle(info);
        }

        private void RefreshTimeStatus()
        {
            if (playerAudio != null && Data.CurrentTrack != null && Data.CurrentTrack.Duration.TotalSeconds != 0)
            {
                playerAudio.GetCurrentTime().ContinueWith((res) =>
                {
                    TimeStatus = (int)(100 * res.Result / Data.CurrentTrack.Duration.TotalSeconds);
                    playerInfo.Refresh(TimeStatus);
                });
            }
            Functions.SetTimeout(RefreshTimeStatus, 900);
        }

        private async Task UpdateTitle(TrackInfo info)
        {
            if (info != null)
                await Document.UpdateTitle($"{info.Title}, {info.Artist} - Blazor Song.Net");
            else
                await Document.UpdateTitle("Blazor Song.Net");
        }
    }
}