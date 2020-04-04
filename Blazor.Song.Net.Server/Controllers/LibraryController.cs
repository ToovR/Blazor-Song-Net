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
    public class LibraryController : Controller
    {
        public LibraryController(IFileHelper fileHelper, ILibraryStore libraryStore)
        {
            _fileHelper = fileHelper;
            _libraryStore = libraryStore;
        }

        private readonly IFileHelper _fileHelper;
        private readonly ILibraryStore _libraryStore;
        private readonly string directoryRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

        [HttpGet("[action]")]
        public async Task<ActionResult> Download(string path)
        {
            byte[] file = await _fileHelper.ReadFile(Path.Combine(directoryRoot, path.Trim('/').Replace("/", "\\")));
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