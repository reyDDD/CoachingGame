using TamboliyaLibrary.DAL;

namespace Tamboliya.Repositories
{
    public interface IGameLogRepository
    {
        Task<List<string>> GetGameLogLines(int gameId);
        Task AddMessageToLog(LogLineDTOModel newMessage);
    }
}
