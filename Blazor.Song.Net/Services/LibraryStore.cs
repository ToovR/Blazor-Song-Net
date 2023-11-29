using Blazor.Song.Indexer;
using Blazor.Song.Net.Shared;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Blazor.Song.Net.Services
{
    public class LibraryStore : ILibraryStore
    {
        private readonly Regex _filterSentenceRegex = new Regex("([^\\s]*\"[^\"]+[\"][^\\s]*)|[^\" ]?[^\" ]+[^\" ]?");
        private readonly ITrackParserService _trackParser;
        private TrackInfo[] _allTracks;

        public LibraryStore(ITrackParserService trackParser)
        {
            _trackParser = trackParser;
        }

        public async Task<byte[]> Download(string path)
        {
            return await _trackParser.Download(path);
        }

        public IEnumerable<TrackInfo> GetTracks(IEnumerable<long> ids)
        {
            if (!_trackParser.IsLibraryFileExists())
            {
                return Enumerable.Empty<TrackInfo>();
            }
            LoadLibrary();
            return _allTracks.Where(track => ids.Contains(track.Id));
        }

        public TrackInfo[] GetTracks(string filter)
        {
            if (!_trackParser.IsLibraryFileExists())
            {
                return Array.Empty<TrackInfo>();
            }
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

        public bool LoadLibrary()
        {
            if (_allTracks == null)
            {
                if (!_trackParser.IsLibraryFileExists())
                {
                    _trackParser.UpdateTrackData();
                }

                string trackContent = _trackParser.GetTrackContent();
                _allTracks = JsonSerializer.Deserialize<TrackInfo[]>(trackContent);
            }
            return _allTracks.Count() > 0;
        }

        public async Task<string> LoadPlaylist()
        {
            return await _trackParser.LoadPlaylist();
        }

        public async Task SavePlaylist(string idList)
        {
            await _trackParser.SavePlaylist(idList);
        }
    }
}