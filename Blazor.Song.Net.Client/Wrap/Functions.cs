using Microsoft.JSInterop;
using System;

namespace Blazor.Song.Net.Client.Wrap
{
    public class Functions
    {
        public Functions(IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
        }

        private static Action _action;
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