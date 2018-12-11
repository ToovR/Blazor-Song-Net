using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Wrap
{
    public class Document
    {
        public static void UpdateTitle(string newValue)
        {
            JSRuntime.Current.InvokeAsync<bool>("document.updateTitle", newValue);
        }

    }
}
