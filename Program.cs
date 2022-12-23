using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Tamboliya;
using Tamboliya.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddHttpClient("Tamboliya.ServerAPI", client => client.BaseAddress = new Uri("https://localhost:7212/"));
 

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
	.CreateClient("Tamboliya.ServerAPI"));

 

var host = builder.Build();

await host.RunAsync();
