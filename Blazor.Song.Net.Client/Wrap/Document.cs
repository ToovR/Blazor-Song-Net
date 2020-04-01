using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Wrap
{
    public class Document
    {
        public Document(IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
        }

        public IJSRuntime JsRuntime { get; }

        public void UpdateTitle(string newValue)
        {
            JsRuntime.InvokeAsync<bool>("document.updateTitle", newValue);
        }

    }
}
