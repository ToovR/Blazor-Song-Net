using Blazor.Song.Net.Server.Services;
using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Server.Controllers
{
    [Route("api/[controller]")]
    public class PodcastController : ControllerBase
    {
        private readonly IPodcastStore _podcastStore;

        public PodcastController(IPodcastStore podcastStore)
        {
            _podcastStore = podcastStore;
        }

        [HttpGet("[action]")]
        public PodcastChannel[] Channels(string filter)
        {
            PodcastChannel[] channels = _podcastStore.GetChannels(filter);
            return channels;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> GetChannelEpisode(int collectionId, string link, long id)
        {
            byte[] file = await _podcastStore.GetChannelEpisodeFile(collectionId, link, id);
            return File(file, "audio/mpeg");
        }

        [HttpGet("[action]")]
        public async Task<Feed> GetChannelEpisodes(Int64 collectionId)
        {
            Feed feed = await _podcastStore.GetChannelEpisodes(collectionId);
            return feed;
        }

        [HttpGet("[action]")]
        public async Task<PodcastChannelResponse> NewChannels(string filter)
        {
            PodcastChannelResponse channels = await _podcastStore.GetNewChannels(filter);
            return channels;
        }

        [HttpPost("[action]")]
        public async Task NewChannels([FromBody] PodcastChannel podcast)
        {
            await _podcastStore.AddNewChannel(podcast);
        }
    }
}