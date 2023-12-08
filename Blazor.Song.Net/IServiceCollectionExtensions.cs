using Blazor.Song.Indexer;
using Blazor.Song.Net.Client.Interfaces;
using Blazor.Song.Net.Services;

namespace Blazor.Song.Net
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddServerServices(this IServiceCollection services, bool isAzure)
        {
            if (isAzure)
            {
                services.AddScoped<ITrackParserService, AzureTrackParserService>();
            }
            else
            {
                services.AddScoped<ITrackParserService, LocalTrackParserService>();
            }

            services.AddScoped<IPodcastStore, PodcastStore>();
            services.AddScoped<ILibraryStore, LibraryStore>();
            services.AddScoped<IDataManager, ServerDataManager>();
            services.AddScoped<IAudioService, ServerAudioService>();

            return services;
        }
    }
}