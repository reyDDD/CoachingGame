using TamboliyaLibrary.DAL;
using TamboliyaLibrary.Models;

namespace Tamboliya.Repositories
{
    public interface IGameRepository
    {
        Task<OracleDTO> CreateNewGame(NewUserGame userGame);
        Task<IEnumerable<GameDTO>> GetLastGamesToJoin(DateTime startDate, DateTime endDate, int offset);
    }
}
