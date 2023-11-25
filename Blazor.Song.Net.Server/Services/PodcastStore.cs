using Blazor.Song.Indexer;
using Blazor.Song.Net.Server.Helpers;
using Blazor.Song.Net.Shared;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Blazor.Song.Net.Server.Services
{
    public class PodcastStore : IPodcastStore
    {
        private const string _podcastFolder = "./podcasts/";
        private readonly ITrackParserService _trackParserService;
        private List<PodcastChannel> _channels;
        private List<TrackInfo> _episodesInfo;
        private IMemoryCache _memoryCache;

        public PodcastStore(IMemoryCache memoryCache, ITrackParserService trackParserService)
        {
            _trackParserService = trackParserService;
            // Load File
            _memoryCache = memoryCache;
            LoadChannels();
            LoadEpisodeInfo();
        }

        public static XmlReader GenerateXmlReaderFromString(string content)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;
            return XmlReader.Create(stream);
        }

        public async Task AddNewChannel(PodcastChannel podcast)
        {
            _channels.Add(podcast);
            FileStream fs = null;
            try
            {
                _trackParserService.UpdateChannelFile(JsonSerializer.Serialize(_channels));
                await JsonSerializer.SerializeAsync(fs, _channels);
            }
            catch (Exception)
            {
                _channels = new List<PodcastChannel>();
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        public async Task<byte[]> GetChannelEpisodeFile(int collectionId, string link, long id)
        {
            string path = await _trackParserService.GetChannelEpisode(collectionId, link, id);
            Feed feed = await GetFeed(collectionId);
            TrackInfo episodeInfo = _trackParserService.GetTrackInfo(path, path.GetHashCode());

            episodeInfo.Id = id;
            FeedItem item = feed.Items.Single(item => item.Id == id);
            episodeInfo.Title = item.Title;
            episodeInfo.CollectionId = collectionId;
            episodeInfo.Id = id;
            episodeInfo.Path = item.Uri;
            episodeInfo.DownloadPath = path;
            SaveEpisodeInfo();

            if (episodeInfo.Id != 0)
            {
                _episodesInfo.Add(episodeInfo);
            }

            return await _trackParserService.Download(episodeInfo.DownloadPath);
        }

        public async Task<Feed> GetChannelEpisodes(long collectionId)
        {
            try
            {
                Feed feed = await GetFeed(collectionId);
                if (feed == null)
                    return null;
                feed.Items.ToList().ForEach(i =>
                {
                    var episodeInfo = _episodesInfo.FirstOrDefault(e => e.Title == i.Title);
                    if (episodeInfo != null)
                    {
                        i.Duration = episodeInfo.Duration;
                    }
                });

                return feed;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PodcastChannel[] GetChannels(string filter)
        {
            if (filter == null)
                return _channels.ToArray();
            return _channels.Where(c => c.CollectionName.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToArray();
        }

        public async Task<Feed> GetFeed(long collectionId)
        {
            PodcastChannel channel = _channels.SingleOrDefault(c => c.CollectionId == collectionId);
            if (channel == null)
                return null;
            if (!_memoryCache.TryGetValue(channel.FeedUrl, out string feedContent))
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    Uri uri = new Uri(channel.FeedUrl);
                    var response = await httpClient.GetAsync(uri);
                    feedContent = await response.Content.ReadAsStringAsync();
                    SyndicationFeed sfeed = SyndicationFeed.Load(GenerateXmlReaderFromString(feedContent));
                    Feed podcastFeed = sfeed.ToFeed();
                    _memoryCache.Set(channel.FeedUrl, JsonSerializer.Serialize(podcastFeed));
                    return podcastFeed;
                }
            }
            return JsonSerializer.Deserialize<Feed>(feedContent);
        }

        public async Task<PodcastChannelResponse> GetNewChannels(string filter)
        {
            using var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync($"https://itunes.apple.com/search?term={HttpUtility.UrlEncode(filter)}&media=podcast&country=fr");
            var channels = JsonSerializer.Deserialize<PodcastChannelResponse>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            return channels;
        }

        public IEnumerable<TrackInfo> GetTracks(IEnumerable<long> ids)
        {
            return _episodesInfo.Where(episodeInfo => ids.Contains(episodeInfo.Id));
        }

        private void LoadChannels()
        {
            if (!Directory.Exists(_podcastFolder))
                Directory.CreateDirectory(_podcastFolder);
            try
            {
                _channels = JsonSerializer.Deserialize<List<PodcastChannel>>(_trackParserService.GetPodcastChannelListContent());
            }
            catch (Exception)
            {
                _channels = new List<PodcastChannel>();
            }
        }

        private void LoadEpisodeInfo()
        {
            try
            {
                _episodesInfo = JsonSerializer.Deserialize<List<TrackInfo>>(_trackParserService.GetPodcastDownloadedEpisodesContent());
            }
            catch (Exception)
            {
                _episodesInfo = new List<TrackInfo>();
            }
        }

        private void SaveEpisodeInfo()
        {
            try
            {
                string episodeFileContent = JsonSerializer.Serialize<List<TrackInfo>>(_episodesInfo);
                _trackParserService.UpdateEpisodeFile(episodeFileContent);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}