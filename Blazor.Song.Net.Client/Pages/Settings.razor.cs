using Blazor.Song.Net.Client.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Blazor.Song.Net.Client.Pages
{
    public partial class Settings : PageBase
    {
        private int _currentBalance;
        private int _currentBass;
        private int _currentTreble;
        private string _imageUrl = string.Empty;

        public int CurrentBalance
        {
            get { return _currentBalance; }
            set
            {
                if (_currentBalance == value)
                {
                    return;
                }
                _currentBalance = value;
                AudioService.SetBalance((double)value / 50);
            }
        }

        public int CurrentBass
        {
            get { return _currentBass; }
            set
            {
                if (_currentBass == value)
                {
                    return;
                }
                _currentBass = value;
                AudioService.SetBass(value);
            }
        }

        public int CurrentTreble
        {
            get { return _currentTreble; }
            set
            {
                if (_currentTreble == value)
                {
                    return;
                }
                _currentTreble = value;
                AudioService.SetTreble(value);
            }
        }

        [Inject]
        protected IAudioService AudioService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _currentBass = await AudioService.GetBass();
            _currentTreble = await AudioService.GetTreble();
            _currentBalance = (int)(await AudioService.GetBalance() * 50);
            await base.OnInitializedAsync();
        }
    }
}