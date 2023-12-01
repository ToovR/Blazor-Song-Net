using Microsoft.JSInterop;

namespace Blazor.Song.Net.Client.Wrap
{
    public class Document
    {
        public Document(IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
        }

        public IJSRuntime JsRuntime { get; }

        public async Task UpdateTitle(string newValue)
        {
            await JsRuntime.InvokeVoidAsync("document.updateTitle", newValue);
        }
    }
}