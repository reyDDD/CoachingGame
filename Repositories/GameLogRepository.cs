using System.Net.Http.Headers;
using System.Net.Http.Json;
using Tamboliya.Services;
using TamboliyaLibrary.DAL;

namespace Tamboliya.Repositories
{
    public class GameLogRepository : IGameLogRepository
    {
        private readonly HttpClient _http;
        private readonly IAuthService _authService;

        public GameLogRepository(HttpClient http, IAuthService authService)
        {
            _http = http;
            _authService = authService;
            Task.Run(async () => await GetToken());
        }

        public async Task AddMessageToLog(LogLineDTOModel newMessage)
        {
            await _http.PostAsJsonAsync<LogLineDTOModel>($"api/GameLog/addMessageToLog", newMessage);
        }

        public async Task<List<string>> GetGameLogLines(int gameId)
        {
            return (await _http.GetFromJsonAsync<List<string>>($"api/GameLog/info?GameId={gameId}"))!;
        }

        private async Task GetToken()
        {
            var token = await _authService.GetHttpClientToken();
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        }
    }
}
