using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Wrap
{
    public class ClassElement
    {
        IJSRuntime JsRuntime { get; set; }

        protected readonly string _className;

        public ClassElement(string className, IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
            _className = className;
        }

        public void UpdateBackgroundImage(string source)
        {
            JsRuntime.InvokeAsync<bool>("classElement.updateBackgroundImage", _className, source);
        }
    }
}
