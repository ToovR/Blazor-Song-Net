using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazor.Song.Net.Client.Pages
{
    public class SettingsComponent : ComponentBase
    {
        [Inject]
        protected Services.IDataManager Data { get; set; }

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        string _imageUrl = string.Empty;

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
    }
}
