using Blazor.Song.Net.Client.Interfaces;
using Blazor.Song.Net.Client.Services;

namespace Blazor.Song.Net.Client
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddClientServices(this IServiceCollection services)
        {
            services.AddScoped<IDataManager, DataManager>();
            services.AddScoped<IAudioService, AudioService>();
            services.AddScoped<IJsWrapperService, JsWrapperService>();

            return services;
        }
    }
}