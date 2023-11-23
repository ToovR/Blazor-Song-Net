using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Blazor.Song.Net.Shared;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;

namespace Blazor.Song.Indexer
{
    public class AzureTrackParserService : ITrackParserService
    {
        string _libraryFile = "tracks.json";
        private string _blobConnectionstring;
        private const string _channelListFile = "channelList.json";
        private const string _episodeListFile = "downloadedEpisodes.json";


        public AzureTrackParserService(IConfiguration configuration) 
        {
            _blobConnectionstring = configuration.GetSection("azure.blobconnectionstring").Value;
        }
        public bool IsLibraryFileExists()
        {
            ShareClient share = GetShare("music");
            ShareDirectoryClient directory = share.GetDirectoryClient("/");
            ShareFileClient file = directory.GetFileClient(_libraryFile);
            return file.Exists();
        }

        public void UpdateTrackData()
        {
            string fileContent = GetTrackData();
            UploadFile("/", _libraryFile, fileContent);
        }

        public string GetTrackContent()
        {
            return GetFileContent("music", "/", _libraryFile);
        }

        public string GetPodcastChannelListContent()
        {
            return GetFileContent("podcast", "/", _channelListFile);
        }

        public string GetPodcastDownloadedEpisodesContent()
        {
            return GetFileContent("podcast", "/", _episodeListFile);
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

        private static IEnumerable<ShareFileClient> GetShareFiles(string directoryName, string[] extensions)
        {
            List<ShareFileClient> foundFiles = new List<ShareFileClient>();
            string connectionString = "";

            // Name of the share, directory
            string shareName = "media";

            // Get a reference to a share
            ShareClient share = new ShareClient(connectionString, shareName);

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

        public string GetTrackData()
        {

            int counter = 0;

            ShareClient share = GetShare("music");
            // Get a reference to a directory and create it
            ShareDirectoryClient directory = share.GetDirectoryClient("/");
            Azure.Pageable<Azure.Storage.Files.Shares.Models.ShareFileItem> files = directory.GetFilesAndDirectories();
            IEnumerable<ShareFileClient> trackEnum = GetShareFiles("/", new string[] { ".mp3", ".ogg", ".flac" });
            int numberOfTracks = trackEnum.Count();
            var allTracks = trackEnum.AsParallel()
                    .Select((musicFilePath, index) =>
                    {
                        counter++;
                        return GetTrackInfo(musicFilePath, index);
                    }).ToArray();
            return JsonConvert.SerializeObject(allTracks);
        }

     

        public static TrackInfo GetTrackInfo(ShareFileClient musicFile, int index)
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

        private ShareClient GetShare(string name)
        {
            // Name of the share, directory, and file we'll create
            string shareName = "music";

            // Get a reference to a share and then create it
            ShareClient share = new ShareClient(_blobConnectionstring, name);

            return share;
        }

        private void UploadFile(string directoryName, string fileName, string content)
        {
            ShareClient share = GetShare("music");
            // Get a reference to a directory and create it
            ShareDirectoryClient directory = share.GetDirectoryClient(directoryName);

            // Get a reference to a file and upload it
            ShareFileClient file = directory.GetFileClient(fileName);
            using (MemoryStream stream = new MemoryStream())
            {
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(content);
                writer.Flush();
                stream.Position = 0;

                file.Create(stream.Length);
                file.UploadRange(new HttpRange(0, stream.Length), stream);
            }
        }

        public async Task<string> GetChannelEpisode(int collectionId, string link, long id)
        {
            return await GetFileContentAsync("podcast", "/", _channelListFile);
        }

        private async Task<string> GetFileContentAsync(string sharename, string directoryname, string filename)
        {
            ShareClient share = GetShare(sharename);
            ShareDirectoryClient directory = share.GetDirectoryClient(directoryname);
            ShareFileClient file = directory.GetFileClient(filename);

            using (var stream = await file.OpenReadAsync())
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

        private void UpdateFile(string sharename, string directoryname, string filename, string fileContent) 
        {
            ShareClient share = GetShare(sharename);
            ShareDirectoryClient directory = share.GetDirectoryClient(directoryname);
            ShareFileClient file = directory.GetFileClient(filename);
            using (Stream stream = file.OpenWrite(true, 0))
            {
                stream.Write(Encoding.Default.GetBytes(fileContent));
            }
        }

        public void UpdateChannelFile(string fileContent)
        {
            UpdateFile("podcast", "/", _channelListFile, fileContent);
        }

        public void UpdateEpisodeFile(string episodeFileContent)
        {
            UpdateFile("podcast", "/", _episodeListFile, episodeFileContent);
        }
    }
}