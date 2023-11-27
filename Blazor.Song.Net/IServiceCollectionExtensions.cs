using Blazor.Song.Indexer;
using Blazor.Song.Net.Client.Services;
using Blazor.Song.Net.Services;
using Microsoft.Extensions.DependencyInjection;

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

            services.AddScoped<IFileHelper, FileHelper>();
            services.AddScoped<IPodcastStore, PodcastStore>();
            services.AddScoped<ILibraryStore, LibraryStore>();

            return services;
        }
    }
}
