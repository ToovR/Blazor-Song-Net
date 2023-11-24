using Blazor.Song.Net.Server.Services;
using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Server.Controllers
{
    [Route("api/[controller]")]
    public class LibraryController : ControllerBase
    {
        public LibraryController(ILibraryStore libraryStore)
        {
            _libraryStore = libraryStore;
        }

        private readonly ILibraryStore _libraryStore;

        [HttpGet("[action]")]
        public async Task<ActionResult> Download(string path)
        {
            byte[] file = await _libraryStore.Download(path);  
            return File(file, "audio/mpeg");
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