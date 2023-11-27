using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Wrap
{
    public class AudioElement
    {
        private readonly IJSRuntime _jsRuntime;

        public AudioElement(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public delegate void OnEndedDelegate();

        public static event OnEndedDelegate OnEnded;

        [JSInvokable]
        public static void AudioEnded()
        {
            OnEnded?.Invoke();
        }

        public async Task<double> GetCurrentTime()
        {
            double currentTime = await _jsRuntime.InvokeAsync<double>("audioElement.get_currentTime");
            return currentTime;
        }

        public void Pause()
        {
            _jsRuntime.InvokeAsync<bool>("audioElement.pause");
        }

        public void Play(string trackPath)
        {
            _jsRuntime.InvokeAsync<bool>("audioElement.play", trackPath);
        }

        public void SetBass(int value)
        {
            _jsRuntime.InvokeAsync<bool>("audioElement.set_bass", value);
        }

        public async Task SetCurrentTime(double value)
        {
            await _jsRuntime.InvokeAsync<bool>("audioElement.set_currentTime", value);
        }

        public void SetTreble(int value)
        {
            _jsRuntime.InvokeAsync<bool>("audioElement.set_treble", value);
        }

        internal async Task<int> GetBass()
        {
            return await _jsRuntime.InvokeAsync<int>("audioElement.get_bass");
        }

        internal async Task<int> GetTreble()
        {
            return await _jsRuntime.InvokeAsync<int>("audioElement.get_treble");
        }
    }
}