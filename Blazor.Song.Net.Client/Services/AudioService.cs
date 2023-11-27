using Blazor.Song.Net.Client.Interfaces;
using Blazor.Song.Net.Client.Wrap;
using Microsoft.JSInterop;

namespace Blazor.Song.Net.Client.Services
{
    public class AudioService : IAudioService
    {
        private AudioElement _audio;

        public AudioService(IJSRuntime jSRuntime)
        {
            _audio = new Wrap.AudioElement(jSRuntime);
        }

        public async Task AddTime(int numberOfSeconds, double total)
        {
            await SetTime(await _audio.GetCurrentTime() + numberOfSeconds, total);
        }

        public async Task<int> GetBass()
        {
            return await _audio.GetBass();
        }

        public async Task<double> GetCurrentTime()
        {
            return await _audio.GetCurrentTime();
        }

        public async Task<int> GetTreble()
        {
            return await _audio.GetTreble();
        }

        public void Pause()
        {
            _audio.Pause();
        }

        public void Play(string path)
        {
            _audio.Play(path);
        }

        public void SetBass(int value)
        {
            _audio.SetBass(value);
        }

        public async Task SetTime(double newTime, double total)
        {
            if (newTime < 0)
                newTime = 0;
            if (newTime > total)
                newTime = (int)total;
            await _audio.SetCurrentTime(newTime);
        }

        public void SetTreble(int value)
        {
            _audio.SetTreble(value);
        }
    }
}