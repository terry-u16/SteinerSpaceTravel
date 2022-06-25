using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SteinerSpaceTravel.Web;
using SteinerSpaceTravel.Web.Services;
using BlazorDownloadFile;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<VisualizeService>();
builder.Services.AddScoped<GenerateService>();
builder.Services.AddBlazorDownloadFile();

await builder.Build().RunAsync();
