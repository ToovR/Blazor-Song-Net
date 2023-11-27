using Blazor.Song.Net;
using Blazor.Song.Net.Client.Interfaces;
using Blazor.Song.Net.Client.Pages;
using Blazor.Song.Net.Components;
using Blazor.Song.Net.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

bool isAzure = Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME") != null;
builder.Services.AddServerServices(isAzure);
builder.Services.AddScoped<IDataManager, DataManager>();
builder.Services.AddScoped<IJsWrapperService, JsWrapperService>();
builder.Services.AddScoped<IAudioService, AudioService>();

var app = builder.Build();

app.MapWhen(context => context.Request.Path.Value.EndsWith(".mp3") ||
         context.Request.Path.Value.EndsWith(".ogg") ||
         context.Request.Path.Value.EndsWith(".flac"), config =>
                 config.Run(async context =>
                 {
                     context.Response.Redirect($"/api/Library/Download?path={context.Request.Path.Value}");
                 }));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Playlist).Assembly);

app.MapControllers();

app.Run();