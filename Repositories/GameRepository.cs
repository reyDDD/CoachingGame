using TamboliyaLibrary.DAL;
using TamboliyaLibrary.Models;
using System.Net.Http.Json;
using System.Globalization;
using static System.Net.WebRequestMethods;
using System.Reflection;
using System.Net.Http.Headers;
using Tamboliya.Services;

namespace Tamboliya.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly HttpClient _http;
        private readonly IAuthService _authService;

        public GameRepository(HttpClient http, IAuthService authService)
        {
            _http = http;
            Task.Run(async () => await GetToken());
            _authService = authService;
        }

        public async Task<OracleDTO> CreateNewGame(NewUserGame userGame)
        {
            var response = await _http.PostAsJsonAsync<NewUserGame>($"api/Game/new", userGame);

            return (await response?.Content?.ReadFromJsonAsync<OracleDTO>()!)!;
        }

        public async Task<string> Finish(EndGameDTO game)
        {
            var response = await _http.PostAsJsonAsync<EndGameDTO>($"api/Game/finish", game);

            if (response.IsSuccessStatusCode)
            {
                return (await response.Content.ReadFromJsonAsync<string>())!;
            }
            throw new InvalidOperationException("Закінчити гру не вдалось");
        }

        public async Task<GameDTO> GetGameInfo(int? gameId)
        {
            if (gameId == null)
                throw new ArgumentNullException(nameof(gameId));

            return (await _http.GetFromJsonAsync<GameDTO>($"api/Game/info?gameId={gameId}"))!;
        }

        public async Task<GameLogDTO> GetGameLog(int? gameId)
        {
            if (gameId.HasValue && gameId > 0)
            {
                return (await _http.GetFromJsonAsync<GameLogDTO>($"api/Game/log/{gameId}"))!;
            }
            throw new InvalidOperationException("Game's log not get");
        }

        public async Task<bool> GetGameStatus(int? gameId)
        {
            if (gameId == null)
                throw new ArgumentNullException(nameof(gameId));

            return (await _http.GetFromJsonAsync<bool>($"api/Game/status/{gameId}"))!;
        }

        public async Task<IEnumerable<GameDTO>> GetLastGamesToJoin(DateTime startDate, DateTime endDate, int offset)
        {
            //TODO: працює дуже повільно!!!
            var _startDate = startDate.ToString(CultureInfo.InvariantCulture);
            var _endDate = endDate.ToString(CultureInfo.InvariantCulture);
            return (await _http.GetFromJsonAsync<List<GameDTO>>(
                $"api/game/lastGamesToJoin?DateBeginning={_startDate}&DateEnding={_endDate}&Offset={offset}"))!;
        }

        public async Task<List<GameDTO>> GetListGamesInfo()
        {
            return (await _http.GetFromJsonAsync<List<GameDTO>>("api/game/gamesInfo"))!;
        }

        public async Task<GameDTO> Move(MoveModel moveModel)
        {
            var response = await _http.PostAsJsonAsync<MoveModel>($"api/Game/move", moveModel);
            if (response.IsSuccessStatusCode)
            {
                return (await response.Content.ReadFromJsonAsync<GameDTO>())!;
            }
            throw new InvalidOperationException("Перехід на нову позицію не виконано");
        }

        private async Task GetToken()
        {
            var token = await _authService.GetHttpClientToken();
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        }
    }
}
