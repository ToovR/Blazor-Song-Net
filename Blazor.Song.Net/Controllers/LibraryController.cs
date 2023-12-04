using Blazor.Song.Net.Services;
using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Blazor.Song.Net.Controllers
{
    [Route("api/[controller]")]
    public class LibraryController : ControllerBase
    {
        private readonly ILibraryStore _libraryStore;

        public LibraryController(ILibraryStore libraryStore)
        {
            _libraryStore = libraryStore;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> Download(string path)
        {
            byte[] file = await _libraryStore.Download(path);
            return File(file, "audio/mpeg");
        }

        [HttpGet("Playlist")]
        public async Task<string?> GetPlaylist()
        {
            return await _libraryStore.LoadPlaylist();
        }

        [HttpPost("Playlist")]
        public async Task PostPlaylist([FromBody] string idList)
        {
            await _libraryStore.SavePlaylist(idList);
        }

        [HttpGet("[action]")]
        public IEnumerable<TrackInfo> Tracks(string filter)
        {
            try
            {
                TrackInfo[] filteredTracks = _libraryStore.GetTracks(filter);
                return filteredTracks;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}