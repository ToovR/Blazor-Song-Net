using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Wrap
{
    public class AudioElement : Element
    {

        public delegate void OnEndedDelegate();
        public static event OnEndedDelegate OnEnded;

        public AudioElement(string id) : base(id)
        {
            JSRuntime.Current.InvokeAsync<bool>("audioElement.onended", _id);
        }

        [JSInvokable]
        public static void  AudioEnded()
        {
            OnEnded?.Invoke();
        }

        public async Task<int> GetCurrentTime()
        {
            return await JSRuntime.Current.InvokeAsync<int>("audioElement.get_currentTime", _id);
        }

        public void Load()
        {
            JSRuntime.Current.InvokeAsync<bool>("audioElement.load", _id);
        }

        public void Pause()
        {
            JSRuntime.Current.InvokeAsync<bool>("audioElement.pause", _id);
        }

        public void Play()
        {
            JSRuntime.Current.InvokeAsync<bool>("audioElement.play", _id);
        }

        public async Task SetCurrentTime(int value)
        {
            await JSRuntime.Current.InvokeAsync<bool>("audioElement.set_currentTime", _id, value);
        }

    }
}


