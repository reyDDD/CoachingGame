
using System.ComponentModel.DataAnnotations;

namespace TamboliyaLibrary.DAL
{
    public class LogsDTOModel
    {
        [Required]
        public int GameId { get; set; }
        public Guid? UserId { get; set; }
        public HashSet<string> Messages { get; set; } = null!;
    }
}
