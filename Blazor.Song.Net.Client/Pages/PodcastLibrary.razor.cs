using Blazor.Song.Net.Client.Shared;
using Blazor.Song.Net.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Pages
{
    public partial class PodcastLibrary : ComponentBase
    {
        public List<PodcastChannel> ChannelsFiltered { get; private set; }
        public TrackInfo CurrentChannel { get; set; }
        public ChannelSummary CurrentChannelSummary { get; set; }
        public TrackInfo CurrentEpisode { get; set; }
        public List<TrackInfo> EpisodesFiltered { get; private set; }

        [Parameter]
        public string Filter
        {
            get { return Data.Filter; }
            set
            {
                string decodedValue = null;
                if (value != null)
                    decodedValue = Uri.UnescapeDataString(value);
                if (Data.Filter == decodedValue)
                    return;
                Data.Filter = decodedValue;
            }
        }

        public bool IsNewChannels { get; set; }

        [CascadingParameter]
        public ObservableList<TrackInfo> PlaylistTracks { get; set; }

        public List<TrackInfo> TrackListFiltered { get; set; }

        [Inject]
        private Services.IDataManager Data { get; set; }

        public async Task ClickChannelRow(PodcastChannel channel)
        {
            var feed = await Data.GetEpisodes(channel.CollectionId);
            CurrentChannelSummary = new ChannelSummary
            {
                Title = feed.Title,
                Description = feed.Description,
                ImageUrl = feed.ImageUrl,
            };
            EpisodesFiltered = feed.Items.Select(i => i.ToTrackInfo(channel)).ToList();
        }

        public void DoubleclickPlaylistRow(TrackInfo track)
        {
            if (PlaylistTracks.Any(t => t.Id == track.Id))
                return;
            PlaylistTracks.Add(track);
        }

        protected async Task FilterClick()
        {
            string filter = Data.Filter;
            await UpdatePodcastChannel(filter);
        }

        protected override async Task OnInitializedAsync()
        {
            await UpdatePodcastChannel(Data.Filter);
            await base.OnInitializedAsync();
        }

        protected async Task SearchClick()
        {
            string filter = Data.Filter;
            await UpdateNewPodcastChannel(filter);
        }

        protected async Task SearchInputKeyPressed(KeyboardEventArgs keybordEvent)
        {
            if (keybordEvent.Key == "Enter")
            {
                await FilterClick();
            }
        }

        protected async Task SubscribeToPodcast(Int64 id)
        {
            var newPodcast = ChannelsFiltered.Single(p => p.CollectionId== id);
            await Data.SubscribeToPodcast(newPodcast);
        }

        private async Task UpdateLibrary(string filter)
        {
            TrackListFiltered = await Data.GetSongs(filter);
        }

        private async Task UpdateNewPodcastChannel(string filter)
        {
            IsNewChannels = true;
            ChannelsFiltered = (await Data.GetNewChannels(filter));
        }

        private async Task UpdatePodcastChannel(string filter)
        {
            IsNewChannels = false;
            ChannelsFiltered = (await Data.GetChannels(filter)).ToList();
        }
    }
}