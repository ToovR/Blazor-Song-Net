using Blazor.Song.Indexer;
using Blazor.Song.Net.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Linq;

namespace Blazor.Song.Net.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IFileHelper, FileHelper>();
            services.AddSingleton<IPodcastStore, PodcastStore>();
            services.AddSingleton<ILibraryStore, LibraryStore>();
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            string libraryFile = "./tracks.json";
            if (!File.Exists(libraryFile))
            {
                TrackParser parser = new TrackParser();
                var fileContent = parser.GetTrackData();
                File.WriteAllText(libraryFile, fileContent);
            }
            app.MapWhen(context => context.Request.Path.Value.EndsWith(".mp3") ||
            context.Request.Path.Value.EndsWith(".ogg") ||
            context.Request.Path.Value.EndsWith(".flac"), config =>
                    config.Run(async context =>
                    {
                        context.Response.Redirect($"/api/Library/Download?path={context.Request.Path.Value}");
                    }));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
