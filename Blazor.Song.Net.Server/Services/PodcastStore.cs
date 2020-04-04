using Blazor.Song.Indexer;
using Blazor.Song.Net.Server.Helpers;
using Blazor.Song.Net.Shared;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        public PodcastStore(IMemoryCache memoryCache)
        {
            // Load File
            LoadChannels();
            LoadEpisodeInfo();
            _memoryCache = memoryCache;
        }

        private const string _channelListFile = "./podcasts/channelList.json";
        private const string _episodeListFile = "./podcasts/downloadedEpisodes.json";
        private const string _podcastFolder = "./podcasts/";
        private static object _locker = new object();
        private List<PodcastChannel> _channels;
        private List<TrackInfo> _episodesInfo;
        private IMemoryCache _memoryCache;

        public static XmlReader GenerateXmlReaderFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
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
                fs = new FileStream(_channelListFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
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

        public async Task<string> GetChannelEpisodeFile(int collectionId, string link, long id)
        {
            var feedDirectory = Path.Combine(_podcastFolder, collectionId.ToString());
            if (!Uri.TryCreate(link, UriKind.Absolute, out Uri uriResult))
                return Path.Join(".", Path.Join(link.Split(Path.AltDirectorySeparatorChar).Skip(1).ToArray()));
            string urlFileName = uriResult.Segments.Last();

            if (Directory.Exists(feedDirectory) &&
                Directory.GetFiles(feedDirectory, $"*{id}_{urlFileName}").Any())
                return Path.Combine(".", Directory.GetFiles(feedDirectory, $"*{id}_{urlFileName}").First());
            try
            {
                if (!Directory.Exists(feedDirectory))
                    Directory.CreateDirectory(feedDirectory);
                using var client = new WebClient();
                string path = Path.Combine(feedDirectory, $"{id}_{urlFileName}");
                await client.DownloadFileTaskAsync(link, path);

                var episodeInfo = TrackParser.GetTrackInfo(path, urlFileName.GetHashCode(), new Uri(Directory.GetCurrentDirectory()));
                Feed feed = GetFeed(collectionId);

                episodeInfo.Id = id;
                FeedItem item = feed.Items.Single(item => item.Id == id);
                episodeInfo.Title = item.Title;
                episodeInfo.CollectionId = collectionId;
                episodeInfo.Id = id;
                episodeInfo.Path = item.Uri;
                _episodesInfo.Add(episodeInfo);
                SaveEpisodeInfo();

                return path;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Feed GetChannelEpisodes(long collectionId)
        {
            try
            {
                Feed feed = GetFeed(collectionId);
                if (feed == null)
                    return null;
                feed.Items.ToList().ForEach(i =>
                {
                    var episodeInfo = _episodesInfo.FirstOrDefault(e => e.Title == i.Title);
                    if (episodeInfo != null)
                        i.Duration = episodeInfo.Duration;
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

        public async Task<PodcastChannelResponse> GetNewChannels(string filter)
        {
            using var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync($"https://itunes.apple.com/search?term={HttpUtility.UrlEncode(filter)}&media=podcast&country=fr");
            //var json = System.IO.File.ReadAllText("./podMock.json");
            var channels = JsonSerializer.Deserialize<PodcastChannelResponse>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            return channels;
        }

        public IEnumerable<TrackInfo> GetTracks(IEnumerable<long> ids)
        {
            return _episodesInfo.Where(episodeInfo => ids.Contains(episodeInfo.Id));
        }

        private Feed GetFeed(long collectionId)
        {
            var channel = _channels.SingleOrDefault(c => c.CollectionId == collectionId);
            if (channel == null)
                return null;
            string feedContent = null;
            if (!_memoryCache.TryGetValue(channel.FeedUrl, out feedContent))
            {
                using (WebClient client = new WebClient())
                {
                    feedContent = client.DownloadString(channel.FeedUrl);
                    _memoryCache.Set(channel.FeedUrl, feedContent);
                }
            }
            SyndicationFeed sfeed = SyndicationFeed.Load(GenerateXmlReaderFromString(feedContent));
            return sfeed.ToFeed();
        }

        private void LoadChannels()
        {
            if (!Directory.Exists(_podcastFolder))
                Directory.CreateDirectory(_podcastFolder);
            try
            {
                _channels = JsonSerializer.Deserialize<List<PodcastChannel>>(File.ReadAllText(_channelListFile));
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
                _episodesInfo = JsonSerializer.Deserialize<List<TrackInfo>>(File.ReadAllText(_episodeListFile));
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
                lock (_locker)
                {
                    string episodeFileContent = JsonSerializer.Serialize<List<TrackInfo>>(_episodesInfo);
                    File.Delete(_episodeListFile);
                    File.WriteAllText(_episodeListFile, episodeFileContent);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}