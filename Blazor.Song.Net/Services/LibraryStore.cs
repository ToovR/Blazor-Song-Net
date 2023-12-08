using Blazor.Song.Indexer;
using Blazor.Song.Net.Shared;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Blazor.Song.Net.Services
{
    public partial class LibraryStore : ILibraryStore
    {
        private readonly Regex _filterSentenceRegex = FilterSentenceRegex();
        private readonly IServiceProvider _serviceProvider;
        private readonly ITrackParserService _trackParser;
        private TrackInfo[]? _allTracks;

        public LibraryStore(IServiceProvider serviceProvider, ITrackParserService trackParser)
        {
            _serviceProvider = serviceProvider;
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
            _allTracks ??= InitializeAllTracks();
            return _allTracks.Where(track => ids.Contains(track.Id));
        }

        public TrackInfo[] GetTracks(string? filter)
        {
            if (!_trackParser.IsLibraryFileExists())
            {
                return [];
            }
            _allTracks ??= InitializeAllTracks();
            return _allTracks.GetTracks(filter);
        }

        public bool LoadLibrary()
        {
            _allTracks ??= InitializeAllTracks();
            return _allTracks.Length > 0;
        }

        public async Task<string> LoadPlaylist()
        {
            return await _trackParser.LoadPlaylist();
        }

        public async Task SavePlaylist(string idList)
        {
            await _trackParser.SavePlaylist(idList);
        }

        [GeneratedRegex("([^\\s]*\"[^\"]+[\"][^\\s]*)|[^\" ]?[^\" ]+[^\" ]?")]
        private static partial Regex FilterSentenceRegex();

        private TrackInfo[] InitializeAllTracks()
        {
            if (!_trackParser.IsLibraryFileExists())
            {
                _trackParser.UpdateTrackData();
            }

            string trackContent = _trackParser.GetTrackContent();
            TrackInfo[]? allTracks = JsonSerializer.Deserialize<TrackInfo[]>(trackContent);
            return allTracks ?? throw new NullReferenceException(nameof(_allTracks));
        }
    }
}