using Learning.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Learning.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Resolve API base URL: prefer Aspire service discovery, fall back to config, then standalone default
var apiBaseUrl =
    builder.Configuration["services:server:http:0"]
    ?? builder.Configuration["services:server:https:0"]
    ?? builder.Configuration["ApiBaseUrl"]
    ?? "http://localhost:5200";

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl)
});

builder.Services.AddTransient<ApiClient>();
builder.Services.AddScoped<StateService>();

await builder.Build().RunAsync();
