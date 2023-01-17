using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;
using Tamboliya;
using Tamboliya.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();

if (builder.HostEnvironment.IsProduction())
{
    builder.Logging.SetMinimumLevel(LogLevel.Warning);
}
else if(builder.HostEnvironment.IsDevelopment()) {
    builder.Logging.SetMinimumLevel(LogLevel.Information);
}
else
{
    builder.Logging.SetMinimumLevel(LogLevel.Warning);
}


builder.Services.AddHttpClient("Tamboliya.ServerAPI", client => client.BaseAddress = new Uri("https://localhost:7212/"));
 

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
	.CreateClient("Tamboliya.ServerAPI"));


//UNDONE: Додати перевірку статусу http-запитів, якщо відповідь з кодом 401, видаляти з локального сховища запис і направляти на сторінку авторизації
var host = builder.Build();

var logger = host.Services.GetRequiredService<ILoggerFactory>()
    .CreateLogger<Program>();

logger.LogInformation("Logged after the app is built in Program.cs.");

await host.RunAsync();
