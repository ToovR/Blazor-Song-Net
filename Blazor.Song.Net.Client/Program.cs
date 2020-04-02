using Blazor.Song.Net.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddSingleton<IDataManager, DataManager>();
            builder.RootComponents.Add<App>("app");
            builder.Services.AddBaseAddressHttpClient();
            await builder.Build().RunAsync();
        }
    }
}