using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Wrap
{
    public class Functions
    {
        private static Action _action;
        public Functions(IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
        }

        public IJSRuntime JsRuntime { get; }

        [JSInvokable]
        public static void ExecuteTimeoutFunc()
        {
            _action?.Invoke();
        }

        public void SetTimeout(Action action, int time)
        {
            _action = action;
            JsRuntime.InvokeAsync<bool>("functions.setTimeout", time);
        }
    }
}
