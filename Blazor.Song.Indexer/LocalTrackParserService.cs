using Blazor.Song.Net.Shared;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Blazor.Song.Indexer
{
    public partial class LocalTrackParserService : ITrackParserService
    {
        private const string _channelListFile = "./podcasts/channelList.json";
        private const string _episodeListFile = "./podcasts/downloadedEpisodes.json";

        private const string _podcastFolder = "./podcast/";
        private static readonly string _musicDirectoryRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

        private static object _locker = new();
        private readonly string _directoryMusicRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        private readonly Regex _musicFileRegex = MusicFileRegex();
        private string _libraryFile = "./tracks.json";
        private string _playlistFile = "./playlist.txt";

        public async Task<byte[]> Download(string path)
        {
            return await ReadFile(Path.Combine(_directoryMusicRoot, path.Trim('/').Replace("/", "\\")));
        }

        public async Task<string> GetChannelEpisode(int collectionId, string link, long id)
        {
            var feedDirectory = Path.Combine(_podcastFolder, collectionId.ToString());
            if (!Uri.TryCreate(link, UriKind.Absolute, out Uri uriResult))
            {
                return Path.Join(".", Path.Join(link.Split(Path.AltDirectorySeparatorChar).Skip(1).ToArray()));
            }
            string urlFileName = uriResult.Segments.Last();

            if (Directory.Exists(feedDirectory) &&
                Directory.GetFiles(feedDirectory, $"*{id}_{urlFileName}").Any())
            {
                return Path.Combine(".", Directory.GetFiles(feedDirectory, $"*{id}_{urlFileName}").First());
            }
            if (!Directory.Exists(feedDirectory))
                Directory.CreateDirectory(feedDirectory);
            using WebClient client = new();
            string path = Path.Combine(feedDirectory, $"{id}_{urlFileName}");
            await client.DownloadFileTaskAsync(link, path);

            return path;
        }

        public string GetPodcastChannelListContent()
        {
            return File.ReadAllText(_channelListFile);
        }

        public string GetPodcastDownloadedEpisodesContent()
        {
            return File.ReadAllText(_episodeListFile);
        }

        public string GetTrackContent()
        {
            return File.ReadAllText(_libraryFile);
        }

        public TrackInfo GetTrackInfo(string musicFilePath, int index, Uri folderRoot = null)
        {
            if (folderRoot == null)
            {
                folderRoot = new Uri(Directory.GetCurrentDirectory());
            }
            FileInfo musicFileInfo = new(musicFilePath);
            TagLib.File tagMusicFile = TagLib.File.Create(new TagMusicFile(musicFileInfo.FullName));

            string artist = tagMusicFile.Tag.FirstAlbumArtist ?? tagMusicFile.Tag.AlbumArtistsSort.FirstOrDefault() ?? ((TagLib.NonContainer.File)tagMusicFile).Tag.Performers.FirstOrDefault();
            string title = !string.IsNullOrEmpty(tagMusicFile.Tag.Title) ? tagMusicFile.Tag.Title : Path.GetFileNameWithoutExtension(musicFileInfo.FullName);
            return new TrackInfo
            {
                Album = tagMusicFile.Tag.Album,
                Artist = artist,
                Duration = tagMusicFile.Properties.Duration,
                Id = index,
                Name = musicFileInfo.Name,
                Path = Uri.UnescapeDataString(folderRoot.MakeRelativeUri(new Uri(musicFileInfo.FullName)).ToString().Replace("Music/", "")),
                Title = title,
            };
        }

        public bool IsLibraryFileExists()
        {
            return File.Exists(_libraryFile);
        }

        public async Task<string> LoadPlaylist()
        {
            try
            {
                return await System.IO.File.ReadAllTextAsync(_playlistFile);
            }
            catch
            {
                return null;
            }
        }

        public Task SavePlaylist(string idList)
        {
            UpdateFile(_playlistFile, idList);
            return Task.CompletedTask;
        }

        public void UpdateChannelFile(string content)
        {
            UpdateFile(_channelListFile, content);
        }

        public void UpdateEpisodeFile(string episodeFileContent)
        {
            UpdateFile(_episodeListFile, episodeFileContent);
        }

        public void UpdateFile(string filename, string fileContent)
        {
            lock (_locker)
            {
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }
                File.WriteAllText(filename, fileContent);
            }
        }

        public void UpdateTrackData()
        {
            string fileContent = GetTrackData();
            File.WriteAllText(_libraryFile, fileContent);
        }

        [GeneratedRegex(".*\\.(mp3|ogg|flac)$", RegexOptions.IgnoreCase, "fr-FR")]
        private static partial Regex MusicFileRegex();

        private string GetTrackData()
        {
            int counter = 0;
            Uri folderRoot = new(_musicDirectoryRoot);

            var trackEnum = Directory.GetFiles(_musicDirectoryRoot, "*.*", SearchOption.AllDirectories)
                .Where(file => _musicFileRegex.IsMatch(file));
            int numberOfTracks = trackEnum.Count();

            TrackInfo[] allTracks = trackEnum.AsParallel()
                    .Select((musicFilePath, index) =>
                    {
                        counter++;
                        Console.WriteLine($"progess - {counter * 100 / numberOfTracks}%");
                        return GetTrackInfo(musicFilePath, index, folderRoot);
                    }).ToArray();
            return JsonSerializer.Serialize(allTracks);
        }

        private async Task<byte[]> ReadFile(string path)
        {
            using FileStream filestream = new(path, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[filestream.Length];
            await filestream.ReadAsync(buffer.AsMemory(0, (int)filestream.Length));
            return buffer;
        }
    }
}