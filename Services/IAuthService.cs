using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TamboliyaLibrary.Models;

namespace Tamboliya.Services
{
    public interface IAuthService
    {
        Task<LoginResult> Login(LoginModel loginModel);
        Task Logout();
        Task<RegisterResult> Register(RegisterModel registerModel);
        Task<string> GetHttpClientToken();
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthService(IHttpClientFactory clientFactory,
                           AuthenticationStateProvider authenticationStateProvider,
                           ILocalStorageService localStorage)
        {
            _httpClient = clientFactory.CreateClient("Tamboliya.ServerAPI"); ;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
        }

        public async Task<RegisterResult> Register(RegisterModel registerModel)
        {
            var response = await _httpClient.PostAsJsonAsync<RegisterModel>("api/accounts", registerModel);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<RegisterResult>();
                return result;
            }
            return new();
        }

        public async Task<LoginResult> Login(LoginModel loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync<LoginModel>("api/Login", loginModel);
            var result = await response.Content.ReadFromJsonAsync<LoginResult>();
            if (response.IsSuccessStatusCode)
            {
                if (result != null)
                {
                    await _localStorage.SetItemAsync("authToken", result!.Token);
                    ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(result?.Token);
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result!.Token);
                }
            }
            return result!;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<string> GetHttpClientToken()
        {
            if (await _localStorage.GetItemAsync<string>("authToken") != null)
            {
                return await _localStorage.GetItemAsync<string>("authToken");
            }
            else
            {
                throw new Exception("User not authorized");
            }

        }
    }
}
