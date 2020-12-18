using Blazor.Song.Net.Server.Services;
using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Blazor.Song.Net.Server.Controllers
{
    [Route("api/[controller]")]
    public class TrackController : ControllerBase
    {
        public TrackController(ILibraryStore libraryStore, IPodcastStore podcastStore)
        {
            _libraryStore = libraryStore;
            _podcastStore = podcastStore;
        }

        private readonly ILibraryStore _libraryStore;
        private readonly IPodcastStore _podcastStore;

        [HttpGet("[action]")]
        public TrackInfo[] Tracks(string ids)
        {
            var idList = ids.Split("|", StringSplitOptions.RemoveEmptyEntries).Select(id => long.Parse(id));
            try
            {
                var songs = _libraryStore.GetTracks(idList);
                var podcastEpisodes = _podcastStore.GetTracks(idList.Except(songs.Select(song => song.Id)));
                var presentIds = idList.Where(id => songs.Any(song => song.Id == id) || podcastEpisodes.Any(episode => episode.Id == id));
                var tracks = presentIds.Select(id => songs.FirstOrDefault(song => song.Id == id) ?? podcastEpisodes.First(episode => episode.Id == id));
                return tracks.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}