using Microsoft.JSInterop;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Wrap
{
    public class Cookie
    {
        public Cookie(string keyName, IJSRuntime jSRuntime)
        {
            _keyName = keyName;
            JsRuntime = jSRuntime;
        }

        private string _keyName;

        public IJSRuntime JsRuntime { get; }

        public async Task<string> Get()
        {
            string returnValue = await JsRuntime.InvokeAsync<string>("cookie.get");
            string matchingCookieEntry = returnValue.Split(';').Where(cookieEntry => cookieEntry.Trim().StartsWith($"{_keyName}=")).FirstOrDefault();
            if (matchingCookieEntry == null)
                return null;
            return matchingCookieEntry.Substring(_keyName.Length + 1);
        }

        public async Task Set(string valueName)
        {
            string cookieValue = $"{_keyName}={valueName}";
            await JsRuntime.InvokeAsync<bool>("cookie.set", cookieValue);
        }
    }
}