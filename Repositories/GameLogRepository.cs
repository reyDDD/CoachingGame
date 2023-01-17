using System.Net.Http.Json;
using System.Reflection;
using TamboliyaLibrary.DAL;
using static System.Net.WebRequestMethods;

namespace Tamboliya.Repositories
{
    public class GameLogRepository : IGameLogRepository
    {
        private readonly HttpClient _http;

        public GameLogRepository(HttpClient http)
        {
            _http = http;
        }

        public async Task AddMessageToLog(LogLineDTOModel newMessage)
        {
            await _http.PostAsJsonAsync<LogLineDTOModel>($"api/GameLog/addMessageToLog", newMessage);
        }

        public async Task<List<string>> GetGameLogLines(int gameId)
        {
            return (await _http.GetFromJsonAsync<List<string>>($"api/GameLog/info?GameId={gameId}"))!;
        }
    }
}
