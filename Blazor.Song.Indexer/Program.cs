using Blazor.Song.Net.Shared;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Blazor.Song.Indexer
{
    internal class Program
    {
        private static string _musicDirectoryRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        private static TrackInfo[] _allTracks;
        private const string libraryFile = "../../../Blazor.Song.Net.Server/tracks.json";

        private static void Main(string[] args)
        {

            Uri folderRoot = new Uri(_musicDirectoryRoot);
            _allTracks = Directory.GetFiles(_musicDirectoryRoot, "*.*", SearchOption.AllDirectories)
                    .AsParallel()
                    .Where(file => Regex.IsMatch(file, ".*\\.(mp3|ogg|flac)$", RegexOptions.IgnoreCase))
                    .Select((musicFilePath, index) =>
                    {
                        FileInfo musicFileInfo = new FileInfo(musicFilePath);
                        TagLib.File tagMusicFile = TagLib.File.Create(new TagMusicFile(musicFileInfo.FullName));


                        string artist = tagMusicFile.Tag.FirstAlbumArtist ?? tagMusicFile.Tag.AlbumArtistsSort.FirstOrDefault() ?? ((TagLib.NonContainer.File)tagMusicFile).Tag.Performers.FirstOrDefault();
                        string title = !string.IsNullOrEmpty(tagMusicFile.Tag.Title) ? tagMusicFile.Tag.Title : Path.GetFileNameWithoutExtension(musicFileInfo.FullName);
                        if (string.IsNullOrEmpty(tagMusicFile.Tag.Title))
                            ;
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
                    }).ToArray();

            File.WriteAllText(libraryFile, JsonConvert.SerializeObject(_allTracks));
        }
    }
}
