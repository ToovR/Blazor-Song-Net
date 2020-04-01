using Blazor.Song.Net.Client.Wrap;
using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Shared
{
    public class PlayerComponent : ComponentBase
    {
        protected PlayerAudio playerAudio;
        protected PlayerInfo playerInfo;

        [Inject]
        protected Services.IDataManager Data { get; set; }


        protected bool IsPlaying { get; set; }

        [CascadingParameter]
        ObservableList<TrackInfo> PlaylistTracks { get; set; }

        int TimeStatus { get; set; }
        public Document Document { get; private set; }
        public Functions Functions { get; private set; }
        [Inject]
        public IJSRuntime JsRuntime { get; private set; }

        protected override void OnInitialized()
        {
            Document = new Document(JsRuntime);
            Functions = new Functions(JsRuntime);
            Functions.SetTimeout(RefreshTimeStatus, 900);
            if (Data.CurrentTrack != null)
                UpdateTitle(Data.CurrentTrack);
            Data.CurrentTrackChanged += CurrentTrackChanged;
            base.OnInitialized();
        }


        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
        }
    

        public void SetCurrentTrackNext()
        {
            if (PlaylistTracks.Count <= 1)
                return;
            Data.CurrentTrack = PlaylistTracks[(PlaylistTracks.IndexOf(Data.CurrentTrack) + 1) % PlaylistTracks.Count];
        }

        private void CurrentTrackChanged(TrackInfo info)
        {
            UpdateTitle(info);
        }

        private void UpdateTitle(TrackInfo info)
        {
            if (info != null)
                Document.UpdateTitle(info.Title + ", " + info.Artist + " - Blazor Song.Net");
            else
                Document.UpdateTitle("Blazor Song.Net");
        }

        protected void NextTrackClick()
        {
            SetCurrentTrackNext();
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

        void RefreshTimeStatus()
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

    }
}
