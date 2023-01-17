using System.Net.NetworkInformation;
using TamboliyaLibrary.DAL;
using TamboliyaLibrary.Models;
using static System.Net.WebRequestMethods;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Globalization;

namespace Tamboliya.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly HttpClient _http;

        public GameRepository(HttpClient http)
        {
            _http = http;
        }

        public async Task<OracleDTO> CreateNewGame(NewUserGame userGame)
        {
            var response = await _http.PostAsJsonAsync<NewUserGame>($"api/Game/new", userGame);

            return (await response?.Content?.ReadFromJsonAsync<OracleDTO>()!)!;
        }

        public async Task<IEnumerable<GameDTO>> GetLastGamesToJoin(DateTime startDate, DateTime endDate, int offset)
        {
            //TODO: працює дуже повільно!!!
            var _startDate = startDate.ToString(CultureInfo.InvariantCulture);
            var _endDate = endDate.ToString(CultureInfo.InvariantCulture);
            return (await _http.GetFromJsonAsync<List<GameDTO>>(
                $"api/game/lastGamesToJoin?DateBeginning={_startDate}&DateEnding={_endDate}&Offset={offset}"))!;
        }
    }
}
