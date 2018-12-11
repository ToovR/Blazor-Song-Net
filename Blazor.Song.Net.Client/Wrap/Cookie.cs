using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Wrap
{
    public class Cookie
    {
        private string _keyName;

        public Cookie(string keyName)
        {
            _keyName = keyName;
        }

        public async Task<string> Get()
        {
            string returnValue = await JSRuntime.Current.InvokeAsync<string>("cookie.get");
            string matchingCookieEntry = returnValue.Split(';').Where(cookieEntry => cookieEntry.Trim().StartsWith(_keyName + "=")).FirstOrDefault();
            if (matchingCookieEntry == null)
                return null;
            return matchingCookieEntry.Substring(_keyName.Length + 1);
        }

        public async Task Set(string valueName)
        {
            string cookieValue = _keyName + "=" + valueName;
            await JSRuntime.Current.InvokeAsync<bool>("cookie.set", cookieValue);
        }
    }
}
