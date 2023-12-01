using Blazor.Song.Net.Client.Interfaces;
using Blazor.Song.Net.Client.Wrap;
using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazor.Song.Net.Client.Shared
{
    public partial class Player : ComponentBase
    {
        protected PlayerInfo playerInfo;
        private bool _isPlaying;

        [Inject]
        public IAudioService AudioService { get; set; }

        public Document Document { get; private set; }

        [Inject]
        public IJSRuntime JsRuntime { get; private set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        protected IDataManager Data { get; set; }

        protected bool IsPlaying
        {
            get
            { return _isPlaying; }
            set
            {
                if (_isPlaying == value)
                    return;
                _isPlaying = value;
                ModifyPlayPause(_isPlaying);
            }
        }

        [CascadingParameter]
        private ObservableList<TrackInfo> PlaylistTracks { get; set; }

        private double TimeStatus { get; set; }

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
            AudioService.SetTimeout(RefreshTimeStatus, 900);
            if (Data.CurrentTrack != null)
                await UpdateTitle(Data.CurrentTrack);
            Data.CurrentTrackChanged += CurrentTrackChanged;
            await AudioService.SetOnEnded(OnEnded);
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

        private async Task ChangeTrack()
        {
            this.StateHasChanged();
            if (Data.CurrentTrack == null)
            {
                return;
            }
            if (IsPlaying)
            {
                if (Data.CurrentTrack.Path.StartsWith("http"))
                {
                    AudioService.Play(Data.CurrentTrack.Path);
                }
                else
                {
                    AudioService.Play($"{NavigationManager.BaseUri}/{Data.CurrentTrack.Path}");
                }
            }
        }

        private async Task CurrentTrackChanged(TrackInfo info)
        {
            await ChangeTrack();
            await UpdateTitle(info);
        }

        private void ModifyPlayPause(bool isPlaying)
        {
            if (isPlaying)
            {
                if (Data.CurrentTrack.Path.StartsWith("http"))
                {
                    AudioService.Play(Data.CurrentTrack.Path);
                }
                else
                {
                    AudioService.Play($"{NavigationManager.BaseUri}/{Data.CurrentTrack.Path}");
                }
            }
            else
            {
                AudioService.Pause();
            }
        }

        private void OnEnded()
        {
            if (PlaylistTracks.Count <= 1)
                return;
            Data.CurrentTrack = PlaylistTracks[(PlaylistTracks.IndexOf(Data.CurrentTrack) + 1) % PlaylistTracks.Count];
        }

        private void RefreshTimeStatus()
        {
            if (Data.CurrentTrack != null && Data.CurrentTrack.Duration.TotalSeconds != 0)
            {
                AudioService.GetCurrentTime().ContinueWith((res) =>
                {
                    TimeStatus = (100 * res.Result / Data.CurrentTrack.Duration.TotalSeconds);
                    playerInfo.Refresh(TimeStatus);
                });
            }

            AudioService.SetTimeout(RefreshTimeStatus, 900);
        }

        private async Task UpdateTitle(TrackInfo info)
        {
            if (info != null)
                await Document.UpdateTitle($"{info.Title}, {info.Artist} - song.net");
            else
                await Document.UpdateTitle("song.net");
        }
    }
}