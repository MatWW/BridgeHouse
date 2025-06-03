using Frontend;
using Frontend.CookieHandler;
using Frontend.Services.Implementations;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient(new CookieHandler())
{
    BaseAddress = new Uri("https://localhost:7200/") 
});
builder.Services.AddSingleton<ISignalRService, SignalRService>();
builder.Services.AddScoped<IApiService, ApiService>();

HttpClient client = new HttpClient();
client.DefaultRequestHeaders.Add("Cookie", "test=value");

await builder.Build().RunAsync();
