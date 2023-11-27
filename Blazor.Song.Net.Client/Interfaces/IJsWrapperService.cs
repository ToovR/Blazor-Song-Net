using Blazor.Song.Net.Client.Wrap;

namespace Blazor.Song.Net.Client.Interfaces
{
    public interface IJsWrapperService
    {
        Cookie GetSavedPlaylist();
        Task SavePlaylist(string idList);
    }
}
