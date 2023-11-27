using Blazor.Song.Net.Client.Interfaces;
using Blazor.Song.Net.Client.Wrap;

namespace Blazor.Song.Net.Services
{
    public class JsWrapperService : IJsWrapperService
    {
        public Cookie GetSavedPlaylist()
        {
            return null;
        }

        public async Task SavePlaylist(string idList)
        {
        }
    }
}