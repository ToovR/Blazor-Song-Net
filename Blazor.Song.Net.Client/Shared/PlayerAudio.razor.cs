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
    public class PlayerAudioComponent : ComponentBase
    {
        bool _isPlaying;

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        AudioElement _audio = null;
        [Inject]
        protected Services.IDataManager Data { get; set; }

        [Parameter]
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                if (_isPlaying == value)
                    return;
                _isPlaying = value;
                if (IsPlayingChanged != null)
                    IsPlayingChanged(value);
                ModifyPlayPause(_isPlaying);
            }
        }

        [Parameter]
        public Action<bool> IsPlayingChanged { get; set; }

        [CascadingParameter]
        protected ObservableList<TrackInfo> PlaylistTracks { get; set; }

        protected TrackInfo Track { get { return Data.CurrentTrack; } }

        protected override async Task OnInitializedAsync()
        {
            _audio = new AudioElement("playerAudio", JsRuntime);
            Data.CurrentTrackChanged += CurrentTrackChanged;
            AudioElement.OnEnded += OnEnded;
            await base.OnInitializedAsync();
        }


        private void OnEnded()
        {
            if (PlaylistTracks.Count <= 1)
                return;
            Data.CurrentTrack = PlaylistTracks[(PlaylistTracks.IndexOf(Data.CurrentTrack) + 1) % PlaylistTracks.Count];
        }

        private void ChangeTrack()
        {
            this.StateHasChanged();
            if (Track == null)
                return;
            _audio.Load();
            if (IsPlaying)
                _audio.Play();
        }

        private void CurrentTrackChanged(TrackInfo track)
        {
            ChangeTrack();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            _audio = new AudioElement("playerAudio", JsRuntime);
        }

        public async Task<double> GetCurrentTime()
        {
            return await _audio.GetCurrentTime();
        }

        public async Task AddTime(int numberOfSeconds)
        {
            await SetTime(await _audio.GetCurrentTime() + numberOfSeconds);
        }

        public async Task SetTime(double newTime)
        {
            if (newTime < 0)
                newTime = 0;
            if (newTime > Track.Duration.TotalSeconds)
                newTime = (int)Track.Duration.TotalSeconds;
            await _audio.SetCurrentTime(newTime);
        }

        private void ModifyPlayPause(bool isPlaying)
        {
            if (isPlaying)
                _audio.Play();
            else
                _audio.Pause();
        }


    }
}
