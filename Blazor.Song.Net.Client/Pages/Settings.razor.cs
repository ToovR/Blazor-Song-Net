using Blazor.Song.Net.Client.Interfaces;
using Blazor.Song.Net.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Pages
{
    public partial class Settings : ComponentBase
    {
        private int _currentBass;
        private int _currentTreble;
        private string _imageUrl = string.Empty;
        private int _currentBalance;

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

        [Inject]
        protected IDataManager Data { get; set; }

        protected string ImageUrl
        {
            get { return _imageUrl; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;
                if (_imageUrl == value)
                    return;
                _imageUrl = value;
                Wrap.ClassElement mainClassElement = new Wrap.ClassElement("main", JSRuntime);
                mainClassElement.UpdateBackgroundImage(_imageUrl);
            }
        }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _currentBass = await AudioService.GetBass();
            _currentTreble = await AudioService.GetTreble();
            _currentBalance = (int)(await AudioService.GetBalance() * 50);
            await base.OnInitializedAsync();
        }
    }
}