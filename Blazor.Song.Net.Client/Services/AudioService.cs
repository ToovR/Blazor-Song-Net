using Blazor.Song.Net.Client.Wrap;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Services
{
    public class AudioService
    {
        private AudioElement _audio;

        public AudioService(IJSRuntime jSRuntime)
        {
            _audio = new Wrap.AudioElement(jSRuntime);
        }

        public async Task SetTime(double newTime, double total)
        {
            if (newTime < 0)
                newTime = 0;
            if (newTime > total)
                newTime = (int)total;
            await _audio.SetCurrentTime(newTime);
        }

        internal async Task AddTime(int numberOfSeconds, double total)
        {
            await SetTime(await _audio.GetCurrentTime() + numberOfSeconds, total);
        }

        internal async Task<int> GetBass()
        {
            return await _audio.GetBass();
        }

        internal async Task<double> GetCurrentTime()
        {
            return await _audio.GetCurrentTime();
        }

        internal async Task<int> GetTreble()
        {
            return await _audio.GetTreble();
        }

        internal void Pause()
        {
            _audio.Pause();
        }

        internal void Play(string path)
        {
            _audio.Play(path);
        }

        internal void SetBass(int value)
        {
            _audio.SetBass(value);
        }

        internal void SetTreble(int value)
        {
            _audio.SetTreble(value);
        }
    }
}