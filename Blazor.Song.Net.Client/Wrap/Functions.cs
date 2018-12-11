using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Wrap
{
    public static class Functions
    {
        private static Action _action;

        [JSInvokable]
        public static void ExecuteTimeoutFunc()
        {
            _action?.Invoke();
        }

        public static void SetTimeout(Action action, int time)
        {
            _action = action;
            JSRuntime.Current.InvokeAsync<bool>("functions.setTimeout", time);
        }
    }
}
