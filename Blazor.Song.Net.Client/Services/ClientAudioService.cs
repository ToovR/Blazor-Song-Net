using Blazor.Song.Net.Client.Interfaces;
using Microsoft.JSInterop;
using System.Web;

namespace Blazor.Song.Net.Client.Services
{
    public class ClientAudioService : IAudioService
    {
        private readonly IJSRuntime _jsRuntime;
        private Action _timeoutAction;

        public ClientAudioService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public delegate void OnEndedDelegate();

        public static Action OnEnded { get; set; }

        public async Task AddTime(int numberOfSeconds, double total)
        {
            await SetTime(await GetCurrentTime() + numberOfSeconds, total);
        }

        [JSInvokable]
        public void AudioEnded()
        {
            OnEnded?.Invoke();
        }

        public async Task<double> GetBalance()
        {
            return await _jsRuntime.InvokeAsync<double>("audio.get_balance");
        }

        public async Task<int> GetBass()
        {
            return await _jsRuntime.InvokeAsync<int>("audio.get_bass");
        }

        public async Task<double> GetCurrentTime()
        {
            double currentTime = await _jsRuntime.InvokeAsync<double>("audio.get_currentTime");
            return currentTime;
        }

        public async Task<int> GetTreble()
        {
            return await _jsRuntime.InvokeAsync<int>("audio.get_treble");
        }

        public void Pause()
        {
            _jsRuntime.InvokeVoidAsync("audio.pause");
        }

        public void Play(string path)
        {
            _jsRuntime.InvokeVoidAsync("audio.play", $"/api/Library/Download?path={HttpUtility.UrlEncode(path.Replace("//", "/"))}");
        }

        public async Task SetBalance(double value)
        {
            await _jsRuntime.InvokeVoidAsync("audio.set_balance", value);
        }

        public async Task SetBass(int value)
        {
            await _jsRuntime.InvokeVoidAsync("audio.set_bass", value);
        }

        public async Task SetOnEnded(Action onEnded)
        {
            DotNetObjectReference<ClientAudioService>? audioRef = DotNetObjectReference.Create(this);
            await _jsRuntime.InvokeVoidAsync("audio.set_serviceRef", audioRef);
            OnEnded += onEnded;
        }

        public async Task SetTime(double newTime, double total)
        {
            if (newTime < 0)
                newTime = 0;
            if (newTime > total)
                newTime = (int)total;
            await _jsRuntime.InvokeVoidAsync("audio.set_currentTime", newTime);
        }

        public void SetTimeout(Action refreshTimeStatus, int timeout)
        {
            _timeoutAction = refreshTimeStatus;
            _jsRuntime.InvokeVoidAsync("audio.setProgressTimeout", timeout);
        }

        public async Task SetTreble(int value)
        {
            await _jsRuntime.InvokeVoidAsync("audio.set_treble", value);
        }

        [JSInvokable]
        public void TimeoutExecuteFunc()
        {
            _timeoutAction?.Invoke();
        }
    }
}