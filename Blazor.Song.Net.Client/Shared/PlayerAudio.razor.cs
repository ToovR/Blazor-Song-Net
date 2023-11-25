using Blazor.Song.Net.Client.Services;
using Blazor.Song.Net.Client.Wrap;
using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Shared
{
    public partial class PlayerAudio : ComponentBase
    {
        private bool _isPlaying;

        [Inject]
        public AudioService AudioService { get; set; }

        [Parameter]
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                if (_isPlaying == value)
                    return;
                _isPlaying = value;
                IsPlayingChanged?.Invoke(value);
                ModifyPlayPause(_isPlaying);
            }
        }

        [Parameter]
        public Action<bool> IsPlayingChanged { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        protected Services.IDataManager Data { get; set; }

        [CascadingParameter]
        protected ObservableList<TrackInfo> PlaylistTracks { get; set; }

        protected TrackInfo Track
        { get { return Data.CurrentTrack; } }

        public async Task AddTime(int numberOfSeconds)
        {
            await AudioService.AddTime(numberOfSeconds, Track.Duration.TotalSeconds);
        }

        public async Task<double> GetCurrentTime()
        {
            return await AudioService.GetCurrentTime();
        }

        public async Task SetTime(double newTime)
        {
            await AudioService.SetTime(newTime, Track.Duration.TotalSeconds);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
        }

        protected override async Task OnInitializedAsync()
        {
            Data.CurrentTrackChanged += CurrentTrackChanged;
            AudioElement.OnEnded += OnEnded;
            await base.OnInitializedAsync();
        }

        private async Task ChangeTrack()
        {
            this.StateHasChanged();
            if (Track == null)
            {
                return;
            }
            if (IsPlaying)
            {
                AudioService.Play($"{NavigationManager.BaseUri}/{Track.Path}");
            }
        }

        private async Task CurrentTrackChanged(TrackInfo track)
        {
            await ChangeTrack();
        }

        private void ModifyPlayPause(bool isPlaying)
        {
            if (isPlaying)
            {
                AudioService.Play($"{NavigationManager.BaseUri}/{Track.Path}");
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
    }
}