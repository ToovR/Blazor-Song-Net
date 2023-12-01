// Ignore Spelling: Podcast

using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Blazor.Song.Net.Shared;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazor.Song.Indexer
{
    public class AzureTrackParserService : ITrackParserService
    {
        private const string _channelListFile = "channelList.json";
        private const string _directoryRoot = "/";
        private const string _episodeListFile = "downloadedEpisodes.json";
        private const string _libraryFile = "tracks.json";
        private const string _musicsharename = "music";
        private const string _playlistFile = "playlist.txt";
        private const string _podcastsharename = "podcast";
        private const int BLOCK_SIZE = 16 * 1024;
        private readonly string _blobConnectionstring;

        public AzureTrackParserService(IConfiguration configuration)
        {
            _blobConnectionstring = configuration.GetSection("azure.blobconnectionstring").Value;
        }

        public async Task<byte[]> Download(string path)
        {
            (string sharename, string directoryname, string filename) = GetPathParts(path);
            return await GetFileByteAsync(sharename, directoryname, filename);
        }

        public async Task<string> GetChannelEpisode(int collectionId, string link, long id)
        {
            ShareClient share = GetShare(_podcastsharename);
            ShareDirectoryClient feedDirectory = share.GetDirectoryClient(collectionId.ToString());

            if (!Uri.TryCreate(link, UriKind.Absolute, out Uri uriResult))
            {
                return Path.Join(".", Path.Join(link.Split(Path.AltDirectorySeparatorChar).Skip(1).ToArray()));
            }
            string urlFileName = uriResult.Segments.Last();

            if (feedDirectory.Exists() && feedDirectory.GetFilesAndDirectories($"*{id}_{urlFileName}").Any())
            {
                return $"/{_podcastsharename}/{collectionId}/{id}_{urlFileName}";
            }
            if (!feedDirectory.Exists())
            {
                feedDirectory.Create();
            }

            try
            {
                using (Stream stream = await DownloadFromUrlToStream(link))
                {
                    ShareFileClient file = feedDirectory.GetFileClient($"{id}_{urlFileName}");
                    UploadStreamToFile(stream, file);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return $"/{_podcastsharename}/{collectionId}/{id}_{urlFileName}";
        }

        public Tuple<string, string, string> GetPathParts(string path)
        {
            string sharename;
            if (path.StartsWith("/music"))
            {
                path = path["/music".Length..];
                sharename = _musicsharename;
            }
            else if (path.StartsWith("/podcast"))
            {
                path = path["/podcast".Length..];
                sharename = _podcastsharename;
            }
            else
            {
                throw new FormatException();
            }
            int directorySeparator = path.LastIndexOf(_directoryRoot);
            string directoryname = path[..directorySeparator];
            string filename = path[(directorySeparator + 1)..];
            return new Tuple<string, string, string>(sharename, directoryname, filename);
        }

        public string GetPodcastChannelListContent()
        {
            return GetFileContent(_podcastsharename, _directoryRoot, _channelListFile);
        }

        public string GetPodcastDownloadedEpisodesContent()
        {
            return GetFileContent(_podcastsharename, _directoryRoot, _episodeListFile);
        }

        public string GetTrackContent()
        {
            return GetFileContent(_musicsharename, _directoryRoot, _libraryFile);
        }

        public string GetTrackData()
        {
            int counter = 0;

            ShareClient share = GetShare(_musicsharename);
            ShareDirectoryClient directory = share.GetDirectoryClient(_directoryRoot);
            Azure.Pageable<Azure.Storage.Files.Shares.Models.ShareFileItem> files = directory.GetFilesAndDirectories();
            IEnumerable<ShareFileClient> trackEnum = GetShareFiles(_directoryRoot, [".mp3", ".ogg", ".flac"]);
            int numberOfTracks = trackEnum.Count();
            var allTracks = trackEnum.AsParallel()
                    .Select((musicFilePath, index) =>
                    {
                        counter++;
                        return GetTrackInfo(musicFilePath, index);
                    }).ToArray();
            return JsonSerializer.Serialize(allTracks);
        }

        public TrackInfo GetTrackInfo(string musicFilePath, int index, Uri folderRoot = null)
        {
            (string sharename, string directoryname, string filename) = GetPathParts(musicFilePath);
            ShareFileClient musicFile = GetShare(sharename).GetDirectoryClient(directoryname).GetFileClient(filename);
            return GetTrackInfo(musicFile, index);
        }

        public bool IsLibraryFileExists()
        {
            ShareClient share = GetShare(_musicsharename);
            ShareDirectoryClient directory = share.GetDirectoryClient("/");
            ShareFileClient file = directory.GetFileClient(_libraryFile);
            return file.Exists();
        }

        public async Task<string> LoadPlaylist()
        {
            return await GetFileContentAsync(_musicsharename, _directoryRoot, _playlistFile);
        }

        public async Task SavePlaylist(string idList)
        {
            await UploadFileAsync(_podcastsharename, _directoryRoot, _playlistFile, idList);
        }

        public void UpdateChannelFile(string fileContent)
        {
            UploadFile(_podcastsharename, _directoryRoot, _channelListFile, fileContent);
        }

        public void UpdateEpisodeFile(string episodeFileContent)
        {
            UploadFile(_podcastsharename, _directoryRoot, _episodeListFile, episodeFileContent);
        }

        public void UpdateTrackData()
        {
            string fileContent = GetTrackData();
            UploadFile(_musicsharename, _directoryRoot, _libraryFile, fileContent);
        }

        private static void UploadStreamToFile(Stream stream, ShareFileClient file)
        {
            stream.Position = 0;

            if (file.Exists())
            {
                file.Delete();
            }
            file.Create(stream.Length);

            using (BinaryReader reader = new BinaryReader(stream))
            {
                int offset = 0;
                while (true)
                {
                    byte[] buffer = reader.ReadBytes(16 * 1024);
                    if (buffer.Length == 0)
                        break;

                    MemoryStream uploadChunk = new();
                    uploadChunk.Write(buffer, 0, buffer.Length);
                    uploadChunk.Position = 0;

                    HttpRange httpRange = new(offset, buffer.Length);
                    file.UploadRange(httpRange, uploadChunk);
                    offset += buffer.Length;
                }
            }
        }

        private static async Task UploadStreamToFileAsync(Stream stream, ShareFileClient file)
        {
            stream.Position = 0;

            if (await file.ExistsAsync())
            {
                await file.DeleteAsync();
            }
            await file.CreateAsync(stream.Length);

            using (BinaryReader reader = new(stream))
            {
                int offset = 0;
                while (true)
                {
                    byte[] buffer = reader.ReadBytes(16 * 1024);
                    if (buffer.Length == 0)
                        break;

                    MemoryStream uploadChunk = new();
                    await uploadChunk.WriteAsync(buffer);
                    uploadChunk.Position = 0;

                    HttpRange httpRange = new(offset, buffer.Length);
                    await file.UploadRangeAsync(httpRange, uploadChunk);
                    offset += buffer.Length;
                }
            }
        }

        private async Task<Stream> DownloadFromUrlToStream(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                Uri uri = new(url);
                var response = await httpClient.GetAsync(uri);
                MemoryStream ms = new();
                await response.Content.CopyToAsync(ms);
                return ms;
            }
        }

        private async Task<byte[]> GetFileByteAsync(string sharename, string directoryname, string filename)
        {
            ShareClient share = GetShare(sharename);
            ShareDirectoryClient directory = share.GetDirectoryClient(directoryname);
            ShareFileClient file = directory.GetFileClient(filename);
            byte[] buffer = new byte[BLOCK_SIZE];
            using (Stream stream = await file.OpenReadAsync())
            using (MemoryStream memoryStream = new())
            {
                int read;
                while ((read = await stream.ReadAsync(buffer)) > 0)
                {
                    await memoryStream.WriteAsync(buffer.AsMemory(0, read));
                }
                return memoryStream.ToArray();
            }
        }

        private string GetFileContent(string sharename, string directoryname, string filename)
        {
            ShareClient share = GetShare(sharename);
            ShareDirectoryClient directory = share.GetDirectoryClient(directoryname);
            ShareFileClient file = directory.GetFileClient(filename);

            using (var stream = file.OpenRead())
            using (var reader = new StreamReader(stream))
            {
                // read csv file one line by line
                while (!reader.EndOfStream)
                {
                    return reader.ReadToEnd();
                }
            }
            return null;
        }

        private async Task<string> GetFileContentAsync(string sharename, string directoryname, string filename)
        {
            ShareClient share = GetShare(sharename);
            ShareDirectoryClient directory = share.GetDirectoryClient(directoryname);
            ShareFileClient file = directory.GetFileClient(filename);

            using (var stream = await file.OpenReadAsync())
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    return reader.ReadToEnd();
                }
            }
            return null;
        }

        private ShareClient GetShare(string name)
        {
            ShareClient share = new(_blobConnectionstring, name);
            return share;
        }

        private IEnumerable<ShareFileClient> GetShareFiles(string directoryName, string[] extensions)
        {
            List<ShareFileClient> foundFiles = [];
            ShareClient share = GetShare(_musicsharename);

            ShareDirectoryClient directory = share.GetDirectoryClient(directoryName);
            var files = directory.GetFilesAndDirectories();

            foreach (ShareFileItem file in files)
            {
                if (file.IsDirectory)
                {
                    IEnumerable<ShareFileClient> childFoundFiles = GetShareFiles(Path.Combine(directoryName, file.Name), extensions);
                    foundFiles.AddRange(childFoundFiles);
                }
                else if (extensions.Any(e => file.Name.ToLower().EndsWith(e)))
                {
                    foundFiles.Add(directory.GetFileClient(file.Name));
                }
            }
            return foundFiles;
        }

        private TrackInfo GetTrackInfo(ShareFileClient musicFile, int index)
        {
            TagLib.File tagMusicFile = TagLib.File.Create(new FileAzureBlobAbstraction(musicFile));

            string artist = tagMusicFile.Tag.FirstAlbumArtist ?? tagMusicFile.Tag.AlbumArtistsSort.FirstOrDefault() ?? ((TagLib.NonContainer.File)tagMusicFile).Tag.Performers.FirstOrDefault();
            string title = !string.IsNullOrEmpty(tagMusicFile.Tag.Title) ? tagMusicFile.Tag.Title : Path.GetFileNameWithoutExtension(musicFile.Name);
            return new TrackInfo
            {
                Album = tagMusicFile.Tag.Album,
                Artist = artist,
                Duration = tagMusicFile.Properties.Duration,
                Id = index,
                Name = musicFile.Name,
                Path = musicFile.Uri.AbsolutePath,
                Title = title,
            };
        }

        private void UploadFile(string sharename, string directoryName, string fileName, string content)
        {
            ShareClient share = GetShare(sharename);
            ShareDirectoryClient directory = share.GetDirectoryClient(directoryName);
            ShareFileClient file = directory.GetFileClient(fileName);
            using (MemoryStream stream = new())
            {
                StreamWriter writer = new(stream);
                writer.Write(content);
                writer.Flush();

                UploadStreamToFile(stream, file);
            }
        }

        private async Task UploadFileAsync(string sharename, string directoryName, string fileName, string content)
        {
            ShareClient share = GetShare(sharename);
            ShareDirectoryClient directory = share.GetDirectoryClient(directoryName);
            ShareFileClient file = directory.GetFileClient(fileName);
            using (MemoryStream stream = new())
            {
                StreamWriter writer = new(stream);
                await writer.WriteAsync(content);
                await writer.FlushAsync();

                await UploadStreamToFileAsync(stream, file);
            }
        }
    }
}