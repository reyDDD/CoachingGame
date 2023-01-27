using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Tamboliya;
using Tamboliya.Repositories;
using Tamboliya.Services;
using TamboliyaLibrary.Models;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddHttpClientInterceptor();

builder.Services.AddScoped<HttpInterceptorService>();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGameLogRepository, GameLogRepository>();
builder.Services.AddScoped<WebRtcService>();


if (builder.HostEnvironment.IsProduction())
{
    builder.Logging.SetMinimumLevel(LogLevel.Warning);
}
else if (builder.HostEnvironment.IsDevelopment())
{
    builder.Logging.SetMinimumLevel(LogLevel.Information);
}
else
{
    builder.Logging.SetMinimumLevel(LogLevel.Warning);
}


builder.Services.AddHttpClient("Tamboliya.ServerAPI", (IServiceProvider serviceProvider, HttpClient client) =>
{
    client.BaseAddress = new Uri(builder.Configuration[SolutionPathes.WebApiServer]!);
    client.EnableIntercept(serviceProvider);
});

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("Tamboliya.ServerAPI"));

//UNDONE Use library for re-send http requests if server doesn't answer
var host = builder.Build();
var logger = host.Services.GetRequiredService<ILoggerFactory>()
    .CreateLogger<Program>();

logger.LogInformation("Logged after the app is built in Program.cs.");

await host.RunAsync();
