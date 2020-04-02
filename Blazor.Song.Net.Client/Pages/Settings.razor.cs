using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazor.Song.Net.Client.Pages
{
    public class SettingsComponent : ComponentBase
    {
        private string _imageUrl = string.Empty;

        [Inject]
        protected Services.IDataManager Data { get; set; }

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
    }
}