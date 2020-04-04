using Blazor.Song.Net.Shared;
using System.Collections.Generic;

namespace Blazor.Song.Net.Server.Services
{
    public interface ILibraryStore
    {
        TrackInfo[] GetTracks(string filter);

        IEnumerable<TrackInfo> GetTracks(IEnumerable<long> ids);
    }
}