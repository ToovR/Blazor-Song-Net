using Blazor.Song.Net.Client.Interfaces;
using Blazor.Song.Net.Client.Wrap;
using Microsoft.JSInterop;

namespace Blazor.Song.Net.Client.Services
{
    public class JsWrapperService : IJsWrapperService
    {
        private readonly IJSRuntime _jsRuntime;

        public JsWrapperService(IJSRuntime JsRuntime) 
        {
            _jsRuntime = JsRuntime;
        }

        public Cookie GetSavedPlaylist()
        {
            return new Wrap.Cookie("playlist", _jsRuntime);
        }

        public async Task SavePlaylist(string idList)
        {
            Wrap.Cookie playlistCookie = new Wrap.Cookie("playlist", _jsRuntime);
            await playlistCookie.Set(idList);
        }
    }
}
