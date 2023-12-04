using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Blazor.Song.Net.Shared
{
    public static partial class TrackInfoArrayExtensions
    {
        private static readonly Regex _filterSentenceRegex = FilterSentenceRegex();

        private static readonly Dictionary<string, Func<TrackInfo, string>> _trackInfoSearchItems = new()
        {
            { "album:", ti => ti.Album },
            { "artist:", ti => ti.Artist },
            { "id:", ti => ti.Id.ToString() },
            { "title:", ti => ti.Title },
        };

        public static TrackInfo[] GetTracks(this TrackInfo[] allTracks, string filter)
        {
            filter ??= "";
            List<string> filterItems = _filterSentenceRegex.Matches(filter).Select(m => m.Value).ToList();

            IEnumerable<TrackInfo> filteredTracks = allTracks;

            try
            {
                filterItems.ForEach(filterItem =>
                {
                    if (_trackInfoSearchItems.Any(tisikv => filterItem.StartsWith(tisikv.Key)))
                    {
                        KeyValuePair<string, Func<TrackInfo, string>> trackInfoSearchItem = _trackInfoSearchItems.Single(tisikv => filterItem.StartsWith(tisikv.Key));
                        string valuePart = filterItem[(trackInfoSearchItem.Key.Length)..];
                        valuePart = valuePart.Trim('\"');
                        if (valuePart.First() == '/' && valuePart.Last() == '/')
                        {
                            string trimmedFilterItem = valuePart.Trim('/');
                            filteredTracks = filteredTracks.Where(ft => trackInfoSearchItem.Value(ft) != null && trimmedFilterItem.Equals(trackInfoSearchItem.Value(ft), StringComparison.CurrentCultureIgnoreCase));
                        }
                        else
                        {
                            filteredTracks = filteredTracks.Where(ft => trackInfoSearchItem.Value(ft) != null && trackInfoSearchItem.Value(ft).Contains(valuePart, StringComparison.CurrentCultureIgnoreCase));
                        }
                    }
                    else
                    {
                        if (filterItem.First() == '/' && filterItem.Last() == '/')
                        {
                            string trimmedFilterItem = filterItem.Trim('/');
                            filteredTracks = filteredTracks.Where(ft =>
                                trimmedFilterItem == "" ||
                                (ft.Title != null && ft.Title.Equals(trimmedFilterItem, StringComparison.CurrentCultureIgnoreCase)) ||
                                (ft.Artist != null && ft.Artist.Equals(trimmedFilterItem, StringComparison.CurrentCultureIgnoreCase)) ||
                                (ft.Album != null && ft.Album.Equals(trimmedFilterItem, StringComparison.CurrentCultureIgnoreCase)));
                        }
                        else
                        {
                            filteredTracks = filteredTracks.Where(ft =>
                                filterItem is null ||
                                (ft.Title != null && ft.Title.Contains(filterItem, StringComparison.CurrentCultureIgnoreCase)) ||
                                (ft.Artist != null && ft.Artist.Contains(filterItem, StringComparison.CurrentCultureIgnoreCase)) ||
                                (ft.Album != null && ft.Album.Contains(filterItem, StringComparison.CurrentCultureIgnoreCase)));
                        }
                    }
                });
                return filteredTracks.Take(100).ToArray();
            }
            catch (Exception)
            {
                throw new Exception($"exception occurred in {nameof(GetTracks)}");
            }
        }

        [GeneratedRegex("([^\\s]*\"[^\"]+[\"][^\\s]*)|[^\" ]?[^\" ]+[^\" ]?")]
        private static partial Regex FilterSentenceRegex();
    }
}