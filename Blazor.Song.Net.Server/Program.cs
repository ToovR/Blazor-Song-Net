using Blazor.Song.Indexer;
using Blazor.Song.Net.Server;
using Blazor.Song.Net.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

bool isAzure = Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME") != null;
builder.Services.AddServerServices(isAzure);

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
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

