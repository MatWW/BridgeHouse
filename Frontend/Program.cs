using Frontend;
using Frontend.CookieHandler;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient(new CookieHandler())
{
    BaseAddress = new Uri("https://localhost:7200/") 
});

HttpClient client = new HttpClient();
client.DefaultRequestHeaders.Add("Cookie", "test=value");

await builder.Build().RunAsync();
