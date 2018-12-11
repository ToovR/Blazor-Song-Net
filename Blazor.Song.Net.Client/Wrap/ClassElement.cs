using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Wrap
{
    public class ClassElement
    {
        protected readonly string _className;

        public ClassElement(string className)
        {
            _className = className;
        }

        public void UpdateBackgroundImage(string source)
        {
            JSRuntime.Current.InvokeAsync<bool>("classElement.updateBackgroundImage", _className, source);
        }
    }
}
