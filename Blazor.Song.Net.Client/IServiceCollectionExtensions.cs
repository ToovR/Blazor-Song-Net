using Blazor.Song.Net.Client.Interfaces;
using Blazor.Song.Net.Client.Services;
using Blazored.LocalStorage;

namespace Blazor.Song.Net.Client
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddClientServices(this IServiceCollection services)
        {
            services.AddScoped<IDataManager, ClientDataManager>();
            services.AddScoped<IAudioService, ClientAudioService>();
            services.AddBlazoredLocalStorage();
            return services;
        }
    }
}