using TamboliyaLibrary.DAL;
using TamboliyaLibrary.Models;

namespace Tamboliya.Repositories
{
    public interface IGameRepository
    {
        Task<OracleDTO> CreateNewGame(NewUserGame userGame);
        Task<IEnumerable<GameDTO>> GetLastGamesToJoin(DateTime startDate, DateTime endDate, int offset);
        Task<List<GameDTO>> GetListGamesInfo();
        Task<GameDTO> GetGameInfo(int? gameId);
        Task<bool> GetGameStatus(int? gameId);
        Task<GameDTO> Move(MoveModel moveModel);
        Task<GameLogDTO> GetGameLog(int? gameId);
        Task<string> Finish(EndGameDTO game);
        Task<int> GetParentGameId(int gameId);
    }
}
