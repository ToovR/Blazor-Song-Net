using Blazor.Song.Net.Client.Interfaces;

namespace Blazor.Song.Net.Services
{
    public class AudioService : IAudioService
    {
        public async Task AddTime(int numberOfSeconds, double total)
        {
        }

        public async Task<int> GetBass()
        {
            return 0;
        }

        public async Task<double> GetCurrentTime()
        {
            return 0;
        }

        public async Task<int> GetTreble()
        {
            return 0;
        }

        public void Pause()
        {
        }

        public void Play(string path)
        {
        }

        public void SetBass(int value)
        {
        }

        public async Task SetTime(double newTime, double total)
        {
        }

        public void SetTreble(int value)
        {
            throw new NotImplementedException();
        }
    }
}