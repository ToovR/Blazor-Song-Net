using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Wrap
{
    public class AudioElement : Element
    {
        public AudioElement(string id, IJSRuntime jsRuntime) : base(id, jsRuntime)
        {
            JsRuntime.InvokeAsync<bool>("audioElement.onended", _id);
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
            double currentTime = await JsRuntime.InvokeAsync<double>("audioElement.get_currentTime", _id);
            return currentTime;
        }

        public async Task Load()
        {
            await JsRuntime.InvokeAsync<bool>("audioElement.load", _id);
        }

        public void Pause()
        {
            JsRuntime.InvokeAsync<bool>("audioElement.pause", _id);
        }

        public void Play()
        {
            JsRuntime.InvokeAsync<bool>("audioElement.play", _id);
        }

        public async Task SetCurrentTime(double value)
        {
            await JsRuntime.InvokeAsync<bool>("audioElement.set_currentTime", _id, value);
        }
    }
}