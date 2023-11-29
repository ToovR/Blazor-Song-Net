using Blazor.Song.Net.Client.Interfaces;

namespace Blazor.Song.Net.Services
{
    public class ServerAudioService : IAudioService
    {
        public async Task AddTime(int numberOfSeconds, double total)
        {
        }

        public Task<int> GetBass()
        {
            return Task.FromResult(0);
        }

        public Task<double> GetCurrentTime()
        {
            return Task.FromResult(0.0d);
        }

        public Task<int> GetTreble()
        {
            return Task.FromResult(0);
        }

        public void Pause()
        {
        }

        public void Play(string path)
        {
        }

        public Task SetBass(int value)
        {
            return Task.CompletedTask;
        }

        public Task SetOnEnded(Action onEnded)
        {
            return Task.CompletedTask;
        }

        public Task SetTime(double newTime, double total)
        {
            return Task.CompletedTask;
        }

        public void SetTimeout(Action refreshTimeStatus, int v)
        {
        }

        Task IAudioService.SetTreble(int value)
        {
            return Task.CompletedTask;
        }
    }
}