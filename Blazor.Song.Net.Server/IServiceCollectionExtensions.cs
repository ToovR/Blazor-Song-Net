using Blazor.Song.Indexer;
using Blazor.Song.Net.Server.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor.Song.Net.Server
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddServerServices(this IServiceCollection services, bool isAzure)
        {
            if (isAzure)
            {
                services.AddSingleton<ITrackParserService, AzureTrackParserService>();
            }
            else
            {
                services.AddSingleton<ITrackParserService, LocalTrackParserService>();
            }

            services.AddSingleton<IFileHelper, FileHelper>();
            services.AddSingleton<IPodcastStore, PodcastStore>();
            services.AddSingleton<ILibraryStore, LibraryStore>();
            return services;
        }
    }
}
