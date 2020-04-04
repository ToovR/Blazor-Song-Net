using Blazor.Song.Net.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Blazor.Song.Net.Server.Services
{
    public class LibraryStore : ILibraryStore
    {
        private const string libraryFile = "./tracks.json";
        private readonly Regex _filterSentenceRegex = new Regex("([^\\s]*\"[^\"]+[\"][^\\s]*)|[^\" ]?[^\" ]+[^\" ]?");
        private TrackInfo[] _allTracks;

        public IEnumerable<TrackInfo> GetTracks(IEnumerable<long> ids)
        {
            LoadLibrary();
            return _allTracks.Where(track => ids.Contains(track.Id));
        }

        public TrackInfo[] GetTracks(string filter)
        {
            LoadLibrary();
            filter ??= "";
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
                    if (trackInfoSearchItems.Any(tisikv => fi.StartsWith($"{tisikv.Key}:")))
                    {
                        KeyValuePair<string, Func<TrackInfo, string>> trackInfoSearchItem = trackInfoSearchItems.Single(tisikv => fi.StartsWith($"{tisikv.Key}:"));
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

        private void LoadLibrary()
        {
            if (_allTracks == null)
                _allTracks = JsonSerializer.Deserialize<TrackInfo[]>(System.IO.File.ReadAllText(libraryFile));
        }
    }
}