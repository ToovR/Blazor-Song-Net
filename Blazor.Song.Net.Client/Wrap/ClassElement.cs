using Microsoft.JSInterop;

namespace Blazor.Song.Net.Client.Wrap
{
    public class ClassElement
    {
        public ClassElement(string className, IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
            _className = className;
        }

        protected readonly string _className;
        private IJSRuntime JsRuntime { get; set; }

        public void UpdateBackgroundImage(string source)
        {
            JsRuntime.InvokeAsync<bool>("classElement.updateBackgroundImage", _className, source);
        }
    }
}