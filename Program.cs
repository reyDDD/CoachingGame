using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Tamboliya;
using Tamboliya.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7212") });


 

builder.Services.AddApiAuthorization(options => {
	//options.AuthenticationPaths.LogInPath = "api/account/login";
	//options.AuthenticationPaths.RegisterPath = "api/account/register";
	//options.AuthenticationPaths.LogInCallbackPath = "security/login-callback";
	//options.AuthenticationPaths.LogInFailedPath = "security/login-failed";
	//options.AuthenticationPaths.LogOutPath = "security/logout";
	//options.AuthenticationPaths.LogOutCallbackPath = "security/logout-callback";
	//options.AuthenticationPaths.LogOutFailedPath = "security/logout-failed";
	//options.AuthenticationPaths.LogOutSucceededPath = "security/logged-out";
	//options.AuthenticationPaths.ProfilePath = "security/profile";
});

builder.Services.AddScoped<IAccountService, AccountService>()
				.AddScoped<IAlertService, AlertService>()
				.AddScoped<IHttpService, HttpService>()
				.AddScoped<ILocalStorageService, LocalStorageService>();

var host = builder.Build();

var accountService = host.Services.GetRequiredService<IAccountService>();
await accountService.Initialize();

await host.RunAsync();
