using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Blazor.Song.Net.Server.Controllers
{
    [Route("api/[controller]")]
    public class LibraryController : Controller
    {
        private string directoryRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        private TrackInfo[] _allTracks;
        private Regex _filterSentenceRegex = new Regex("([^\\s]*\"[^\"]+[\"][^\\s]*)|[^\" ]?[^\" ]+[^\" ]?");

        [HttpGet("[action]")]
        public IEnumerable<TrackInfo> Tracks(string filter)
        {
            LoadLibrary();
            filter = filter ?? "";
            List<string> filterItems = _filterSentenceRegex.Matches(filter).Select(m => m.Value).ToList();

            IEnumerable<TrackInfo> filteredTracks = _allTracks;


            Dictionary<string, Func<TrackInfo, string>> trackInfoSearchItems =
                new Dictionary<string, Func<TrackInfo, string>>
                {
                    { "album", ti => ti.Album },
                    { "artist", ti => ti.Artist },
                    { "id", ti => ti.Id.ToString() },
                    { "title", ti => ti.Title },
                };


            try
            {
                filterItems.ForEach(fi =>
                {
                    if (trackInfoSearchItems.Any(tisikv => fi.StartsWith(tisikv.Key + ":")))
                    {
                        KeyValuePair<string, Func<TrackInfo, string>> trackInfoSearchItem = trackInfoSearchItems.Single(tisikv => fi.StartsWith(tisikv.Key + ":"));
                        string valuePart = fi.Substring(trackInfoSearchItem.Key.Length + 1);
                        valuePart = valuePart.Trim('\"');
                        if (valuePart.First() == '/' && valuePart.Last() == '/')
                        {
                            Regex valuePartRegex = new Regex(valuePart.Trim('/'), RegexOptions.None, new TimeSpan(0, 0, 2));
                            filteredTracks = filteredTracks.Where(ft => trackInfoSearchItem.Value(ft) != null && valuePartRegex.IsMatch(trackInfoSearchItem.Value(ft)));
                        }
                        else
                        {
                            filteredTracks = filteredTracks.Where(ft => trackInfoSearchItem.Value(ft) != null && trackInfoSearchItem.Value(ft).Contains(valuePart, StringComparison.CurrentCultureIgnoreCase));
                        }
                    }
                    else
                    {
                        string filteredItem = null;
                        if (fi != null)
                            filteredItem = fi.Trim('\"');
                        filteredTracks = filteredTracks.Where(t =>
                            filteredItem is null ||
                            (t.Title != null && t.Title.Contains(filteredItem, StringComparison.CurrentCultureIgnoreCase)) ||
                            (t.Artist != null && t.Artist.Contains(filteredItem, StringComparison.CurrentCultureIgnoreCase)) ||
                            (t.Album != null && t.Album.Contains(filteredItem, StringComparison.CurrentCultureIgnoreCase)));
                    }
                });
                return filteredTracks.Take(100).ToArray();
            }
            catch (ArgumentException)
            {
                return null;
            }
            catch (RegexMatchTimeoutException)
            {
                return null;

            }
            catch (Exception)
            {
                return null;

            }
        }

        [HttpGet("[action]")]
        public ActionResult Download(string path)
        {
            byte[] file = ReadFile(Path.Combine(directoryRoot, path.Trim('/').Replace("/", "\\")));
            return File(file, "audio/mpeg");
        }

        private byte[] ReadFile(string path)
        {
            using (FileStream s = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[s.Length];
                s.Read(buffer, 0, (int)s.Length);
                return buffer;
            }
        }

        private const string libraryFile = "./tracks.json";
        private void LoadLibrary()
        {
            if (_allTracks == null)
                _allTracks = JsonSerializer.Deserialize<TrackInfo[]>(System.IO.File.ReadAllText(libraryFile));            
        }
    }
}
