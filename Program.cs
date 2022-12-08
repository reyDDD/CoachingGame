using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Tamboliya;
using Tamboliya.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("Tamboliya.ServerAPI", client => client.BaseAddress = new Uri("https://localhost:7212"))
    .AddHttpMessageHandler(sp =>
     {
         var handler = sp.GetService<AuthorizationMessageHandler>()!
             .ConfigureHandler(
                 authorizedUrls: new[] { "https://localhost:7212" },
				 scopes: new[] { "tamboliya-Api" });
         return handler;
     });
 

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
	.CreateClient("Tamboliya.ServerAPI"));


//In order to authenticate to IS4:
builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("oidc", options.ProviderOptions);
    options.UserOptions.RoleClaim = "role";
})
    .AddAccountClaimsPrincipalFactory<ArrayClaimsPrincipalFactory<RemoteUserAccount>>();

var host = builder.Build();

await host.RunAsync();
