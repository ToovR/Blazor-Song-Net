namespace Blazor.Song.Net.Client.Interfaces
{
    public interface IAudioService
    {
        public Task AddTime(int numberOfSeconds, double total);

        public Task<int> GetBass();

        public Task<double> GetCurrentTime();

        public Task<int> GetTreble();

        public void Pause();

        public void Play(string path);

        public Task SetBass(int value);

        public Task SetOnEnded(Action onEnded);

        public Task SetTime(double newTime, double total);

        void SetTimeout(Action refreshTimeStatus, int v);

        public Task SetTreble(int value);
    }
}