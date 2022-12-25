
namespace TamboliyaLibrary.DAL
{
    public class LogsDTOModel : GameUserDTO
    {
        public HashSet<string> Messages { get; set; } = null!;
    }
}
